namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record RunStepDefinitions(ScenarioContext Context)
{
    [When("I run with input {string}")]
    public void WhenIRunWithInput(string commandline)
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = engine.Run(commandline);
    }

    [When("I run async with input {string}")]
    public async Task WhenIRunAsyncWithInput(string commandline)
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = await engine.RunAsync(commandline);
    }

    [When("I run headless with input {string}")]
    public void WhenIRunHeadlessWithInput(string commandline)
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = engine.RunHeadless(commandline);
    }

    [When("I run headless async with input {string}")]
    public async Task WhenIRunHeadlessAsyncWithInput(string commandline)
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = await engine.RunHeadlessAsync(commandline);
    }

    [When("I run with command line arguments")]
    public void WhenIRunWithCommandLineArguments()
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = engine.RunWithCommandLineArguments();
    }

    [When("I run with command line arguments async")]
    public async Task WhenIRunWithCommandLineArgumentsAsync()
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = await engine.RunWithCommandLineArgumentsAsync();
    }

    [When("I run headless with command line arguments")]
    public void WhenIRunHeadlessWithCommandLineArguments()
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = engine.RunHeadlessWithCommandLineArgs();
    }

    [When("I run headless with command line arguments async")]
    public async Task WhenIRunHeadlessWithCommandLineArgumentsAsync()
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = await engine.RunHeadlessWithCommandLineArgsAsync();
    }

    [When("I run interactively")]
    public void WhenIRunInteractively()
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = engine.RunInteractively();
    }

    [When("I run interactively in environment {string}")]
    public void WhenIRunInteractivelyInEnvironment(string environment)
    {
        var engine = Context.GetEngine();
        Context["exitcode"] = engine.RunInteractively(environment);
    }
}
