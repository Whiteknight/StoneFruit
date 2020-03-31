using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;

namespace StoneFruit.Execution.Arguments
{
    public static class ParserExtensions
    {
        public static IArguments ParseArguments(this IParser<char, IParsedArgument> parser, string argsString)
        {
            if (parser == null)
                return SyntheticArguments.Empty();

            var sequence = new StringCharacterSequence(argsString);
            var argsList = parser.List().Parse(sequence).Value.ToList();
            return new ParsedArguments(argsString, argsList);
        }

        public static IArguments ParseArguments(this IParser<char, IParsedArgument> parser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(parser, argsString);
        }

        public static IArguments ParseArguments(this IParser<char, IEnumerable<IParsedArgument>> parser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(parser, argsString);
        }

        public static IArguments ParseArguments(this IParser<char, IEnumerable<IParsedArgument>> parser, string argsString)
        {
            if (parser == null)
                return SyntheticArguments.Empty();

            var newParser = parser.Flatten<char, IEnumerable<IParsedArgument>, IParsedArgument>();
            return ParseArguments(newParser, argsString);
        }
    }
}
