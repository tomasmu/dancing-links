using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SudokuSolver
{
    public static class StringExtensions
    {
        public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options) =>
            Regex.Replace(input, pattern, replacement, options);

        public static string RegexReplace(this string input, string pattern, string replacement) =>
            Regex.Replace(input, pattern, replacement);

        public static string RemoveWhiteSpace(this string input) =>
            Regex.Replace(input, @"\s", string.Empty);
    }
}
