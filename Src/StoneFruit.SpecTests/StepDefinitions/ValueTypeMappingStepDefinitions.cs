using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ValueTypeMappingStepDefinitions(ScenarioContext Context)
{
    [Given("I use the value type parsing handler methods")]
    public void GivenIUseTheValueTypeParsingHandlers()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h
            .UsePublicInstanceMethodsAsHandlers(new ValueTypeParsingInstance()));
    }
}
