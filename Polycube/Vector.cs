using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Polycube
{
    public class Vector
    {
        private readonly int[] values;

        public int Length => values.Length;

        public int this[int i]
        {
            get => values[i];
            set => values[i] = value;
        }

        public int this[int i, int j]
        {
            //handle as column vector as well, used it to not break matrix multiplication
            //todo: should probably not silently ignore j if it's != 0 :D
            get => values[i];
            set => values[i] = value;
        }

        public int X => values[0];
        public int Y => values[1];
        public int Z => values[2];

        public void Deconstruct(out int x, out int y)
        {
            x = values[0];
            y = values[1];
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = values[0];
            y = values[1];
            z = values[2];
        }

        [JsonConstructor]
        public Vector(int length) =>
            values = new int[length];

        public Vector(int x, int y)
        {
            values = new int[2];
            values[0] = x;
            values[1] = y;
        }

        public Vector((int x, int y) xy)
        {
            values = new int[2];
            values[0] = xy.x;
            values[1] = xy.y;
        }

        public Vector(int x, int y, int z)
        {
            values = new int[3];
            values[0] = x;
            values[1] = y;
            values[2] = z;
        }

        public Vector((int x, int y, int z) xyz)
        {
            values = new int[3];
            values[0] = xyz.x;
            values[1] = xyz.y;
            values[2] = xyz.z;
        }

        public Vector(int[] values) =>
            this.values = values;

        public static Vector operator +(Vector left, int right)
        {
            //totally unnecessary and unoptimal :-)
            var offset = new Vector(left.Length).Fill(right);
            return left + offset;
        }

        public static Vector operator +(Vector left, Vector right) =>
            Translate(left, right);

        public static Vector operator -(Vector left, int right)
        {
            var offset = new Vector(left.Length).Fill(right);
            return left - offset;
        }

        public static Vector operator -(Vector left, Vector right) =>
            Translate(left, -right);

        public static Vector operator -(Vector unary)
        {
            var result = new Vector(unary.Length);
            for (int i = 0; i < unary.Length; i++)
                result[i] = -unary[i];

            return result;
        }

        public static Vector operator *(int[,] left, Vector right) =>
            Multiply(left, right);

        private static Vector Translate(Vector vector, Vector offset) =>
            MathRotation.GetTranslationMatrix(offset) * vector;

        public Vector Rotate(Vector degrees) =>
            MathRotation.GetRotationMatrix(degrees) * this;

        private static Vector Multiply(int[,] a, Vector b)
        {
            //A might have more columns than B has rows
            //if so, we assume B is filled with 1's
            //[a b c] * [x]            [x]
            //[d e f]   [y]            [y]
            //              --> assume [1]
            //[x,y,z] can then be treated as [x,y,z,1] when multiplying augmented translation matrices
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

        public Vector Fill(int value)
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = value;

            return this;
        }

        public override string ToString() => $"[{values.StringJoin(",")}]";
    }
}
