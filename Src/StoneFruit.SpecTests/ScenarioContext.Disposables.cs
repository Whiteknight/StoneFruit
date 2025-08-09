namespace StoneFruit.SpecTests;

public static class ScenarioContextDisposablesExtensions
{
    public static void AddDisposable(this ScenarioContext context, IDisposable disposable)
    {
        if (!context.TryGetValue<List<IDisposable>>("_disposables", out var disposables))
        {
            disposables = new List<IDisposable>();
            context["_disposables"] = disposables;
        }

        disposables.Add(disposable);
    }

    public static void CleanupDisposables(this ScenarioContext context)
    {
        if (context.TryGetValue<List<IDisposable>>("_disposables", out var disposables))
        {
            foreach (var disposable in disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch { }
            }
            context.Remove("_disposables");
        }
    }
}

[Binding]
public record CleanupDisposablesHooks(ScenarioContext Context)
{
    [AfterScenario]
    public void CleanupDisposables()
    {
        Context.CleanupDisposables();
    }
}
