using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record AsyncHandlerStepDefinitions(ScenarioContext Context)
{
    [Given("I Use the SimpleAsync handler")]
    public void GivenIUseTheSimpleAsyncHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.Add<SimpleAsyncHandler>());
    }
}
