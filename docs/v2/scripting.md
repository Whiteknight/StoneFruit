# Scripting

There are several methods of scripting in StoneFruit. These allow you to translate one command into one or more commands to execute in sequence, or to execute commands in response to specific events in the Engine.

## Scripts

A script is a collection of zero or more commands which are executed in response to a single input command. These are setup in the `IEngineBuilder` and employ a sophisticated mechanism for translating arguments. Here is a simple example:

```csharp
services.SetupEngine(b => b
    .SetupHandlers(handlers => handlers
        .AddScript("say-hello-world", new [] { 
            "echo hello", 
            "echo world" 
        })
    )
);
```

### Script Arguments

Arguments in a script use the Simplified argument syntax (see the [Arguments])(arguments.md) page for more details) with extended placeholder syntax. Names of flags and named arguments, in all cases, can be quoted with single or double quotes and are not case-sensitive.

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

Named arguments can be fetched from the input in a few ways. They can be fetched and consumed with name and value using `{name}` syntax, all remaining named arguments can be fetched and consumed with `{*}` syntax, and positional arguments can be given names using the  `name=[]` syntax. Literal named values can be passed like normal.

**Script**: `test {a} b=['x'] c=[0] d=4 {*}`

**Command**: `myscript a=1 x=2 3 e=5`

**Output**: `test a=1 b=2 c=3 d=4 e=5`

#### Required Args and Default Values

Most of the preceeding argument types can be marked as being required or a default value can be provided if the argument is missing. The `!` syntax marks the argument as required. If a required argument is not provided, an error will be thrown and the script will be aborted. The `!<value>` syntax will denote that the default value will be used if the argument is missing.

**Script**: `test [0]!first ['a']! {b}!second`

In this example, if there are no positionals, the value `first` will be passed as the first positional. If the named value `"b"` is not provided, the argument `b=second` will be passed. If the named value `"a"` is not provided, an exception will be thrown and the script will not be executed. Notice that this mechanism doesn't preclude the `test` handler from throwing it's own exceptions if values are missing, or providing it's own default values. 

## EventScripts

The StoneFruit `EngineState` contains a number of pre-defined scripts which are executed in response to various events. You can modify these scripts in the EngineBuilder to change the behavior of the application:

```csharp
services.SetupEngine(b => b
    .SetupEvents(e => ...)
);
```

You can examine or modify the contents of these scripts within the context of the `EngineBuilder.SetupEvents()` method. This gives a way to customize behavior by setting up data or showing helpful information to users. Notice that some of these events are important to the operation of the system and errors in these scripts can create a fatal condition which will cause the engine to terminate.

For example, if you would like your application to display a custom message to the user when entering interactive mode, you can set it up like this:

```csharp
services.SetupEngine(b => b
    .SetupEvents(scriptCatalog => {
        scriptCatalog.EngineStartInteractive.Add("echo 'hello world!'");
    })
);
```

Event scripts use the same argument formatting syntax as normal scripts (described above), and some scripts take arguments depending on the event. Scripts are available for the following events:

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
env -notset
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

If an invalid verb is specified, in headless or interactive mode, the `VerbNotFound` script is executed. By default it shows a brief error message:

```
echo Verb ['verb'] not found. Please check your spelling or help output and try again.
```

### Maximum Headless Commands

StoneFruit will only execute a limited number of commands without user input, to avoid infinite loops. This can occur in recursive situations when a script references itself, when scripts themselves start to get too long, or when handlers invoke other handlers in sequence. In Interactive mode a prompt will be shown to the user asking if execution should continue. In headless mode, the `MaximumHeadlessCommands` script is executed instead. By default this script shows an error message about maximum limit exceeded and exits with a special exit code.

```
echo Maximum ['limit'] commands executed without user input. Terminating runloop.
exit ['exitcode']
```

The limit of commands to execute without user input can be set in the EngineBuilder. The default value is 20:

```csharp
services.SetupEngine(b => b
    .SetupSettings(s => {
        s.MaxInputlessCommands = 20;
    })
);
```

