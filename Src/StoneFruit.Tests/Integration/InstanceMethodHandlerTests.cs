using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration;

public class InstanceMethodHandlerTests
{

    [Test]
    public void InstanceMethods_Help()
    {
        var output = new TestOutput("help");
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h.UsePublicInstanceMethodsAsHandlers(new MyObject1()))
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        );
        engine.RunInteractively();
        output.Lines.Should().Contain("test a");
        output.Lines.Should().Contain("test b");
        output.Lines.Should().Contain("test c");
    }
}
