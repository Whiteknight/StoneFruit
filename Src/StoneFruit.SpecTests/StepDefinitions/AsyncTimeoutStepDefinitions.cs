using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record AsyncTimeoutStepDefinitions(ScenarioContext Context)
{
    [Given("I use the AsyncTimeout handler")]
    public void GivenIUseTheAsyncTimeoutHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.UseHandlerTypes(typeof(AsyncTimeoutHandler)));
    }

}
