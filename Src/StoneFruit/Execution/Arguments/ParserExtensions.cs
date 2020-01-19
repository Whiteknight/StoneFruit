using System.Collections.Generic;
using ParserObjects;
using ParserObjects.Sequences;

namespace StoneFruit.Execution.Arguments
{
    public static class ParserExtensions
    {
        public static CommandArguments ParseArguments(this IParser<char, IArgument> parser, IEnumerable<string> args)
        {
            var fullParser = CommandArgumentsGrammar.GetParser(parser);
            var argsString = string.Join(" ", args);
            return ParseArguments(fullParser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, IEnumerable<IArgument>> parser, IEnumerable<string> args)
        {
            var fullParser = CommandArgumentsGrammar.GetParser(parser);
            var argsString = string.Join(" ", args);
            return ParseArguments(fullParser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, IArgument> parser, string argsString)
        {
            var fullParser = CommandArgumentsGrammar.GetParser(parser);
            return ParseArguments(fullParser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, IEnumerable<IArgument>> parser, string argsString)
        {
            var fullParser = CommandArgumentsGrammar.GetParser(parser);
            return ParseArguments(fullParser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, CommandArguments> fullParser, IEnumerable<string> args)
        {
            var argsString = string.Join(" ", args);
            return ParseArguments(fullParser, argsString);
        }

        public static CommandArguments ParseArguments(this IParser<char, CommandArguments> fullParser, string argsString)
        {
            var sequence = new StringCharacterSequence(argsString);
            return fullParser.Parse(sequence).Value;
        }
    }
}
