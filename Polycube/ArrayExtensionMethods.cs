using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polycube
{
    public static class ArrayExtensionMethods
    {
        public static Vector GetLengths<T>(this T[,] source) =>
            new(source.GetLength(1), source.GetLength(0));

        public static Vector GetLengths<T>(this T[,,] source) =>
            new(source.GetLength(1), source.GetLength(0), source.GetLength(2));

        public static string ToJson<T>(this T source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this T[] source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this T[,] source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this T[,,] source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this IEnumerable<T[,]> source) => JsonConvert.SerializeObject(source);
        public static IEnumerable<string> ToJsonArray<T>(this IEnumerable<Vector> source) => source.Select(JsonConvert.SerializeObject);
        public static IEnumerable<string> ToJsonArray<T>(this IEnumerable<T[,]> source) => source.Select(JsonConvert.SerializeObject);

        public static T FromJson<T>(this string source) => JsonConvert.DeserializeObject<T>(source);
        public static IEnumerable<T> FromJsonArray<T>(this IEnumerable<string> source) => source.Select(JsonConvert.DeserializeObject<T>);

        //todo: move
        public static int[,] Multiply(this int[,] a, int[,] b)
        {
            //A might have more columns than B has rows
            //if so, we assume B is filled with 1's
            //[a b c] * [x]            [x]
            //[d e f]   [y]            [y]
            //              --> assume [1]
            //then we can treat [x;y;z] as [x;y;z;1] when multiplying translations

            var (axLen, ayLen) = a.GetLengths();
            var (bxLen, byLen) = b.GetLengths();
            if (axLen < byLen)
            {
                throw new Exception(
                    $"Multiplication dimensions too incompatible." +
                    $" Dimensions received: {ayLen}x{axLen}<- < ->{byLen}x{bxLen}");
            }

            var product = new int[ayLen, bxLen];
            for (int ay = 0; ay < ayLen; ay++)
            {
                for (int bx = 0; bx < bxLen; bx++)
                {
                    for (int i = 0; i < axLen; i++)
                    {
                        //if b has too few rows, assume b[i, bx] is 1
                        if (i < byLen)
                            product[ay, bx] += a[ay, i] * b[i, bx];
                        else
                            product[ay, bx] += a[ay, i];
                    }
                }
            }

            return product;
        }

        public static int[,] MultiplySafe(this int[,] a, int[,] b)
        {
            var (axLen, ayLen) = a.GetLengths();
            var (bxLen, byLen) = b.GetLengths();
            if (axLen != byLen)
            {
                throw new ArgumentException(
                    $"Multiplication dimensions not compatible." +
                    $" Dimensions received: {ayLen}x{axLen}<- != ->{byLen}x{bxLen}");
            }

            var product = new int[ayLen, bxLen];
            for (int ay = 0; ay < ayLen; ay++)
            {
                for (int bx = 0; bx < bxLen; bx++)
                {
                    for (int i = 0; i < axLen; i++)
                    {
                        product[ay, bx] += a[ay, i] * b[i, bx];
                    }
                }
            }

            return product;
        }

        public static IEnumerable<IEnumerable<Vector>> GetUniqueRotations(this IEnumerable<Vector> piecePoints)
        {
            var rotatedPieces = new List<IEnumerable<Vector>>();
            foreach (var rot in MathRotation.GetUniqueRotationMatrices())
            {
                var rotatedPiece = piecePoints
                    .Select(point => rot * point)
                    .TranslateToOrigo()
                    //sorting because [[0,1,2],[3,4,5]] == [[3,4,5],[0,1,2]]
                    //todo: there must be a better way
                    .OrderBy(p => Enumerable
                        .Range(0, p.Length)
                        .Select(n => p[n])
                        .StringJoin(","))
                    .ToList();

                rotatedPieces.Add(rotatedPiece);
            }

            //hack: well, ugly but works.. :)
            var unique = rotatedPieces
                .Select(points => points
                    .Select(p => p.ToString())
                    .StringJoin(";"))
                .Distinct()
                .Select(strPoints => strPoints
                    .Split(';')
                    .Select(strPoint =>
                        new Vector(strPoint
                            .RegexRemove(@"[\[\]]")
                            .Split(',')
                            .Select(int.Parse)
                            .ToArray())));

            return unique;
        }

        //todo: perhaps throw away
        //public static int[,] TranslateTest(this int[,] matrix, int[] point)
        //{
        //    var translation = MathRotation.GetTranslationMatrixTest(point);
        //    var result = translation.Multiply(matrix);
        //    return result;
        //}

        public static IEnumerable<Vector> TranslateToOrigo(this IEnumerable<Vector> points)
        {
            var min = points.GetAxesMinValues();
            return points.Select(point => point - min);
        }

        public static Vector GetAxesMinValues(this IEnumerable<Vector> points)
        {
            var yLen = points.First().Length;
            var min = new Vector(yLen);
            for (int i = 0; i < yLen; i++)
            {
                min[i] = int.MaxValue;
            }

            foreach (var point in points)
            {
                for (int i = 0; i < yLen; i++)
                {
                    if (point[i] < min[i])
                    {
                        min[i] = point[i];
                    }
                }
            }

            return min;
        }

        public static Vector GetAxesMaxValues(this IEnumerable<Vector> points)
        {
            var yLen = points.First().Length;
            var max = new Vector(yLen);
            for (int i = 0; i < yLen; i++)
            {
                max[i] = int.MinValue;
            }

            foreach (var point in points)
            {
                for (int i = 0; i < yLen; i++)
                {
                    if (point[i] > max[i])
                    {
                        max[i] = point[i];
                    }
                }
            }

            return max;
        }
    }
}
