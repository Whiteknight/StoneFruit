using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;

namespace StoneFruit.Execution.Arguments
{
    public static class ParserExtensions
    {
        public static ICommandArguments ParseArguments(this IParser<char, IParsedArgument> parser, string argsString)
        {
            if (parser == null)
                return new ParsedCommandArguments(argsString, new IParsedArgument[0]);

            var sequence = new StringCharacterSequence(argsString);
            var argsList = parser.List().Parse(sequence).Value.ToList();
            return new ParsedCommandArguments(argsString, argsList);
        }

        public static ICommandArguments ParseArguments(this IParser<char, IParsedArgument> parser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(parser, argsString);
        }

        public static ICommandArguments ParseArguments(this IParser<char, IEnumerable<IParsedArgument>> parser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(parser, argsString);
        }

        public static ICommandArguments ParseArguments(this IParser<char, IEnumerable<IParsedArgument>> parser, string argsString)
        {
            if (parser == null)
                return new ParsedCommandArguments(argsString, new IParsedArgument[0]);

            var newParser = parser.Flatten<char, IEnumerable<IParsedArgument>, IParsedArgument>();
            return ParseArguments(newParser, argsString);
        }
    }
}
