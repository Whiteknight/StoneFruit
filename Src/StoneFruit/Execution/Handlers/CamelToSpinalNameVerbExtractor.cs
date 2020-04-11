using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Utility;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.DigitParserMethods;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Verb extractor to derive the verb from the name of the handler class. Common
    /// suffixes are removed ('verb', 'command', 'handler'), CamelCase is converted to
    /// spinal-case, and the name is converted to lower-case.
    /// </summary>
    public class CamelToSpinalNameVerbExtractor : ITypeVerbExtractor
    {
        public IReadOnlyList<string> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new string[0];

            var name = type.Name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler");
            name = CamelCaseToSpinalCase(name);
            return new[] { name };
        }

        private static string CamelCaseToSpinalCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            var result = _camelCase.Value.Parse(s);
            if (!result.Success)
                return s.ToLowerInvariant();
            return string.Join("-", result.Value.Select(s => s.ToLowerInvariant()));
        }

        // TODO V2: This logic will be in ParserObjects eventually
        private static readonly Lazy<IParser<char, IEnumerable<string>>> _camelCase = new Lazy<IParser<char, IEnumerable<string>>>(
            () =>
            {
                var lowerCase = Match<char>(char.IsLower);
                var upperCase = Match<char>(char.IsUpper);

                // Any run of digits of any length is a string
                var digits = DigitString();

                // In some cases a string of lower-cases will count
                var lowerString = lowerCase.ListCharToString(true);

                // A string starting with an upper char and continuing with zero or more lower chars
                var camelCaseString = Rule(
                    upperCase,
                    lowerCase.ListCharToString(),
                    (u, l) => u + l
                );

                // A run of all uppercase chars which aren't followed by lower-case chars
                // can be an abbreviation
                var upperString = upperCase
                    .NotFollowedBy(lowerCase)
                    .ListCharToString(true);

                var parts = First(
                    digits,
                    upperString,
                    camelCaseString,
                    lowerString
                );

                return parts.List();
            }
        );
    }
}