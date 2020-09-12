# Arguments and Syntax

A **Command** is a combination of a **verb** and zero or more **arguments**. There are three types of arguments:

1. **Positional** arguments are unnamed values in the argument list and are accessed by index. There can be many positional arguments, each one with a unique index (starting from 0).
1. **Flag** arguments are defined by their name and can appear in any order. A flag does not have a value, but is instead defined by whether or not it exists. Only one flag of a given name can be passed for a single command. Passing the same flag more than once will be ignored.
1. **Named** arguments have a name and a value and can appear in any order. You can have multiple named arguments with the same name. Ordering of named arguments will not be preserved.

The `echo` built-in takes arguments of all three types:

```
echo -nonewline color=Red 'ERROR MESSAGE'
```

`-nonewline` is a flag which tells the echo handler not to write a newline at the end, which can be useful in some scripting scenarios. The `color=Red` is a named argument called "color" with a value of "Red". Finally, the string `ERROR MESSAGE` is a positional argument which does not have a name.

## Argument Syntax

StoneFruit supports multiple different argument formats, which can be set in the `IEngineBuilder`. The default syntax is the "Simplified" version. In all syntaxes, positional arguments are the same: Either a bare word with no whitespace or a quoted value (single- or double-quoted, with backslash escapes).

### Simplified

Simplified syntax is the default, is the simplest, and has no parsing ambiguities:

* `-flag` is the flag "flag"
* `name=value` is the named argument with name "name" and value "value"

Simplified argument grammar is the default, but you can specify it explicitly in your EngineBuilder:

```csharp
services.SetupEngine(b => b
    .SetupArguments(a => a.UseSimplifiedArgumentParser())
);
```

### Posix Style

"POSIX"-style is a little bit different from program to program, and has become quite complex. StoneFruit attempts to cover most of the common patterns from modern apps, though there are some ambiguities in the syntax:

* `-a` is the Flag "a", `-abc` are the three flags "a", "b", "c".
* `--abc` is the Flag "abc".
* `--name=value` is a named argument "name" with value "value".
* `--name value` is an ambiguous case which might be the flag "name" followed by the positional "value", or it might be a named argument "name=value"
* `-a value` is also an ambiguous case, which might be the flag "a" followed by the positional "value", or it might be a named argument "a=value"

Ambiguities are resolved at execution time, if you ask for the named argument it will be treated like a named argument, otherwise if you ask for the flag or the positional, it will be treated as the combination of flag and positional.

```csharp
services.SetupEngine(b => b
    .SetupArguments(a => a.UsePosixStyleArgumentParser())
);
```

### Powershell Style

Powershell style also contains some ambiguities but it is simpler than POSIX-style:

* `-name` is the flag "name"
* `-name value` is ambiguous. It is either the flag "name" followed by the positional "value" or it's the named "name=value"

Again, ambiguities are resolved at runtime depending on how you access the arguments.

```csharp
services.SetupEngine(b => b
    .SetupArguments(a => a.UsePowershellStyleArgumentParser())
);
```

### Windows CMD Style

* `/flag` is a flag
* `/named:value` is a named argument

```csharp
engineBuilder
    .SetupArguments(a => a.UseWindowsCmdArgumentParser())
    ;
```

### Custom Parsers

You can implement your own `IParser<char, IParsedArgument>` grammar for your own syntax if you want. See the documentation for [ParserObjects](https://github.com/Whiteknight/ParserObjects) for more details about how to create your own parser.

```csharp
engineBuilder
    .SetupArguments(a => a.UseArgumentParser(myParser))
    ;
```

## Retrieving Argument Values

TBD
