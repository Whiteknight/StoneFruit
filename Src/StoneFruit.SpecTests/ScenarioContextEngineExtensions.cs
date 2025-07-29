using StoneFruit.Execution.Templating;
using StoneFruit.SpecTests.Support;

namespace StoneFruit.SpecTests;

public static class ScenarioContextEngineExtensions
{
    public static StoneFruitApplicationBuilder GetEngineBuilder(this ScenarioContext context)
    {
        if (context.TryGetValue<StoneFruitApplicationBuilder>("engineBuilder", out var builder))
            return builder;
        var io = context.GetIo();
        builder = StoneFruitApplicationBuilder.Create();
        builder.SetupIo(o => o
            .UseOutput(io)
            .UseInput(io)
        );
        context["engineBuilder"] = builder;
        return builder;
    }

    public static StoneFruitApplication GetEngine(this ScenarioContext context)
    {
        if (context.TryGetValue<StoneFruitApplication>("engine", out var engine))
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
        output = new TestInputOutput(DefaultTemplateFormat.Parser.Create());
        context["io"] = output;
        return output;
    }
}
