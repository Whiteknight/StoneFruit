using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ObjectHandlerMethodsStepDefinitions(ScenarioContext Context)
{
    [Given("I use ObjectWithHandlerMethod handlers")]
    public void GivenIUseObjectWithHandlerMethodHandlers()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .UsePublicInstanceMethodsAsHandlers(new ObjectWithHandlerMethods())
            .UsePublicInstanceMethodsAsHandlers(new ObjectWithAsyncHandlerMethods()));
    }
}
