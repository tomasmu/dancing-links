using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentominoes
{
    public static class ExtensionMethods
    {
        public static bool[][][] ToRectangleMatrix(this IEnumerable<string> stringLiterals, params char[] blockingChars) =>
            stringLiterals.Select(str => str.ToRectangleMatrix(blockingChars)).ToArray();

        public static bool[][] ToRectangleMatrix(this string stringLiteral, params char[] blockingChars)
        {
            static bool FirstRowNotEmpty(string str, int index) => index != 0 || str.Length > 0;

            var stringRows = stringLiteral
                .Split(Environment.NewLine)
                .Where(FirstRowNotEmpty);
            var maxColumnLength = stringRows.Max(str => str.Length);

            bool MapCharToBool(char chr) => blockingChars.Contains(chr);

            var matrix = stringRows
                .Select(str => str
                    .PadRight(maxColumnLength, ' ')
                    .Select(MapCharToBool)
                    .ToArray())
                .ToArray();

            return matrix;
        }

        public static int[][] ToIntMatrix(this bool[][] matrix)
        {
            var (rows, cols) = (matrix.Length, matrix[0].Length);
            var result = new int[rows][];
            for (var row = 0; row < matrix.Length; row++)
            {
                result[row] = new int[cols];
                for (var col = 0; col < cols; col++)
                    result[row][col] = matrix[row][col] ? 1 : 0;
            }

            return result;
        }

        public static string ToStringThing(this bool[][] matrix, string delimiter = " ")
        {
            var sb = new StringBuilder();
            for (var row = 0; row < matrix.Length; row++)
            {
                sb.Append(string.Join(delimiter, matrix[row].Select(b => b ? '1' : '0')));
                if (row != matrix.Length - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string ToStringThing<T>(this T[][] matrix, string delimiter = " ")
        {
            var sb = new StringBuilder();
            for (var row = 0; row < matrix.Length; row++)
            {
                sb.Append(string.Join(delimiter, matrix[row]));
                if (row != matrix.Length - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        //not optimal, but quick n easy to make without errors :-)
        public static T[][] Rotate0<T>(this T[][] matrix) => matrix;
        public static T[][] Rotate90<T>(this T[][] matrix) => matrix.Transpose().MirrorHorizontal();
        public static T[][] Rotate180<T>(this T[][] matrix) => matrix.MirrorHorizontal().MirrorVertical();
        public static T[][] Rotate270<T>(this T[][] matrix) => matrix.Transpose().MirrorVertical();
        public static T[][] Rotate0Mirror<T>(this T[][] matrix) => matrix.MirrorHorizontal();
        public static T[][] Rotate90Mirror<T>(this T[][] matrix) => matrix.MirrorHorizontal().MirrorVertical().Transpose();
        public static T[][] Rotate180Mirror<T>(this T[][] matrix) => matrix.MirrorVertical();
        public static T[][] Rotate270Mirror<T>(this T[][] matrix) => matrix.Transpose();

        private static IEnumerable<T[][]> GetAllRotationsAndMirrors<T>(this T[][] matrix, bool includeMirrors = true)
        {
            yield return matrix.Rotate0();
            yield return matrix.Rotate90();
            yield return matrix.Rotate180();
            yield return matrix.Rotate270();

            if (includeMirrors)
            {
                yield return matrix.Rotate0Mirror();
                yield return matrix.Rotate90Mirror();
                yield return matrix.Rotate180Mirror();
                yield return matrix.Rotate270Mirror();
            }
        }

        public static T[][,] ToArrayMatrixArray<T>(this T[][][] array)
        {
            //  {o,0} -hoo let the owls out?
            //  (,,())

            return array.Select(arr => arr.ToArrayMatrix()).ToArray();
        }

        public static T[,] ToArrayMatrix<T>(this T[][] matrix)
        {
            var result = new T[matrix.Length, matrix[0].Length];
            for (var row = 0; row < matrix.Length; row++)
            {
                for (var col = 0; col < matrix[0].Length; col++)
                {
                    result[row, col] = matrix[row][col];
                }
            }

            return result;
        }

        public static T[][][] GetUniqueRotationsAndMirrors<T>(this T[][] matrix) =>
            GetAllRotationsAndMirrors(matrix)
                .GroupBy(m => m.ToStringThing())
                .Select(g => g.First())
                .ToArray();

        private static T[][] Transpose<T>(this T[][] matrix)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            var result = new T[cols][];
            for (var col = 0; col < cols; col++)
                result[col] = new T[rows];

            for (var row = 0; row < rows; row++)
                for (var col = 0; col < cols; col++)
                    result[col][row] = matrix[row][col];

            return result;
        }

        private static T[][] MirrorHorizontal<T>(this T[][] matrix) => PerformAssignment(matrix, MirrorHorizontalAssignment);
        private static T[][] MirrorVertical<T>(this T[][] matrix) => PerformAssignment(matrix, MirrorVerticalAssignment);

        private static void MirrorHorizontalAssignment<T>(T[][] result, T[][] matrix, int rows, int cols, int row, int col) =>
            result[row][col] = matrix[row][cols - 1 - col];

        private static void MirrorVerticalAssignment<T>(T[][] result, T[][] matrix, int rows, int cols, int row, int col) =>
            result[row][col] = matrix[rows - 1 - row][col];

        private static T[][] PerformAssignment<T>(this T[][] matrix, Action<T[][], T[][], int, int, int, int> action)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            var result = new T[rows][];
            for (var row = 0; row < rows; row++)
                result[row] = new T[cols];

            for (var row = 0; row < rows; row++)
                for (var col = 0; col < cols; col++)
                    action(result, matrix, rows, cols, row, col);

            return result;
        }

        public static string ToBase36Digit(this int number) =>
            number <= 9
            ? number.ToString()
            : ((char)(number - 10 + 'A')).ToString();

        public static string StringJoin<T>(this IEnumerable<T> str, string delimiter) =>
            string.Join(delimiter, str);

        public static (int rows, int cols) GetSize<T>(this T[,] matrix) =>
            (rows: matrix.GetLength(0), cols: matrix.GetLength(1));

        public static (int rows, int cols) GetSize<T>(this T[][] matrix) =>
            (rows: matrix.Length, cols: matrix[0].Length);
    }
}
