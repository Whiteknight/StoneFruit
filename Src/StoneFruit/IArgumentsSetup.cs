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
    /// Specify a parser to use for scripts. Notice that you cannot set a Script
    /// parser if you specify a Command parser. If null is passed the default script parser
    /// will be used.
    /// </summary>
    /// <param name="scriptParser"></param>
    /// <returns></returns>
    IArgumentsSetup UseScriptParser(IParser<char, CommandFormat> scriptParser);

    IArgumentsSetup UseTypeParser<T>(IValueTypeParser<T> typeParser)
         where T : class;
}

public static class ArgumentsSetupExtensions
{
    /// <summary>
    /// Use the StoneFruit simplified argument syntax.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IArgumentsSetup UseSimplifiedArgumentParser(this IArgumentsSetup setup)
        => NotNull(setup).UseArgumentParser(SimplifiedArgumentGrammar.GetParser());

    /// <summary>
    /// Use a POSIX-style argument syntax similar to many existing POSIX utilities.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IArgumentsSetup UsePosixStyleArgumentParser(this IArgumentsSetup setup)
        => NotNull(setup).UseArgumentParser(PosixStyleArgumentGrammar.GetParser());

    /// <summary>
    /// Use a Windows PowerShell syntax for arguments.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IArgumentsSetup UsePowershellStyleArgumentParser(this IArgumentsSetup setup)
        => NotNull(setup).UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());

    /// <summary>
    /// Use a class Windows-CMD syntax for arguments.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IArgumentsSetup UseWindowsCmdArgumentParser(this IArgumentsSetup setup)
        => NotNull(setup).UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());

    public static IArgumentsSetup UseTypeParser<T>(this IArgumentsSetup setup, Func<string, T> parse)
        where T : class
        => NotNull(setup).UseTypeParser<T>(new DelegateValueTypeParser<T>(NotNull(parse)));
}
