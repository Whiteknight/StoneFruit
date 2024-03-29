# StoneFruit

**StoneFruit** is a commandlet host and execution engine. It allows you to rapidly develop new command-line utility routines without having to create a new application for each one. StoneFruit is designed from the ground-up to be configurable, pluggable and flexible, and to easily integrate with your existing code. It deeply integrates with your DI container, if you have one, to make all your application services available at every point of the execution pipeline.

## Quick Example: Git

Git is a powerful development tool but typing "`git`" by itself at the prompt doesn't do much. To access the varied functionality of git, you must specify a **verb** like "`git clone`", "`git add`" or "`git remote add`". Each different verb causes the application to do different tasks, often with completely separate sets of input arguments and program output. StoneFruit is like this: It allows you to create a program with many verbs, each of which can do something different and useful.

## Basic Terminology

A **Verb** is the name of an action which the user types at the commandline, which may have arguments and output effects. Verbs in StoneFruit are case-insensitive words.

A **Handler** is a class which is invoked by StoneFruit when a verb is input. A single Handler class may correspond to one or more verbs.

A **Command** is a combination of a Verb and all input arguments.

**Headless** is a mode where the StoneFruit application executes one or more commands without user input.

**Interactive** is a mode where the StoneFruit application displays an interactive prompt where commands can be input one after the other.

## Quick Start

To get started, we need to create an `EngineBuilder`, configure it, and build an `Engine`. This is done in slightly different ways depending on whether you're using a DI container or not, and which one.

* [Lamar Quick Start](start_lamar.md)
* [StructureMap Quick Start](start_structuremap.md)
* [Microsoft DI Quick Start](start_microsoft.md)
* [No DI Quick Start](start_basic.md)

## Pages

* [Handlers](handlers.md)
* [Arguments](arguments.md)
* [DI Containers](containers.md)
* [Execution](execution.md)
* [Scripting](scripting.md)
* [Simple Example](example1.md)
