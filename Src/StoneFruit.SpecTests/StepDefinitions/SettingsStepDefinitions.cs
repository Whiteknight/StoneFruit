namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record SettingsStepDefinitions(ScenarioContext Context)
{
    [Given("I set the MaxInputlessCommands setting to {int}")]
    public void GivenISetTheMaxInputlessCommandsSettingTo(int number)
    {
        Context.GetEngineBuilder().SetupSettings(s => s.MaxInputlessCommands = number);
    }

    [Given("I set the MaxExecuteTimeout setting to {string}")]
    public void GivenISetTheMaxExecuteTimeoutSettingTo(string ts)
    {
        Context.GetEngineBuilder().SetupSettings(s => s.MaxExecuteTimeout = TimeSpan.Parse(ts));
    }
}
