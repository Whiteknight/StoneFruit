# Input and Output

StoneFruit provides wrappers around `System.Console` in the form of `IInput` and `IOutput`. These interfaces are simple, and you very likely may need to construct your own I/O objects for more advanced use cases.

## Input

The `IInput` object has one basic method: `.Prompt()`. This method allows you to request an input from the user.

## Output

The `IOutput` object is more feature-full, allowing writing values to the console, changing colors, and doing basic template formatting.

### Basic Outputs

`.Write()`
`.WriteLine()`

### Colors

```csharp
output.Color(ConsoleColor.Red).WriteLine("test");
```

### Templating

StoneFruit provides a very simple implementation of a template syntax similar to Go `template`. With this template feature you can format an object using template strings and write it to the output.

```csharp
output.WriteLineFormatted("{{ .Property1 }} {{ .Property2 }}", new MyObject());
```

