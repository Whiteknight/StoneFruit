# StoneFruit

**StoneFruit** is a commandlet host and execution engine. It allows you to rapidly develop new command-line utility routines without having to create a new application for each one. StoneFruit is designed from the ground-up to be pluggable and flexible, and to easily integrate with your existing code, including DI/IoC containers if desired.

## Quick Example: Git

Git is a powerful development tool but typing "`git`" by itself at the prompt doesn't do much. To access the varied functionality of git, you must specify a **verb** like "`git clone`", "`git add`" or "`git rebase`". Each different verb causes the application to do different tasks, often with completely separate sets of input arguments and program output. StoneFruit is like this: It allows you to create a program with many verbs, each of which can do something different and useful.

## Basic Terminology

A **Verb** is the name of an action which the user types at the commandline, which may have arguments and output effects. Verbs in StoneFruit are case-insensitive

A **Handler** is a class which is invoked by StoneFruit when a verb is input. A single Handler class may correspond to one or more verbs.

A **Command** is a combination of a Verb and all input arguments.

**Headless** is a mode where the StoneFruit application executes one or more commands without user input.

**Interactive** is a mode where the StoneFruit application displays an interactive prompt where commands can be input one after the other.

## Quick Start

To get started, we need to create an `EngineBuilder`, configure it, and build an `Engine`:

```csharp
var engineBuilder = new EngineBuilder();
    // Setup the builder here
    // ...
var engine = engineBuilder.Build();
```

## Pages

* [Handlers](handlers.md)
* [Arguments](arguments.md)
* [DI Containers](containers.md)
* [Execution](execution.md)
* [Scripting](scripting.md)
