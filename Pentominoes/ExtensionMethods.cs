using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pentominoes
{
    public static class ExtensionMethods
    {
        public static IEnumerable<bool[][]> ToRectangleMatrix(this IEnumerable<string> stringLiterals, params char[] blockingChars) =>
            stringLiterals.Select(str => str.ToRectangleMatrix(blockingChars));

        public static bool[][] ToRectangleMatrix(this string stringLiteral, params char[] blockingChars)
        {
            static bool FirstRowNotEmpty(string str, int index) => index != 0 || str.Length > 0;
            var newLinePattern = @"\r\n|\r|\n";
            var stringRows = Regex
                .Split(stringLiteral, newLinePattern)
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

        public static bool[][][] ToCuboidMatrix(this string stringLiteral) =>
            stringLiteral.ToCuboidMatrix('-');

        public static bool[][][] ToCuboidMatrix(this string stringLiteral, char emptyChar)
        {
            var singleNewLinePattern = @"\r\n|\r|\n";
            var doubleNewLinePattern = @"(\r\n){2}|\r{2}|\n{2}";
            var jaggedArray = stringLiteral
                .Trim()
                .RegexSplit(doubleNewLinePattern, RegexOptions.ExplicitCapture)
                .Select(str => str
                    .RegexSplit(singleNewLinePattern, RegexOptions.ExplicitCapture)
                    .Select(str => str.ToCharArray())
                    .ToArray())
                .ToArray();

            var yLen = jaggedArray.Length;
            var xLen = jaggedArray.Max(array => array.Length);
            var zLen = jaggedArray.Max(array => array.Max(arr => arr.Length));

            //create cuboid array
            var cuboidArray = new bool[yLen][][];
            for (int y = 0; y < yLen; y++)
            {
                cuboidArray[y] = new bool[xLen][];
                for (int x = 0; x < xLen; x++)
                {
                    cuboidArray[y][x] = new bool[zLen];
                }
            }

            //map jagged values to cuboid
            bool MapCharToBool(char chr) => chr != emptyChar;
            for (int y = 0; y < jaggedArray.Length; y++)
            {
                for (int x = 0; x < jaggedArray[y].Length; x++)
                {
                    for (int z = 0; z < jaggedArray[y][x].Length; z++)
                    {
                        cuboidArray[y][x][z] = MapCharToBool(jaggedArray[y][x][z]);
                    }
                }
            }

            return cuboidArray;
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

        public static string ToStringThing(this bool[][][] matrix)
        {
            static string cellFormat(bool b) => b ? "X" : " ";
            var cellDelimiter = " ";
            return matrix.Select((y, i) => y
                    .Select(x => $"{new string(' ', i)}[ {x.Select(cellFormat).StringJoin(cellDelimiter)} ]")
                    .StringJoin(Environment.NewLine))
                .StringJoin(Environment.NewLine);
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
        //replaced! saving for now
        //public static T[][] Rotate0<T>(this T[][] matrix) => matrix;
        //public static T[][] Rotate90<T>(this T[][] matrix) => matrix.Transpose().MirrorHorizontal();
        //public static T[][] Rotate180<T>(this T[][] matrix) => matrix.MirrorHorizontal().MirrorVertical();
        //public static T[][] Rotate270<T>(this T[][] matrix) => matrix.Transpose().MirrorVertical();
        //public static T[][] Rotate0Mirror<T>(this T[][] matrix) => matrix.MirrorHorizontal();
        //public static T[][] Rotate90Mirror<T>(this T[][] matrix) => matrix.MirrorHorizontal().MirrorVertical().Transpose();
        //public static T[][] Rotate180Mirror<T>(this T[][] matrix) => matrix.MirrorVertical();
        //public static T[][] Rotate270Mirror<T>(this T[][] matrix) => matrix.Transpose();

        //private static T[][] Transpose<T>(this T[][] matrix)
        //{
        //    var rows = matrix.Length;
        //    var cols = matrix[0].Length;
        //    var result = new T[cols][];
        //    for (var col = 0; col < cols; col++)
        //        result[col] = new T[rows];

        //    for (var row = 0; row < rows; row++)
        //        for (var col = 0; col < cols; col++)
        //            result[col][row] = matrix[row][col];

        //    return result;
        //}

        //private static T[][] MirrorHorizontal<T>(this T[][] matrix) => PerformAssignment(matrix, MirrorHorizontalAssignment);
        //private static T[][] MirrorVertical<T>(this T[][] matrix) => PerformAssignment(matrix, MirrorVerticalAssignment);

        //private static void MirrorHorizontalAssignment<T>(T[][] result, T[][] matrix, int rows, int cols, int row, int col) =>
        //    result[row][col] = matrix[row][cols - 1 - col];

        //private static void MirrorVerticalAssignment<T>(T[][] result, T[][] matrix, int rows, int cols, int row, int col) =>
        //    result[row][col] = matrix[rows - 1 - row][col];

        //private static T[][] PerformAssignment<T>(this T[][] matrix, Action<T[][], T[][], int, int, int, int> action)
        //{
        //    var rows = matrix.Length;
        //    var cols = matrix[0].Length;
        //    var result = new T[rows][];
        //    for (var row = 0; row < rows; row++)
        //        result[row] = new T[cols];

        //    for (var row = 0; row < rows; row++)
        //        for (var col = 0; col < cols; col++)
        //            action(result, matrix, rows, cols, row, col);

        //    return result;
        //}

        public static (int x, int y, int z) RotatePoint((int x, int y, int z) point, (int x, int y, int z) degrees)
        {
            var rotation = GetRotationMatrix(degrees);
            var point2 = MultiplyPoint(rotation, point);
            return point2;
        }

        //3D rotation in a 2D project :D
        public static T[][][] Rotate<T>(this T[][][] matrix, (int x, int y, int z) degrees)
        {
            var dimension = matrix.GetDimension();
            var rotation = GetRotationMatrix(degrees);

            //create a mapping: (x, y, z) rotates to (x2, y2, z2)
            //GetMapping(etc)
            var mapping = new Dictionary<(int x, int y, int z), (int x2, int y2, int z2)>();
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    for (int z = 0; z < dimension.z; z++)
                    {
                        var point = (x, y, z);
                        var point2 = RotatePoint(point, degrees);
                        mapping[point] = point2;
                    }
                }
            }

            //move to origo so we don't have any negative coordinates
            var p2Min = mapping.Values.Aggregate((acc, cur) =>
                (Math.Min(cur.x2, acc.x2), Math.Min(cur.y2, acc.y2), Math.Min(cur.z2, acc.z2)));
            var mapping2 = new Dictionary<(int x, int y, int z), (int x2, int y2, int z2)>();
            foreach (var (p, p2) in mapping)
            {
                var p3 = OffsetPoint(p2, p2Min);
                mapping2[p] = p3;
            }

            //create array with new dimensions
            //Create(etc)
            var p2Max = mapping2.Values.Aggregate((acc, cur) =>
                (Math.Max(cur.x2, acc.x2), Math.Max(cur.y2, acc.y2), Math.Max(cur.z2, acc.z2)));
            var matrix2 = new T[p2Max.y2 + 1][][];
            //having y,x,z is slightly confusing, rotate the axes some day?
            for (int y = 0; y < p2Max.y2 + 1; y++)
            {
                matrix2[y] = new T[p2Max.x2 + 1][];
                for (int x = 0; x < p2Max.x2 + 1; x++)
                {
                    matrix2[y][x] = new T[p2Max.z2 + 1];
                }
            }

            //copy to new place
            foreach (var ((x, y, z), (x2, y2, z2)) in mapping2)
            {
                matrix2[y2][x2][z2] = matrix[y][x][z];
            }

            return matrix2;
        }

        //Translate?
        public static (int x, int y, int z) OffsetPoint(
            (int x, int y, int z) point,
            (int dx, int dy, int dz) offset) =>
                (point.x - offset.dx,
                 point.y - offset.dy,
                 point.z - offset.dz);

        public static T[][] Rotate<T>(this T[][] matrix, int degrees) =>
            Rotate(matrix, degrees, false);

        public static T[][] Rotate<T>(this T[][] matrix, int degrees, bool mirror) =>
            mirror
            ? Rotate(matrix, (0, 180, degrees))
            : Rotate(matrix, (0, 0, degrees));

        //Rotating in 2D by using 3D rotation :)
        public static T[][] Rotate<T>(this T[][] matrix, (int x, int y, int z) degrees)
        {
            //create
            var matrix3d = new T[matrix.Length][][];
            for (int y = 0; y < matrix.Length; y++)
            {
                matrix3d[y] = new T[matrix[y].Length][];
                for (int x = 0; x < matrix[y].Length; x++)
                {
                    matrix3d[y][x] = new T[1];
                    //copy
                    matrix3d[y][x][0] = matrix[y][x];
                }
            }

            //rotate
            var rotated3d = matrix3d.Rotate(degrees);
            //3D -> 2D
            var rotated2d = new T[rotated3d.Length][];
            for (int y = 0; y < rotated3d.Length; y++)
            {
                rotated2d[y] = new T[rotated3d[y].Length];
                for (int x = 0; x < rotated3d[y].Length; x++)
                {
                    rotated2d[y][x] = rotated3d[y][x][0];
                }
            }

            return rotated2d;
        }

        public static int[][] GetRotationMatrix((int x, int y, int z) degrees)
        {
            var (x, y, z) = degrees;

            var (sinx, cosx) = (SinInt(x), CosInt(x));
            var rx = new int[][]
            {
                new int[] { 1, 0,    0 },
                new int[] { 0, cosx, -sinx },
                new int[] { 0, sinx, cosx },
            };

            var (siny, cosy) = (SinInt(y), CosInt(y));
            var ry = new int[][]
            {
                new int[] { cosy,  0, siny },
                new int[] { 0,     1, 0 },
                new int[] { -siny, 0, cosy },
            };

            var (sinz, cosz) = (SinInt(z), CosInt(z));
            var rz = new int[][]
            {
                new int[] { cosz, -sinz, 0 },
                new int[] { sinz, cosz,  0 },
                new int[] { 0,    0,     1 },
            };

            var result = MultiplyMatrix(MultiplyMatrix(rz, ry), rx);
            return result;
        }

        //private static int SinInt(int degrees) => (int)Math.Round(Math.Sin(Math.PI / 180 * degrees));
        //private static int CosInt(int degrees) => (int)Math.Round(Math.Cos(Math.PI / 180 * degrees));

        //simple grid trigonometry
        private static int SinInt(int degrees) => (degrees % 360) switch
        {
            0 => 0,
            90 => 1,
            180 => 0,
            270 => -1,
            _ => throw new ArgumentException($"{nameof(SinInt)} argument must be a multiple of 90, but was {degrees}")
        };

        private static int CosInt(int degrees) => (degrees % 360) switch
        {
            0 => 1,
            90 => 0,
            180 => -1,
            270 => 0,
            _ => throw new ArgumentException($"{nameof(CosInt)} argument must be a multiple of 90, but was {degrees}")
        };

        //should be extension method
        private static int[][] MultiplyMatrix(int[][] a, int[][] b)
        {
            var result = new int[a.Length][];
            for (int aRow = 0; aRow < a.Length; aRow++)
            {
                result[aRow] = new int[b[aRow].Length];
                for (int bCol = 0; bCol < b[0].Length; bCol++)
                {
                    for (int i = 0; i < a[aRow].Length; i++)
                    {
                        result[aRow][bCol] += a[aRow][i] * b[i][bCol];
                    }
                }
            }

            return result;
        }

        //hmm
        private static (int x, int y, int z) MultiplyPoint(int[][] rotation, (int x, int y, int z) point)
        {
            var p = new int[][]
            {
                new int[] { point.x },
                new int[] { point.y },
                new int[] { point.z },
            };

            var point2 = MultiplyMatrix(rotation, p);

            var x2 = point2[0][0];
            var y2 = point2[1][0];
            var z2 = point2[2][0];

            return (x2, y2, z2);
        }

        private static IEnumerable<T[][]> GetAllRotationsAndMirrors<T>(this T[][] matrix, bool includeRotations = true, bool includeMirrors = true)
        {
            yield return matrix.Rotate(0);

            if (includeRotations)
            {
                yield return matrix.Rotate(90);
                yield return matrix.Rotate(180);
                yield return matrix.Rotate(270);

                if (includeMirrors)
                {
                    yield return matrix.Rotate(0, true);
                    yield return matrix.Rotate(90, true);
                    yield return matrix.Rotate(180, true);
                    yield return matrix.Rotate(270, true);
                }
            }
        }


        public static T[][][] GetUniqueRotationsAndMirrors<T>(this T[][] matrix) =>
            GetAllRotationsAndMirrors(matrix, true, true)
                .GroupBy(m => m.ToStringThing())
                .Select(g => g.First())
                .ToArray();

        public static string ToBase36Digit(this int number) =>
            number <= 9
            ? number.ToString()
            : ((char)(number - 10 + 'A')).ToString();

        public static string StringJoin<T>(this IEnumerable<T> str, string delimiter) =>
            string.Join(delimiter, str);

        public static string[] RegexSplit(this string input, string pattern, RegexOptions options) =>
            Regex.Split(input, pattern, options);

        public static string[] RegexSplit(this string input, string pattern) =>
            Regex.Split(input, pattern);

        public static (int x, int y) GetDimension<T>(this T[,] matrix) =>
            (x: matrix.GetLength(1), y: matrix.GetLength(0));

        public static (int x, int y) GetDimension<T>(this T[][] matrix) =>
            (x: matrix[0].Length, y: matrix.Length);

        public static (int x, int y, int z) GetDimension<T>(this T[][][] matrix) =>
            (x: matrix[0].Length, y: matrix.Length, z: matrix[0][0].Length);
    }
}
