# Scripting

There are several methods of scripting in StoneFruit. These allow you to translate one command into one or more commands to execute in sequence, or to execute commands in response to specific events in the Engine.

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

## Scripts

A script is a collection of zero or more commands which are executed in response to a single input command. These are setup in the `EngineBuilder` and employ a sophisticated mechanism for translating arguments. Here is a simple example:

```csharp
engineBuilder
    .SetupHandlers(handlers => handlers
        .AddScript("say-hello-world", new [] { 
            "echo hello", 
            "echo world" 
        })
    )
    ;
```

### Script Arguments

Arguments in a script use the Simplified argument syntax (see the [Arguments])(arguments.md) page for more details) with extended placeholder syntax. Names of flags and named arguments, in all cases, can be quoted with single or double quotes.

#### Positional Arguments

Positional arguments can be fetched from the input using the `[]` syntax. An integer index will fetch and consume the argument with that index, while a `[*]` will fetch all remaining unconsumed positional arguments. Positional arguments can also be gotten from the values of named input arguments using the `[]` syntax with a string integer of an integer argument. Literal positional values will be passed as-is:

**Script**: `test [1] [*] [x] z`

**Command**: `myscript a b c x=y`

**Output**: `test b a c y z`

#### Flag Arguments

Flag arguments can be fetched and consumed from the input by name using the `?` syntax, or passed as literal flags with `-`

**Script**: `test ?a ?b -c`

**Command**: `myscript -a`

**Output**: `test -a -c`

#### Named Arguments

Named arguments can be fetched from the input in a few ways. They can be fetched and consumed with name and value using `{name}` syntax, all remaining named arguments can be fetched and consumed with `{*}` syntax, and values can fetched with literal names using `name=[]` syntax, where the argument can be an integer to get the value from a positional argument and a string to get the value of another named argument. Literal named values can be passed like normal.

**Script**: `test {a} b=['x'] c=[0] d=4 {*}`

**Command**: `myscript a=1 x=2 3 e=5`

**Output**: `test a=1 b=2 c=3 d=4 e=5`

## EventScripts

The StoneFruit `EngineState` contains a number of pre-defined scripts which are executed in response to various events. You can modify these scripts in the EngineBuilder to change the behavior of the application:

```csharp
    .SetupEvents(e => ...);
```

You can examine or modify the contents of these scripts within the context of the `EngineBuilder.SetupEvents()` method. This gives a way to customize behavior by setting up data or showing helpful information to users. Notice that some of these events are important to the operation of the system and errors in these scripts can create a fatal condition which will cause the engine to terminate.

For example, if you would like your application to display a custom message to the user when entering interactive mode, you can set it up like this:

```csharp
engineBuilder
    .SetupEvents(scriptCatalog => {
        scriptCatalog.EngineStartInteractive.Add("echo 'hello world!'");
    })
    ;
```

Event scripts use the same argument formatting syntax as normal scripts (described above). Scripts are available for the following events:

### Unhandled Errors

When a Handler throws an exception or there's an error during operation of the engine, the `EngineError` script will be executed with arguments derived from the exception object. The default script for this is:

```
echo color=Red ['message']
echo ['stacktrace']
```

If an exception is thrown while handling a previous exception, the Engine will show a message and exit to prevent infinite loops. Make sure you test your error-handling script to avoid abrupt exits like this.

### Start and Stop Headless Mode

A script is executed when the engine enters and exits headless mode. These scripts are `EngineStartHeadless` and `EngineStopHeadless` respectively. By default both these scripts are empty, but you can configure them to show helpful information to the user, to setup/cleanup contextual data, check credentials, or any number of other tasks.

### Start Interactive Mode

A script executes when the engine enters interactive mode, `EngineStartInteractive`. This script prompts the user for an environment if necessary and shows a brief welcome message. This is the default script:

```
env-change-notset
echo -nonewline Enter command
echo -nonewline color=DarkGray " ('help' for help, 'exit' to quit)"
echo ':'
```

A common usage of this script is to show additional information or even full "help" output to the user, or to setup contextual data before the user begins executing commands.

### Environment Changed

When the current environment changes, the `EnvironmentChanged` script is executed with the name of the environment as an argument. The default script is:

```
echo -noheadless Environment changed to ['environment']
```

### Headless Help

If the application is started in headless mode with just the "help" command, the `HeadlessHelp` script is executed. By default this script shows output of the help handler and then exits with the special headless-help error code:

```
help
exit ['exitcode']
```

You can modify this script to show any helpful information you want.

### HeadlessNoArgs

If the engine enters headless mode with no arguments, the `HeadlessNoArgs` script is executed. By default this script shows a brief error message and exits with the special no-verb exit code:

```
echo No command provided
exit ['exitcode']
```

Whatever you put in this script will be executed by default when no verbs are specified, so it's a good place to fill in some default commands (show help, etc). 

### Verb Not Found

If a verb is specified, in headless or interactive mode, the `VerbNotFound` script is executed. By defailt it shows a brief error message:

```
echo Verb ['verb'] not found. Please check your spelling or help output and try again.
```

### Maximum Headless Commands

StoneFruit will only execute a limited number of commands without user input, to avoid infinite loops. This can occur in recursive situations when a script references itself (directly or through an alias) or when scripts themselves start to get too long. In Interactive mode a prompt will be shown to the user asking if execution should continue. In headless mode, the `MaximumHeadlessCommands` script is executed instead. By default this script shows an error message about maximum limit exceeded and exits with a special exit code.

```
echo Maximum ['limit'] commands executed without user input. Terminating runloop.
exit ['exitcode']
```

The limit of commands to execute without user input can be set in the EngineBuilder. The default value is 20:

```csharp
engineBuilder
    .SetupSettings(s => {
        s.MaxInputlessCommands = 20;
    })
    ;
```

