using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments
{
    public class SimplifiedArgumentGrammarTests
    {
        private static IParser<char, IEnumerable<IArgument>> GetParser() 
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
            var result = parser.Parse("testa").Value.FirstOrDefault();
            result.Value.Should().Be("testa");
        }

        [Test]
        public void Positionals_SingleQuotedString()
        {
            var parser = GetParser();
            var result = parser.Parse("'test a'").Value.FirstOrDefault();
            result.Value.Should().Be("test a");
        }

        [Test]
        public void Positionals_DoubleQuotedString()
        {
            var parser = GetParser();
            var result = parser.Parse("\"test a\"").Value.FirstOrDefault();
            result.Value.Should().Be("test a");
        }

        [Test]
        public void Positionals_LeadingWhitespace()
        {
            var parser = GetParser();
            var result = parser.Parse("   testa").Value.FirstOrDefault();
            result.Value.Should().Be("testa");
        }

        [Test]
        public void Named_Tests()
        {
            var parser = GetParser();
            var result = parser.Parse("name1=value1").Value.FirstOrDefault() as NamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Named_LeadingWhitespace()
        {
            var parser = GetParser();
            var result = parser.Parse("    name1=value1").Value.FirstOrDefault() as NamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Named_SingleQuotedValue()
        {
            var parser = GetParser();
            var result = parser.Parse("name1='value1'").Value.FirstOrDefault() as NamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Named_DoubleQuotedValue()
        {
            var parser = GetParser();
            var result = parser.Parse("name1=\"value1\"").Value.FirstOrDefault() as NamedArgument;
            result.Name.Should().Be("name1");
            result.Value.Should().Be("value1");
        }

        [Test]
        public void Flags_Tests()
        {
            var parser = GetParser();
            var result = parser.Parse("-testa").Value.FirstOrDefault() as FlagArgument;
            result.Name.Should().Be("testa");
            result.Value.Should().Be("");
        }
    }
}
