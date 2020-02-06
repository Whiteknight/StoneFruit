using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments
{
    public class CommandArgumentsTests
    {
        [Test]
        public void Get_Named_Consumed()
        {
            var target = new CommandArguments(new[]
            {
                new NamedArgument("a", "1"),
                new NamedArgument("a", "2")
            });
            var result = target.Get("a");
            result.Value.Should().Be("1");
            result.MarkConsumed();

            result = target.Get("a");
            result.Value.Should().Be("2");
            result.MarkConsumed();

            result = target.Get("a");
            result.Should().BeOfType<MissingArgument>();
        }

        [Test]
        public void Shift_Test()
        {
            var target = new CommandArguments(new[]
            {
                new PositionalArgument("a"),
                new PositionalArgument("b")
            });
            target.Shift().Value.Should().Be("a");
            target.Shift().Value.Should().Be("b");
            target.Shift().Should().BeOfType<MissingArgument>();
        }

        [Test]
        public void GetAll_Named_Test()
        {
            var target = new CommandArguments(new[]
            {
                new NamedArgument("a", "1"),
                new NamedArgument("a", "2")
            });
            var result = target.GetAll("a").ToList();
            result.Count.Should().Be(2);
            result[0].Value.Should().Be("1");
            result[1].Value.Should().Be("2");
        }

        [Test]
        public void GetAll_Named_Empty()
        {
            var target = new CommandArguments(new[]
            {
                new NamedArgument("a", "1"),
                new NamedArgument("a", "2")
            });
            var result = target.GetAll("XXX").ToList();
            result.Count.Should().Be(0);
        }

        private class TestArgs1
        {
            [ArgumentIndex(0)]
            public string A { get; set; }

            public string B { get; set; }

            public bool C { get; set; }

            public bool D { get; set; }
        }

        [Test]
        public void MapTo_Test()
        {
            var target = new CommandArguments(new IArgument[]
            {
                new PositionalArgument("test1"),
                new NamedArgument("b", "test2"),
                new FlagArgument("c")
            });
            var result = target.MapTo<TestArgs1>();
            result.A.Should().Be("test1");
            result.B.Should().Be("test2");
            result.C.Should().BeTrue();
            result.D.Should().BeFalse();
        }
    }
}
