using System.Text.RegularExpressions;
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record HelpStepDefinitions(ScenarioContext Context)
{
    [Then("The help output should contain these verbs:")]
    public void ThenTheHelpOutputShouldContainGroupWithTheseVerbs(DataTable dataTable)
    {
        var expectedVerbs = dataTable.CreateSet<ExpectedVerb>().ToList();
        var output = Context.GetIo();
        var groups = new Dictionary<string, List<string>>()
        {
            { "", [] }
        };
        var currentGroup = "";
        foreach (var line in output.Lines)
        {
            var groupMatch = Regex.Match(line, "== (.*) ==");
            if (groupMatch.Success)
            {
                currentGroup = groupMatch.Groups[1].Value;
                groups.Add(currentGroup, []);
                continue;
            }

            groups[currentGroup].Add(line.Trim());
        }

        foreach (var expected in expectedVerbs)
        {
            groups.Should()
                .ContainKey(expected.Group)
                .WhoseValue.Should().Contain(s => s.StartsWith(expected.Verb));
        }
    }
}
