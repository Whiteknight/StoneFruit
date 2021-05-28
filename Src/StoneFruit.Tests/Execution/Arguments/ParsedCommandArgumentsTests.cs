using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments
{
    public class ParsedCommandArgumentsTests
    {
        [Test]
        public void Get_Named_Consumed()
        {
            var target = new ParsedArguments(new[]
            {
                new ParsedNamedArgument("a", "1"),
                new ParsedNamedArgument("a", "2")
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
            var target = new ParsedArguments(new[]
            {
                new ParsedPositionalArgument("a"),
                new ParsedPositionalArgument("b")
            });
            target.Shift().Value.Should().Be("a");
            target.Shift().Value.Should().Be("b");
            target.Shift().Should().BeOfType<MissingArgument>();
        }

        [Test]
        public void GetAll_Named_Test()
        {
            var target = new ParsedArguments(new[]
            {
                new ParsedNamedArgument("a", "1"),
                new ParsedNamedArgument("a", "2")
            });
            var result = target.GetAllNamed("a").Cast<INamedArgument>().ToList();
            result.Count.Should().Be(2);
            result[0].Value.Should().Be("1");
            result[1].Value.Should().Be("2");
        }

        [Test]
        public void GetAll_Named_Empty()
        {
            var target = new ParsedArguments(new[]
            {
                new ParsedNamedArgument("a", "1"),
                new ParsedNamedArgument("a", "2")
            });
            var result = target.GetAllNamed("XXX").ToList();
            result.Count.Should().Be(0);
        }

        private class TestArgs1
        {
            [ArgumentIndex(0)]
            public string A { get; set; }

            public int B { get; set; }

            public bool C { get; set; }

            public bool D { get; set; }

            public long E { get; set; }
            public bool F { get; set; }
        }

        [Test]
        public void MapTo_Test()
        {
            var target = new ParsedArguments(new IParsedArgument[]
            {
                new ParsedPositionalArgument("test1"),
                new ParsedNamedArgument("b", "512"),
                new ParsedFlagArgument("c"),
                new ParsedNamedArgument("e", "1024"),
                new ParsedNamedArgument("f", "true")
            });
            var result = target.MapTo<TestArgs1>();
            result.A.Should().Be("test1");
            result.B.Should().Be(512);
            result.C.Should().BeTrue();
            result.D.Should().BeFalse();
            result.E.Should().Be(1024L);
            result.F.Should().BeTrue();
        }

        [Test]
        public void VerifyAllAreConsumed_NotConsumed()
        {
            var target = new ParsedArguments(new IParsedArgument[]
            {
                new ParsedPositionalArgument("test1"),
                new ParsedNamedArgument("b", "test2"),
                new ParsedFlagArgument("c")
            });
            Action act = () => target.VerifyAllAreConsumed();
            act.Should().Throw<StoneFruit.Execution.Arguments.ArgumentParseException>();
        }

        [Test]
        public void VerifyAllAreConsumed_NotConsumed_Accessed()
        {
            var target = new ParsedArguments(new IParsedArgument[]
            {
                new ParsedPositionalArgument("test1"),
                new ParsedNamedArgument("b", "test2"),
                new ParsedFlagArgument("c")
            });
            target.Get(0);
            target.Get("b");
            target.GetFlag("c");
            Action act = () => target.VerifyAllAreConsumed();
            act.Should().Throw<StoneFruit.Execution.Arguments.ArgumentParseException>();
        }

        [Test]
        public void VerifyAllAreConsumed_Consumed()
        {
            var target = new ParsedArguments(new IParsedArgument[]
            {
                new ParsedPositionalArgument("test1"),
                new ParsedNamedArgument("b", "test2"),
                new ParsedFlagArgument("c")
            });
            target.Consume(0);
            target.Consume("b");
            target.ConsumeFlag("c");
            Action act = () => target.VerifyAllAreConsumed();
            act.Should().NotThrow();
        }
    }
}
