using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record HandlerInstanceStepDefinitions(ScenarioContext Context)
{
    [Given("I use a predefined handler instance")]
    public void GivenIUseAPredefinedHandlerInstance()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .Add("predefined", new PredefinedInstanceHandler())
            .Add("predefinedasync", new PredefinedInstanceAsyncHandler()));
    }
}
