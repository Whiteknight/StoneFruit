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
            var ws = Whitespace();
            var ows = OptionalWhitespace();

            IParser<char, TemplatePart>? itemRaw = null;
            var item = Deferred(() => itemRaw!);

            IParser<char, TemplatePart> literal = CreateLiteralParser();
            var pipeline = CreatePipelineParser();

            var pipelineTag = Rule(
                Match("{{"),
                ows,
                pipeline,
                ows,
                Match("}}"),
                (_, _, p, _, _) => TemplatePart.Pipeline(p));

            var brush = CreateBrushParser();

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

            itemRaw = First(
                pipelineTag,
                colorTag,
                ifTag,
                literal);

            return Rule(
                item.List(),
                End(),
                (items, _) => (ITemplate)new Template(items));
        }

        private static IParser<char, TemplatePart> CreateLiteralParser()
        {
            var openCurly = MatchChar('{');
            var basicChar = First(
                openCurly.NotFollowedBy(openCurly),
                Match(c => c != '{' && c != '\0')
            );
            return basicChar.ListCharToString().Map(s => TemplatePart.Literal(s));
        }

        private static IParser<char, Brush> CreateBrushParser()
        {
            var color = First(
                Drawing.Color(),
                Drawing.ConsoleColor().Transform(Brush.ToColor));

            return First(
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
        }

        private static IParser<char, Pipeline> CreatePipelineParser()
        {
            var dotIdentifier = Rule(
                MatchChar('.'),
                Identifier(),
                (_, id) => PipelineFilter.DotIdentifier(id));

            var pipelineFilter = First(
                dotIdentifier);

            return pipelineFilter.List(true).Map(l => new Pipeline(l));
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

    public sealed record Template(IReadOnlyList<TemplatePart> Parts) : ITemplate
    {
        public static ITemplate Empty() => new Template([]);

        public IEnumerable<OutputMessage> Render(object? value)
        {
            if (Parts == null || Parts.Count == 0)
                return [];
            var currentBrush = Brush.Default;
            var messages = new List<OutputMessage>();
            foreach (var part in Parts)
            {
                (var msgs, currentBrush) = part.Render(value, currentBrush);
                messages.AddRange(msgs);
            }

            return messages;
        }
    }

    public abstract record TemplatePart()
    {
        public static TemplatePart Literal(string text) => new LiteralPart(text);

        public static TemplatePart Color(Brush color) => new ColorPart(color);

        public static TemplatePart Pipeline(Pipeline filters) => new PipelinePart(filters);

        public static TemplatePart If(Pipeline predicate, IReadOnlyList<TemplatePart> thenBlock, IReadOnlyList<TemplatePart> elseBlock)
            => new IfPart(predicate, thenBlock, elseBlock);

        public abstract (IEnumerable<OutputMessage> Messages, Brush Brush) Render(object? value, Brush brush);
    }

    public sealed record LiteralPart(string Text) : TemplatePart
    {
        public override (IEnumerable<OutputMessage> Messages, Brush Brush) Render(object? value, Brush brush)
        {
            return ([new OutputMessage(Text, Brush: brush)], brush);
        }
    }

    public sealed record ColorPart(Brush Brush) : TemplatePart
    {
        public override (IEnumerable<OutputMessage> Messages, Brush Brush) Render(object? value, Brush brush)
            => ([], Brush);
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
        public override (IEnumerable<OutputMessage> Messages, Brush Brush) Render(object? value, Brush brush)
        {
            if (value is null)
                return ([], default);
            var accessed = Filters.GetStringValue(value);
            return ([new OutputMessage(accessed ?? string.Empty, Brush: brush)], brush);
        }
    }

    public sealed record IfPart(Pipeline Predicate, IReadOnlyList<TemplatePart> ThenBlock, IReadOnlyList<TemplatePart> ElseBlock) : TemplatePart
    {
        public override (IEnumerable<OutputMessage> Messages, Brush Brush) Render(object? value, Brush brush)
        {
            return Render(value, string.IsNullOrEmpty(Predicate.GetStringValue(value)) ? ElseBlock : ThenBlock, brush);
        }

        private static (IEnumerable<OutputMessage> Messages, Brush Brush) Render(object? value, IReadOnlyList<TemplatePart> parts, Brush brush)
        {
            var currentBrush = brush;
            var output = new List<OutputMessage>();
            foreach (var part in parts)
            {
                (var msgs, currentBrush) = part.Render(value, currentBrush);
                output.AddRange(msgs);
            }

            return (output, currentBrush);
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
