using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentominoes
{
    public static class ExtensionMethods
    {
        public static int[][] ToRectangleMatrix(this string stringLiteral)
        {
            static bool FirstRowNotEmpty(string str, int index) => index != 0 || str.Length > 0;

            var stringRows = stringLiteral
                .Split(Environment.NewLine)
                .Where(FirstRowNotEmpty);
            var maxColumnLength = stringRows.Max(str => str.Length);

            int MapCharToInt(char chr) => chr == '.' ? 1 : 0;

            var matrix = stringRows
                .Select(str => str
                    .PadRight(maxColumnLength, ' ')
                    .Select(MapCharToInt)
                    .ToArray())
                .ToArray();

            return matrix;
        }

        public static bool[][] ToRectangleMatrixBool(this string stringLiteral)
        {
            static bool FirstRowNotEmpty(string str, int index) => index != 0 || str.Length > 0;

            var stringRows = stringLiteral
                .Split(Environment.NewLine)
                .Where(FirstRowNotEmpty);
            var maxColumnLength = stringRows.Max(str => str.Length);

            bool MapCharToBool(char chr) => chr == '.' ? true : false;

            var matrix = stringRows
                .Select(str => str
                    .PadRight(maxColumnLength, ' ')
                    .Select(MapCharToBool)
                    .ToArray())
                .ToArray();

            return matrix;
        }

        public static string ToStringThing(this int[][] matrix, string delimiter = " ")
        {
            var sb = new StringBuilder();
            for (int row = 0; row < matrix.Length; row++)
            {
                //todo: kanske padding?
                sb.Append(string.Join(delimiter, matrix[row]));
                if (row != matrix.Length - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string ToStringThing(this bool[,] matrix, string delimiter = " ")
        {
            var strings = new List<string>();
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                var sb = new StringBuilder();
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (col != 0)
                        sb.Append(" ");
                    sb.Append(matrix[row, col] ? "*" : " ");
                }

                strings.Add(sb.ToString());
            }

            return string.Join(Environment.NewLine, strings.OrderBy(x => x));
        }

        //not optimal, but quick n easy to make without errors :-)
        public static int[][] Rotate0(this int[][] matrix) => matrix;
        public static int[][] Rotate90(this int[][] matrix) => matrix.Transpose().MirrorHorizontal();
        public static int[][] Rotate180(this int[][] matrix) => matrix.MirrorHorizontal().MirrorVertical();
        public static int[][] Rotate270(this int[][] matrix) => matrix.Transpose().MirrorVertical();

        public static int[][] Rotate0Mirror(this int[][] matrix) => matrix.MirrorHorizontal();
        public static int[][] Rotate90Mirror(this int[][] matrix) => matrix.MirrorHorizontal().MirrorVertical().Transpose();
        public static int[][] Rotate180Mirror(this int[][] matrix) => matrix.MirrorVertical();
        public static int[][] Rotate270Mirror(this int[][] matrix) => matrix.Transpose();

        private static IEnumerable<int[][]> GetAllRotationsAndMirrors(this int[][] matrix)
        {
            yield return matrix.Rotate0();
            yield return matrix.Rotate90();
            yield return matrix.Rotate180();
            yield return matrix.Rotate270();
            
            yield return matrix.Rotate0Mirror();
            yield return matrix.Rotate90Mirror();
            yield return matrix.Rotate180Mirror();
            yield return matrix.Rotate270Mirror();
        }

        public static bool[][,] ToArrayMatrix(this IEnumerable<int[][]> gnägg)
        {
            //  {o,0} -HOO!
            //  (,,())

            return gnägg.Select(arr => arr.ToBoolMatrix()).ToArray();
        }

        public static bool[,] ToBoolMatrix(this int[][] matrix)
        {
            var result = new bool[matrix.Length, matrix[0].Length];
            for (int row = 0; row < matrix.Length; row++)
            {
                for (int col = 0; col < matrix[0].Length; col++)
                {
                    result[row, col] = matrix[row][col] != 0;
                }
            }

            return result;
        }

        public static IEnumerable<int[][]> GetUniqueRotationsAndMirrors(this int[][] matrix) =>
            GetAllRotationsAndMirrors(matrix)
                .GroupBy(m => m.ToStringThing())
                .Select(g => g.First());

        private static int[][] Transpose(this int[][] matrix)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            var result = new int[cols][];
            for (int col = 0; col < cols; col++)
                result[col] = new int[rows];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    result[col][row] = matrix[row][col];

            return result;
        }

        private static int[][] MirrorHorizontal(this int[][] matrix) => PerformAssignment(matrix, MirrorHorizontalAssignment);
        private static int[][] MirrorVertical(this int[][] matrix) => PerformAssignment(matrix, MirrorVerticalAssignment);

        private static void MirrorHorizontalAssignment(int[][] result, int[][] matrix, int rows, int cols, int row, int col) =>
            result[row][col] = matrix[row][cols - 1 - col];

        private static void MirrorVerticalAssignment(int[][] result, int[][] matrix, int rows, int cols, int row, int col) =>
            result[row][col] = matrix[rows - 1 - row][col];

        private static int[][] PerformAssignment(this int[][] matrix, Action<int[][], int[][], int, int, int, int> action)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            var result = new int[rows][];
            for (int row = 0; row < rows; row++)
                result[row] = new int[cols];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    action(result, matrix, rows, cols, row, col);

            return result;
        }
    }
}
