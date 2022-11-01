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

        public static bool RegexIsMatch(this string input, string pattern) =>
            Regex.IsMatch(input, pattern);

        public static string TrimNewLine(this string input) =>
            input.Trim('\r', '\n');

        public static string StringJoin<T>(this IEnumerable<T> values, string separator) =>
            string.Join(separator, values);

        public static bool IsEmptyOrWhiteSpace(this char chr) =>
            chr == 0 || char.IsWhiteSpace(chr);

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

        //todo: these should not be extension methods, parser class?
        public static IEnumerable<Vector> ToPoints(this string piece, char ignoreChar)
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
                {
                    for (int z = 0; z < xs[x].Length; z++)
                    {
                        if (xs[x][z] != ignoreChar)
                        {
                            yield return new Vector(x, y, z);    
                        }
                    }
                }
            }
        }

        public static bool[,,] ToGrid(this string gridString, char blockedChar)
        {
            var doubleNewLine = @"(\r\n){2}|\r{2}|\n{2}";
            var singleNewLine = @"(\r\n){1}|\r{1}|\n{1}";

            var gridArray = gridString
                .Trim()
                .RegexSplit(doubleNewLine, RegexOptions.ExplicitCapture)
                .Select(y => y
                    .RegexSplit(singleNewLine, RegexOptions.ExplicitCapture)
                        .Select(x => x.ToArray())
                    .ToArray())
                .ToArray();

            var yLength = gridArray.Length;
            var xLength = gridArray.Max(y => y.Length);
            var zLength = gridArray.Max(y => y.Max(x => x.Length));

            var grid = new bool[yLength, xLength, zLength];
            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        if (gridArray[y][x][z] == blockedChar)
                            grid[y, x, z] = true;
                    }
                }
            }

            return grid;
        }
    }
}
