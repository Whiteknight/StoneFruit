using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ArgumentMappingStepDefinitions(ScenarioContext Context)
{
    [Given("I use the PositionalMapping handler")]
    public void GivenIUseThePositionalMappingHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.UseHandlerTypes(typeof(PositionalMappingHandler)));
    }
}
