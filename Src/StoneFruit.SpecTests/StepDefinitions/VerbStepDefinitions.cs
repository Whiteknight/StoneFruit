using StoneFruit.Execution.Handlers;
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

    [Given("I use the CamelCaseVerbExtractor")]
    public void GivenIUseTheCamelCaseVerbExtractor()
    {
        var builder = Context.GetEngineBuilder().SetupHandlers(h => h
            .UseVerbExtractor(new CamelCaseVerbExtractor()));
    }

    [Given("I use the CamelCaseToSpinalCaseVerbExtractor")]
    public void GivenIUseTheCamelCaseToSpinalCaseVerbExtractor()
    {
        var builder = Context.GetEngineBuilder().SetupHandlers(h => h
            .UseVerbExtractor(new CamelCaseToSpinalCaseVerbExtractor()));
    }

    [Given("I use the ToLowerNameVerbExtractor")]
    public void GivenIUseTheToLowerNameVerbExtractor()
    {
        var builder = Context.GetEngineBuilder().SetupHandlers(h => h
            .UseVerbExtractor(new ToLowerNameVerbExtractor()));
    }
}
