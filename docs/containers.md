# Dependency-Injection Containers

StoneFruit is developed with DI containers in mind. If you have a DI container you prefer to use, you can scan for instances of `IHandler` and `IAsyncHandler`. Or, you can use one of the existing bindings for popular containers. Install the respective nuget packages and then you can extend your code:

## Ninject

Install the `StoneFruit.Containers.Ninject` nuget package.

```csharp
using StoneFruit.Containers.Ninject;
```

Now you can tell your `EngineBuilder` to use a Ninject container:

```csharp
engineBuilder
    // Create a new ninject IKernel and use it internally
    .SetupHandlers(h => h.UseNinjectHandlerSource())

    // Or use an existing IKernel instance with all your registrations:
    .SetupHandlers(h => h.UseNinjectHandlerSource(kernel))
```

## StructureMap

Install the `StoneFruit.Containers.StructureMap` nuget package.

```csharp
using StoneFruit.Containers.StructureMap;
```

Now you can tell your `EngineBuilder` to use a StructureMap container:

```csharp
engineBuilder
    // Create a new StructureMap IContainer and use it internally
    .SetupHandlers(h => h.UseStructureMapHandlerSource())

    // Or use an existing IContainer instance with all your registrations:
    .SetupHandlers(h => h.UseStructureMapHandlerSource(container))
```

## Lamar

Install the `StoneFruit.Containers.Lamar` nuget package.

```csharp
using StoneFruit.Containers.Lamar;
```

Now you can tell your `EngineBuilder` to use a Lamar container:

```csharp
engineBuilder
    // Create a new Lamar IContainer and use it internally
    .SetupHandlers(h => h.UseLamarHandlerSource())

    // Or use an existing IContainer instance with all your registrations:
    .SetupHandlers(h => h.UseLamarHandlerSource(container))
```

## Other Containers and Custom Implementations

Bindings for other popular containers are in development and will be offered as separate nuget packages when they are available. However, you can create your own integration by implementing the `IHandlerSource` interface. The `IHandlerSource` has two responsibilities: Create an instance of an `IHandler` object on request, and get help information about all available handlers for the `help` command.
