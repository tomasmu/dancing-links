using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PolycubeSolver
{
    public static class StringExtensionMethods
    {
        public static string RegexReplace(this string input, string pattern, string replacement) =>
            Regex.Replace(input, pattern, replacement);

        public static string[] RegexSplit(this string input, string pattern) =>
            Regex.Split(input, pattern);

        public static string[] RegexSplit(this string input, string pattern, RegexOptions options) =>
            Regex.Split(input, pattern, options);

        public static string TrimNewLine(this string input) =>
            input.Trim('\r', '\n');

        public static string StringJoin<T>(this IEnumerable<T> values, string separator) =>
            string.Join(separator, values);

        public static bool IsEmptyOrWhiteSpace(this char chr) =>
            chr == 0 || char.IsWhiteSpace(chr);

        //todo: these should not be extension methods, parser class?
        public static IEnumerable<Vector> ToPoints(this string piece, params char[] ignoreChars)
        {
            var doubleNewLine = @"(\r\n){2}|\r{2}|\n{2}";
            var singleNewLine = @"(\r\n){1}|\r{1}|\n{1}";
            var ys = piece
                .Trim()
                .RegexSplit(doubleNewLine, RegexOptions.ExplicitCapture);
            
            for (int y = 0; y < ys.Length; y++)
            {
                var xs = ys[y].RegexSplit(singleNewLine, RegexOptions.ExplicitCapture);
                for (int x = 0; x < xs.Length; x++)
                    for (int z = 0; z < xs[x].Length; z++)
                        if (!ignoreChars.Contains(xs[x][z]))
                            yield return new Vector(x, y, z);
            }
        }
    }
}
