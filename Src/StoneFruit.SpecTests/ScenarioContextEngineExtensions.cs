
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests;

public static class ScenarioContextEngineExtensions
{
    public static EngineBuilder GetEngineBuilder(this ScenarioContext context)
    {
        if (context.TryGetValue<EngineBuilder>("engineBuilder", out var builder))
            return builder;
        var io = context.GetIo();
        builder = EngineBuilder.Create();
        builder.SetupIo(o => o
            .DoNotUseConsole()
            .Add(io)
            .Use(io)
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

    public static TestInputOutput GetIo(this ScenarioContext context)
    {
        if (context.TryGetValue<TestInputOutput>("io", out var output))
            return output;
        output = new TestInputOutput();
        context["io"] = output;
        return output;
    }
}
