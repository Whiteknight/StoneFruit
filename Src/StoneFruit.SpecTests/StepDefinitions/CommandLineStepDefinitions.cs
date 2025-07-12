namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record CommandLineStepDefinitions(ScenarioContext Context)
{
    [Given("I set the command line text to {string}")]
    public void GivenISetTheCommandLineTextTo(string commandline)
    {
        Context.GetEngineBuilder().SetCommandLine(commandline);
    }
}
