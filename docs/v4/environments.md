# StoneFruit Environments

StoneFruit has a concept of "environments". An environment is just one of a pre-defined list of names with a little bit of functionality built around it. 

## Setting Environments

By default there is one environment named `""` (the empty string). 

To set additional environments:

```csharp
builder.SetupEnvironments(e => e
    .SetEnvironments(["first", "second", "third"])
);
```

## Uses

Environments allow you to set up multiple execution or data contexts if desired. For example, consider an internal utility program to aide in system development and maintenance. For that scenario, it might make sense to have configured environments for "Dev", "QA" and "Production", etc. 

If you want to have utilities and scripts to interact with multiple databases, it might make sense to have environments for "Data Cluster 1" and "Data Cluster 2", etc.

## DI Registrations.

There are two ways to get per-environment values from the DI container. In the first way, an `IServiceCollection.AddPerEnvironment()` method allows you to call a factory method with the current environment name as a parameter. This registration will be `Scoped`, to the current handler invocation and the result will be *cached* until the environment is changed. That is, the object will be created once per environment and will persist across multiple commands so long as the environment is not changed:

```csharp
builder.Services.AddPerEnvironment<T>((provider, name) => ...);
```

Another way is to use the `IEnvironment` object as an injected parameter. `IEnvironment` is a `Scoped` registration. When you resolve an object with an `IEnvironment` dependency, it will contain the name of the current environment.

```csharp
public class MyObjectFactory
{
    private readonly IEnvironment _env;
    public MyObjectFactory(IEnvironment env) => _env = env;
    public MyObject Create() => _env.Name switch {
        "Local" => new MyLocalObject(...),
        "Test" => new MyTestObject(...),
        "Production" => new MyProdObject(...)
    };
}

builder.Services.AddScoped<MyObjectFactory>();
builder.Services.AddScoped(provider => provider.GetRequiredService<MyObjectFactory>().Create());
```

## Changing Environments

The `env` handler will allow you to list and change environments

```bash
# List all environments
env -list

# show a prompt to select an environment by name or number
env

# Switch to the environment by name
env Production

# Switch to the environment by number
env 2
```

When the environment is changed the `EnvironmentChanged` script is executed.
