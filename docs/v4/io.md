# Input and Output

StoneFruit provides wrappers around `System.Console` in the form of `IInput` and `IOutput`. These interfaces are simple, and you very likely may need to construct your own I/O objects for more advanced use cases.

## Input

The `IInput` object has one basic method: `.Prompt()`. This method allows you to request an input from the user with a text prompt. The default implementation will loop until a valid response is given.

## Output

The `IOutput` object is more feature-full, allowing writing values to the console, changing colors, and doing basic template formatting. The basic method is `.WriteMessage(OutputMessage message)` which can take several options and configurations.

* `.IsError` if true, the text will default to a Red color. If Standard Error has been redirected to a pipe, color will be omitted and the text will be written to the `Console.Error`.
* `.IncludeNewline` if true, includes a trailing newline on the output.
* `.IsTemplate` if true, treats output like a template to parsed and rendered, see below.

### Basic Outputs

`.Write()`
`.WriteLine()`

### Colors

```csharp
output.WriteLine("test", ConsoleColor.Red);
```

### Templating

StoneFruit provides a very simple implementation of a template syntax similar to Go `template`. With this template feature you can format an object using template strings and write it to the output.

```csharp
output.WriteLineFormatted("{{ .Property1 }} {{ .Property2 }}", new MyObject());
```

* `{{color fg}}` or `{{color fg,bg}}` changes the color of text going forward to the provided foreground color. If a background color is provided, use that as well. 
* `{{color}}` with no colors specified will revert back to the default colors
* `{{ ...pipeline... }}` a pipeline is a series of selectors which allow object navigation. A tag with just a pipeline will output the value of the property at that location.
* `{{if pipeline}}...{{end}}` if the given pipeline has a non-null, non-empty value, output the text between the `if` and `end` tags. If the pipeline returns a null or empty value, nothing is output.
* `{{if pipeline}}...{{else}}...{{end}}` outputs either the first or second block of text, depending on whether the pipeline returns a null or empty value or not.
