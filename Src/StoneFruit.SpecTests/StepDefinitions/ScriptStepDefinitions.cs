using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ScriptStepDefinitions(ScenarioContext Context)
{
    [Given("I have a script {string} with lines:")]
    public void GivenIHaveAScriptWithLines(string name, DataTable dataTable)
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h
            .AddScript(
                name,
                dataTable.CreateSet<LineOfText>().Select(l => l.Line).ToArray()));
    }
}
