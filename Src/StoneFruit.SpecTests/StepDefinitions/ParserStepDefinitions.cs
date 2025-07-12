namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ParserStepDefinitions(ScenarioContext Context)
{
    [Given("I use the SimplifiedArgumentParser")]
    public void GivenIUseTheSimplifiedArgumentParser()
    {
        Context.GetEngineBuilder().SetupArguments(h => h.UseSimplifiedArgumentParser());
    }

    [Given("I use the PosixStyleArgumentParser")]
    public void GivenIUseThePosixStyleArgumentParser()
    {
        Context.GetEngineBuilder().SetupArguments(h => h.UsePosixStyleArgumentParser());
    }

    [Given("I use the PowershellStyleArgumentParser")]
    public void GivenIUseThePowershellStyleArgumentParser()
    {
        Context.GetEngineBuilder().SetupArguments(h => h.UsePowershellStyleArgumentParser());
    }

    [Given("I use the WindowsCmdArgumentParser")]
    public void GivenIUseTheWindowsCmdArgumentParser()
    {
        Context.GetEngineBuilder().SetupArguments(h => h.UseWindowsCmdArgumentParser());
    }
}
