# Quick Start - Lamar

You can use the **Lamar** DI Container to find your handlers and manage your dependencies. Start by installing the `StoneFruit` and `StoneFruit.Containers.Lamar` packages from Nuget.

```csharp
using Lamar;
using StoneFruit;
using StoneFruit.Containers.Lamar;
```

```csharp
public static void Main(string[] args)
{
    var services = new ServiceRegistry();
    services.SetupEngine<MyEnvironment>(engineBuilder => 
        ...
    );
    var container = new Container(services);
    var engine = container.GetService<Engine>();
    engine.RunWithCommandLineArguments();
}
```
