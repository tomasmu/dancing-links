using Newtonsoft.Json;

namespace PolycubeSolver
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

        public Vector(int x, int y) =>
            values = new int[] { x, y };

        public Vector((int x, int y) xy) =>
            values = new int[] { xy.x, xy.y };

        public Vector(int x, int y, int z) =>
            values = new int[] { x, y, z };

        public Vector((int x, int y, int z) xyz) =>
            values = new int[] { xyz.x, xyz.y, xyz.z };

        public Vector(int[] values) =>
            this.values = values;

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
            //really slow:
            //return Translate(left, right);
        }

        public static Vector operator -(Vector left, Vector right)
        {
            var result = new Vector(left.Length);
            for (int i = 0; i < left.Length; i++)
                result[i] = left[i] - right[i];
            
            return result;
        }

        private static Vector Translate(Vector vector, Vector offset) =>
            MathRotation.GetTranslationMatrixAugmented(offset) * vector;

        public Vector Rotate(Vector degrees) =>
            MathRotation.GetRotationMatrixAugmented(degrees) * this;

        public override bool Equals(object obj)
        {
            if (obj is Vector vector)
            {
                if (values.Length != vector.Length)
                    return false;

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] != vector[i])
                        return false;
                }

                return true;
            }

            return false;
        }

        //deterministic gethashcode
        public override int GetHashCode()
        {
            //no collisions for x,y and x,y,z in 0..500
            var hash = unchecked((int)2166136261);
            for (int i = 0; i < values.Length; i++)
                hash = (hash ^ values[i]) * 16_777_619;

            return hash;
        }

        public override string ToString() => $"[{values.StringJoin(",")}]";
    }
}
