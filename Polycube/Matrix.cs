using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolycubeSolver
{
    public class Matrix : IEquatable<Matrix>
    {
        private readonly int[,] Grid;

        public Matrix(int[,] matrix) => Grid = matrix;

        public Matrix(int yLength, int xLength) => Grid = new int[yLength, xLength];

        public int this[int y, int x]
        {
            get => Grid[y, x];
            set => Grid[y, x] = value;
        }

        public static Matrix operator *(Matrix left, Matrix right) => MultiplyMatrix(left, right);
        public static Vector operator *(Matrix left, Vector right) => MultiplyVector(left, right);

        public Vector Length => new(Grid.GetLength(1), Grid.GetLength(0));

        private static Matrix MultiplyMatrix(Matrix a, Matrix b)
        {
            //A might have more columns than B has rows
            //if so, we assume B is filled with 1's
            //[a b c] * [x]            [x]
            //[d e f]   [y]            [y]
            //              --> assume [1]
            //[x,y,z] then works as [x,y,z,1] when multiplying augmented matrices

            if (a.Length.X < b.Length.Y)
            {
                throw new Exception(
                    $"Multiplication dimensions too incompatible." +
                    $" Dimensions received: {a.Length.Y}x{a.Length.X}<- < ->{b.Length.Y}x{b.Length.X}");
            }

            var product = new Matrix(a.Length.Y, b.Length.X);
            for (int ay = 0; ay < a.Length.Y; ay++)
            {
                for (int bx = 0; bx < b.Length.X; bx++)
                {
                    for (int i = 0; i < a.Length.X; i++)
                    {
                        //if b has too few rows, assume b[i, bx] is 1
                        var bi = i < b.Length.Y ? b[i, bx] : 1;
                        product[ay, bx] += a[ay, i] * bi;
                    }
                }
            }

            return product;
        }

        private static Vector MultiplyVector(Matrix a, Vector b)
        {
            //A might have more columns than B has rows
            //if so, we assume B is filled with 1's
            //[a b c] * [x]            [x]
            //[d e f]   [y]            [y]
            //              --> assume [1]
            //[x,y,z] then works as [x,y,z,1] when multiplying augmented matrices

            if (a.Length.X < b.Length)
            {
                throw new Exception(
                    $"Multiplication dimensions too incompatible." +
                    $" Dimensions received: {a.Length.X}x{a.Length.Y}<- < ->{b.Length}x1");
            }

            var product = new Vector(a.Length.Y);
            for (int ay = 0; ay < a.Length.Y; ay++)
            {
                for (int ax = 0; ax < a.Length.X; ax++)
                {
                    //if b has too few rows, assume b[ax] is 1
                    var b_ax = ax < b.Length ? b[ax] : 1;
                    product[ay] += a[ay, ax] * b_ax;
                }
            }

            return product;
        }

        public override bool Equals(object obj) => Equals(obj as Matrix);

        public bool Equals(Matrix right)
        {
            if (right is null)
                return false;

            if (ReferenceEquals(this, right))
                return true;

            if (Length != right.Length)
                return false;

            for (int y = 0; y < Length.Y; y++)
                for (int x = 0; x < Length.X; x++)
                    if (Grid[y, x] != right.Grid[y, x])
                        return false;

            return true;
        }

        public override int GetHashCode()
        {
            //deterministic gethashcode
            var hash = unchecked((int)2_166_136_261);
            for (int y = 0; y < Length.Y; y++)
                for (int x = 0; x < Length.X; x++)
                    hash = (hash ^ Grid[y, x]) * 16_777_619;

            return hash;
        }

        public override string ToString() => JsonConvert.SerializeObject(Grid);
    }
}
