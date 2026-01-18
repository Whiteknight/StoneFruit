using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    IArgumentsSetup UseArgumentParser(IParser<char, IArgumentToken> argParser);

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

/// <summary>
/// Represents a single argument, either positional, named, or otherwise.
/// </summary>
public interface IArgument
{
    /// <summary>
    /// Gets or sets a value indicating whether this value has already been
    /// consumed. A consumed argument cannot be retrieved again.
    /// </summary>
    bool Consumed { get; set; }
}

/// <summary>
/// Represents an argument with a value.
/// </summary>
public interface IValuedArgument : IArgument
{
    /// <summary>
    /// Gets the raw string value of the argument.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Get the value of the argument as a string, with a default value if the argument doesn't exist.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    string AsString(string defaultValue = "");

    /// <summary>
    /// Get the value of the argument as a boolean, with a default value if the argument doesn't exist
    /// or can't be converted.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    bool AsBool(bool defaultValue = false);

    /// <summary>
    /// Get the value of the argument as an integer, with a default value if the argument doesn't exist
    /// or can't be converted.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    int AsInt(int defaultValue = 0);

    /// <summary>
    /// Get the value of the argument as a long, with a default value if the argument doesn't exist or
    /// can't be converted.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    long AsLong(long defaultValue = 0L);
}

/// <summary>
/// A positional argument is a value argument with no name.
/// </summary>
public interface IPositionalArgument : IValuedArgument;

/// <summary>
/// A named argument is a value argument with a name.
/// </summary>
public interface INamedArgument : IValuedArgument
{
    /// <summary>
    /// Gets the name of the argument.
    /// </summary>
    string Name { get; }
}

/// <summary>
/// A flag argument is an argument with a name but no value.
/// </summary>
public interface IFlagArgument : IArgument
{
    /// <summary>
    /// Gets the flag name.
    /// </summary>
    string Name { get; }
}

public interface IArgumentCollection
{
    /// <summary>
    /// Gets the raw, unparsed argument string if available. This string is usually only
    /// populated from the parser and is not usually available otherwise.
    /// </summary>
    string Raw { get; }

    /// <summary>
    /// Resets the Consumed state of all arguments. Useful if the arguments object is
    /// going to be reused.
    /// </summary>
    void Reset();

    /// <summary>
    /// Get the next unconsumed positional value in order.
    /// </summary>
    /// <returns></returns>
    IPositionalArgument Shift();

    /// <summary>
    /// Get the positional value at the given index. The first argument is at position
    /// 0. Returns the argument regardless of whether it has been consumed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IPositionalArgument Get(int index);

    /// <summary>
    /// Get the unconsumed named argument with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    INamedArgument Get(string name, bool caseSensitive = true);

    /// <summary>
    /// Get all remaining unconsumed positional arguments. Arguments which are
    /// ambiguous will be treated as positionals during iteration.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IPositionalArgument> GetAllPositionals();

    /// <summary>
    /// Get the unconsumed flag argument with the given name. If the argument has
    /// already been consumed, it will not be returned.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    IFlagArgument GetFlag(string name, bool caseSensitive = true);

    /// <summary>
    /// Returns true if the collection contains a flag with the given name, regardless
    /// of consumed state.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="markConsumed"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    bool HasFlag(string name, bool markConsumed = false, bool caseSensitive = true);

    /// <summary>
    /// Get all remaining unconsumed flag arguments. Arguments which are ambiguous
    /// will be treated as flags during iteration.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IFlagArgument> GetAllFlags();

    /// <summary>
    /// Get all remaining unconsumed named arguments with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    IEnumerable<INamedArgument> GetAllNamed(string name, bool caseSensitive = true);

    /// <summary>
    /// Get all remaining unconsumed named arguments.
    /// </summary>
    /// <returns></returns>
    IEnumerable<INamedArgument> GetAllNamed();

    /// <summary>
    /// Get the values of all unconsumed arguments, for debug purposes. Notice that unconsumed
    /// arguments might be ambiguously defined as positional, named or flag arguments, so
    /// we cannot return a typed IArgument here.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<string> GetUnconsumed();
}

/// <summary>
/// Collection of parsed arguments. All names are case-insensitive.
/// </summary>
public interface IArguments : IArgumentCollection
{
    public ArgumentValueMapper Mapper { get; }
}

public static class ArgumentsExtensions
{
    /// <summary>
    /// Get the positional argument at the given index, and mark it consumed.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static IPositionalArgument Consume(this IArgumentCollection args, int index)
        => NotNull(args).Get(index).MarkConsumed();

    /// <summary>
    /// Get the flag argument with the given name and mark it consumed.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static INamedArgument Consume(this IArgumentCollection args, string name, bool caseSensitive = true)
        => NotNull(args).Get(name, caseSensitive).MarkConsumed();

    /// <summary>
    /// Get the named argument with the given name and mark it consumed.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IFlagArgument ConsumeFlag(this IArgumentCollection args, string name, bool caseSensitive = true)
        => NotNull(args).GetFlag(name, caseSensitive).MarkConsumed();

    /// <summary>
    /// Create a new instance of type T and attempt to map argument values into the
    /// public properties of the new object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T MapTo<T>(this IArguments args)
        where T : new()
        => NotNull(args).Mapper.Map<T>(args);

    /// <summary>
    /// Attempt to map argument values onto the public properties of the given object
    /// instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="obj"></param>
    public static void MapOnto<T>(this IArguments args, T obj)
        => NotNull(args).Mapper.MapOnto(args, obj);

    /// <summary>
    /// Get a list of all argument objects.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<IArgument> GetAllArguments(this IArgumentCollection args)
        => NotNull(args).GetAllPositionals().Cast<IArgument>()
            .Concat(args.GetAllNamed())
            .Concat(args.GetAllFlags())
            .Where(p => !p.Consumed);

    /// <summary>
    /// Throw an exception if any arguments have not been consumed. Useful to alert
    /// the user if extra/unnecessary arguments have been passed.
    /// </summary>
    public static void VerifyAllAreConsumed(this IArgumentCollection args)
    {
        var unconsumed = NotNull(args).GetUnconsumed();
        if (!unconsumed.Any())
            return;

        var sb = new StringBuilder();
        sb.AppendLine("Arguments were provided which were not consumed.");
        sb.AppendLine();
        foreach (var line in unconsumed)
            sb.AppendLine(line);
        throw new ArgumentParseException(sb.ToString());
    }

    /// <summary>
    /// Throw an exception if the argument does not exist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argument"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static T Require<T>(this T argument, string errorMessage = "")
        where T : IArgument
    {
        NotNull(argument);
        (argument as MissingArgument)?.Throw(errorMessage);
        return argument;
    }

    /// <summary>
    /// Mark the value as being consumed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argument"></param>
    /// <param name="consumed"></param>
    /// <returns></returns>
    public static T MarkConsumed<T>(this T argument, bool consumed = true)
        where T : IArgument
    {
        NotNull(argument);
        argument.Consumed = consumed;
        return argument;
    }

    /// <summary>
    /// Returns true if the argument exists, false otherwise.
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    public static bool Exists(this IArgument argument)
        => argument is not null and not MissingArgument;

    /// <summary>
    /// Helper method to convert the value of the argument to a different format, using a default
    /// value if the conversion fails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argument"></param>
    /// <param name="transform"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T? As<T>(this IValuedArgument argument, Func<string, T> transform, T? defaultValue = default)
    {
        if (argument is MissingArgument)
            return defaultValue;
        try
        {
            return transform(argument.Value);
        }
        catch
        {
            return defaultValue;
        }
    }
}
