using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolycubeSolver
{
    public static class ArrayExtensionMethods
    {
        public static bool NotIn<T>(this T value, IEnumerable<T> collection) =>
            !collection.Contains(value);

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
                for (int i = 0; i < yLen; i++)
                    if (point[i] < min[i])
                        min[i] = point[i];

            return min;
        }

        public static (Vector min, Vector max) GetAxesMinMaxValues(this IEnumerable<Vector> points)
        {
            var p = points.First();
            var min = new Vector(p.Length);
            var max = new Vector(p.Length);
            for (int i = 0; i < p.Length; i++)
            {
                min[i] = int.MaxValue;
                max[i] = int.MinValue;
            }

            foreach (var point in points)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    if (point[i] > max[i])
                        max[i] = point[i];
                    if (point[i] < min[i])
                        min[i] = point[i];
                }
            }

            return (min, max);
        }
    }
}
