using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record VerbStepDefinitions(ScenarioContext Context)
{
    [Given("I use the MultiWordVerb handler")]
    public void GivenIUseTheMultiWordVerbHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h.UseHandlerTypes(typeof(MultiWordVerbHandler)));
    }
}
