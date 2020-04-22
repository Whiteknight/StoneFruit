# Quick Start - Ninject

StoneFruit setup with Ninject is a little bit different from the other DI containers because it does not implement the `Microsoft.Extensions.DependencyInjection.Abstractions` interfaces. First, install the `StoneFruit` and `StoneFruit.Containers.Ninject` packages from Nuget.

```csharp
using Ninject;
using StoneFruit;
using StoneFruit.Containers.Ninject;
```

```csharp
public static void Main(string[] args)
{
    var kernel = new StandardKernel();

    // Build the engine, telling it to use Ninject to manage handlers
    var engine = new EngineBuilder()
        .SetupHandlers(h => h.UseNinjectHandlerSource(kernel))
        ...
        .Build();

    // Optional: Tell ninject about your environment type
    kernel
        .Bind<MyEnvironment>()
        .ToMethod(c => (MyEnvironment)engine.Environments.Current)
        .InTransientScope();

    engine.RunWithCommandLineArguments();
}
```

In this configuration Ninject will scan your application for handlers. Ninject will resolve handler instances and make available all dependency types registered with your container. 