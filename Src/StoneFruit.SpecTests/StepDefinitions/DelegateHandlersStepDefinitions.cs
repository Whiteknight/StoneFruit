namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record DelegateHandlersStepDefinitions(ScenarioContext Context)
{
    [Given("I register a simple delegate handler {string}")]
    public void GivenIRegisterASimpleDelegateHandler(string name)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .Add(name, (args, c) => c.Output.WriteLine($"Invoked: {name}")));
    }

    [Given("I register a simple async delegate handler {string}")]
    public void GivenIRegisterASimpleAsyncDelegateHandler(string name)
    {
        Task handle(IArguments args, HandlerContext context, CancellationToken token)
        {
            context.Output.WriteLine($"Invoked: {name}");
            return Task.CompletedTask;
        }

        Context.GetEngineBuilder().SetupHandlers(h => h
            .Add(name, handle));
    }

    [Given("I register a simple delegate handler {string} in section {string}")]
    public void GivenIRegisterASimpleDelegateHandlerInSection(string name, string section)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .AddSection(section, s => s
                .Add(name, (args, c) => c.Output.WriteLine($"Invoked: {name}"))));
    }

    [Given("I register a simple async delegate handler {string} in section {string}")]
    public void GivenIRegisterASimpleAsyncDelegateHandlerInSection(string name, string section)
    {
        Task handle(IArguments args, HandlerContext context, CancellationToken token)
        {
            context.Output.WriteLine($"Invoked: {name}");
            return Task.CompletedTask;
        }

        Context.GetEngineBuilder().SetupHandlers(h => h
            .AddSection(section, s => s
                .Add(name, handle)));
    }
}
