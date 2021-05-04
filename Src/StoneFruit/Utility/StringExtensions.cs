using System;

namespace StoneFruit.Utility
{
    public static class StringExtensions
    {
        /// <summary>
        /// If the string ends in the given suffix, remove the suffix and return the remaining
        /// substring.
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
    }
}
