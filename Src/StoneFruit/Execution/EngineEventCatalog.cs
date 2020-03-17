﻿using StoneFruit.Handlers;

namespace StoneFruit.Execution
{
    public class EngineEventCatalog
    {
        public EngineEventCatalog()
        {
            // When there's an unhandled exception caught by the engine
            EngineError = new EventScript(
                $"{ShowExceptionHandler.Name}"
            );

            // Events when the engine starts and then stops headless mode
            EngineStartHeadless = new EventScript();
            EngineStopHeadless = new EventScript();

            // Events when the engine starts and stops interactive mode
            EngineStartInteractive = new EventScript(
                // Call the env-change command to make sure we have an environment set
                $"{EnvironmentChangeHandler.NotSetName}"
            );
            EngineStopInteractive = new EventScript();
            
            // Event when the environment has been successfully changed
            EnvironmentChanged = new EventScript();

            // We're executing basic help command headlessly
            HeadlessHelp = new EventScript(
                $"{HelpHandler.Name}",
                // Call 'exit' explicitly so we can set the exit code
                $"{ExitHandler.Name} {Constants.ExitCodeHeadlessHelp}"
            );

            // Attempt to enter headless mode without providing any arguments
            HeadlessNoArgs = new EventScript(
                $"{EchoHandler.Name} 'Please provide a verb'",
                // Call 'exit' so we can set an explicit error exit code
                $"{ExitHandler.Name} {Constants.ExitCodeHeadlessNoVerb}"
            );

            // TODO: It would be nice to be able to pass the name of the unknown verb here, so we could
            // make suggestions or give more insight
            // Attempt to execute an unknown verb
            VerbNotFound = new EventScript(
                $"{EchoHandler.Name} 'Verb not found. Please check your spelling or help output and try again.'"
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