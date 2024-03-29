﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PolycubeSolver
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

        public static Matrix GetRotationMatrix(Vector degrees)
        {
            var sinx = SinInt(degrees.X);
            var cosx = CosInt(degrees.X);
            var rx = new Matrix(new int[,]
            {
                { 1,    0,     0 },
                { 0, cosx, -sinx },
                { 0, sinx,  cosx },
            });

            var siny = SinInt(degrees.Y);
            var cosy = CosInt(degrees.Y);
            var ry = new Matrix(new int[,]
            {
                {  cosy, 0, siny },
                {     0, 1,    0 },
                { -siny, 0, cosy },
            });

            var sinz = SinInt(degrees.Z);
            var cosz = CosInt(degrees.Z);
            var rz = new Matrix(new int[,]
            {
                { cosz, -sinz, 0 },
                { sinz,  cosz, 0 },
                {    0,     0, 1 },
            });

            var result = rz * ry * rx;
            return result;
        }

        public static Matrix GetRotationMatrixAugmented(Vector degrees)
        {
            //using 4x4 rotation matrices
            //to be compatible with translation matrices
            //hack: 3x4 matrices suffice for now
            var sinx = SinInt(degrees.X);
            var cosx = CosInt(degrees.X);
            var rx = new Matrix(new int[,]
            {
                { 1,    0,     0, 0 },
                { 0, cosx, -sinx, 0 },
                { 0, sinx,  cosx, 0 },
              //{ 0,    0,     0, 1 },
            });

            var siny = SinInt(degrees.Y);
            var cosy = CosInt(degrees.Y);
            var ry = new Matrix(new int[,]
            {
                {  cosy, 0, siny, 0 },
                {     0, 1,    0, 0 },
                { -siny, 0, cosy, 0 },
              //{     0, 0,    0, 1 },
            });

            var sinz = SinInt(degrees.Z);
            var cosz = CosInt(degrees.Z);
            var rz = new Matrix(new int[,]
            {
                { cosz, -sinz, 0, 0 },
                { sinz, cosz,  0, 0 },
                {    0,    0,  1, 0 },
              //{    0,    0,  0, 1 },
            });

            var result = rz * ry * rx;
            return result;
        }

        public static IEnumerable<Matrix> GetUniqueRotationMatrices()
        {
            var rotationList = new List<Matrix>();
            for (int x = 0; x < 360; x += 90)
            {
                for (int y = 0; y < 360; y += 90)
                {
                    for (int z = 0; z < 360; z += 90)
                    {
                        var degrees = new Vector(x, y, z);
                        var rotation = GetRotationMatrixAugmented(degrees);
                        rotationList.Add(rotation);
                    }
                }
            }

            //hack: easiest way of getting all unique matrices :-)
            var unique = rotationList
                .GroupBy(r => r.Grid.ToJson())
                .Select(g => g.First());

            return unique;
        }

        public static Matrix GetTranslationMatrixAugmented(Vector offset)
        {
            //augmented translation matrix that's missing the last row
            var ys = offset.Length;
            var xs = offset.Length + 1;
            var translation = new Matrix(ys, xs);
            for (int y = 0; y < ys; y++)
            {
                translation[y, y] = 1;
                translation[y, xs - 1] = offset[y];
            }

            return translation;
        }
    }
}
