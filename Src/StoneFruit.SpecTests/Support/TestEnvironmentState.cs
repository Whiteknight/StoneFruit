namespace StoneFruit.SpecTests.Support;

public class TestEnvironmentState
{
    public string Name { get; }

    public int IncrementCount { get; private set; }

    public TestEnvironmentState(string name)
    {
        Name = name;
        IncrementCount = 0;
    }

    public void Increment()
    {
        IncrementCount++;
    }
}

[Verb("increment-env-state-count")]
public class IncrementEnvStateCount : IHandler
{
    private readonly TestEnvironmentState _state;
    private readonly IOutput _output;

    public IncrementEnvStateCount(TestEnvironmentState state, IOutput output)
    {
        _state = state;
        _output = output;
    }

    public void Execute()
    {
        _state.Increment();
        _output.WriteLine($"{_state.Name}: {_state.IncrementCount}");
    }
}
