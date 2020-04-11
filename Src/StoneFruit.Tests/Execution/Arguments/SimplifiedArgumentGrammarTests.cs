using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments
{
    public class SimplifiedArgumentGrammarTests
    {
        private static IParser<char, IParsedArgument> GetParser() 
            => SimplifiedArgumentGrammar.GetParser();

        [Test]
        public void Empty_Tests()
        {
            var parser = GetParser();
            var result = parser.Parse("");
            result.Success.Should().BeFalse();
        }

        [Test]
        public void Positionals_Tests()
        {
            var parser = GetParser();
            var result = parser.Parse("testa").Value as ParsedPositionalArgument;
            result.Value.Should().Be("testa");
        }

        [Test]
        public void Positionals_SingleQuotedString()
        {
            var parser = GetParser();
            var result = parser.Parse("'test a'").Value as ParsedPositionalArgument;
            result.Value.Should().Be("test a");
        }

        [Test]
        public void Positionals_DoubleQuotedString()
        {
            var parser = GetParser();
            var result = parser.Parse("\"test a\"").Value as ParsedPositionalArgument;
            result.Value.Should().Be("test a");
        }

        [Test]
        public void Positionals_LeadingWhitespace()
        {
            var parser = GetParser();
            var result = parser.Parse("   testa").Value as ParsedPositionalArgument;
            result.Value.Should().Be("testa");
        }

        [Test]
        public void Named_Tests()
        {
            var parser = GetParser();
            var result = parser.Parse("name1=value1").Value as ParsedNamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Named_LeadingWhitespace()
        {
            var parser = GetParser();
            var result = parser.Parse("    name1=value1").Value as ParsedNamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Named_SingleQuotedValue()
        {
            var parser = GetParser();
            var result = parser.Parse("name1='value1'").Value as ParsedNamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Named_DoubleQuotedValue()
        {
            var parser = GetParser();
            var result = parser.Parse("name1=\"value1\"").Value as ParsedNamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Flags_Tests()
        {
            var parser = GetParser();
            var result = parser.Parse("-testa").Value as ParsedFlagArgument;
            result.Name.Should().Be("testa");
        }
    }
}
