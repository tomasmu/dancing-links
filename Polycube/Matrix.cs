using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolycubeSolver
{
    public class Matrix
    {
        public readonly int[,] Grid;

        public Matrix(int[,] matrix) =>
            Grid = matrix;

        public Matrix(int yLength, int xLength) =>
            Grid = new int[yLength, xLength];

        public int this[int y, int x]
        {
            get => Grid[y, x];
            set => Grid[y, x] = value;
        }

        public static Matrix operator *(Matrix left, Matrix right) =>
            Multiply(left, right);

        public static Vector operator *(Matrix left, Vector right) =>
            Multiply(left, right);

        public Vector GetLengths() =>
            new(Grid.GetLength(1), Grid.GetLength(0));

        private static Matrix Multiply(Matrix a, Matrix b)
        {
            //A might have more columns than B has rows
            //if so, we assume B is filled with 1's
            //[a b c] * [x]            [x]
            //[d e f]   [y]            [y]
            //              --> assume [1]
            //[x,y,z] can then be treated as [x,y,z,1] when multiplying augmented matrices

            var aLen = a.GetLengths();
            var bLen = b.GetLengths();
            if (aLen.X < bLen.Y)
            {
                throw new Exception(
                    $"Multiplication dimensions too incompatible." +
                    $" Dimensions received: {aLen.Y}x{aLen.X}<- < ->{bLen.Y}x{bLen.X}");
            }

            var product = new Matrix(aLen.Y, bLen.X);
            for (int ay = 0; ay < aLen.Y; ay++)
            {
                for (int bx = 0; bx < bLen.X; bx++)
                {
                    for (int i = 0; i < aLen.X; i++)
                    {
                        //if b has too few rows, assume b[i, bx] is 1
                        var bi = i < bLen.Y ? b[i, bx] : 1;
                        product[ay, bx] += a[ay, i] * bi;
                    }
                }
            }

            return product;
        }

        private static Vector Multiply(Matrix a, Vector b)
        {
            //A might have more columns than B has rows
            //if so, we assume B is filled with 1's
            //[a b c] * [x]            [x]
            //[d e f]   [y]            [y]
            //              --> assume [1]
            //[x,y,z] can then be treated as [x,y,z,1] when multiplying augmented matrices

            var aLen = a.GetLengths();
            if (aLen.X < b.Length)
            {
                throw new Exception(
                    $"Multiplication dimensions too incompatible." +
                    $" Dimensions received: {aLen.X}x{aLen.Y}<- < ->{b.Length}x1");
            }

            var product = new Vector(aLen.Y);
            for (int ay = 0; ay < aLen.Y; ay++)
            {
                for (int ax = 0; ax < aLen.X; ax++)
                {
                    //if b has too few rows, assume b[ax] is 1
                    var b_ax = ax < b.Length ? b[ax] : 1;
                    product[ay] += a[ay, ax] * b_ax;
                }
            }

            return product;
        }
    }
}
