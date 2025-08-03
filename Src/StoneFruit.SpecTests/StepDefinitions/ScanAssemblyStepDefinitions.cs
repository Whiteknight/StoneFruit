using System.Reflection;
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ScanAssemblyStepDefinitions(ScenarioContext Context)
{
    [Given("I scan the assembly")]
    public void GivenIScanTheAssembly()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .ScanAssemblyForHandlers(Assembly.GetExecutingAssembly()));
    }

    [Given("I scan the current assembly")]
    public void GivenIScanTheCurrentAssembly()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .ScanHandlersFromCurrentAssembly());
    }

    [Given("I scan the assembly containing the scanned handler type")]
    public void GivenIScanTheAssemblyContainingTheScannedHandlerType()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .ScanHandlersFromAssemblyContaining<ScannedHandler>());
    }

    [Given("I scan the assembly with prefix {string}")]
    public void GivenIScanTheAssemblyWithPrefix(string prefix)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .ScanAssemblyForHandlers(Assembly.GetExecutingAssembly(), prefix));
    }

    [Given("I scan the current assembly with prefix {string}")]
    public void GivenIScanTheCurrentAssemblyWithPrefix(string prefix)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .ScanHandlersFromCurrentAssembly(prefix));
    }

    [Given("I scan the assembly containing the scanned handler type with prefix {string}")]
    public void GivenIScanTheAssemblyContainingTheScannedHandlerTypeWithPrefix(string prefix)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .ScanHandlersFromAssemblyContaining<ScannedHandler>(prefix));
    }
}
