using System;
using System.Collections.Generic;
using ParserObjects;
using StoneFruit.Execution.IO;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers<char>;
using static ParserObjects.Parsers.C;

namespace StoneFruit.Execution.Templating;

public static class DefaultTemplateFormat
{
    public static class Grammar
    {
        private static readonly Lazy<IParser<char, ITemplate>> _parser = new(CreateParserInternal);

        public static IParser<char, ITemplate> CreateParser() => _parser.Value;

        private static IParser<char, ITemplate> CreateParserInternal()
        {
            var openCurly = MatchChar('{');
            var ws = Whitespace();
            var ows = OptionalWhitespace();
            IParser<char, TemplatePart>? itemRaw = null;
            var item = Deferred(() => itemRaw!);

            var basicChar = First(
                openCurly.NotFollowedBy(openCurly),
                Match(c => c != '{' && c != '\0')
            );
            var literal = basicChar.ListCharToString().Map(s => TemplatePart.Literal(s));

            var dotIdentifier = Rule(
                MatchChar('.'),
                Identifier(),
                (_, id) => PipelineFilter.DotIdentifier(id));

            var pipelineFilter = First(
                dotIdentifier);

            var pipeline = pipelineFilter.List(true).Map(l => new Pipeline(l));

            var pipelineTag = Rule(
                Match("{{"),
                ows,
                pipeline,
                ows,
                Match("}}"),
                (_, _, p, _, _) => TemplatePart.Pipeline(p));

            var color = First(
                Drawing.Color(),
                Drawing.ConsoleColor().Transform(Brush.ToColor));

            var brush = First(
                MatchChars("default", true).Transform(_ => Brush.Default),
                Rule(
                    color,
                    Optional(
                        Rule(
                            MatchChar(','),
                            color,
                            (_, bg) => bg
                        ),
                        () => Brush.Default.GetColors().Background
                    ),
                    (fg, bg) => new Brush(fg, bg)
                )
            );

            var colorTag = Rule(
                Match("{{"),
                ows,
                Match("color"),
                Optional(
                    Rule(
                        ws,
                        brush,
                        (_, b) => b
                    ),
                    () => Brush.Default
                ),
                ows,
                Match("}}"),
                (_, _, _, b, _, _) => TemplatePart.Color(b));

            var ifStartTag = Rule(
                Match("{{"),
                ows,
                Match("if"),
                ws,
                pipeline,
                ows,
                Match("}}"),
                (_, _, _, _, p, _, _) => p);

            var ifTag = Rule(
                ifStartTag,
                item.List(),
                Optional(
                    Rule(
                        Tag("else"),
                        item.List(),
                        (_, e) => e
                    ),
                    () => []
                ),
                Tag("end"),
                (p, thens, elses, _) => TemplatePart.If(p, thens, elses));

            var tag = First(
                pipelineTag,
                colorTag,
                ifTag);

            itemRaw = First(
                tag,
                literal);

            return Rule(
                item.List(),
                End(),
                (items, _) => (ITemplate)new Template(items));
        }

        private static IParser<char, string> Tag(string name)
            => Rule(
                Match("{{"),
                OptionalWhitespace(),
                Match(name),
                OptionalWhitespace(),
                Match("}}"),
                (_, _, _, _, _) => name);
    }

    public sealed record Parser(IParser<char, ITemplate> TemplateParser) : ITemplateParser
    {
        public static ITemplateParser Create()
            => new Parser(Grammar.CreateParser());

        public ITemplate Parse(string format)
            => TemplateParser.Parse(format).Value;
    }

    public readonly record struct Template(IReadOnlyList<TemplatePart> Parts) : ITemplate
    {
        public void Render(IOutput output, object? value)
        {
            var currentBrush = Brush.Default;
            foreach (var part in Parts)
                currentBrush = part.Render(output, value, currentBrush);
        }
    }

    public abstract record TemplatePart()
    {
        public static TemplatePart Literal(string text) => new LiteralPart(text);

        public static TemplatePart Color(Brush color) => new ColorPart(color);

        public static TemplatePart Pipeline(Pipeline filters) => new PipelinePart(filters);

        public static TemplatePart If(Pipeline predicate, IReadOnlyList<TemplatePart> thenBlock, IReadOnlyList<TemplatePart> elseBlock)
            => new IfPart(predicate, thenBlock, elseBlock);

        public abstract Brush Render(IOutput output, object? value, Brush brush);
    }

    public sealed record LiteralPart(string Text) : TemplatePart
    {
        public override Brush Render(IOutput output, object? value, Brush brush)
        {
            output.Write(Text, brush);
            return brush;
        }
    }

    public sealed record ColorPart(Brush Brush) : TemplatePart
    {
        public override Brush Render(IOutput output, object? value, Brush brush)
            => Brush;
    }

    public readonly record struct Pipeline(IReadOnlyList<PipelineFilter> Filters)
    {
        public string GetStringValue(object? value)
        {
            var current = value;
            if (current is null)
                return string.Empty;

            foreach (var filter in Filters)
            {
                current = filter.Access(current);
                if (current is null)
                    return string.Empty;
            }

            return current?.ToString() ?? string.Empty;
        }
    }

    public sealed record PipelinePart(Pipeline Filters) : TemplatePart
    {
        public override Brush Render(IOutput output, object? value, Brush brush)
        {
            if (value is null)
                return brush;
            var accessed = Filters.GetStringValue(value);
            output.Write(accessed?.ToString() ?? string.Empty);
            return brush;
        }
    }

    public sealed record IfPart(Pipeline Predicate, IReadOnlyList<TemplatePart> ThenBlock, IReadOnlyList<TemplatePart> ElseBlock) : TemplatePart
    {
        public override Brush Render(IOutput output, object? value, Brush brush)
        {
            return Render(output, value, string.IsNullOrEmpty(Predicate.GetStringValue(value)) ? ElseBlock : ThenBlock, brush);
        }

        private static Brush Render(IOutput output, object? value, IReadOnlyList<TemplatePart> parts, Brush brush)
        {
            var currentBrush = brush;
            foreach (var part in parts)
                currentBrush = part.Render(output, value, currentBrush);
            return currentBrush;
        }
    }

    public abstract record PipelineFilter()
    {
        public static PipelineFilter DotIdentifier(string id) => new DotIdentifier(id);

        public abstract object? Access(object value);
    }

    public sealed record DotIdentifier(string Identifier) : PipelineFilter
    {
        public override object? Access(object value)
            => value.GetType().GetProperty(Identifier)?.GetValue(value);
    }
}
