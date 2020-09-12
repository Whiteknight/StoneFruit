# Handlers

Handlers are the central mechanism for encapsulation of responsibility in StoneFruit.

## Basic Control Flow

Control flow in StoneFruit starts with an Engine:

1. The `Engine` runloop gets an input string of text from the user
1. The string of text is parsed into a `Command` object
1. The `Command` object is passed to the `CommandDispatcher`
1. The `CommandDispatcher` looks up the appropriate Handler for the command
1. The Handler is constructed using dependency injection and executed
1. Control flow returns to the runloop to wait for the next input from the user

## Important Objects

There are several objects in StoneFruit which will be important for the development of your handlers, each of which can be injected into the handler:

1. `StoneFruit.Engine` is the execution engine which runs the application. The engine gathers input commands from a variety of sources and dispatches them to the appropriate handlers.
1. `StoneFruit.EngineState` is a top-level context object to hold information about the state of the engine. The EngineState object will allow you to exit the engine runloop, store metadata values between commands, or schedule additional commands to execute.
1. `StoneFruit.IOutput` is an abstraction over output. By default output is sent to the `System.Console`, and helper methods are provided to assist with color and formatting. 
1. `StoneFruit.IHandlerSource` is used to find handlers. There are several types of built-in sources, and also sources which wrap calls to your DI container (if you're using one)
1. `StoneFruit.Command` holds information about the command the user entered, including the invoked verb and parsed arguments.
1. `StoneFruit.IArguments` holds the parsed arguments of a command, and can be used to access them or map them to objects
1. `StoneFruit.CommandDispatcher` takes a command, finds a suitable handler, and invokes it. The dispatcher also contains references to several of the other objects in this list.
1. `StoneFruit.IEnvironmentCollection` is used to hold environments. An environment is a custom object, whose lifespan is somewhere between an EngineState and a Command. Environments are usually used to provide access to application configuration data.

## Types of Handlers

Handlers can take several forms:

1. Classes which implement one of the interfaces `IHandler` or `IAsyncHandler`
1. The public named methods on an object instance 
1. Action delegates
1. Scripted combinations of other handlers

StoneFruit comes with a few built-in handlers for basic operations, but most of the interesting handlers will be developed by you. The built-in `help` verb will list all the verbs currently registered with the StoneFruit engine.

### Handler Classes

Here is an example of a synchronous handler class:

```csharp
[Verb("example")]
public class MyExampleHandler : IHandler
{
    private readonly IArguments _args;
    private readonly IOutput _output;

    public MyExampleHandler(IArguments args, IOutput output)
    {
        _args = args;
        _output = output;
    }

    public static string Description => "...";
    public static string Usage => "...";

    public void Execute()
    {
        ...
    }
}
```

A new handler instance is created for every invocation, with the requested objects injected into the constructor. You can request any of the "Important Objects" listed above or any other type registered with your DI container (if you are using a container). The `Execute()` method is where work happens. The `[Verb]` attribute allows you to specify which verb(s) to associate with your handler. If you do not specify a `[Verb]` attribute, the name of the handler will be used to create a verb for you (`MyExampleHandler` => `"my-example"`). 

Here is an example of an asynchronous handler class, identical in function to the above example:

```csharp
[Verb("example")]
public class MyExampleHandler : IAsyncHandler
{
    private readonly IArguments _args;
    private readonly IOutput _output;

    public MyExampleHandler(IArguments args, IOutput output)
    {
        _args = args;
        _output = output;
    }

    public static string Description => "";
    public static string Usage => "";

    public async Task ExecuteAsync(CancellationToken token)
    {
        ...
    }
}
```

The `CancellationToken` is constrained by a timeout configurable during engine setup. 

#### Description and Usage

The special static string properties `Description` and `Usage` will display a short description and detailed usage information, respectively, in the output of the `help` built-in command. If you do not provide these properties, no information will be shown.

#### DI Containers

You can use StoneFruit with a DI container of your choice, to allow you to inject more types of objects into your handler constructors. See this page for more details:

* [Dependency Injection Containers](containers.md)

### Named Methods as Handlers

You can use the named public methods of an object as handlers:

```csharp
services.SetupEngine(b => b
    .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject()))
);
```

For each public method, if the return type is `void` it will be invoked synchronously and if it is `Task` it will be invoked asynchronously. StoneFruit will attempt to fill in method parameters from the list of "Important Objects" above, and also some primitive values (`string`, `int`, `long`, `bool`) which will attempt to be derived from the list of passed arguments. If an unknown type is found, a default value will be provided instead. Here is some example code:

```csharp
public class MyObject
{
    public void TestA(IOutput output)
    {
        output.WriteLine("A");
    }

    public void TestB(IOutput output, string name)
    {
        output.WriteLine($"Name: {name}");
    }

    public Task TestC(IOutput output)
    {
        output.WriteLine("C");
        return Task.CompletedTask;
    }
}
``` 

### Function Delegates as Handlers

You can use lambdas and function delegates as handlers:

```csharp
services.SetupEngine(b => b
    // synchronous
    .SetupHandlers(h => h.Add("example", (c, d) => { ... }))

    // asynchronous
    .SetupHandlers(h => h.Add("example", async (c, d) => { ... }))
);
```

The delegate should be assignable to `Action<Command, CommandDispatcher>` for synchronous or `Func<Command, CommandDispatcher, Task>` for asynchronous. Most of the "Important Objects" described above are available as public properties on the `CommandDispatcher`.
