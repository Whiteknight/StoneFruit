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

    [Verb("method-with-return-value")]
    public object MethodWithReturnValue()
    {
        return new { Value = _value };
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

public record ExampleBusinessService(string Value)
{
    public string GetData() => Value;
}

public class ObjectWithDependenciesAndHandlerMethods
{
    private readonly ExampleBusinessService _service;

    public ObjectWithDependenciesAndHandlerMethods(ExampleBusinessService service)
    {
        _service = service;
    }

    [Verb("injected-method")]
    public void InjectedMethod(IOutput output)
    {
        var value = _service.GetData();
        output.WriteLine($"{value} Injected");
    }

    [Verb("injected-method-with-one-named-arg")]
    public void MethodWithOneNamedArg(string name, IOutput output)
    {
        var value = _service.GetData();
        output.WriteLine($"{value} Named: {name}");
    }

    [Verb("injected-method-with-one-named-arg2")]
    public void MethodWithOneNamedArg2(IOutput output, string name)
    {
        var value = _service.GetData();
        output.WriteLine($"{value} Named: {name}");
    }

    [Verb("injected-method-with-one-flag-arg")]
    public void MethodWithOneFlagArg(IOutput output, bool flag)
    {
        var value = _service.GetData();
        output.WriteLine($"{value} Flag: {flag}");
    }
}
