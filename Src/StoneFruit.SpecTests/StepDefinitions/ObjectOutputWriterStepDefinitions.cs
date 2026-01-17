namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ObjectOutputWriterStepDefinitions(ScenarioContext Context)
{
    [Given("I use JsonObjectWriter for output")]
    public void GivenIUseJsonObjectWriterForOutput()
    {
        Context.GetEngineBuilder()
            .SetupIo(io => io
                .UseJsonObjectWriter());
    }
}

