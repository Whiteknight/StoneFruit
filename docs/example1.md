# Simple Example

First let's create a handler to do some basic work:

```csharp
public class MyFirstHandler : IHandler
{
    private readonly IOutput _output;

    public MyFirstHandler(IOutput output) 
    {
        _output = output;
    }

    public void Execute()
    {
        _output.WriteLine("Starting the job...");
        // .. Do work here ..
        _output.WriteLine("Done.");
    }
}
```

Now in our entrypoint, we can setup and run the engine. For the sake of argument we're going to use the Ninject handler source from the `StoneFruit.Containers.Ninject` package, though any of the DI containers will work the same way in this example:

```csharp
using StoneFruit;
using StoneFruit.Containers.Ninject;

public static void Main(string[] args)
{
    var engine = new EngineBuilder()
        .SetupHandlers(h => h.UseNinjectHandlerSource())
        .Build();
    engine.RunInteractive();
}
```

When we execute this program we will see a prompt:

```
Enter command ('help' for help, 'exit' to quit):
> 
```

At the prompt we can type the `help` command to see what our options are:

```
Enter command ('help' for help, 'exit' to quit):
> help
env-change  Change environment if multiple environments are configured
env-list    Lists all available environments
exit        Exits the application
help        List all commands or get detailed information for a single command
my-first
Type 'help <command-name>' to get more information, if available.
```

Our handler `MyFirstHandler` has been registered with the verb `my-first`. If we type that at the prompt we can see the output:

```
> my-first
Starting the job...
Done.
```

## Change the Verb

We'd like to change the Verb we use with our handler. We can either change the class name or we can use the `VerbAttribute`.


```csharp
[Verb("do-work")]
public class MyFirstHandler : IHandler
```

Now when we execute `help` we'll see the new verb:

```
> help
env-change  Change environment if multiple environments are configured
env-list    Lists all available environments
exit        Exits the application
help        List all commands or get detailed information for a single command
do-work
Type 'help <command-name>' to get more information, if available.
```

## Add Some Help

We want to add a helpful description that will be displayed in the `help` command output. We can do that by adding a static `Description` property to our class:

```csharp
[Verb("do-work")]
public class MyFirstHandler : IHandler
{
    ...
    public static string Description => "Do some work on the database";
    ...
}
```

Now when we execute `help` we'll see the description:

```
> help
env-change  Change environment if multiple environments are configured
env-list    Lists all available environments
exit        Exits the application
help        List all commands or get detailed information for a single command
do-work     Do some work on the database
Type 'help <command-name>' to get more information, if available.
```

## Environments

The work that we need to do can be done in different environments. We have, for example, our local environment, a pre-production Testing environment and a Production environment. Each one of these environments has specific configurations, such as a connection string for a database. Each environment is going to have a config file with the name of that environment. We will have a configuration object and an environment factory to create instances for us from the list of files (You'll need to add these config files to your solution and set them to copy to the output directory on build):

```csharp
public class MyEnvironment
{
    private readonly IConfigurationRoot _config;

    public string Name { get; }

    public MyEnvironment(string name)
    {
        Name = name;
        _config = new ConfigurationBuilder().AddJsonFile($"Configs/{name}.json").Build();
    }

    public string DatabaseConnectionString => _config.GetSections("ConnectionStrings")["MyDatabase"];
}

public class MyEnvironmentFactory : IEnvironmentFactory
{
    private readonly IReadOnlyList<string> _environments;

    public MyEnvironmentFactory()
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        _environments = Directory.EnumerateFiles($"{baseDir}\\Configs\\")
            .Where(s => s.EndsWith(".json"))
            .Select(Path.GetFileName)
            .Select(s => s.Substring(0, s.Length - ".json".Length))
            .ToArray();
    }

    public object Create(string arg) => new MyEnvironment(arg);

    public IReadOnlyCollection<string> ValidEnvironments => _environments;
}
```

We upgrade our EngineBuilder to account for environments:

```csharp
var engine = new EngineBuilder()
        .SetupHandlers(h => h.UseNinjectHandlerSource())
        .SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
        .Build();
    engine.RunInteractive();
```

Now when we execute the application and enter the prompt, we are asked to select an environment before continuing:

```
Please select an environment:
1) Local
2) Testing
3) Production
> 1
Environment changed to Local
Enter command ('help' for help, 'exit' to quit):
Local>
```

Now, we can update our handler to take the environment object and get the connection string:

```csharp
public class MyFirstHandler : IHandler
{
    private readonly IOutput _output;
    private readonly MyEnvironment _env;

    public MyFirstHandler(IOutput output, MyEnvironment environment) 
    {
        _output = output;
        _env = environment
    }

    public void Execute()
    {
        _output.WriteLine("Starting the job...");
        var connectionString = _env.DatabaseConnectionString;
        // .. Connect to the DB and do work here ..
        _output.WriteLine("Done.");
    }
}
```

## Tips To Keep Going

1. Create your EntityFramework `DbContext` or other DB access objects inside your environment object, so it's always available when you need it
1. Add your program to your `%PATH%` (or `$PATH` on Linux) so you can easily access it from your terminal of choice