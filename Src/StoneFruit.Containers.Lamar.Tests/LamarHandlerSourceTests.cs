using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Output;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Containers.Lamar.Tests
{
    public class LamarHandlerSourceTests
    {
        [Test]
        public void GetByName_echo()
        {
            var target = new LamarHandlerSource<object>();
            var result = target.GetByName("echo");
            result.Verb.Should().Be("echo");
        }

        [Test]
        public void GetInstance_echo()
        {
            var target = new LamarHandlerSource<object>();
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, new InstanceEnvironmentCollection(null), new EngineState(true, null), new ConsoleOutput());
            var result = target.GetInstance(new Command("echo", CommandArguments.Empty()), dispatcher);
            result.Should().BeOfType<EchoHandler>();
        }

        [Test]
        public void GetAll_Test()
        {
            var target = new LamarHandlerSource<object>();
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, new InstanceEnvironmentCollection(null), new EngineState(true, null), new ConsoleOutput());
            var result = target.GetAll().ToList();
            result.Count.Should().BeGreaterThan(1);
        }

        [Test]
        public void Environment_Test()
        {
            var target = new LamarHandlerSource<TestEnvironment>();
            var environments = new InstanceEnvironmentCollection(new TestEnvironment("test"));
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, environments, new EngineState(true, null), new ConsoleOutput());
            var result = target.GetInstance(new Command("test-environment", CommandArguments.Empty()), dispatcher) as EnvironmentInjectionTestHandler;

            result.Should().NotBeNull();
            result.Environment.Should().NotBeNull();
            result.Environment.Should().BeOfType<TestEnvironment>();
        }
    }
}