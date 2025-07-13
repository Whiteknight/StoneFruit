using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record DynamicDispatchStepDefinitions(ScenarioContext Context)
{
    [Given("I use the DynamicDispatch handlers")]
    public void GivenIUseTheDynamicDispatchHandlers()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .Add<DispatchTargetHandler>()
            .Add<DynamicDispatchHandler>()
            .Add<AsyncDynamicDispatchHandler>());
    }
}
