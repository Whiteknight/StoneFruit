using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class SanityTests
    {
        [Test]
        public void RunHeadless_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("echo 'test'");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public async Task RunHeadlessASync_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            await engine.RunHeadlessAsync("echo 'test'");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public void RunWithCommandLineArguments_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetCommandLine("echo 'test'")
            );
            engine.RunWithCommandLineArguments();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public async Task RunWithCommandLineArgumentsAsync_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetCommandLine("echo 'test'")
            );
            await engine.RunWithCommandLineArgumentsAsync();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public void RunHeadlessWithCommandLineArguments_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetCommandLine("echo 'test'")
            );
            engine.RunHeadlessWithCommandLineArgs();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public async Task RunHeadlessWithCommandLineArgumentsAsync_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetCommandLine("echo 'test'")
            );
            await engine.RunHeadlessWithCommandLineArgsAsync();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public void Run_Argument_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.Run("echo 'test'");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }

        [Test]
        public async Task RunAsync_Argument_Echo()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            await engine.RunAsync("echo 'test'");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }
    }
}
