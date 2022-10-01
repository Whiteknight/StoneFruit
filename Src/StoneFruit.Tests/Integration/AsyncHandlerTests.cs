using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class AsyncHandlerTests
    {
        [Test]
        public void Test()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("test");
            output.Lines[0].Should().Be("TEST");
        }

        public class TestHandler : IAsyncHandler
        {
            private readonly IOutput _output;

            public TestHandler(IOutput output)
            {
                _output = output;
            }

            public Task ExecuteAsync(CancellationToken cancellation)
            {
                _output.WriteLine("TEST");
                return Task.CompletedTask;
            }
        }
    }
}
