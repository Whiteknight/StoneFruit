# Simple Example

First let's create a handler to do some basic work:

```csharp
public class MyFirstHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        var output = context.Output;
        output.WriteLine("Starting the job...");
        // .. Do work here ..
        output.WriteLine("Done.");
    }
}
```

Now in our entry point, we can setup and run the application.

```csharp

```


```csharp
using StoneFruit;

public static void Main(string[] args)
{
    var stonefruit = StoneFruitApplicationBuilder.BuildDefault();
    stonefruit.RunWithCommandLineArguments();
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
my first
Type 'help <command-name>' to get more information, if available.
```

Our handler `MyFirstHandler` has been registered with the verb `my first`. If we type that at the prompt we can see the output:

```
> my first
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

The work that we need to do can be done in different environments. We have, for example, our local environment, a pre-production Testing environment and a Production environment. Each one of these environments has specific configurations, such as a connection string for a database. There are many ways we could configure these separate environments. For ease, we're going to use a named config file for each, which our system will detect and load. We will have a configuration object and an environment factory to create instances for us from the list of files (You'll need to add these config files to your solution and set them to copy to the output directory on build):

```csharp
// The environment class which will contain the current configs
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

public static void Main(string[] args)
{
    // Get a list of config files. These are our environments.
    var environments = Directory.EnumerateFiles($"{baseDir}\\Configs\\")
        .Where(s => s.EndsWith(".json"))
        .Select(Path.GetFileName)
        .Select(s => s.Substring(0, s.Length - ".json".Length))
        .ToArray()

    var builder = StoneFruitApplicationBuilder.Create();
    builder.SetupEnvironments(e => e
        .SetEnvironments(environments));

    // Register the MyEnvironment object for each individual environment.
    builder.Services.AddPerEnvironment((provider, name) => new MyEnvironment(name));
    var stonefruit = builder.Build();
    stonefruit.RunWithCommandLineArguments();
}
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

(you can select the environment at the prompt by name or number)

Now, we can update our handler to take the environment object in the constructor (through the magic of the DI container) and get the connection string:

```csharp
public class MyFirstHandler : IHandler
{
    private readonly MyEnvironment _env;

    public MyFirstHandler(MyEnvironment environment) 
    {
        _env = environment
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var output = context.Output;
        output.WriteLine("Starting the job...");
        var connectionString = _env.DatabaseConnectionString;
        // .. Connect to the DB and do work here ..
        output.WriteLine("Done.");
    }
}
```

## Tips To Keep Going

1. Create your EntityFramework `DbContext` or other DB access objects per environment, so they always use the current connection string
1. Or better yet, inject your current environment object into your `DbContext` to get the connection string directly, and inject the `DbContext` into your handler
1. Add your program to your `%PATH%` (or `$PATH` on Linux) so you can easily access it from your terminal of choice