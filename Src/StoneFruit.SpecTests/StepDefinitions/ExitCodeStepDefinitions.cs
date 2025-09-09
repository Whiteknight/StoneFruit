namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public sealed record ExitCodeStepDefinitions(ScenarioContext Context)
{
    [Then("The exit code is {int}")]
    public void ThenTheExitCodeIs(int expected)
    {
        var actual = Context.Get<int>("exitcode");
        actual.Should().Be(expected);
    }
}
