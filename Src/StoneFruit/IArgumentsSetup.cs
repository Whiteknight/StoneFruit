using System;
using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// Setup argument parsing.
/// </summary>
public interface IArgumentsSetup
{
    /// <summary>
    /// Specify an argument parser to use. Notice that you cannot set an Argument
    /// parser if you specify a Command parser. If null is passed the default argument
    /// parser will be used.
    /// </summary>
    /// <param name="argParser"></param>
    /// <returns></returns>
    IArgumentsSetup UseArgumentParser(IParser<char, ArgumentToken> argParser);

    /// <summary>
    /// Use the StoneFruit simplified argument syntax.
    /// </summary>
    /// <returns></returns>
    IArgumentsSetup UseSimplifiedArgumentParser()
        => UseArgumentParser(SimplifiedArgumentGrammar.GetParser());

    /// <summary>
    /// Use a POSIX-style argument syntax similar to many existing POSIX utilities.
    /// </summary>
    /// <returns></returns>
    IArgumentsSetup UsePosixStyleArgumentParser()
        => UseArgumentParser(PosixStyleArgumentGrammar.GetParser());

    /// <summary>
    /// Use a Windows PowerShell syntax for arguments.
    /// </summary>
    /// <returns></returns>
    IArgumentsSetup UsePowershellStyleArgumentParser()
        => UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());

    /// <summary>
    /// Use a class Windows-CMD syntax for arguments.
    /// </summary>
    /// <returns></returns>
    IArgumentsSetup UseWindowsCmdArgumentParser()
        => UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());

    /// <summary>
    /// Specify a parser to use for scripts. Notice that you cannot set a Script
    /// parser if you specify a Command parser. If null is passed the default script parser
    /// will be used.
    /// </summary>
    /// <param name="scriptParser"></param>
    /// <returns></returns>
    IArgumentsSetup UseScriptParser(IParser<char, CommandFormat> scriptParser);

    IArgumentsSetup UseTypeParser<T>(IValueTypeParser<T> typeParser);

    IArgumentsSetup UseTypeParser<T>(Func<IValuedArgument, T> parse)
        where T : class
        => UseTypeParser<T>(new DelegateValueTypeParser<T>(NotNull(parse)));
}