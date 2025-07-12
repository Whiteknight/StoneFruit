
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record EnvironmentStepDefinitions(ScenarioContext Context)
{
    [Given("I use the environments:")]
    public void GivenIUseTheEnvironments(DataTable dataTable)
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupEnvironments(e => e.SetEnvironments(dataTable.CreateSet<EnvironmentName>().Select(e => e.Environment).ToList()));

    }
}
