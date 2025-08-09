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

    [Given("I have a script {string} in section {string} with lines:")]
    public void GivenIHaveAScriptInSectionWithLines(string name, string section, DataTable dataTable)
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h
            .AddSection(section, s => s
                .AddScript(
                    name,
                    dataTable.CreateSet<LineOfText>().Select(l => l.Line).ToArray())));
    }

    public readonly record struct ScriptInfo(string Line, string Description, string Usage, string Group);

    [Given("I have a script {string} with:")]
    public void GivenIHaveAScriptWith(string name, DataTable dataTable)
    {
        var info = dataTable.CreateInstance<ScriptInfo>();
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h
                .AddScript(name, [info.Line], info.Description, info.Usage, info.Group));
    }
}
