# Quick Start - StructureMap

StructureMap is an older container and the developers prefer you to switch to Lamar. However, if you're still using StructureMap in your projects, you can use it with StoneFruit. First you create a `Container` and call the `SetupEngine` extension method to setup all the necessary registrations to run StoneFruit. 

```csharp
using StructureMap;
using StoneFruit;
using StoneFruit.Containers.StructureMap;
```

```csharp
public static void Main(string[] args)
{
    var container = new Container();
    container.SetupEngine<MyEnvironment>(engineBuilder => 
        ...
    );
    var engine = container.GetInstance<Engine>();
    engine.RunWithCommandLineArguments();
}
```

In this configuration StructureMap will scan your application for handler classes, and those handlers will be able to inject all types registered in your container.