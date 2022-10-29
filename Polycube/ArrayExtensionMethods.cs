using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polycube
{
    public static class ArrayExtensionMethods
    {
        public static (int x, int y) GetLengths<T>(this T[,] source) =>
            (
                x: source.GetLength(1),
                y: source.GetLength(0)
            );

        public static (int x, int y, int z) GetLengths<T>(this T[,,] source) =>
            (
                x: source.GetLength(1),
                y: source.GetLength(0),
                z: source.GetLength(2)
            );

        public static string ToJson<T>(this T[] source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this T[,] source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this T[,,] source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this IEnumerable<T[,]> source) => JsonConvert.SerializeObject(source);
        public static IEnumerable<string> ToJsonArray<T>(this IEnumerable<T[,]> source) => source.Select(JsonConvert.SerializeObject);

        public static T FromJson<T>(this string source) => JsonConvert.DeserializeObject<T>(source);
        public static IEnumerable<T> FromJsonArray<T>(this IEnumerable<string> source) => source.Select(JsonConvert.DeserializeObject<T>);

        //todo: move
        //todo: change int[,] to Matrix/Point classes?
        //todo: create MultiplyPoint so we can multiply with [x,y,z] instead of [[x],[y],[z]]?
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

        public static int[,] Rotate(this int[,] matrix, int x, int y, int z)
        {
            var rotation = MathRotation.GetRotationMatrix(x, y, z);
            var result = rotation.Multiply(matrix);
            return result;
        }

        public static int[,] Translate(this int[,] matrix, int dx, int dy, int dz)
        {
            var translation = MathRotation.GetTranslationMatrix(dx, dy, dz);
            var result = translation.Multiply(matrix);
            return result;
        }

        public static IEnumerable<IEnumerable<int[,]>> GetUniqueRotations(this IEnumerable<int[,]> piecePoints)
        {
            var rotatedPieces = new List<IEnumerable<int[,]>>();
            foreach (var rot in MathRotation.GetUniqueRotationMatrices())
            {
                var rotatedPiece = piecePoints
                    .Select(point => rot.Multiply(point))
                    .TranslateToOrigo()
                    //sorting because [[0,1,2],[3,4,5]] == [[3,4,5],[0,1,2]]
                    //todo: there must be a better way
                    .OrderBy(p => Enumerable
                        .Range(0, p.GetLength(0))
                        .Select(n => p[n, 0])
                        .StringJoin(","));

                rotatedPieces.Add(rotatedPiece);
            }

            //hack: well, it works :)
            var unique = rotatedPieces
                .Select(points => points.ToJson())
                .Distinct()
                .Select(str => str.FromJson<IEnumerable<int[,]>>());

            return unique;
        }

        //todo: perhaps throw away
        //public static int[,] TranslateTest(this int[,] matrix, int[] point)
        //{
        //    var translation = MathRotation.GetTranslationMatrixTest(point);
        //    var result = translation.Multiply(matrix);
        //    return result;
        //}

        public static IEnumerable<int[,]> TranslateToOrigo(this IEnumerable<int[,]> points)
        {
            var xyz = points.GetAxesMinValues();
            return points.Select(point => point.Translate(-xyz[0], -xyz[1], -xyz[2]));
        }

        //todo: vertical matrices for points should probably be made horizontal?
        public static int[] GetAxesMinValues(this IEnumerable<int[,]> points)
        {
            var yLen = points.First().GetLengths().y;
            var min = new int[yLen];
            for (int i = 0; i < yLen; i++)
            {
                min[i] = int.MaxValue;
            }

            foreach (var point in points)
            {
                for (int i = 0; i < yLen; i++)
                {
                    if (point[i, 0] < min[i])
                    {
                        min[i] = point[i, 0];
                    }
                }
            }

            return min;
        }

        public static int[] GetAxesMaxValues(this IEnumerable<int[,]> points)
        {
            var yLen = points.First().GetLengths().y;
            var min = new int[yLen];
            for (int i = 0; i < yLen; i++)
            {
                min[i] = int.MinValue;
            }

            foreach (var point in points)
            {
                for (int i = 0; i < yLen; i++)
                {
                    if (point[i, 0] > min[i])
                    {
                        min[i] = point[i, 0];
                    }
                }
            }

            return min;
        }
    }
}
