using System;
using System.Collections.Generic;
using System.Linq;

namespace Polycube
{
    public static class MathRotation
    {
        public static int SinInt(int degrees) => Modulo(degrees, 360) switch
        {
            0 => 0,
            90 => 1,
            180 => 0,
            270 => -1,
            _ => throw new ArgumentException($"Argument must be a multiple of 90, but got {degrees}")
        };

        public static int CosInt(int degrees) => Modulo(degrees, 360) switch
        {
            0 => 1,
            90 => 0,
            180 => -1,
            270 => 0,
            _ => throw new ArgumentException($"Argument must be a multiple of 90, but got {degrees}")
        };

        //modulo that's always >= 0
        //e.g. Modulo(-90, 360) -> 270
        public static int Modulo(int a, int b)
        {
            var remainder = a % b;

            return remainder >= 0
                ? remainder
                : remainder + b;
        }

        public static int[,] GetRotationMatrix(int x, int y, int z)
        {
            var sinx = SinInt(x);
            var cosx = CosInt(x);
            var rx = new int[,]
            {
                { 1,    0,     0 },
                { 0, cosx, -sinx },
                { 0, sinx,  cosx },
            };

            var siny = SinInt(y);
            var cosy = CosInt(y);
            var ry = new int[,]
            {
                {  cosy, 0, siny },
                {     0, 1,    0 },
                { -siny, 0, cosy },
            };

            var sinz = SinInt(z);
            var cosz = CosInt(z);
            var rz = new int[,]
            {
                { cosz, -sinz, 0 },
                { sinz,  cosz, 0 },
                {    0,     0, 1 },
            };

            var result = rz.Multiply(ry).Multiply(rx);
            return result;
        }

        public static int[,] GetRotationMatrix_4x4(int x, int y, int z)
        {
            //using 4x4 rotation matrices
            //to be compatible with translation matrices
            //hack: 3x4 matrices suffice for now
            var sinx = SinInt(x);
            var cosx = CosInt(x);
            var rx = new int[,]
            {
                { 1,    0,     0, 0 },
                { 0, cosx, -sinx, 0 },
                { 0, sinx,  cosx, 0 },
              //{ 0,    0,     0, 1 },
            };

            var siny = SinInt(y);
            var cosy = CosInt(y);
            var ry = new int[,]
            {
                {  cosy, 0, siny, 0 },
                {     0, 1,    0, 0 },
                { -siny, 0, cosy, 0 },
              //{     0, 0,    0, 1 },
            };

            var sinz = SinInt(z);
            var cosz = CosInt(z);
            var rz = new int[,]
            {
                { cosz, -sinz, 0, 0 },
                { sinz, cosz,  0, 0 },
                {    0,    0,  1, 0 },
              //{    0,    0,  0, 1 },
            };

            var result = rz.Multiply(ry).Multiply(rx);
            return result;
        }

        public static IEnumerable<int[,]> GetUniqueRotationMatrices()
        {
            var rotationList = new List<int[,]>();
            for (int x = 0; x < 360; x += 90)
            {
                for (int y = 0; y < 360; y += 90)
                {
                    for (int z = 0; z < 360; z += 90)
                    {
                        var rotation = GetRotationMatrix(x, y, z);
                        rotationList.Add(rotation);
                    }
                }
            }

            //hack: easiest way of getting all unique matrices :-)
            var unique = rotationList
                .ToJsonArray()
                .ToHashSet()
                .FromJsonArray<int[,]>();

            return unique;
        }

        public static int[,] GetTranslationMatrix(int dx, int dy, int dz)
        {
            //hack: 3x4 matrix suffice for now
            var translation = new int[,]
            {
                { 1, 0, 0, dx },
                { 0, 1, 0, dy },
                { 0, 0, 1, dz },
              //{ 0, 0, 0,  1 },
            };

            return translation;
        }
    }
}
