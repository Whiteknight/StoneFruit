namespace StoneFruit.SpecTests.Support;

public class ObjectWithHandlerMethods
{
    private readonly string _value;

    public ObjectWithHandlerMethods(string value)
    {
        _value = value;
    }

    [Verb("simple-method")]
    public void SimpleMethod(IOutput output)
    {
        output.WriteLine($"{_value} Simple");
    }

    [Verb("method-with-one-named-arg")]
    public void MethodWithOneNamedArg(string name, IOutput output)
    {
        output.WriteLine($"{_value} Named: {name}");
    }

    [Verb("method-with-one-named-arg2")]
    public void MethodWithOneNamedArg2(IOutput output, string name)
    {
        output.WriteLine($"{_value} Named: {name}");
    }

    [Verb("method-with-one-flag-arg")]
    public void MethodWithOneFlagArg(IOutput output, bool flag)
    {
        output.WriteLine($"{_value} Flag: {flag}");
    }
}

public class ObjectWithAsyncHandlerMethods
{
    private readonly string _value;

    public ObjectWithAsyncHandlerMethods(string value)
    {
        _value = value;
    }

    [Verb("simple-method-async")]
    public Task SimpleMethodAsync(IOutput output)
    {
        output.WriteLine($"{_value} Simple");
        return Task.CompletedTask;
    }

    [Verb("method-with-one-named-arg-async")]
    public Task MethodWithOneNamedArgAsync(IOutput output, string name)
    {
        output.WriteLine($"{_value} Named: {name}");
        return Task.CompletedTask;
    }
}
