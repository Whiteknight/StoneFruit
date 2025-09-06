using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record LoggingStepDefinitions(ScenarioContext Context)
{
    [Given("I use the LoggingHandler")]
    public void GivenIUseTheLoggingHandler()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .Add<LoggingHandler>());
    }
}
