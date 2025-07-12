namespace StoneFruit.SpecTests.Support;

public class ObjectWithHandlerMethods
{
    [Verb("simple-method")]
    public void SimpleMethod(IOutput output)
    {
        output.WriteLine("Simple");
    }

    [Verb("method-with-one-named-arg")]
    public void MethodWithOneNamedArg(string name, IOutput output)
    {
        output.WriteLine($"Named: {name}");
    }

    [Verb("method-with-one-flag-arg")]
    public void MethodWithOneFlagArg(IOutput output, bool flag)
    {
        output.WriteLine($"Flag: {flag}");
    }
}

public class ObjectWithAsyncHandlerMethods
{
    [Verb("simple-method-async")]
    public Task SimpleMethodAsync(IOutput output)
    {
        output.WriteLine("Simple");
        return Task.CompletedTask;
    }

    [Verb("method-with-one-named-arg-async")]
    public Task MethodWithOneNamedArgAsync(IOutput output, string name)
    {
        output.WriteLine($"Named: {name}");
        return Task.CompletedTask;
    }
}
