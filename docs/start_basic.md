# Quick Start - No DI

If you do not wish to use StoneFruit with a DI container, you can build the `Engine` directly and use it:

```csharp
var engineBuilder = new EngineBuilder();
    // Setup the builder here
    // ...
var engine = engineBuilder.Build();
```

Without a DI container, you will need to manually configure the handlers you wish to use, and you will be limited in what dependencies those handlers can require.

