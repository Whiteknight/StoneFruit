using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ValueTypeMappingStepDefinitions(ScenarioContext Context)
{
    [Given("I use the value type parsing handlers")]
    public void GivenIUseTheValueTypeParsingHandlers()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupArguments(a => a
            .UseTypeParser(s => new FileInfo(s)));
        builder.SetupHandlers(h => h
            .UsePublicInstanceMethodsAsHandlers(new ValueTypeParsingInstance()));
    }
}
