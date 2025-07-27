﻿using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests.StepDefinitions;

[Binding]
public record ArgumentMappingStepDefinitions(ScenarioContext Context)
{
    [Given("I use the ArgumentMapping handlers")]
    public void GivenIUseTheArgumentMappingHandler()
    {
        var builder = Context.GetEngineBuilder();
        builder.SetupHandlers(h => h
            .Add<SimpleMapToObjectHandler>()
            .Add<ComplexMapToObjectHandler>());
    }
}
