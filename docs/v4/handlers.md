# Handlers

Handlers are the central mechanism for encapsulation of responsibility and functionality in StoneFruit.

## Basic Control Flow

Control flow in StoneFruit starts with an StoneFruitApplication:

1. The `StoneFruitApplication` runloop gets an input string of text from the user
1. The string of text is parsed into an `IArguments` object
1. The `IArguments` object is passed to the `CommandDispatcher`
1. The `CommandDispatcher` looks up the appropriate Handler for the command
1. The Handler is constructed using dependency injection and executed
1. Control flow returns to the runloop to get the next input from the sources.
   
## Important Objects

There are several objects in StoneFruit which will be important for the development of your handlers, each of which can be injected into the handler:

1. `StoneFruit.StoneFruitApplication` is the execution engine which runs the application. The application gathers input commands from a variety of sources and dispatches them to the appropriate handlers.
1. `StoneFruit.EngineState` is a top-level context object to hold information about the state of the engine. The EngineState object will allow you to exit the engine runloop, store metadata values between commands, or schedule additional commands to execute.
1. `StoneFruit.IOutput` is an abstraction over output. By default output is sent to the `System.Console`, and helper methods are provided to assist with color and formatting. 
2. `StoneFruit.IInput` is an abstraction over user input. By default it prompts from the console. In headless mode there is no input.
3. `StoneFruit.IHandlerSource` is used to find handlers. There are several types of built-in sources, and also sources which wrap calls to your DI container (if you're using one)
4. `StoneFruit.IArguments` holds the parsed arguments of a command, and can be used to access them or map them to objects. Internally, this object holds the parsed verb as well, though the verb will be removed before the handler is executed.
5. `StoneFruit.CommandDispatcher` takes `IArguments`, finds a suitable handler, and invokes it.
6. `StoneFruit.IEnvironments` is used to hold environments. An environment is a custom object, whose lifespan is somewhere between an EngineState and a Command. Environments are usually used to provide access to application configuration data.

## Types of Handlers

Handlers can take several forms:

1. Classes which implement one of the interfaces `IHandler` or `IAsyncHandler`
1. The public named methods on an object instance 
1. Anonymous `Action<IArguments, HandlerContext>` or asynchronous `Func<IArguments, HandlerContext, CancellationToken, Task>` delegates
1. [Scripted](scripting.md) combinations of other handlers

StoneFruit comes with a few built-in handlers for basic operations, but most of the interesting handlers will be developed by you. The built-in `help` verb will list all the verbs currently registered with the StoneFruit engine.

### Handler Classes

Here is an example of a synchronous handler class:

```csharp
[Verb("example")]
public class MyExampleHandler : IHandler
{
    public static string Description => "...";
    public static string Usage => "...";

    public void Execute(IArguments arguments, HandlerContext context)
    {
        ...
    }
}
```

A new handler instance is created for every invocation, with the requested objects injected into the constructor. You can request any of the "Important Objects" listed above or any other type registered with your DI container (if you are using a container). The `Execute()` method is where work happens. The `[Verb]` attribute allows you to specify which verb(s) to associate with your handler. If you do not specify a `[Verb]` attribute, the name of the handler will be used to create a verb for you (`MyExampleHandler` => `"my example"`). 

Here is an example of an asynchronous handler class, identical in function to the above example:

```csharp
[Verb("example")]
public class MyExampleHandler : IAsyncHandler
{
    public static string Description => "";
    public static string Usage => "";

    public async Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken token)
    {
        ...
    }
}
```

The `CancellationToken` is constrained by a timeout configurable during engine setup. It is good practice to check the token periodically if you are executing a long-running command.

#### Description and Usage

The special static string properties `Description` and `Usage` will display a short description and detailed usage information, respectively, in the output of the `help` built-in command. If you do not provide these properties, no information will be shown.

#### Registering Handler Classes

Handler classes can be added to the system in two ways:
1. Registered with the DI container using the `.AddHandler()` method, or
2. Create an instance and add it to the engine manually.

```csharp
builder.SetupHandlers(h => h
    // Add an existing instance
    .Add(new MyHandler())
    
    // Register the type with DI
    .Add<MyHandler>()
);

builder.Services.AddHandler<MyHandler>();
```

In addition you can scan an assembly for public `IHandler` types and have them added to DI automatically.

```csharp
builder.SetupHandlers(h => h
    // Scan this assembly
    .ScanAssemblyForHandlers(assembly)

    // Scan the entry assembly
    .ScanHandlersFromEntryAssembly()

    // Scan the current assembly
    .ScanHandlersFromCurrentAssembly()

    // Scan the assembly where MyType is defined
    .ScanHandlersFromAssemblyContaining<MyType>()
);
```


### Named Methods as Handlers

You can use the named public methods of an object as handlers:

```csharp
services.SetupEngine(b => b
    .SetupHandlers(h => h
        .UsePublicMethodsAsHandlers(new MyObject1())
        .UsePublicMethodsAsHandlers<MyObject2>()
    )
);
```

Each public method on the given object or class will be turned into a handler with that name. If the return type is a `Task` or `Task<>` it will be invoked asynchronously. Otherwise it will be invoked synchronously. StoneFruit will attempt to fill in method parameters from the DI container and from values in the current `IArguments` object. If an unknown type is found, an attempt will be made to provide a default value instead. Here is some example code:

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

#### Return Values 

If the method has a non-`void` return value, or is asynchronous with `Task<T>` the return value can be captured and acted upon. By default StoneFruit will do nothing, but you can write the result values out to console by registering an `IObjectOutputWriter` object with the DI. StoneFruit offers a JSON-formatted output option:

```csharp
builder.SetupIo(io => io
    .UseJsonObjectWriter()
);
```

### Function Delegates as Handlers

You can use lambdas and function delegates as handlers, with any arbitrary parameters or return values (so long as parameters can be filled by either `IArguments` or the DI container):

```csharp
services.SetupEngine(b => b
    // synchronous
    .SetupHandlers(h => h.Add("example", (...) => { ... }))

    // asynchronous
    .SetupHandlers(h => h.Add("example", async (...) => { ... }))
);
```

If the delegate returns a `Task` or `Task<T>` it will be invoked asynchronously. 

#### Return Values

Like the public instance method handlers above, if the method returns a value you can register an `IObjectOutputWriter` to write that object to the console, to logs, or take some other action on it.
