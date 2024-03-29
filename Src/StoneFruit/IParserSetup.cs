﻿using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts.Formatting;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Setup argument parsing
    /// </summary>
    public interface IParserSetup
    {
        /// <summary>
        /// Set the Command parser instance. Notice that you may not set a Command parser
        /// at the same time as Verb/Argument/Script parsers. If null is passed, the default
        /// command parser will be used.
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        IParserSetup UseParser(ICommandParser parser);

        /// <summary>
        /// Specify an argument parser to use. Notice that you cannot set an Argument
        /// parser if you specify a Command parser. If null is passed the default argument
        /// parser will be used.
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        IParserSetup UseArgumentParser(IParser<char, IParsedArgument> argParser);

        /// <summary>
        /// Specify a parser to use for scripts. Notice that you cannot set a Script
        /// parser if you specify a Command parser. If null is passed the default script parser
        /// will be used.
        /// </summary>
        /// <param name="scriptParser"></param>
        /// <returns></returns>
        IParserSetup UseScriptParser(IParser<char, CommandFormat> scriptParser);
    }

    public static class ParserSetupExtensions
    {
        /// <summary>
        /// Use the StoneFruit simplified argument syntax.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IParserSetup UseSimplifiedArgumentParser(this IParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(SimplifiedArgumentGrammar.GetParser());
        }

        /// <summary>
        /// Use a POSIX-style argument syntax similar to many existing POSIX utilities.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IParserSetup UsePosixStyleArgumentParser(this IParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(PosixStyleArgumentGrammar.GetParser());
        }

        /// <summary>
        /// Use a Windows PowerShell syntax for arguments.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IParserSetup UsePowershellStyleArgumentParser(this IParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());
        }

        /// <summary>
        /// Use a class Windows-CMD syntax for arguments.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IParserSetup UseWindowsCmdArgumentParser(this IParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());
        }
    }
}
