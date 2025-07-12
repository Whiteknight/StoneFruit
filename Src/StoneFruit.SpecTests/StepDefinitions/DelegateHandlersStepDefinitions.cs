using StoneFruit.Execution;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record DelegateHandlersStepDefinitions(ScenarioContext Context)
{
    [Given("I register a simple delegate handler {string}")]
    public void GivenIRegisterASimpleDelegateHandler(string name)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .Add(name, (c, d) => d.Output.WriteLine($"Invoked: {name}")));
    }

    [Given("I register a simple async delegate handler {string}")]
    public void GivenIRegisterASimpleAsyncDelegateHandler(string name)
    {
        Task handle(IArguments arguments, CommandDispatcher d)
        {
            d.Output.WriteLine($"Invoked: {name}");
            return Task.CompletedTask;
        }

        Context.GetEngineBuilder().SetupHandlers(h => h
            .AddAsync(name, handle));
    }
}
