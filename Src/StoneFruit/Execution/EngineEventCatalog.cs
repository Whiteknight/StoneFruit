using StoneFruit.Execution.Environments;
using StoneFruit.Handlers;

// For purposes of this file, readability increases if we allow space between lines of scripts
#pragma warning disable SA1114 // Parameter list should follow declaration

// Documentation here doesn't follow standards because we want to explain the purpose of the
// script, not the purpose of the getter
#pragma warning disable SA1623 // Property summary documentation should match accessors

namespace StoneFruit.Execution;

/// <summary>
/// Catalog of scripts to run in response to various Engine events.
/// </summary>
public class EngineEventCatalog
{
    /// <summary>
    /// The engine has been run in headless mode without a command. This is the default
    /// behavior when no arguments are provided.
    /// </summary>
    public EventScript HeadlessNoArgs { get; } = new EventScript(
        // Has argument 'exitcode' with the intended exit code
        $"{EchoHandler.Name} No command provided",

        // Call 'exit' so we can set an explicit error exit code
        $"{ExitHandler.Name} ['exitcode']"
    );

    /// <summary>
    /// The engine has started in headless mode. This script is run, followed by the
    /// input command, and then the EngineStopHeadless script.
    /// </summary>
    public EventScript EngineStartHeadless { get; } = new EventScript();

    /// <summary>
    /// The engine has stopped headless mode. This script is executed after
    /// EngineStartHeadless script and the user command.
    /// </summary>
    public EventScript EngineStopHeadless { get; } = new EventScript();

    /// <summary>
    /// The engine has started in interactive mode. This script will execute, then the
    /// user will be shown a REPL prompt.
    /// </summary>
    public EventScript EngineStartInteractive { get; } = new EventScript(
        // Call the env-change command to make sure we have an environment set
        $"{EnvironmentHandler.Name} -notset",

        // Show a quick helpful message
        $"{EchoHandler.Name} -nonewline Enter command",
        $"{EchoHandler.Name} -nonewline color=DarkGray \" ('help' for help, 'exit' to quit)\"",
        $"{EchoHandler.Name} ':'"
    );

    /// <summary>
    /// There is no handler for the specified verb.
    /// </summary>
    public EventScript VerbNotFound { get; } = new EventScript(
        // Has argument 'verb' with the name of the verb
        $"{EchoHandler.Name} Verb ['verb'] not found. Please check your spelling or help output and try again.",
        $"{HelpHandler.Name} -startswith ['verb']"
    );

    /// <summary>
    /// The environment has been changed.
    /// </summary>
    public EventScript EnvironmentChanged { get; } = new EventScript(
        // Has argument 'environment' with name of new environment
        $"{EchoHandler.Name} -noheadless Environment changed to ['environment']"
    );

    /// <summary>
    /// "help" has been executed in headless mode. Can be used to show more detail than
    /// a simple help call. Notice that this script may be run without a valid
    /// environment set.
    /// </summary>
    public EventScript HeadlessHelp { get; } = new EventScript(
        $"{HelpHandler.Name}",

        // Call 'exit' explicitly so we can set the exit code
        // Has argument 'exitcode' with the intended exit code
        $"{ExitHandler.Name} ['exitcode']"
    );

    /// <summary>
    /// An unhandled exception has been received by the engine. The exception may have
    /// come from engine internals or from within one of the handlers.
    /// </summary>
    public EventScript EngineError { get; } = new EventScript(
        // Contains the exception message and exception stack trace as arguments
        $"{EchoHandler.Name} color=Red ['message']",
        $"{EchoHandler.Name} -ignoreempty ['stacktrace']"
    );

    /// <summary>
    /// A maximum number of commands have been executed without user input.
    /// </summary>
    public EventScript MaximumHeadlessCommands { get; } = new EventScript(
        // Has argument 'limit' with the number of commands specified in the Settings, and
        // 'exitcode' with the intended exit code value
        // Notice that the command counter is reset before executing this script, but it
        // is not disabled. This script cannot contain more than limit entries
        $"{EchoHandler.Name} Maximum ['limit'] commands executed without user input. Terminating runloop.",
        $"{ExitHandler.Name} ['exitcode']"
    );
}
