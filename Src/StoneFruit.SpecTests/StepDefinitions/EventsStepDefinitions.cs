using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;
[Binding]
public record EventsStepDefinitions(ScenarioContext Context)
{
    [Given("I set the EngineStartInteractive script to:")]
    public void GivenISetTheEngineStartInteractiveScriptTo(DataTable dataTable)
    {
        Context.GetEngineBuilder().SetupEvents(e =>
        {
            e.EngineStartInteractive.Clear();
            foreach (var line in dataTable.CreateSet<LineOfText>())
                e.EngineStartInteractive.Add(line.Line);
        });
    }

    [Given("I clear the EngineStartInteractive script")]
    public void GivenIClearTheEngineStartInteractiveScript()
    {
        Context.GetEngineBuilder().SetupEvents(e =>
        {
            e.EngineStartInteractive.Clear();
        });
    }

    [Given("I clear the EnvironmentChanged script")]
    public void GivenIClearTheEnvironmentChangedScript()
    {
        Context.GetEngineBuilder().SetupEvents(e =>
        {
            e.EnvironmentChanged.Clear();
        });
    }

}
