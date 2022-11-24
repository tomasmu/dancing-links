using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolycubeSolver
{
    public class Vector : IComparable, IComparable<Vector>, IEquatable<Vector>
    {
        private readonly int[] values;

        public int Length => values.Length;

        public int this[int i]
        {
            get => values[i];
            set => values[i] = value;
        }

        public int X => values[0];
        public int Y => values[1];
        public int Z => values[2];

        [JsonConstructor]
        public Vector(int length) => values = new int[length];
        public Vector(int x, int y) => values = new int[] { x, y };
        public Vector(int x, int y, int z) => values = new int[] { x, y, z };
        public Vector((int x, int y) xy) => values = new int[] { xy.x, xy.y };
        public Vector((int x, int y, int z) xyz) => values = new int[] { xyz.x, xyz.y, xyz.z };
        public Vector(int[] values) => this.values = values.ToArray();
        public Vector(Vector vector) => values = vector.values.ToArray();

        public void Deconstruct(out int x, out int y) => (x, y) = (values[0], values[1]);
        public void Deconstruct(out int x, out int y, out int z) => (x, y, z) = (values[0], values[1], values[2]);

        public static Vector operator +(Vector left, int right)
        {
            var result = new Vector(left.Length);
            for (int i = 0; i < left.Length; i++)
                result[i] = left[i] + right;

            return result;
        }

        public static Vector operator +(Vector left, Vector right)
        {
            var result = new Vector(left.Length);
            for (int i = 0; i < left.Length; i++)
                result[i] = left[i] + right[i];

            return result;
            //return Translate(left, right);    //slow
        }

        public static Vector operator -(Vector left, Vector right)
        {
            var result = new Vector(left.Length);
            for (int i = 0; i < left.Length; i++)
                result[i] = left[i] - right[i];
            
            return result;
        }

        private static Vector Translate(Vector vector, Vector offset) =>
            MathRotation.GetTranslationMatrix(offset) * vector;

        public Vector Rotate(Vector degrees) =>
            MathRotation.GetRotationMatrix(degrees) * this;

        public static bool operator >(Vector left, Vector right) => Compare(left, right) > 0;
        public static bool operator <(Vector left, Vector right) => Compare(left, right) < 0;
        public static bool operator >=(Vector left, Vector right) => Compare(left, right) >= 0;
        public static bool operator <=(Vector left, Vector right) => Compare(left, right) <= 0;

        public static bool operator !=(Vector left, Vector right) => !(left == right);
        public static bool operator ==(Vector left, Vector right) =>
            left is null
            ? right is null
            : left.Equals(right);

        public override bool Equals(object obj) => Equals(obj as Vector);
        public bool Equals(Vector right) => CompareTo(right) == 0;

        public static int Compare(Vector left, Vector right) =>
            left is null
            ? -1
            : left.CompareTo(right);

        public int CompareTo(object obj) => CompareTo(obj as Vector);

        public int CompareTo(Vector right)
        {
            if (right is null)
                return 1;   //null sorted first

            if (ReferenceEquals(this, right))
                return 0;

            var length = Math.Min(values.Length, right.Length);
            for (int i = 0; i < length; i++)
            {
                if (values[i] > right[i])
                    return 1;
                if (values[i] < right[i])
                    return -1;
            }

            //sort short before long
            if (values.Length > right.Length)
                return 1;
            if (values.Length < right.Length)
                return -1;

            return 0;
        }

        public override int GetHashCode()
        {
            //deterministic gethashcode
            var hash = unchecked((int)2_166_136_261);
            for (int i = 0; i < values.Length; i++)
                hash = (hash ^ values[i]) * 16_777_619;

            return hash;
        }

        public IEnumerable<Vector> AsEnumerable()
        {
            yield return this;  // :D
        }

        public override string ToString() => JsonConvert.SerializeObject(values);
    }
}
