using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class CommandDispatchTests
    {
        public class SyncDispatchHandler : IHandler
        {
            private readonly IOutput _output;
            private readonly IArguments _arguments;
            private readonly CommandDispatcher _dispatcher;

            public SyncDispatchHandler(IOutput output, IArguments arguments, CommandDispatcher dispatcher)
            {
                _output = output;
                _arguments = arguments;
                _dispatcher = dispatcher;
            }

            public void Execute()
            {
                _dispatcher.Execute("target", _arguments);
            }
        }

        public class AsyncDispatchHandler : IAsyncHandler
        {
            private readonly IOutput _output;
            private readonly IArguments _arguments;
            private readonly CommandDispatcher _dispatcher;

            public AsyncDispatchHandler(IOutput output, IArguments arguments, CommandDispatcher dispatcher)
            {
                _output = output;
                _arguments = arguments;
                _dispatcher = dispatcher;
            }

            public async Task ExecuteAsync(CancellationToken cancellation)
            {
                var type = _arguments.Get("type").Require().MarkConsumed().AsString();
                if (type == "string")
                    await _dispatcher.ExecuteAsync("target test");
                if (type == "verbargs")
                    await _dispatcher.ExecuteAsync("target", _arguments);
                if (type == "args")
                {
                    var args = new PrependedVerbArguments(new[] { "target" }, _arguments);
                    await _dispatcher.ExecuteAsync(args);
                }
            }
        }

        public class TargetHandler : IHandler
        {
            private readonly IOutput _output;
            private readonly IArguments _arguments;

            public TargetHandler(IOutput output, IArguments arguments)
            {
                _output = output;
                _arguments = arguments;
            }

            public void Execute()
            {
                _output.WriteLine("invoked: " + _arguments.Get(0).AsString());
            }
        }

        [Test]
        public void SyncExecute_VerbArgs()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(SyncDispatchHandler), typeof(TargetHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("sync dispatch test");
            output.Lines[0].Should().Be("invoked: test");
        }

        [Test]
        public void AsyncExecute_Command()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(AsyncDispatchHandler), typeof(TargetHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("async dispatch type=string");
            output.Lines[0].Should().Be("invoked: test");
        }

        [Test]
        public void AsyncExecute_VerbArgs()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(AsyncDispatchHandler), typeof(TargetHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("async dispatch type=verbargs test");
            output.Lines[0].Should().Be("invoked: test");
        }

        [Test]
        public void AsyncExecute_Args()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                 .SetupHandlers(h => h.UseHandlerTypes(typeof(AsyncDispatchHandler), typeof(TargetHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("async dispatch type=args test");
            output.Lines[0].Should().Be("invoked: test");
        }
    }
}
