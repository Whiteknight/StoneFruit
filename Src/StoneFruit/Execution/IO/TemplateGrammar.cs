using System;
using System.Collections.Generic;
using ParserObjects;
using static ParserObjects.Parsers<char>;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers.C;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.IO;

public static class OutputTemplateExtensions
{
    public static IOutput WriteLineFormatted(this IOutput output, string format, object value)
    {
        NotNull(output);
        NotNullOrEmpty(format);
        var template = TemplateGrammar.GetParser().Parse(format).Value;
        template.Render(output, value);
        return output.WriteLine();
    }
}

public readonly record struct Template(IReadOnlyList<TemplatePart> Parts)
{
    public void Render(IOutput output, object value)
    {
        var currentOutput = output;
        foreach (var part in Parts)
        {
            currentOutput = part.Render(currentOutput, value);
        }
    }
}

public abstract record TemplatePart()
{
    public static TemplatePart Literal(string text) => new LiteralPart(text);
    public static TemplatePart Color(Brush color) => new ColorPart(color);
    public static TemplatePart Pipeline(Pipeline filters) => new PipelinePart(filters);
    public static TemplatePart If(Pipeline predicate, IReadOnlyList<TemplatePart> thenBlock, IReadOnlyList<TemplatePart> elseBlock)
        => new IfPart(predicate, thenBlock, elseBlock);

    public abstract IOutput Render(IOutput output, object value);
}

public sealed record LiteralPart(string Text) : TemplatePart
{
    public override IOutput Render(IOutput output, object value)
    {
        output.Write(Text);
        return output;
    }
}

public sealed record ColorPart(Brush Brush) : TemplatePart
{
    public override IOutput Render(IOutput output, object value)
    {
        return output.Color(Brush);
    }
}

public readonly record struct Pipeline(IReadOnlyList<PipelineFilter> Filters)
{
    public string GetStringValue(object value)
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
    public override IOutput Render(IOutput output, object value)
    {
        if (value is null)
            return output;
        var accessed = Filters.GetStringValue(value);
        return output.Write(accessed?.ToString() ?? string.Empty);
    }
}

public sealed record IfPart(Pipeline Predicate, IReadOnlyList<TemplatePart> ThenBlock, IReadOnlyList<TemplatePart> ElseBlock) : TemplatePart
{
    public override IOutput Render(IOutput output, object value)
    {
        Render(output, value, string.IsNullOrEmpty(Predicate.GetStringValue(value)) ? ElseBlock : ThenBlock);
        return output;
    }

    private static void Render(IOutput output, object value, IReadOnlyList<TemplatePart> parts)
    {
        var currentOutput = output;
        foreach (var part in parts)
        {
            currentOutput = part.Render(currentOutput, value);
        }
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

public static class TemplateGrammar
{
    private static readonly Lazy<IParser<char, Template>> _parser = new Lazy<IParser<char, Template>>(CreateParserInternal);

    public static IParser<char, Template> GetParser() => _parser.Value;

    private static IParser<char, Template> CreateParserInternal()
    {
        var openCurly = MatchChar('{');
        var ws = Whitespace();
        var ows = OptionalWhitespace();
        IParser<char, TemplatePart>? itemRaw = null;
        var item = Deferred(() => itemRaw!);

        var basicChar = First(
            openCurly.NotFollowedBy(openCurly),
            Match(c => c != '{')
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
            pipeline,
            Match("}}"),
            (_, p, _) => TemplatePart.Pipeline(p));

        var colorTag = Rule(
            Match("{{"),
            ows,
            Match("color"),
            Optional(
                Rule(
                    ws,
                    CSharp.Enum<ConsoleColor>(),
                    Optional(
                        Rule(
                            MatchChar(','),
                            CSharp.Enum<ConsoleColor>(),
                            (_, bg) => bg),
                        () => Brush.Default.Background),
                    (_, fg, bg) => new Brush(fg, bg)
                ),
                () => Brush.Default
            ),
            Match("}}"),
            (_, _, _, brush, _) => TemplatePart.Color(brush));

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
            (items, _) => new Template(items));
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
