using System.Collections.Generic;
using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts.Formatting;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Setup argument parsing
    /// </summary>
    public interface IArgumentParserSetup
    {
        /// <summary>
        /// Set the Command parser instance. Notice that you may not set a Command parser
        /// at the same time as Verb/Argument/Script parsers.
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseParser(ICommandParser parser);

        /// <summary>
        /// Specify a parser to use for verbs. Notice that you cannot set a Verb parser
        /// if you specify a Command parser.
        /// </summary>
        /// <param name="verbParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseVerbParser(IParser<char, string> verbParser);

        /// <summary>
        /// Specify an argument parser to use. Notice that you cannot set an Argument
        /// parser if you specify a Command parser.
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseArgumentParser(IParser<char, IParsedArgument> argParser);

        /// <summary>
        /// Specify an argument parser to use. Notice that you cannot set an Argument
        /// parser if you specify a Command parser.
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseArgumentParser(IParser<char, IEnumerable<IParsedArgument>> argParser);

        /// <summary>
        /// Specify a parser to use for scripts. Notice that you cannot set a Script
        /// parser if you specify a Command parser.
        /// </summary>
        /// <param name="scriptParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseScriptParser(IParser<char, CommandFormat> scriptParser);
    }

    public static class ParserSetupExtensions
    {
        /// <summary>
        /// Use the StoneFruit simplified argument syntax
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UseSimplifiedArgumentParser(this IArgumentParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(SimplifiedArgumentGrammar.GetParser());
        }

        /// <summary>
        /// Use a POSIX-style argument syntax similar to many existing POSIX utilities
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UsePosixStyleArgumentParser(this IArgumentParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(PosixStyleArgumentGrammar.GetParser());
        }

        /// <summary>
        /// Use a Windows PowerShell syntax for arguments
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UsePowershellStyleArgumentParser(this IArgumentParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());
        }

        /// <summary>
        /// Use a class Windows-CMD syntax for arguments
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UseWindowsCmdArgumentParser(this IArgumentParserSetup setup)
        {
            Assert.ArgumentNotNull(setup, nameof(setup));
            return setup.UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());
        }
    }
}
