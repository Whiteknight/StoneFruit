using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.DigitParserMethods;

namespace StoneFruit.Utility
{
    public static class StringExtensions
    {
        /// <summary>
        /// If the string ends in the given suffix, remove the suffix and return the remaining substring.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string s, string suffix)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            if (s.Length < suffix.Length)
                return s;
            if (s.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return s.Substring(0, s.Length - suffix.Length);
            return s;
        }

        public static string CamelCaseToSpinalCase(this string s)
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