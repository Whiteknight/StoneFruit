# Modes of Execution

The engine provides several methods to begin execution. You can select either interactive mode or headless mode explicitly, or you can let the engine decide based on the presence or absence of command-line arguments.

## Interactive Mode

If you call `engine.RunInteractively()` the engine will be started in interactive mode and will ignore any commands on the commandline. If you support environments and call `engine.RunInteractively(envName)` the engine will start interactive mode with the specified environment active. If your application supports multiple environments and you enter interactive mode without an environment set, the engine will prompt you to select an environment before showing you the prompt.

If you `.Run()` your engine without command-line arguments, or if you support environments and specify only the name of an environment on the commandline, the engine will be started in interactive mode. You can enter commands at the prompt, and can type "exit" or "quit" to exit the loop. 

### Scripting

In interactive mode, the Engine will first execute the `EngineStartInteractive` script, then it will prompt the user for input, and then it will execute the `EngineStopInteractive` script. See the page on [Scripting](scripting.md) for more details.

## Headless Mode

If you call `engine.RunHeadlessWithCommandLineArgs()` it will always run in headless mode using the commandline arguments as the only input. If a valid command is not present, an error will be shown.

If you call `engine.RunHeadless(cmd)` it will always run in headless mode using the given string as the only command input. If the string is null or empty, or contains an invalid verb, an error will be shown.

If you call `engine.Run()` with command on the commandline your application will start in headless mode. If you support environments, the first argument must be the name of the environment to use, followed by the verb and arguments. If you do not support environments, the first argument should be the verb followed by the arguments. If there is no valid command on the commandline, `engine.Run()` will choose interactive mode instead.

```bash
mystonefruitapp Production echo 'test'
mystonefruitapp echo 'test'
```

### Scripting

In headless ode, the Engine will first execute the `EngineStartHeadless` script, then it will execute the command from the commandline, and then it will execute the `EngineStopHeadless` script. See the page on [Scripting](scripting.md) for more details.

### Headless Help

If you call `engine.RunHeadless("help")` or `engine.RunHeadlessWithCommandLineArgs()` where `help` is the only commandline argument, StoneFruit will enter "headless help" mode. It will execute the `HeadlessHelp` script (see [Scripting](scripting.md) for more details) and will exit immediately. You can configure this behavior by modifying the contents of the `HeadlessHelp` script in the EngineBuilder.