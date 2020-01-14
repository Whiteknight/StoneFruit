using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

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
            var args = target.ParseArguments("x y z");
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
            var args = target.ParseArguments("-x -y");
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
            var args = target.ParseArguments("x=a y=b z=c");
            var result = args.MapTo<TestArgsNamed>();
            result.X.Should().Be("a");
            result.Y.Should().Be("b");
            result.Z.Should().Be("c");
        }
    }
}
