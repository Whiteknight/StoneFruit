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

    [Given("I set the EngineStartHeadless script to:")]
    public void GivenISetTheEngineStartHeadlessScriptTo(DataTable dataTable)
    {
        Context.GetEngineBuilder().SetupEvents(e =>
        {
            e.EngineStartHeadless.Clear();
            foreach (var line in dataTable.CreateSet<LineOfText>())
                e.EngineStartHeadless.Add(line.Line);
        });
    }

    [Given("I set the EngineStopHeadless script to:")]
    public void GivenISetTheEngineStopHeadlessScriptTo(DataTable dataTable)
    {
        Context.GetEngineBuilder().SetupEvents(e =>
        {
            e.EngineStopHeadless.Clear();
            foreach (var line in dataTable.CreateSet<LineOfText>())
                e.EngineStopHeadless.Add(line.Line);
        });
    }

    [Given("I set the HeadlessHelp script to:")]
    public void GivenISetTheHeadlessHelpScriptTo(DataTable dataTable)
    {
        Context.GetEngineBuilder().SetupEvents(e =>
        {
            e.HeadlessHelp.Clear();
            foreach (var line in dataTable.CreateSet<LineOfText>())
                e.HeadlessHelp.Add(line.Line);
        });
    }
}
