using System;
using StoneFruit.Execution;

namespace StoneFruit.Tests.Execution.HandlerSources;

public class TypeListConstructSourceTests
{
    private class TestCommandHandler : IHandler
    {
        public IOutput Output { get; }
        public EngineState State { get; }
        public IArguments Args { get; }
        public CommandDispatcher Dispatcher { get; }

        public TestCommandHandler(IOutput output, EngineState state, IArguments args, CommandDispatcher dispatcher)
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

    //[Test]
    //public void GetInstance_Test()
    //{
    //    var target = new TypeListConstructSource(new [] { typeof(TestCommandHandler) }, null);
    //    var args = new ParsedArguments();
    //    var command = Command.Create("test", args);
    //    var parser = CommandParser.GetDefault();
    //    var verbSource = new NamedInstanceHandlerSource();
    //    var environments = new InstanceEnvironmentCollection(null);
    //    var state = new EngineState(true, new EngineEventCatalog(), new EngineSettings());
    //    var output = new ConsoleOutput();
    //    var dispatcher = new CommandDispatcher(parser, verbSource, environments, state, output);
    //    var result = target.GetInstance<TestCommandHandler>(command, dispatcher) as TestCommandHandler;
    //    result.Args.Should().BeSameAs(args);
    //    result.Dispatcher.Should().BeSameAs(dispatcher);
    //    result.Output.Should().BeSameAs(output);
    //    result.State.Should().BeSameAs(state);
    //}
}
