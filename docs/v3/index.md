# StoneFruit

**StoneFruit** is a commandlet host and execution engine. It allows you to rapidly develop new command-line utility routines without having to create a new application for each one. StoneFruit is designed from the ground-up to be configurable, pluggable, flexible, and to easily integrate with your existing code. It integrates with DI to allow deep customization and injection of all your business types.

## Quick Example: Git

Git is a powerful development tool but typing "`git`" by itself at the prompt doesn't do much. To access the varied functionality of git, you must specify a **verb** like "`git clone`", "`git add`" or "`git remote add`". Each different verb causes the application to do different tasks, often with completely separate sets of input arguments and program output. **This is what StoneFruit is**: It allows you to create a program with many verbs, each of which can do something different and useful.

## Basic Terminology

A **Verb** is the name of an action which the user types at the command-line, which may have arguments and output effects. Verbs in StoneFruit may be one or several words.

A **Handler** is a class or delegate which is invoked by StoneFruit in response to an input verb.

A **Command** is a combination of a Verb and all input arguments.

**Headless** is a mode where the StoneFruit application executes one or more commands without user input.

**Interactive** is a mode where the StoneFruit application displays an interactive prompt where commands can be input one after the other.

## Quick Start

To get started, we need to create a `StoneFruitApplicationBuilder`, configure it, and build a `StoneFruitApplication`. To get started immediately with default options:

```csharp
var stonefruit = StoneFruitApplicationBuilder.BuildDefault();
stonefruit.RunWithCommandLineArguments();
```

To have more control over your setup and access advanced features:


```csharp
var builder = StoneFruitApplicationBuilder.Create();
... configure builder here ...
var stonefruit = builder.Build();
stonefruit.RunWithCommandLineArguments();
```

## Pages

* [Overview](overview.md)
* [Handlers](handlers.md)
* [Arguments](arguments.md)
* [Environments](environments.md)
* [Execution](execution.md)
* [Scripting](scripting.md)
* [Simple Example](example1.md)

