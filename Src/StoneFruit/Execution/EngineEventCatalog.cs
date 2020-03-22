using StoneFruit.Handlers;

namespace StoneFruit.Execution
{
    public class EngineEventCatalog
    {
        public EngineEventCatalog()
        {
            // When there's an unhandled exception caught by the engine
            EngineError = new EventScript(
                $"{EchoHandler.Name} color=Red ['message']",
                $"{EchoHandler.Name} ['stacktrace']"
            );

            // Events when the engine starts and then stops headless mode
            EngineStartHeadless = new EventScript();
            EngineStopHeadless = new EventScript();

            // Events when the engine starts and stops interactive mode
            EngineStartInteractive = new EventScript(
                // Call the env-change command to make sure we have an environment set
                $"{EnvironmentChangeHandler.NotSetName}",

                // Show a quick helpful message
                $"{EchoHandler.Name} -nonewline Enter command ",
                $"{EchoHandler.Name} -nonewline color=DarkGray \" ('help' for help, 'exit' to quit)\"",
                $"{EchoHandler.Name} ':'"
            );
            EngineStopInteractive = new EventScript();
            
            // Event when the environment has been successfully changed
            // Has argument 'environment' with name of new environment
            EnvironmentChanged = new EventScript(
                $"{EchoHandler.Name} Environment changed to ['environment']"
            );

            // We're executing basic help command headlessly
            // Has argument 'exitcode' with the intended exit code 
            HeadlessHelp = new EventScript(
                $"{HelpHandler.Name}",

                // Call 'exit' explicitly so we can set the exit code
                $"{ExitHandler.Name} ['exitcode']"
            );

            // Attempt to enter headless mode without providing any arguments
            // Has argument 'exitcode' with the intended exit code
            HeadlessNoArgs = new EventScript(
                $"{EchoHandler.Name} No command provided",

                // Call 'exit' so we can set an explicit error exit code
                $"{ExitHandler.Name} ['exitcode']"
            );

            // Attempt to execute an unknown verb
            // Has argument 'verb' with the name of the verb
            VerbNotFound = new EventScript(
                $"{EchoHandler.Name} Verb ['verb'] not found. Please check your spelling or help output and try again."
            );
        }

        /// <summary>
        /// The engine has been run in headless mode without a command.
        /// </summary>
        public EventScript HeadlessNoArgs { get; }

        /// <summary>
        /// The engine has started in headless mode. This script is run, followed by the
        /// input command, and then the EngineStopHeadless script.
        /// </summary>
        public EventScript EngineStartHeadless { get; }

        /// <summary>
        /// The engine has stopped headless mode. This script is executed after EngineStartHeadless script
        /// and the user command.
        /// </summary>
        public EventScript EngineStopHeadless { get; }

        /// <summary>
        /// The engine has started in interactive mode. This script will execute, then the user will be
        /// shown a REPL prompt.
        /// </summary>
        public EventScript EngineStartInteractive { get; }

        /// <summary>
        /// The engine has ended interactive mode. This script will not usually be called because most of
        /// the time the REPL will exit directly.
        /// </summary>
        public EventScript EngineStopInteractive { get; }
        
        /// <summary>
        /// There is no handler for the specified verb.
        /// </summary>
        public EventScript VerbNotFound { get; }

        /// <summary>
        /// The environment has been changed
        /// </summary>
        public EventScript EnvironmentChanged { get; }

        /// <summary>
        /// "help" has been executed in headless mode. Can be used to show more detail than a simple help
        /// handle. Notice that this script may be run without a valid environment set.
        /// </summary>
        public EventScript HeadlessHelp { get; }

        /// <summary>
        /// An unhandled exception has been received by the engine. The exception may have come from engine
        /// internals or from within one of the handlers.
        /// </summary>
        public EventScript EngineError { get; }
    }
}