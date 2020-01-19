# StoneFruit

StoneFruit is a command-line hosting framework for commandlets and verbs. Stone Fruit will help you build small utilities for a variety of purposes, which allow easy functionality extensions.

## What Does It Do?

Think about the popular `git` application. The command `git` doesn't do much by itself, the power comes when you specify a **verb** like `git add` or `git checkout`. Each different verb causes the application to behave in a particular way, including taking a different set of arguments and producing different output effects. StoneFruit is sort of like that, an ability to create a hosting application which hosts **verbs**. Each verb is like a tiny, self-contained program which takes it's own set of arguments and does it's own thing. 

## Design Considerations

StoneFruit is designed to work with a DI/IoC container but this is not required. It is designed to be as easy as possible to add new verbs to the application. The system is designed to be modular, so several key components can be plugged with custom versions.

## Setup

Install the `StoneFruit` package and, optionally, one of the DI integrations such as `StoneFruit.StructureMap`. Then you'll need to create an engine:

```csharp
public static void Main(string[] args)
{
    var engine = new EngineBuilder()
        // ... setup the engine here ...
        .Build();
    engine.Run(args);
}
```

## Key Concepts

### Interactive and Headless Modes

StoneFruit supports two modes of operation: Interactive and Headless. In Interactive Mode StoneFruit will provide a prompt where you can enter verbs one after the other. In headless mode there is no prompt, and a single verb will be read from the commandline arguments passed to the program. 

### Environments

StoneFruit supports the concept of "Environments". An environment is an execution context in which multiple subsequent verbs are executed. Consider the case where you have multiple execution environments ("Local", "Integration" and "Production" for example) with different configurations for each. Or consider that you want to work with multiple different resources, but only connect to one at a time. What's important is that each environment has a unique name.

An environment object can be any type of object you want. It can be a wrapper around a config file, or storage for runtime metadata, or whatever you want it to be. The `IEnvironmentFactory` is tasked with creating a new environment object given the selected environment name and providing a list of possible names. *You must provide or configure an `IEnvironmentFactory` implementation yourself*. StoneFruit cannot infer what your execution context is.

Here are some options you can use:

```csharp
new EngineBuilder()

    // Use a custom IEnvironmentFactory, provided by you:
    .UseEnvironmentFactory(myFactory)

    // If you only have a single environment to use and you do not want to switch
    .UseSingleEnvironment(myObject)

    // You don't need an environment and don't care:
    .NoEnvironment()
    ;
```

As an example, here's an `IEnvironmentFactory` implementation which supports 3 environments ("Local", "Integration" and "Production") and the environment object is an `IConfigurationRoot` object using a config file with the name of the environment:

```csharp
public class MyEnvironmentFactory : IEnvironmentFactory
{
    public object Create(string name)
        => ConfigurationBuilder().AddJsonFile($"Configs/{name}.json").Build();

    public IReadOnlyCollection<string> ValidEnvironments 
        => new[] { "Local", "Integration", "Production" };
}
```

If your application does not use environments or only uses a single environment, you don't need to worry about it after setup. If you support multiple environments, you will be required to specify. In headless mode you must provide the name of the environment to use as the first argument on the command line. If you are running in interactive mode, StoneFruit will prompt you to select an environment before any verbs may be executed. You can change your current active environment at any time by using the `change-env` verb.

### Verbs and `ICommandSource`

Verbs are implemented in code with `ICommandVerb`. Each verb class must provide a `.Execute()` method. Any dependencies required by the class should be injected into the constructor. There are several types which can be injected by default, and more if you integrate your own DI/IoC container:

* `ITerminalOutput` An abstraction over the output stream. By default wraps `System.Console` methods with a few helper methods thrown in (you can use `System.Console` if you prefer)
* `IEnvironmentCollection` Provides access to the current environment and the list of possible environments
* `CommandArguments` Provides access to the arguments passed to the verb, if any
* `EngineState` Provides access to the internal state of the execution engine, allowing you to control program execution.
* `ICommandSource` Provides access to all verbs in the system.

When a verb is executed, a new instance of the corresponding `ICommandVerb` class is instantiated with appropriate arguments, and the `.Execute()` method is called.

The `ICommandSource` abstraction is responsible for managing a list of available verbs. This is where your DI/IoC container is most useful: scanning assemblies in the solution to find available `ICommandVerb` implementations and then to construct them using available dependencies.

### Get Help

Type the `help` verb in interactive mode or use the command-line argument "help" in headless mode, to show a list of all verbs in the application. Type `help <verb>` to get more detailed help information if available.

