using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolycubeSolver
{
    public static class ArrayExtensionMethods
    {
        public static Vector GetLengths<T>(this T[,,] source) =>
            new(source.GetLength(1), source.GetLength(0), source.GetLength(2));

        public static string ToJson<T>(this T source) => JsonConvert.SerializeObject(source);
        public static string ToJson<T>(this T[,] source) => JsonConvert.SerializeObject(source);

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

            //hack: well, it works.. :)
            var unique = rotatedPieces
                .GroupBy(r => r.ToJson())
                .Select(g => g.First());

            return unique;
        }

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
