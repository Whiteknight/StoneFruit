using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ThrowExceptionHandlerStepDefinitions(ScenarioContext Context)
{
    [Given("I use the ThrowExceptionHandler")]
    public void GivenIUseTheThrowExceptionHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.UseHandlerTypes(typeof(ThrowExceptionHandler)));
    }
}
