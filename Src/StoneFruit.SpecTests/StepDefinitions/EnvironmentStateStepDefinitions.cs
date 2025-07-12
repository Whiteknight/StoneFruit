using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record EnvironmentStateStepDefinitions(ScenarioContext Context)
{
    [Given("I use the IncrementEnvStateCount handler")]
    public void GivenIUseTheIncrementEnvStateCountHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.UseHandlerTypes(typeof(IncrementEnvStateCount)));
        builder.Services.AddPerEnvironment((p, n) => new TestEnvironmentState(n));
    }
}
