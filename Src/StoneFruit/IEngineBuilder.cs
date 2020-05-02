using System;
using StoneFruit.Execution;

namespace StoneFruit
{
    public interface IEngineBuilder
    {
        /// <summary>
        /// Setup verbs and their handlers
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupHandlers(Action<IHandlerSetup> setup);

        /// <summary>
        /// Setup environments, if any
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupEnvironments(Action<IEnvironmentSetup> setup);

        /// <summary>
        /// Setup argument parsing and handling
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupArguments(Action<IParserSetup> setup);

        /// <summary>
        /// Setup output
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupOutput(Action<IOutputSetup> setup);

        /// <summary>
        /// Setup the scripts which are executed in response to various events
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupEvents(Action<EngineEventCatalog> setup);

        /// <summary>
        /// Setup the settings for the engine
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupSettings(Action<EngineSettings> setup);
    }
}