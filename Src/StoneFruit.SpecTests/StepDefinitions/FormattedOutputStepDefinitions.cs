using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record FormattedOutputStepDefinitions(ScenarioContext Context)
{
    [Given("I use the FormatHandler")]
    public void GivenIUseTheFormatHandler()
    {
        Context.GetEngineBuilder().SetupHandlers(b => b
            .Add<FormatHandler>());
    }
}
