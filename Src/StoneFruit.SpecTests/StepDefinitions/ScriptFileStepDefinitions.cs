namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ScriptFileStepDefinitions(ScenarioContext Context)
{
    [Given("I use script file {string} by file name")]
    public void GivenIUseScriptFileByFileName(string fileName)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .AddScriptFile(fileName));
    }

    [Given("I use script file {string} by stream")]
    public void GivenIUseScriptFileByStream(string fileName)
    {
        var stream = File.OpenRead(fileName);
        Context.AddDisposable(stream);
        Context.GetEngineBuilder().SetupHandlers(h => h
            .AddScript(stream, Path.GetFileNameWithoutExtension(fileName)));
    }
}
