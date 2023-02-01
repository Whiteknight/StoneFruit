using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration;

public class ServiceCollectionTests
{
    [Test]
    public void SetupEngine_Test()
    {
        var output = new TestOutput();
        var services = new ServiceCollection();
        services.SetupEngine(b => b
            .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        );
        var engine = services.BuildServiceProvider().GetRequiredService<Engine>();

        engine.RunHeadless("echo 'test'");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("test");
    }
}
