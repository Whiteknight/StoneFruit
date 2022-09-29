# Quick Start - Microsoft

You can use the **Microsoft.Extensions.DependencyInjection** DI Container to find your handlers and manage your dependencies. Start by installing the `StoneFruit` and `StoneFruit.Containers.Microsoft` packages from Nuget.

```csharp
using Microsoft.Extensions.DependencyInjection;
using StoneFruit;
using StoneFruit.Containers.Lamar;
```

```csharp
public static void Main(string[] args)
{
    IServiceProvider provider = null;
    var services = new ServiceCollection();
    services.SetupEngine<MyEnvironment>(builder => {
        ...
    }, () => provider);
    provider = services.BuildServiceProvider();
    var engine = provider.GetService<Engine>();
    engine.RunWithCommandLineArguments();
}
```

In this configuration the container will scan your application to find all handler classes, and those handlers will be resolved by the Lamar container and have access to all your registered types as dependencies. Notice that, due to limitations in the interfaces, you must pass both the `ServiceCollection` and also an accessor for the `IServiceProvider` when using the Microsoft DependencyInjection container. 