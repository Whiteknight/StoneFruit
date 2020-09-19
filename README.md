# StoneFruit

StoneFruit is a command-line hosting framework for commandlets and verbs. Stone Fruit will help you build small utilities for a variety of purposes, which allow easy functionality extensions.

## What Does It Do?

Think about the popular `git` application. The command `git` doesn't do much by itself, the power comes when you specify a **verb** like `git add` or `git checkout`. Each different verb causes the application to behave in a particular way, including taking a different set of arguments and producing different output effects. StoneFruit is sort of like that, an ability to create a hosting application which hosts **verbs**. Each verb is like a tiny, self-contained program which takes it's own set of arguments and does it's own thing. 

## Design Considerations

StoneFruit is designed to work with a DI/IoC container but this is not required. It is designed to be as easy as possible to add new verbs to the application. The system is designed to be modular, so several key components can be plugged with custom versions.

## Setup

Install the `StoneFruit` package and, optionally, one of the DI integrations such as `StoneFruit.Lamar`. Then you'll need to create an engine:

```csharp
var services = new ServiceRegistry();
services.SetupEngine(b => b
    ...
);
var container = new Container(services);
var engine = container.GetInstance<Engine>();
Environment.ExitCode = engine.RunWithCommandLineArguments();
```

## Key Concepts

### Verbs, Commands and Handlers

A **verb** is the name of the thing you want to do. A **command** is a combination of a verb and an optional list of arguments. A **handler** is the class which is invoked for a specific verb.

Handlers are implemented in code with the `IHandler` or `IAsyncHandler` interfaces which expose a `.Execute()` or `.ExecuteAsync()` method, respectively. Any dependencies required by the class should be injected into the constructor and will be satisfied by your DI container. There are several types which can be injected by default, and more if you integrate your own DI/IoC container:

* `IOutput` An abstraction over the output stream. By default wraps `System.Console` methods with a few helper methods thrown in for color and formatting (you can use `System.Console` if you prefer)
* `IEnvironmentCollection` Provides access to the current environment and the list of possible environments
* `IArguments` Provides access to the arguments of the command, if any
* `EngineState` Provides access to the internal state of the execution engine, allowing you to control program execution and store metadata.
* `IHandlerSource` Provides access to all handlers in the system and their corresponding verbs.
* `CommandDispatcher` allows you to execute commands
* `ICommandParser` allows you to parse commands and argument lists into objects

The `IHandlerSource` abstraction is responsible for managing a list of available verbs. This is where your DI/IoC container is most useful: scanning assemblies in the solution to find available `IHandler` implementations and then to construct them using available dependencies.

### Interactive and Headless Modes

StoneFruit supports two modes of operation: **Interactive** and **Headless**. In Interactive Mode StoneFruit will provide a prompt where you can enter commands one after the other, with state captured between each. In headless mode there is no prompt, and a single command will be read from the commandline arguments passed to the program. You can instruct the Engine to run in headless or interactive mode, or to see if there are any command-line arguments and decide which mode to enter.

### Environments

StoneFruit supports the concept of "Environments". An environment is an execution context in which commands are executed. Consider the case where you have multiple execution environments ("Local", "Integration" and "Production" for example) with different configurations for each. Or consider that you want to work with multiple different resources, but only connect to one at a time. What's important is that each environment has a unique name.

An environment object can be any type of object you want. It can be a wrapper around a config file, or storage for runtime metadata, or whatever. The `IEnvironmentFactory` is tasked with creating a new environment object given the selected environment name and providing a list of possible names. *You must provide or configure an `IEnvironmentFactory` implementation yourself*. StoneFruit cannot infer what your execution context is.

Here are some options you can use:

```csharp
new EngineBuilder()
    .SetupEnvironments(e => e
        // Use a custom IEnvironmentFactory, provided by you:
        .UseFactory(myFactory)

        // If you only have a single environment to use and you do not want to switch
        .UseInstance(myObject)

        // You don't need an environment and don't care:
        .None()
    );
```

As an example, here's an `IEnvironmentFactory` implementation which supports 3 environments ("Local", "Integration" and "Production") and the environment object is an `IConfigurationRoot` object using a config file with the name of the environment:

```csharp
public class MyEnvironmentFactory : IEnvironmentFactory
{
    public object Create(string name)
        => new ConfigurationBuilder()
            .AddJsonFile($"Configs/{name}.json")
            .Build();

    public IReadOnlyCollection<string> ValidEnvironments 
        => new[] { "Local", "Integration", "Production" };
}
```

If your application does not use environments or only uses a single environment, you don't need to worry about it after setup. If you support multiple environments, you will be required to specify. In headless mode you must provide the name of the environment to use as the first argument on the command line. If you are running in interactive mode, StoneFruit will prompt you to select an environment before any verbs may be executed. You can change your current active environment at any time by using the `change-env` verb.

### Get Help

Type the `help` verb in interactive mode or use the command-line argument "help" in headless mode, to show a list of all verbs in the application. Type `help <verb>` to get more detailed help information if available.

