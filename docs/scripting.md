# Scripting

There are several methods of scripting in StoneFruit. These allow you to translate one command into one or more commands to execute in sequence, or to execute commands in response to specific events in the Engine.

## EventScripts

The StoneFruit `EngineState` contains a number of pre-defined scripts which are executed in response to various events.

```csharp
    .SetupEvents(e => ...);
```

Scripts are executed in the following scenarios:

1. An unhandled exception is thrown by a handler (`EngineError`)
1. Headless mode is entered or exited (`EngineStartHeadless`, `EngineStopHeadless`)
1. Interactive mode is intered or exited (`EngineStartInteractive`, `EngineStopInteractive`)
1. The current environment is changed
1. Headless mode is entered without a valid command to execute
1. A verb is specified which is not known to the system
1. The single "help" argument is specified on the commandline in headless mode.

You can examine or modify the contents of these scripts within the context of the `EngineBuilder.SetupEvents()` method. This gives a way to customize behavior by setting up data or showing helpful information to users. Notice that some of these events are important to the operation of the system and errors in these scripts can create a fatal condition which will cause the engine to terminate.

For example, if you would like your application to display a custom message to the user when entering interactive mode, you can set it up like this:

```csharp
engineBuilder
    .SetupEvents(scriptCatalog => {
        scriptCatalog.EngineStartInteractive.Add("echo 'hello world!'");
    })
    ;
```

## Verb Aliases

Aliases allow you to invoke a single handler with multiple different verbs. An alias maps one verb to another one. When you use an alias, the verb is translated first, with the input alias and target verb both stored in the `Command` argument, before the handler is dispatched. You can setup aliases in your `EngineBuilder`:

```csharp
engineBuilder
    .SetupHandlers(handlers => handlers
        .AddAlias("echo", "test")
    )  
    ;
```

Now when you pass a command `test 'hello world'` the `echo` handler will be invoked with the given arguments. Inside your handler you can inspect the contents of the `Command` object to determine if an alias was used and possibly change behavior. For example, we can create a command that echos text to the output normally, but converts it to upper-case if we invoke it with the `yell` alias:

```csharp
[Verb("say")]
public class SayHandler : IHandler
{
    private readonly Command _command;
    private readonly IOutput _output;

    public SayHandler(Command command, IOutput output)
    {
        _command = command;
        _output = output;
    }

    public void Execute()
    {
        var text = _command.Arguments.Shift().AsString();
        if (command.Alias == "yell")
            text = text.ToUpperInvariant();
        _output.WriteLine(text);
    }
}
```

Aliases are only resolved once per command, so you cannot have one alias reference another alias. Notice you can also apply multiple `[Verb()]` attributes to a single handler, which will produce almost the same effect.

## Script Handlers

A script handler is a collection of zero or more commands which are executed in response to a single input command. These are setup in the `EngineBuilder` and employ a sophisticated mechanism for translating arguments. You can setup scripts in the `EngineBuilder`, here is a simple example:

```csharp
engineBuilder
    .SetupHandlers(handlers => handlers
        .AddScript("my-verb", new [] { 
            "echo hello", 
            "echo world" 
        })
    )
    ;
```

### Script Arguments

Arguments in a script use the Simplified argument syntax (see the [Arguments])(arguments.md) page for more details) with extended placeholder syntax. 

Positional arguments can be fetched from the input using the `[]` syntax. An integer index will fetch and consume the argument with that index, while a `[*]` will fetch all remaining unconsumed positional arguments. Positional arguments can also be gotten from the values of named input arguments using the `[]` syntax with a string integer of an integer argument. Literal positional values will be passed as-is:

**Script**
`test [1] [*] [x] z`
**Command**
`myscript a b c x=y`
**Output**
`test b a c y z`

Flag arguments can be fetched and consumed from the input by name using the `?` syntax, or passed as literal flags with `-`

**Script**
`test ?a ?b -c`
**Command**
`myscript -a`
**Output**
`test -a -c`

Named arguments can be fetched from the input in a few ways. They can be fetched and consumed with name and value using `{name}` syntax, all remaining named arguments can be fetched and consumed with `{*}` syntax, and values can fetched with literal names using `name=[]` syntax, where the argument can be an integer to get the value from a positional argument and a string to get the value of another named argument. Literal named values can be passed like normal.

**Script**
`test {a} b=['x'] c=[0] d=4 {*}`
**Command**
`myscript a=1 x=2 3 e=5`
**Output**
`test a=1 b=2 c=3 d=4 e=5`