using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments;

public class PowershellStyleArgumentGrammarTests
{
    private static ParsedArguments Parse(string args)
    {
        var parser = PowershellStyleArgumentGrammar.GetParser();
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
    public void Flag_Test()
    {
        var result = Parse("-a");
        result.HasFlag("a").Should().BeTrue();
        result.HasFlag("x").Should().BeFalse();
    }

    [Test]
    public void Flag_DoubleDash()
    {
        var result = Parse("--a");
        result.HasFlag("a").Should().BeTrue();
        result.HasFlag("x").Should().BeFalse();
    }

    [Test]
    public void FlagOrNamed_ConsumeNamed()
    {
        // If we consume the named arg, it consumes the entire production
        var result = Parse("-abc xyz");
        result.Consume("abc").Value.Should().Be("xyz");
        result.HasFlag("abc").Should().BeFalse();
        result.Get(0).Exists().Should().BeFalse();
    }

    [Test]
    public void FlagOrNamed_ConsumePositional()
    {
        // If we consume the positional, it removes the named arg but keeps the flag available
        var result = Parse("-abc xyz");
        result.Consume(0).Value.Should().Be("xyz");
        result.HasFlag("abc").Should().BeTrue();
        result.Get("abc").Exists().Should().BeFalse();
    }

    [Test]
    public void FlagOrNamed_ConsumeFlag()
    {
        // If we consume the flag it removes the named arg but keeps the positional available
        var result = Parse("-abc xyz");
        result.ConsumeFlag("abc").Exists().Should().BeTrue();
        result.Get("abc").Exists().Should().BeFalse();
        result.Get(0).Exists().Should().BeTrue();
    }

    [Test]
    public void FlagOrNamed_DoubleDash_ConsumeNamed()
    {
        // If we consume the named arg, it consumes the entire production
        var result = Parse("--abc xyz");
        result.Consume("abc").Value.Should().Be("xyz");
        result.HasFlag("abc").Should().BeFalse();
        result.Get(0).Exists().Should().BeFalse();
    }

    [Test]
    public void FlagOrNamed_DoubleDash_ConsumePositional()
    {
        // If we consume the positional, it removes the named arg but keeps the flag available
        var result = Parse("--abc xyz");
        result.Consume(0).Value.Should().Be("xyz");
        result.HasFlag("abc").Should().BeTrue();
        result.Get("abc").Exists().Should().BeFalse();
    }

    [Test]
    public void FlagOrNamed_DoubleDash_ConsumeFlag()
    {
        // If we consume the flag it removes the named arg but keeps the positional available
        var result = Parse("--abc xyz");
        result.ConsumeFlag("abc").Exists().Should().BeTrue();
        result.Get("abc").Exists().Should().BeFalse();
        result.Get(0).Exists().Should().BeTrue();
    }
}
