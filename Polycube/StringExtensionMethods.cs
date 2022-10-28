using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Polycube
{
    public static class StringExtensionMethods
    {
        public static string RegexReplace(this string input, string pattern, string replacement) =>
            Regex.Replace(input, pattern, replacement);

        public static string RegexRemove(this string input, string pattern) =>
            Regex.Replace(input, pattern, string.Empty);

        public static string[] RegexSplit(this string input, string pattern) =>
            Regex.Split(input, pattern);

        public static string[] RegexSplit(this string input, string pattern, RegexOptions options) =>
            Regex.Split(input, pattern, options);

        public static string TrimNewLine(this string input) =>
            input.Trim('\r', '\n');

        public static string StringJoin(this IEnumerable<string> values, string separator) =>
            string.Join(separator, values);

        //todo: move
        //todo: don't use
        //public static char[][][] ToPiece(this string stringLiteral) =>
        //    stringLiteral
        //        .Trim()
        //        .RegexSplit(@"(\r\n){2}|\r{2}|\n{2}", RegexOptions.ExplicitCapture)
        //        .Select(str => str
        //            .RegexSplit(@"(\r\n){1}|\r{1}|\n{1}", RegexOptions.ExplicitCapture)
        //            .Select(str => str.ToCharArray())
        //            .ToArray())
        //        .ToArray();

        public static IEnumerable<int[,]> ToCoordinates(this string piece, char blankChar)
        {
            var doubleNewLine = @"(\r\n){2}|\r{2}|\n{2}";
            var singleNewLine = @"(\r\n){1}|\r{1}|\n{1}";
            var ys = piece
                .Trim()
                .RegexSplit(doubleNewLine, RegexOptions.ExplicitCapture)
                .ToArray();
            
            for (int y = 0; y < ys.Length; y++)
            {
                var xs = ys[y]
                    .RegexSplit(singleNewLine, RegexOptions.ExplicitCapture)
                    .ToArray();
                
                for (int x = 0; x < xs.Length; x++)
                {
                    for (int z = 0; z < xs[x].Length; z++)
                    {
                        if (xs[x][z] != blankChar)
                        {
                            yield return new int[,] { { x }, { y }, { z } };    
                        }
                    }
                }
            }
        }
    }
}
