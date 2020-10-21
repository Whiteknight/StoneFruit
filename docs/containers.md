# Dependency-Injection Containers

StoneFruit is developed with DI containers in mind. If you have a DI container you prefer to use, you can scan for instances of `IHandler` and `IAsyncHandler`. Or, you can use one of the existing bindings for popular containers. For DI containers, the Stonefruit engine is configured as a series of registrations in your container, and then the engine instance is created by your container. 

## StructureMap

See the [StructureMap Quick Start](start_structuremap.md) for information on configuring StructureMap to run StoneFruit.

## Lamar

See the [Lamar Quick Start](start_lamar.md) for information on configuring Lamar to run StoneFruit.

## Microsoft DependencyInjection

See the [Microsoft DI Quick Start](start_microsoft.md) for information on configuring the microsoft DI container to run StoneFruit.

## Other Containers and Custom Implementations

Bindings for other popular containers are in development and will be offered as separate nuget packages when they are available. If your container implements the `Microsoft.Extensions.DependencyInjection.Abstractions` contracts, you might be able to integrate it easily using some of the existing code.
