using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution
{
    public class CompleteCommandGrammarTests
    {
        private IParser<char, CompleteCommand> GetSimplifiedParser() 
            => CompleteCommandGrammar.GetParser(SimplifiedArgumentGrammar.GetParser());

        [Test]
        public void Simplified_Positional()
        {
            var parser = GetSimplifiedParser();
            var result = parser.Parse("my-verb testa testb testc").Value;
            result.Verb.Should().Be("my-verb");
            result.Arguments.Get(0).Value.Should().Be("testa");
            result.Arguments.Get(1).Value.Should().Be("testb");
            result.Arguments.Get(2).Value.Should().Be("testc");
        }

        [Test]
        public void Simplified_Named()
        {
            var parser = GetSimplifiedParser();
            var result = parser.Parse("my-verb name1=value1 name2=value2").Value;
            result.Verb.Should().Be("my-verb");
            var named1 = result.Arguments.Get("name1");
            named1.Value.Should().Be("value1");
            var named2 = result.Arguments.Get("name2");
            named2.Value.Should().Be("value2");
        }

        [Test]
        public void Simplified_Flags()
        {
            var parser = GetSimplifiedParser();
            var result = parser.Parse("my-verb -flaga -flagb").Value;
            result.Verb.Should().Be("my-verb");
            result.Arguments.HasFlag("flaga").Should().BeTrue();
            result.Arguments.HasFlag("flagb").Should().BeTrue();
            result.Arguments.HasFlag("flagc").Should().BeFalse();
        }
    }
}
