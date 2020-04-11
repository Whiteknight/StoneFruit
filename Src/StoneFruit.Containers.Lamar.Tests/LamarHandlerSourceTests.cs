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
            var target = new LamarHandlerSource<object>(null, null);
            var result = target.GetByName("echo");
            result.Verb.Should().Be("echo");
        }

        [Test]
        public void GetInstance_echo()
        {
            var target = new LamarHandlerSource<object>(null, null);
            var state = new EngineState(true, new EngineEventCatalog(), new EngineSettings());
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, new InstanceEnvironmentCollection(null), state, new ConsoleOutput());
            var result = target.GetInstance(Command.Create("echo", SyntheticArguments.Empty()), dispatcher);
            result.Should().BeOfType<EchoHandler>();
        }

        [Test]
        public void GetAll_Test()
        {
            var target = new LamarHandlerSource<object>(null, null);
            var result = target.GetAll().ToList();
            result.Count.Should().BeGreaterThan(1);
        }

        [Test]
        public void Environment_Test()
        {
            var target = new LamarHandlerSource<TestEnvironment>(null, null);
            var environments = new InstanceEnvironmentCollection(new TestEnvironment("test"));
            var state = new EngineState(true, new EngineEventCatalog(), new EngineSettings());
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, environments, state, new ConsoleOutput());
            var result = target.GetInstance(Command.Create("test-environment", SyntheticArguments.Empty()), dispatcher) as EnvironmentInjectionTestHandler;

            result.Should().NotBeNull();
            result.Environment.Should().NotBeNull();
            result.Environment.Should().BeOfType<TestEnvironment>();
        }
    }
}