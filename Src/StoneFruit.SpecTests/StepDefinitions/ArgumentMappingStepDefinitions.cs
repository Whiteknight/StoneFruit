using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ArgumentMappingStepDefinitions(ScenarioContext Context)
{
    [Given("I use the ArgumentMapping handler")]
    public void GivenIUseTheArgumentMappingHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.UseHandlerTypes(typeof(ArgumentMappingHandler)));
    }
}
