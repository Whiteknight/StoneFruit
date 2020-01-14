using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;
using static ParserObjects.Parsers.ParserMethods;

namespace StoneFruit.Execution
{
    public static class CommandArgumentsGrammar
    {
        public static IParser<char, CommandArguments> GetParser(IParser<char, IArgument> argParser = null)
        {
            argParser ??= SimplifiedArgumentGrammar.GetParser();
            return argParser.List().Transform(a => new CommandArguments(a));
        }

        public static IParser<char, CommandArguments> GetParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            if (argParser == null)
                return GetParser();
            return argParser.List().Transform(a => new CommandArguments(a.SelectMany(x => x)));
        }
    }

    /// <summary>
    /// Grammar for parsing a complete command with verb and arguments
    /// </summary>
    public static class CompleteCommandGrammar
    {
        public static IParser<char, CompleteCommand> GetParser(IParser<char, CommandArguments> argParser)
        {
            var firstChar = Match<char>(c => c == '_' || char.IsLetter(c));
            var bodyChars = Match<char>(c => c == '_' || c == '-' || char.IsLetterOrDigit(c));
            var commandName = Rule(
                firstChar,
                bodyChars.ListCharToString(),
                (first, rest) => (first + rest).ToLowerInvariant()
            );

            return Rule(
                commandName,
                argParser,

                (name, args) => new CompleteCommand(name, args)
            );
        }
    }
}