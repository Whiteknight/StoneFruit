
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests;

public static class ScenarioContextEngineExtensions
{
    public static EngineBuilder GetEngineBuilder(this ScenarioContext context)
    {
        if (context.TryGetValue<EngineBuilder>("engineBuilder", out var builder))
            return builder;
        var output = context.GetOutput();
        builder = EngineBuilder.Create();
        builder.SetupOutput(o => o
            .DoNotUseConsole()
            .Add(output)
        );
        context["engineBuilder"] = builder;
        return builder;
    }

    public static Engine GetEngine(this ScenarioContext context)
    {
        if (context.TryGetValue<Engine>("engine", out var engine))
            return engine;
        var builder = context.GetEngineBuilder();
        engine = builder.Build();
        context["engine"] = engine;
        return engine;
    }

    public static TestOutput GetOutput(this ScenarioContext context)
    {
        if (context.TryGetValue<TestOutput>("output", out var output))
            return output;
        output = new TestOutput();
        context["output"] = output;
        return output;
    }
}
