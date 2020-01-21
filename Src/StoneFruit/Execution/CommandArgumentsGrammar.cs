using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    public static class CommandArgumentsGrammar
    {
        public static IParser<char, CommandArguments> GetParser(IParser<char, IArgument> argParser = null)
        {
            argParser ??= SimplifiedArgumentGrammar.GetParser();
            return argParser.List().Transform(a => new CommandArguments(a.ToList()));
        }

        public static IParser<char, CommandArguments> GetParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            if (argParser == null)
                return GetParser();
            return argParser.List().Transform(a => new CommandArguments(a.SelectMany(x => x).ToList()));
        }
    }
}