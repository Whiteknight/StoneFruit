using System.Collections.Generic;
using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts.Formatting;

namespace StoneFruit
{
    /// <summary>
    /// Setup argument parsing
    /// </summary>
    public interface IArgumentParserSetup
    {
        /// <summary>
        /// Specify a parser to use for verbs
        /// </summary>
        /// <param name="verbParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseVerbParser(IParser<char, string> verbParser);

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseArgumentParser(IParser<char, IParsedArgument> argParser);

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        IArgumentParserSetup UseArgumentParser(IParser<char, IEnumerable<IParsedArgument>> argParser);

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
            => setup.UseArgumentParser(SimplifiedArgumentGrammar.GetParser());

        /// <summary>
        /// Use a POSIX-style argument syntax similar to many existing POSIX utilities
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UsePosixStyleArgumentParser(this IArgumentParserSetup setup)
            => setup.UseArgumentParser(PosixStyleArgumentGrammar.GetParser());

        /// <summary>
        /// Use a Windows PowerShell syntax for arguments
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UsePowershellStyleArgumentParser(this IArgumentParserSetup setup)
            => setup.UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());

        /// <summary>
        /// Use a class Windows-CMD syntax for arguments
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IArgumentParserSetup UseWindowsCmdArgumentParser(this IArgumentParserSetup setup)
            => setup.UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());
    }
}
