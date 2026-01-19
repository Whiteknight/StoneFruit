# StoneFruit

StoneFruit is a command-line hosting framework for commandlets and verbs. StoneFruit will help you build small utilities for a variety of purposes, which allow easy functionality extensions.

## What Does It Do?

Think about the popular `git` application. The command `git` doesn't do much by itself, the power comes when you specify a **verb** like `git add` or `git checkout`. Each different verb causes the application to behave in a particular way, including taking a different set of arguments and producing different output effects. StoneFruit is sort of like that, an ability to create a hosting application which maps **verbs** to handling code. Each verb is like a tiny, self-contained program which takes it's own set of arguments and does it's own thing. 

## Design Considerations

StoneFruit is designed to work with a DI/IoC container, which uses `Microsoft.Extensions.DependencyInjection` by default. While it is not directly compatible with `Microsoft.Extensions.Hosting.HostApplicationBuilder`, it is set up in a way to mirror that flow and should be easy for new developers to get started with. See Src/ARCHITECTURE.md for more information about design goals and structure.

## Setup

Install the `StoneFruit` package.

    dotnet install StoneFruit

Then you'll need to create a `StoneFruitApplication`:

```csharp
public void Main(string[] args)
{
    var app = StoneFruitApplicationBuilder.BuildDefault();
    app.RunWithCommandLineArguments();
}
```

## Key Concepts

### Verbs, Commands and Handlers

A **verb** is the name of an action, a word that you type in to get behavior. A **command** is a combination of a verb and an optional list of arguments. A **handler** is the class or method which is invoked in response to the verb.

Handlers are implemented in a variety of ways:

**Easy**: public instance methods on any object can become handlers. Inject dependencies either into the object constructor or the method parameter list:

```csharp
public class MyDomainService
{
    public void HandlerMethod(...)
    {
        ... your logic here ...
    }
}
```

**Low-Level**: The `IHandler` or `IAsyncHandler` interfaces which expose a `.Execute()` or `.ExecuteAsync()` method, respectively. Inject dependencies into the object constructor.

```csharp
public class MyHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        ... your logic here ...
    }
}
```

There are several built-in objects from StoneFruit which can be injected by default to provide helpful functionality. These are some of the highlights

* `IOutput` and `IInput` abstractions allow controlled, overridable access to the `System.Console`, with basic color and formatting features.
* `IEnvironmentCollection` Provides access to the current environment and the list of possible environments. If you inject `IEnvironment` you will get the current environment.
* `IArguments` Provides access to the arguments of the command
* `EngineState` Provides access to the internal state of the execution engine, allowing you to control program execution and store metadata.
* `CommandDispatcher` allows you to execute commands and delegate functionality

### Interactive and Headless Modes

StoneFruit supports two modes of operation: **Interactive** and **Headless**. In Interactive Mode StoneFruit will provide a prompt where you can enter commands one after the other. In headless mode there is no prompt, and a single command will be read from the commandline arguments passed to the program. You can instruct the Engine to run in headless or interactive mode, or allow the application to decide on the mode based on the presence or absence of commandline arguments.

### Environments

StoneFruit supports the concept of "Environments". An environment is an execution context in which behavior and data may be expected to change. 

Consider the case where you writing maintenance tooling for an application which is deployed to multiple environments ("Local", "Integration" and "Production" for example) with different configurations for each. In this case your maintenance application can supply multiple configurations, and switch the active configuration for different operations. The important part of environments is that they each have a unique name.

StoneFruit provides an `IServiceCollection.AddPerEnvironment()` method which allows you to register a dependency that is resolved once per changed environment. You can also register any transient dependency to rely on `IEnvironment` to include the current environment object in your build-up.

If your application does not use environments or only uses a single environment, you don't need to worry about it after setup. If you support multiple environments, you will be required to specify which environment you are using in application start-up and can switch between them with the `env` command. In headless mode you must provide the name of the environment to use as the first argument on the command line. If you are running in interactive mode, StoneFruit will prompt you to select an environment before any verbs may be executed. You can change your current active environment at any time by using the `change-env` verb.

### Get Help

Type the `help` verb in interactive mode or use the command-line argument "help" in headless mode, to show a list of all verbs in the application. Type `help <verb>` to get more detailed help information if available.

