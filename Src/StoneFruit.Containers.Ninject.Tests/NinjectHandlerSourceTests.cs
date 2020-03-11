using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Output;
using StoneFruit.Handlers;

namespace StoneFruit.Containers.Ninject.Tests
{
    public class NinjectHandlerSourceTests
    {
        [Test]
        public void GetByName_echo()
        {
            var target = new NinjectHandlerSource();
            var result = target.GetByName("echo");
            result.Verb.Should().Be("echo");
        }

        [Test]
        public void GetInstance_echo()
        {
            var target = new NinjectHandlerSource();
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, new InstanceEnvironmentCollection(null), new EngineState(true, null), new ConsoleOutput());
            var result = target.GetInstance(new Command("echo", CommandArguments.Empty()), dispatcher);
            result.Should().BeOfType<EchoHandler>();
        }

        [Test]
        public void GetAll_Test()
        {
            var target = new NinjectHandlerSource();
            var dispatcher = new CommandDispatcher(CommandParser.GetDefault(), target, new InstanceEnvironmentCollection(null), new EngineState(true, null), new ConsoleOutput());
            var result = target.GetAll().ToList();
            result.Count.Should().BeGreaterThan(1);
        }
    }
}