﻿using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ObjectHandlerMethodsStepDefinitions(ScenarioContext Context)
{
    [Given("I use ObjectWithHandlerMethod handlers")]
    public void GivenIUseObjectWithHandlerMethodHandlers()
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .UsePublicInstanceMethodsAsHandlers(new ObjectWithHandlerMethods(""))
            .UsePublicInstanceMethodsAsHandlers(new ObjectWithAsyncHandlerMethods("")));
    }

    [Given("I use ObjectWithHandlerMethod handlers with value {string}")]
    public void GivenIUseObjectWithHandlerMethodHandlers(string value)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .UsePublicInstanceMethodsAsHandlers(new ObjectWithHandlerMethods(value))
            .UsePublicInstanceMethodsAsHandlers(new ObjectWithAsyncHandlerMethods(value)));
    }

    [Given("I use ObjectWithHandlerMethod handlers in section {string}")]
    public void GivenIUseObjectWithHandlerMethodHandlersInSection(string value)
    {
        Context.GetEngineBuilder().SetupHandlers(h => h
            .AddSection(value, s => s
                .UsePublicInstanceMethodsAsHandlers(new ObjectWithHandlerMethods(value))
                .UsePublicInstanceMethodsAsHandlers(new ObjectWithAsyncHandlerMethods(value))));
    }
}
