using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments
{
    public class PosixStyleArgumentGrammarTests
    {
        private static CommandArguments Parse(string args)
        {
            var parser = PosixStyleArgumentGrammar.GetParser();
            var arguments = parser.List().Parse(args).Value.ToList();
            return new CommandArguments(arguments);
        }

        [Test]
        public void Positional_Test()
        {
            var result = Parse("a b c");
            result.Shift().Value.Should().Be("a");
            result.Shift().Value.Should().Be("b");
            result.Shift().Value.Should().Be("c");
            result.Shift().Value.Should().Be(null);
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
        public void Flags_ShortImpliedNamed()
        {
            var result = Parse("-a test");
            result.Get("a").Value.Should().Be("test");
            result.HasFlag("a").Should().BeTrue();
            result.Get(0).Value.Should().Be("test");
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
        public void Flags_LongImpliedNamed()
        {
            var result = Parse("--abc test");
            result.Get("abc").Value.Should().Be("test");
            result.HasFlag("abc").Should().BeTrue();
            result.Get(0).Value.Should().Be("test");
        }
    }

    public class PowershellStyleArgumentGrammarTests
    {
        private static CommandArguments Parse(string args)
        {
            var parser = PowershellStyleArgumentGrammar.GetParser();
            var arguments = parser.List().Parse(args).Value.ToList();
            return new CommandArguments(arguments);
        }

        [Test]
        public void Positional_Test()
        {
            var result = Parse("a b c");
            result.Shift().Value.Should().Be("a");
            result.Shift().Value.Should().Be("b");
            result.Shift().Value.Should().Be("c");
            result.Shift().Value.Should().Be(null);
        }

        [Test]
        public void Flag_Test()
        {
            var result = Parse("-a");
            result.HasFlag("a").Should().BeTrue();
            result.HasFlag("x").Should().BeFalse();
        }

        [Test]
        public void FlagOrNamed_Test()
        {
            //  This production is ambiguous, so we return a flag, a named and a positional
            var result = Parse("-abc xyz");
            result.HasFlag("abc").Should().BeTrue();
            result.Get("abc").Value.Should().Be("xyz");
            result.Get(0).Value.Should().Be("xyz");
        }
    }
}
