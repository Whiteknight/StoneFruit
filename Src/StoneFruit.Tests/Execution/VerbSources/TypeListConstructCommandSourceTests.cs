using System;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.HandlerSources;
using StoneFruit.Execution.Output;

namespace StoneFruit.Tests.Execution.VerbSources
{
    public class TypeListConstructCommandSourceTests
    {
        private class TestCommandHandler : ICommandHandler
        {
            public ITerminalOutput Output { get; }
            public EngineState State { get; }
            public CommandArguments Args { get; }
            public CommandDispatcher Dispatcher { get; }

            public TestCommandHandler(ITerminalOutput output, EngineState state, CommandArguments args, CommandDispatcher dispatcher)
            {
                Output = output;
                State = state;
                Args = args;
                Dispatcher = dispatcher;
            }

            public void Execute()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void GetInstance_Test()
        {
            var target = new TypeListConstructCommandSource(new [] { typeof(TestCommandHandler) });
            var args = new CommandArguments();
            var command = new CompleteCommand("test", args);
            var parser = CommandParser.GetDefault();
            var verbSource = new NamedInstanceCommandSource();
            var environments = new InstanceEnvironmentCollection(null);
            var state = new EngineState(true, null);
            var output = new ConsoleTerminalOutput();
            var dispatcher = new CommandDispatcher(parser, verbSource, environments, state, output);
            var result = target.GetInstance<TestCommandHandler>(command, dispatcher) as TestCommandHandler;
            result.Args.Should().BeSameAs(args);
            result.Dispatcher.Should().BeSameAs(dispatcher);
            result.Output.Should().BeSameAs(output);
            result.State.Should().BeSameAs(state);
        }
    }
}
