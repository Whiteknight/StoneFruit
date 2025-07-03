using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;

namespace StoneFruit.Tests.Integration;

public class FunctionDelegateHandlersTests
{
    [Test]
    public void Test_Sync()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h.Add("test", (c, d) => d.Output.WriteLine("TEST")))
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        );
        engine.RunHeadless("test");
        output.Lines[0].Should().Be("TEST");
    }

    [Test]
    public void Test_Asynchronous()
    {
        Task handle(IArguments arguments, CommandDispatcher d)
        {
            d.Output.WriteLine("TEST");
            return Task.CompletedTask;
        }

        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h.AddAsync("test", handle))
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        );
        engine.RunHeadless("test");
        output.Lines[0].Should().Be("TEST");
    }
}
