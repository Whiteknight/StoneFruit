using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;

namespace StoneFruit.Execution.Arguments
{
    public static class ParserExtensions
    {
        public static CommandArguments ParseArguments(this IParser<char, IArgument> parser, string argsString)
        {
            if (parser == null)
                return new CommandArguments(argsString, new IArgument[0]);

            var sequence = new StringCharacterSequence(argsString);
            var argsList = parser.List().Parse(sequence).Value.ToList();
            return new CommandArguments(argsString, argsList);
        }

        public static CommandArguments ParseArguments(this IParser<char, IArgument> parser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(parser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, IEnumerable<IArgument>> parser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(parser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, IEnumerable<IArgument>> parser, string argsString)
        {
            if (parser == null)
                return new CommandArguments(argsString, new IArgument[0]);

            var newParser = parser.Flatten<char, IEnumerable<IArgument>, IArgument>();
            return ParseArguments(newParser, argsString);
        }
    }
}
