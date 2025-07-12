
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record OutputStepDefinitions(ScenarioContext Context)
{
    [Then("The output should contain:")]
    public void ThenTheOutputShouldContain(DataTable dataTable)
    {
        var lines = dataTable.CreateSet<LineOfText>().ToList();
        var output = Context.GetOutput();
        output.Lines.Length.Should().Be(lines.Count, $"Output contains:\n{string.Join("\n", output.Lines)}");
        for (int i = 0; i < lines.Count; i++)
            output.Lines[i].Should().Be(lines[i].Line);
    }

    [Then("The output should contain at least:")]
    public void ThenTheOutputShouldContainAtLeast(DataTable dataTable)
    {
        var lines = dataTable.CreateSet<LineOfText>().ToList();
        var output = Context.GetOutput();
        output.Lines.Length.Should().BeGreaterThanOrEqualTo(lines.Count, $"Output contains:\n{string.Join("\n", output.Lines)}");
        for (int i = 0; i < lines.Count; i++)
            output.Lines[i].Should().Be(lines[i].Line);
    }

    [Given("I input the following lines:")]
    public void GivenIInputTheFollowingLines(DataTable dataTable)
    {
        var output = Context.GetOutput();
        foreach (var line in dataTable.CreateSet<LineOfText>())
            output.EnqueueInputLine(line.Line);
    }
}
