using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DancingLinks
{
    public static class ExtensionMethods
    {
        public static bool[] GetRow(this bool[,] matrix, int row) =>
            Enumerable.Range(0, matrix.GetLength(1))
                .Select(col => matrix[row, col])
                .ToArray();
    }
}
