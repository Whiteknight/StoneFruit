using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;
using static ParserObjects.Parsers.ParserMethods;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Grammar for parsing a complete command with verb and arguments
    /// </summary>
    public static class CompleteCommandGrammar
    {
        public static IParser<char, CompleteCommand> GetParser(IParser<char, IEnumerable<IArgument>> argParser = null)
        {
            argParser ??= SimplifiedArgumentGrammar.GetParser();

            var firstChar = Match<char>(c => c == '_' || char.IsLetter(c));
            var bodyChars = Match<char>(c => c == '_' || c == '-' || char.IsLetterOrDigit(c));
            var commandName = Rule(
                firstChar,
                bodyChars.List(c => new string(c.ToArray())),
                (first, rest) => (first + rest).ToLowerInvariant()
            );

            return Rule(
                commandName,
                argParser.List(a => new CommandArguments(a.SelectMany(x => x))),

                (name, args) => new CompleteCommand(name, args)
            );
        }
    }
}