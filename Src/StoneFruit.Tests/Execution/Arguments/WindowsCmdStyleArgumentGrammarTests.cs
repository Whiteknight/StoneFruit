using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments;

public class WindowsCmdStyleArgumentGrammarTests
{
    private static ParsedArguments Parse(string args)
    {
        var parser = WindowsCmdArgumentGrammar.GetParser();
        var arguments = parser.List().Parse(args).Value.ToList();
        return new ParsedArguments(arguments);
    }

    [Test]
    public void Positional_Test()
    {
        var result = Parse("a b c");
        result.Shift().Value.Should().Be("a");
        result.Shift().Value.Should().Be("b");
        result.Shift().Value.Should().Be("c");
        result.Shift().Value.Should().Be("");
    }

    [Test]
    public void Flags_Flag()
    {
        var result = Parse("/a");
        result.HasFlag("a").Should().BeTrue();
        result.HasFlag("x").Should().BeFalse();
    }

    [Test]
    public void Flags_Named()
    {
        var result = Parse("/a:test");
        result.Get("a").Value.Should().Be("test");
    }

    [Test]
    public void Flags_MaybeNamed()
    {
        var result = Parse("/a test");
        result.Get("a").Value.Should().Be("test");
    }
}
