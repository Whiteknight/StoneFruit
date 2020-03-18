using System.Collections.Generic;
using ParserObjects;
using StoneFruit.Execution.Arguments;

namespace StoneFruit
{
    public interface IParserSetup
    {
        IParserSetup UseArgumentParser(IParser<char, IArgument> argParser);

        IParserSetup UseArgumentParser(IParser<char, IEnumerable<IArgument>> argParser);
    }

    public static class ParserSetupExtensions
    {
        public static IParserSetup UseSimplifiedArgumentParser(this IParserSetup setup)
            => setup.UseArgumentParser(SimplifiedArgumentGrammar.GetParser());

        public static IParserSetup UsePosixStyleArgumentParser(this IParserSetup setup)
            => setup.UseArgumentParser(PosixStyleArgumentGrammar.GetParser());

        public static IParserSetup UsePowershellStyleArgumentParser(this IParserSetup setup)
            => setup.UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());

        public static IParserSetup UseWindowsCmdArgumentParser(this IParserSetup setup)
            => setup.UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());
    }
}
