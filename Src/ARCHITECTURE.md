# StoneFruit Library Architecture

This document describes the overall, high-level architecture of the StoneFruit library. It will not generally go into individual classes or implementation details.

# Basic Definitions

* **User** The programmer of the downstream CLI application which uses StoneFruit
* **End-User** The user of the downstream CLI application, who enters commands into the application

# High Level Flow

1. The user builds an `Engine`, using `EngineBuilder` and (preferrably) the DI container of their choice.
2. The end-user starts the application, possibly in headless or interactive mode
3. The `Engine` starts a runloop.
   1. The runloop invokes `ICommandSource` to get the next command to execute
   2. The command is parsed into an `IArguments`, if necessary, using the `ICommandParser`
   3. The `IArguments` is passed to the `CommandDispatcher`
     1. The `CommandDispatcher` calls the `IHandlerSource` to find a matching handler
     2. The `CommandDispatcher` invokes the handler

At the highest level, StoneFruit accepts CLI commands from the user or a script, and dispatches those commands to an appropriate handler. For StoneFruit to be justified as a dependency, it must be extremely easy to setup an application to use StoneFruit and it must be very easy to add new handlers or modify existing handlers. Care should be taken to make sure that the interfaces and abstractions which the user interacts with are easy to use.

## Design Goals

StoneFruit should:

* Be extremely easy for a user to configure and execute an application.
* Be completely pluggable, so the advanced user may replace any component or any behavior as required.
* Provide sane and powerful defaults so the advanced user will not be required to replace components for most use-cases.
* Provide a small interface for most setup operations, so the user can easily explore options and settings.

# Code Map

This section will talk about some of the basic organizational ideas and tell where some important subsystems are located. Each sub-heading here is a folder/namespace in the `StoneFruit` solution.

## StoneFruit/

At the root of the `StoneFruit` project are the `Engine`, `EngineBuilder` and important abstractions. In general, the user should be able to build a complete StoneFruit application without any more preparation than `using StoneFruit;`.

## StoneFruit/Utility

This directory contains utility and cross-cutting code which is not specifically related to any particular subsystem. Extension methods, Guard methods, basic data structures and algorithms all live here.

## StoneFruit/Handlers

The built-in handler types live here. This list should be kept very small and simple, to avoid unnecessary collisions with user-defined verbs. These built-in handlers are the only ones which are guaranteed to exist, and the only ones which built-in scripts may make use of.

## StoneFruit/Execution

This folder and subfolders contains all the operational code for making StoneFruit run. This root directory contains several important objects used by the `EngineBuilder` and `Engine`, a few abstractions which are used internally, and several data objects for communicating state.

**Invariant**: `Engine` contains a runloop but should delegate quickly to other objects to implement most logic, because so much of the system is pluggable. `Engine` should only contain the most fundamental code which cannot be meaningfully overridden.

## StoneFruit/Execution/Arguments

This folder contains argument parsers/grammars, objects to represent parsed arguments, and setup code for configuring parsers. `ParserSetup` is used by `EngineBuilder` to configure parsers. The `CommandParser` uses the configured grammar to turn a command string into an `IArguments`.

**Invariant**: StoneFruit supports multiple syntaxes for specifying arguments. The system cannot make assumptions about which parser has been configured, or assume that the default parser will always be used. Certain syntaxes contain ambiguities in their grammar. It may not be possible in some parsers to determine the exact nature of arguments until they are accessed by the handler. The parser returns a raw, ambiguous format which may contain extra details, and these will be converted to definite argument values when they are accessed by the handler.

## StoneFruit/Execution/CommandSource

This folder contains implementations of `ICommandSource` and related code for getting a stream of commands for consumption by the `Engine`.

## StoneFruit/Execution/Environments

This folder contains classes for working with environments. An environment is an arbitrary user-defined object which serves as an execution context for a set of commands. Environments are configured during setup, may be constructed by a factory, and are held in an environment collection object.

## StoneFruit/Execution/Handlers

This folder contains code for working with handlers: extractors to get a verb from the handler class name and `IHandlerSource` implementations to hold lists of handlers and be able to return them by name.

## StoneFruit/Execution/Output

This folder contains logic for working with output vectors, including implementations of `IOutput`. For most cases, `IOutput` uses the console to display text. `OutputSetup` is used by the `EngineBuilder` to configure output vectors.

## StoneFruit/Execution/Scripts

This folder contains logic for working with scripts. The Script parser parses a format string into an accessor. An accessor turns an input `IArguments` into one or more output `IArguments`.

## StoneFruit.Containers.Autofac

The Autofac bindings

## StoneFruit.Containers.Lamar

The Lamar bindings

## StoneFruit.Containers.Microsoft

The Microsoft.Extensions.DependencyInjection bindings

## StoneFruit.Containers.StructureMap

The StructureMap bindings

# Scripts and Handlers

All handler types should generally be interchangable. Handlers which are implemented as classes, instance methods, or scripts should be able to be swapped for one another without any degredation in user experience or capability. 

# Error-Handling

Being completely modular, care must be taken to make sure error conditions are handled even in the presence of configuration or scripting errors by the user. Several error events will cause a user-modifiable script to be invoked, and those scripts may themselves trigger additional errors. 

Any system which may handle an error should also be able to detect recursion and avoid additional errors which occur during handling from creating an infinite loop. The exception-handling script should not be invoked again if it causes another execution to be thrown. The command-limit script should not recurse if it contains more commands than the limit. Etc.

# Testing

Because StoneFruit is designed to be completely modular, and because it can be hard to construct some of the core pieces without also instantating several dependencies, end-to-end functional tests are generally preferred over simple unit tests. The individual classes and methods are free to change so long as the user experience remains stable.

Extensive integration makes sure that all the defaults and invariants are sane, and also that add-ons work together with the system as a whole.

# Argument Parsing and Verb Dispatch

StoneFruit supports arguments in multiple formats so that a familiar and intuitive interface can be created for diverse teams. Unfortunately some of these syntaxes are inherently ambiguous. For example in a Unix-style system, the sequence `--foo bar` might be interpreted by the application as the flag `--foo` followed by the positional argument `bar`, or it may be a single argument `bar` with name `foo`. 

Because of the ambiguity, argument parsing is done in two phases. First arguments are parsed into a raw form which preserves ambiguities if they exist. Second arguments are identified unambiguously when a handler attempts to access them. From the example above, if we have the ambiguous sequence `--foo bar`, the user can either request the named argument `foo` or it can search for a flag `--foo` (which marks `bar` as a positional) or it can search for the next positional argument (which returns `bar` and marks `--foo` unambiguously as a flag).

When a user send an input command to the engine, the following steps happen:

1. The string is parsed into an `IArguments` which maintains any ambiguities
2. A list of leading positionals (unambiguous positionals which are not preceeded by a named or flag argument) are pulled out as Verb Candidates
3. The Verb Candidates are used to try and find a best match in the `IHandlerSource`s
4. When a best-match handler is found, the matching Verb Candidates are converted into a Verb, and remaining candidates are returned to the front of the `IArguments` as positionals. Positionals are renumbered to ignore the Verb
5. When the handler executes, it will access arguments which will disambiguate them permanently. 

