using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments
{
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