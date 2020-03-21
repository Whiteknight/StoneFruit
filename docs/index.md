# StoneFruit

**StoneFruit** is a commandlet host and execution engine. It allows you to rapidly develop new command-line utility routines without having to create a new application for each one. StoneFruit is designed from the ground-up to be pluggable and flexible, and to easily integrate with your existing code, including DI/IoC containers if desired.

## Quick Example: Git

Git is a powerful development tool but typing "`git`" by itself at the prompt doesn't do much. To access the varied functionality of git, you must specify a **verb** like "`git clone`", "`git add`" or "`git rebase`". Each different verb causes the application to do different tasks, often with completely separate sets of input arguments and program output. StoneFruit is like this: It allows you to create a program with many verbs, each of which can do something different and useful.

## Basic Terminology

A **Verb** is a command which the user types at the commandline, which may have arguments and output effects. Verbs in StoneFruit are case-insensitive

A **Handler** is a class which is invoked by StoneFruit when a verb is input. A single Handler class may correspond to many different verbs.

A **Command** is a combination of a Verb and all input arguments.

**Headless** is a mode where the StoneFruit application executes one or more commands without user input.

**Interactive** is a mode where the StoneFruit application displays an interactive prompt where commands can be input one after the other.

## Quick Start

To get started, we need to create an Engine:

```csharp
var engine = new EngineBuilder()
    ...
    .Build();
```

## Important Objects

There are several objects in StoneFruit which will be important for the development of your handlers:

1. `StoneFruit.Engine` is the execution engine which runs the application. The engine gathers input commands from a variety of sources and dispatches them to the appropriate handlers.
1. `StoneFruit.EngineState` is a top-level context object to hold information about the state of the engine. The EngineState object will allow you to exit the engine runloop, store metadata values between commands, or schedule additional commands to execute.
1. `StoneFruit.IOutput` is an abstraction over output. By default output is sent to the `System.Console`, and helper methods are provided to assist with color and formatting. 
1. `StoneFruit.IHandlerSource` is used to find handlers. There are several types of built-in sources, and also sources which wrap calls to your DI container (if you're using one)
1. `StoneFruit.Command` holds information about the command the user entered, including the invoked verb and parsed arguments.
1. `StoneFruit.CommandArguments` holds the parsed arguments of a command, and can be used to access them or map them to objects
1. `StoneFruit.CommandDispatcher` takes a command, finds a suitable handler, and invokes it. The dispatcher also contains references to several of the other objects in this list.
1. `IEnvironmentCollection` is used to hold environments. An environment is a custom object, whose lifespan is somewhere between an EngineState and a Command. Environments are usually used to provide access to application configuration data.

## Handlers

Handlers can take several forms:

1. Classes which implement one of the interfaces `IHandler` or `IHandlerAsync`
1. The public named methods on an object instance 
1. action delegates
1. Scripted combinations of other handlers

StoneFruit comes with a few built-in handlers for basic operations, but most of the interesting handlers will be developed by you. The built-in `help` verb will list all the verbs currently registered with the StoneFruit engine.

### Handler Classes

Here is an example of a synchronous handler class:

```csharp
[Verb("example")]
public class MyExampleHandler : IHandler
{
    private readonly CommandArguments _args;
    private readonly IOutput _output;

    public MyExampleHandler(CommandArguments args, IOutput output)
    {
        _args = args;
        _output = output;
    }

    public static string Description => "";
    public static string Usage => "";

    public void Execute()
    {
        ...
    }
}
```

A new handler instance is created for every invokation, with the requested objects injected into the constructor. You can request any of the "Important Objects" listed above or any other type registered with your DI container (if you are using a container). The `Execute()` method is where work happens. The `[Verb]` attribute allows you to specify which verb(s) to associate with your handler. If you do not specify a `[Verb]` attribute, the name of the handler will be used to create a verb for you (`MyExampleHandler` => `"myexample"`). 

Here is an example of an asynchronous handler class, identical in function to the above example:

```csharp
[Verb("example")]
public class MyExampleHandler : IAsyncHandler
{
    private readonly CommandArguments _args;
    private readonly IOutput _output;

    public MyExampleHandler(CommandArguments args, IOutput output)
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

The special static string properties `Description` and `Usage` will display a short description and detailed usage information, respectively, in the output of the `help` built-in verb. If you do not provide these properties, no description will be shown.

### DI Containers

StoneFruit is developed with DI containers in mind. If you have a DI container you prefer to use, you can scan for instances of `IHandler` and `IAsyncHandler`. Or, you can use one of the existing bindings for popular containers. Install the respective nuget packages and then you can extend your code:

```csharp
using StoneFruit.Containers.Ninject;

    // Call this on your EngineBuilder:
    .SetupHandlers(h => h.UseNinjectHandlerSource())
```

```csharp
using StoneFruit.Containers.StructureMap;

    // Call this on your EngineBuilder:
    .SetupHandlers(h => h.UseStructureMapHandlerSource())
```

```csharp
using StoneFruit.Containers.Lamar;

    // Call this on your EngineBuilder:
    .SetupHandlers(h => h.UseLamarHandlerSource())
```

Bindings for other popular containers are in development.

### Named Methods as Handlers

You can use the named public methods of an object as handlers:

```csharp
    .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject()))
```

For each public method, if the return type is `void` it will be invoked synchronously and if it is `Task` it will be invoked asynchronously. StoneFruit will attempt to fill in method parameters from the list of "Important Objects" above. If an unknown type is found, a default value will be provided instead.

### Function Delegates as Handlers

You can use lambdas and function delegates as handlers:

```csharp
    // synchronous
    .SetupHandlers(h => h.Add("example", (c, d) => { ... }));

    // asynchronous
    .SetupHandlers(h => h.Add("example", async (c, d) => { ... }));
```

The delegate should be assignable to `Action<Command, CommandDispatcher>` for synchronous or `Func<Command, CommandDispatcher, Task>` for asynchronous.

## Execution

The engine provides several methods to begin execution. You can select either interactive mode or headless mode explicitly, or you can let the engine decide based on the presence of command-line arguments.

### Interactive Mode

if you call `engine.RunInteractively()` the engine will be started in interactive mode and will ignore any commands on the commandline. If you support environments and call `engine.RunInteractively(envName)` the engine will start interactive mode with the specified environment active. If your application supports multiple environments and you enter interactive mode without an environment set, the engine will prompt you to select an environment before showing you the prompt.

If you `.Run()` your engine without command-line arguments, or if you support environments and specify only the name of an environment on the commandline, the engine will be started in interactive mode. You can enter commands at the prompt, and can type "exit" or "quit" to exit the loop. 

### Headless Mode

If you call `engine.RunHeadlessWithCommandLineArgs()` it will always run in headless mode using the commandline arguments as the only input. If a valid command is not present, an error will be shown.

If you call `engine.RunHeadless(cmd)` it will always run in headless mode using the given string as the only command input. If the string is null or empty, or contains an invalid verb, an error will be shown.

If you call `engine.Run()` with command on the commandline your application will start in headless mode. If you support environments, the first argument must be the name of the environment to use, followed by the verb and arguments. If you do not support environments, the first argument should be the verb followed by the arguments. If there is no valid command on the commandline, `engine.Run()` will choose interactive mode instead.

```bash
mystonefruitapp Production echo 'test'
mystonefruitapp echo 'test'
```
## Scripting

There are two methods of scripting in StoneFruit. Scripts allow you to execute zero or more commands at a time.

### EventScripts

The StoneFruit `EngineState` contains a number of pre-defined scripts which are executed in response to various events.

```csharp
    .SetupEvents(e => ...);
```

Scripts are executed in the following scenarios:

1. An unhandled exception is thrown by a handler
1. Headless mode is entered or exited
1. Interactive mode is intered or exited
1. The current environment is changed
1. Headless mode is entered without a valid command to execute
1. A verb is specified which is not known to the system
1. The single "help" argument is specified on the commandline in headless mode.

You can examine or modify the contents of these scripts within the context of the `EngineBuilder.SetupEvents()` method. This gives a way to customize behavior by setting up data or showing helpful information to users. Notice that some of these events are important to the operation of the system and errors in these scripts can create a fatal condition which will cause the engine to terminate.

### Script Handlers

A script handler is a collection of zero or more commands which are executed in response to a single command.