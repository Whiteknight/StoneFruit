# Arguments and Syntax

A **Command** is a combination of a **verb** and zero or more **arguments**. There are three types of arguments:

1. **Positional** arguments are unnamed values in the argument list and are accessed by position index. There can be many positional arguments, each one with a unique index (starting from 0). Positional arguments at the beginning of the argument list (before any flag or named arguments) may be used as a verb to dispatch control flow to a handler. Arguments which are used as Verbs are removed from the list of arguments before the handler is executed.
1. **Flag** arguments are defined by their name and can appear in any order. A flag does not have a value, but is instead is a boolean value, true if it exists and false otherwise. Only one flag of a given name can be passed for a single command. Passing the same flag more than once will be ignored.
1. **Named** arguments have a name and a value and can appear in any order. You can have multiple named arguments with the same name. Ordering of named arguments will not be preserved.

As an example, the `echo` built-in takes arguments of all three types:

```
echo -nonewline color=Red 'ERROR MESSAGE'
```

`-nonewline` is a flag which tells the echo handler not to write a newline at the end. The `color=Red` is a named argument called "color" with a value of "Red". Finally, the string `ERROR MESSAGE` is a positional argument which does not have a name.

## Argument Syntax

StoneFruit supports multiple different argument formats, which can be set in the builder. The default syntax is the "Simplified" version. In all syntaxes, positional arguments are the same: Either a bare word with no whitespace or a quoted value (single- or double-quoted, with backslash escapes).

### Simplified

Simplified syntax is the default, is the simplest, and has no parsing ambiguities:

* `-flag` is the flag "flag"
* `name=value` is the named argument with name "name" and value "value"
* `value` is the positional argument with the value "value"

Simplified argument grammar is the default, but you can specify it explicitly in your builder:

```csharp
builder.SetupHandlers(b => b
    .SetupArguments(a => a.UseSimplifiedArgumentParser())
);
```

An example of a command with simplified argument syntax is:

```bash
myverb value1 name=value2 -flag1
```

### Posix Style

While there is no hard-and-fast standard for "POSIX" applications, there is a general style among GNU and linux apps which StoneFruit can attempt to emulate with it's Posix-style parser.

* `-a` is the Flag "a"
* `-abc` are the three flags "a", "b", "c".
* `--abc` is the flag "abc".
* `--name=value` is a named argument "name" with value "value".
* `--name value` is an ambiguous case which might be the flag "name" followed by the positional "value", or it might be a named argument "--name=value".
* `-a value` is also an ambiguous case, which might be the flag "a" followed by the positional "value", or it might be a named argument "--a=value"

Ambiguities are resolved at handler execution time, depending on how the code attempts to access the values. If the handler asks for the named argument it will be treated like a named argument, otherwise if you ask for the flag and/or the positional, it will be treated as the combination of flag and positional.

```csharp
builder.SetupHandlers(b => b
    .SetupArguments(a => a.UsePosixStyleArgumentParser())
);
```

### Powershell Style

Powershell style also contains some ambiguities but it is simpler than POSIX-style:

* `-name` is the flag "name"
* `-name value` is ambiguous. It is either the flag "name" followed by the positional "value" or the named "name=value"

Again, ambiguities are resolved at runtime depending on how you access the arguments.

```csharp
builder.SetupHandlers(b => b
    .SetupArguments(a => a.UsePowershellStyleArgumentParser())
);
```

### Windows CMD Style

* `/flag` is a flag
* `/named:value` is a named argument

```csharp
builder.SetupHandlers(a => a.UseWindowsCmdArgumentParser());
```

### Custom Parsers

You can implement your own `IParser<char, IParsedArgument>` grammar for your own syntax if you want. See the documentation for [ParserObjects](https://github.com/Whiteknight/ParserObjects) for more details about how to create your own parser.

```csharp
builder.SetupHandlers(a => a.UseArgumentParser(myParser));
```

## Retrieving Argument Values

When you execute a command like `"my command arg1 arg2 name1=value1 name2=value2 ..."` StoneFruit will parse the string into an `IArguments` object and then pass that to the dispatcher. The dispatcher will pull positional arguments from the front of this list to try and find the longest sequence which matches a known verb. In the above case, let us assume that the verb turns out to be the two-word `"my command"`. These two values are pulled off the front, and we are left with a Verb `"my command"` and arguments `"arg1 arg2 name1=value1 name2=value2 ..."`. The verb is used by the dispatcher to find the appropriate handler, and then the handler is invoked with the `IArguments` with the remaining values.

An `IArgument` is a single value which is one of `PositionalArgument`, `NamedArgument` or `FlagArgument`. The first two of these have values, and the last two have names. An `IArgument` may be **consumed**. That is, if you use an argument and do not wish to see it again, marking it consumed will "hide" it from future accesses so you can process remaining arguments without confusion.

There are several ways to get an `IArgument`. All of these methods will return either the argument in question or a `MissingArgument` object:

```csharp
// Get the next unconsumed positional
var pos = args.Shift();

// Get the value at position i (starting with 0)
var pos = args.Get(1);

// Get hte value at position 1 and mark it consumed.
var pos = args.Consume(1)

// Get the first unconsumed argument with name "name"
// By default named values are case-sensitive
var named = args.Get("name");

// Get the first unconsumed argument with name "name"
// and mark it consumed.
var named = args.Consume("name");

// Get the flag by name. 
var flag = args.GetFlag("name");

// Get the flag by name and mark it consumed
var flag = args.ConsumeFlag("name");

// returns true if the flag exists, false otherwise
var ok = args.HasFlag("name");

// returns an enumerable of all remaining unconsumed positionals
var positionals = args.GetAllPositionals();

// returns an enumerable of all remaining unconsumed named arguments with "name"
var nameds = args.GetAllNamed("name");

// returns an enumerable of all remaining unconsumed named arguments
var nameds = args.GetAllNamed();
```

### `IArgument` Methods

Once you have an argument, you can operate on it.

```csharp
// Return the value as either a string, int or bool, according to standard C# parsing rules.
var val = arg.AsString();
var val = arg.AsInt();
var val = arg.AsBool();

// Transform the value according to a user-defined callback, or return a default value 
// of that type
var val = arg.As<T>(s => Transform(s));

// Require the argument or throw an exception if it does not exist.
arg.Require();

// Mark the argument as consumed so subsequent IArguments methods will not return it.
arg.MarkConsumed();

// Returns true if the arg was passed, false otherwise.
var ok = arg.Exists();
```

### `IHandler` Example

```csharp
public class MyHandler : IHandler
{
    private readonly IArguments _args;
    
    public MyHandler(IArguments args) 
    {
        _args = args;
    }

    public void Execute()
    {
        var arg1 = _args.Consume(0);
        var arg2 = _args.Consume("name1");
        var hasFlag = _args.HasFlag("flag1");
        ...
    }
}
```

### Mapping Objects

You can map an `IArguments` collection to the public properties of an object.

```csharp
// Create a new object and initialize properties
var myobj = args.MapTo<MyObject>();

// Fill property values into an existing object instance
args.MapOnTo(myobj);
```

Property mapping rules are as follows:
1. If the property has the `[ArgumentIndexAttribute(1)]` attribute, the mapper will attempt to `.Get(1)` the positional at that index and fill in the value.
2. If the property has the `[ArgumentNamedAttribute("name")]` attribute, the mapper will attempt to `.Get("name")` the named argument and fill in the value.
3. If the property has the same name (case-insensitive) as a named argument, it will get the value
4. If the proprety is a `bool` and there exists a flag with that name (case-insensitive), it will set the property to `true`.

### Instance Methods

In some cases, StoneFruit can automatically pull values out of `IArguments` and fill them into the arguments of a method. For example, if you use public instance method handlers (see [Handlers](/handlers.md) for more details) you can specify method parameters of mappable types:

```csharp
public class MyObject
{
    public void MyHandlerMethod(string arg1, string arg2, bool flag1)
    {
        ...
    }
}
```

For each parameter, if there is a named `IArgument` with the same name (case-insensitive) it will be filled in. Otherwise if it's a flag it will set a `bool` to true or it will take the next unconsumed positional argument.


