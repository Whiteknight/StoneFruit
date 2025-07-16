using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments;

public class PosixStyleArgumentGrammarTests
{
    private static ParsedArguments Parse(string args)
    {
        var parser = PosixStyleArgumentGrammar.GetParser();
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
    public void Flags_ShortC()
    {
        var result = Parse("-a");
        result.HasFlag("a").Should().BeTrue();
        result.HasFlag("x").Should().BeFalse();
    }

    [Test]
    public void Flags_ShortCombined()
    {
        var result = Parse("-abc");
        result.HasFlag("a").Should().BeTrue();
        result.HasFlag("b").Should().BeTrue();
        result.HasFlag("c").Should().BeTrue();
        result.HasFlag("x").Should().BeFalse();
    }

    [Test]
    public void Flags_Long()
    {
        var result = Parse("--abc");
        result.HasFlag("abc").Should().BeTrue();

        // A single long flag is not treated like three short flags
        result.HasFlag("a").Should().BeFalse();
        result.HasFlag("b").Should().BeFalse();
        result.HasFlag("c").Should().BeFalse();
    }

    [Test]
    public void Flags_LongEqualsNamed()
    {
        var result = Parse("--abc=test");
        result.Get("abc").Value.Should().Be("test");
    }

    [Test]
    public void ShortImpliedNamed_ConsumeNamed()
    {
        // If we consume the named arg, it consumes the entire production
        var result = Parse("-a xyz");
        result.Consume("a").Value.Should().Be("xyz");
        result.HasFlag("a").Should().BeFalse();
        result.Get(0).Exists().Should().BeFalse();
    }

    [Test]
    public void ShortImpliedNamed_ConsumePositional()
    {
        // If we consume the positional, it removes the named arg but keeps the flag available
        var result = Parse("-a xyz");
        result.Consume(0).Value.Should().Be("xyz");
        result.HasFlag("a").Should().BeTrue();
        result.Get("a").Exists().Should().BeFalse();
    }

    [Test]
    public void ShortImpliedNamed_ConsumeFlag()
    {
        // If we consume the flag it removes the named arg but keeps the positional available
        var result = Parse("-a xyz");
        result.ConsumeFlag("a").Exists().Should().BeTrue();
        result.Get("a").Exists().Should().BeFalse();
        result.Get(0).Exists().Should().BeTrue();
    }

    [Test]
    public void LongImpliedNamed_ConsumeNamed()
    {
        // If we consume the named arg, it consumes the entire production
        var result = Parse("--abc xyz");
        result.Consume("abc").Value.Should().Be("xyz");
        result.HasFlag("abc").Should().BeFalse();
        result.Get(0).Exists().Should().BeFalse();
    }

    [Test]
    public void LongImpliedNamed_ConsumePositional()
    {
        // If we consume the positional, it removes the named arg but keeps the flag available
        var result = Parse("--abc xyz");
        result.Consume(0).Value.Should().Be("xyz");
        result.HasFlag("abc").Should().BeTrue();
        result.Get("abc").Exists().Should().BeFalse();
    }

    [Test]
    public void LongImpliedNamed_ConsumeFlag()
    {
        // If we consume the flag it removes the named arg but keeps the positional available
        var result = Parse("--abc xyz");
        result.ConsumeFlag("abc").Exists().Should().BeTrue();
        result.Get("abc").Exists().Should().BeFalse();
        result.Get(0).Exists().Should().BeTrue();
    }
}
