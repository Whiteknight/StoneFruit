using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Tests.Execution
{
    public class CommandParserTests
    {
        public class TestArgsPositional
        {
            [ArgumentIndex(0)]
            public string Arg1 { get; set; }

            [ArgumentIndex(1)]
            public string Arg2 { get; set; }

            [ArgumentIndex(2)]
            public string Arg3 { get; set; }
        }

        [Test]
        public void ParseArguments_ToObject_Positional()
        {
            var target = CommandParser.GetDefault();
            var args = target.ParseCommand("x y z");
            var result = args.MapTo<TestArgsPositional>();
            result.Arg1.Should().Be("x");
            result.Arg2.Should().Be("y");
            result.Arg3.Should().Be("z");
        }

        public class TestArgsFlags
        {
            public bool X { get; set; }
            public bool Y { get; set; }
            public bool Z { get; set; }
        }

        [Test]
        public void ParseArguments_ToObject_Flags()
        {
            var target = CommandParser.GetDefault();
            var args = target.ParseCommand("-x -y");
            var result = args.MapTo<TestArgsFlags>();
            result.X.Should().BeTrue();
            result.Y.Should().BeTrue();
            result.Z.Should().BeFalse();
        }

        public class TestArgsNamed
        {
            public string X { get; set; }
            public string Y { get; set; }
            public string Z { get; set; }
        }

        [Test]
        public void ParseArguments_ToObject_Named()
        {
            var target = CommandParser.GetDefault();
            var args = target.ParseCommand("x=a y=b z=c");
            var result = args.MapTo<TestArgsNamed>();
            result.X.Should().Be("a");
            result.Y.Should().Be("b");
            result.Z.Should().Be("c");
        }

        private CommandParser GetSimplifiedParser()
        {
            var argParser = SimplifiedArgumentGrammar.GetParser();
            var scriptParser = ScriptFormatGrammar.GetParser();

            return new CommandParser(argParser, scriptParser);
        }

        [Test]
        public void ParseCommand_Simplified_Positional()
        {
            var parser = GetSimplifiedParser();
            var result = parser.ParseCommand("my-verb testa testb testc");
            result.Get(0).Value.Should().Be("my-verb");
            result.Get(1).Value.Should().Be("testa");
            result.Get(2).Value.Should().Be("testb");
            result.Get(3).Value.Should().Be("testc");
        }

        [Test]
        public void ParseCommand_Simplified_Named()
        {
            var parser = GetSimplifiedParser();
            var result = parser.ParseCommand("my-verb name1=value1 name2=value2");
            result.Get(0).Value.Should().Be("my-verb");
            var named1 = result.Get("name1");
            named1.Value.Should().Be("value1");
            var named2 = result.Get("name2");
            named2.Value.Should().Be("value2");
        }

        [Test]
        public void ParseCommand_Simplified_Flags()
        {
            var parser = GetSimplifiedParser();
            var result = parser.ParseCommand("my-verb -flaga -flagb");
            result.Get(0).Value.Should().Be("my-verb");
            result.Raw.Should().Be("my-verb -flaga -flagb");
            result.HasFlag("flaga").Should().BeTrue();
            result.HasFlag("flagb").Should().BeTrue();
            result.HasFlag("flagc").Should().BeFalse();
        }

        [Test]
        public void ParseCommand_Simplified_Raw()
        {
            var parser = GetSimplifiedParser();
            var result = parser.ParseCommand("my-verb positional named=value -flag");

            result.Raw.Should().Be("my-verb positional named=value -flag");
        }
    }
}
