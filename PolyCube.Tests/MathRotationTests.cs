using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Polycube.Tests
{
    public class MathRotationTests
    {
        [Theory]
        [InlineData(  0)]
        [InlineData( 90)]
        [InlineData(180)]
        [InlineData(270)]
        [InlineData(360 +   0)]
        [InlineData(360 +  90)]
        [InlineData(360 + 180)]
        [InlineData(360 + 270)]
        public void SinInt(int degrees)
        {
            var result = MathRotation.SinInt(degrees);

            var expected = (int)Math.Sin(Math.PI / 180 * degrees);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(90)]
        [InlineData(180)]
        [InlineData(270)]
        [InlineData(360 +   0)]
        [InlineData(360 +  90)]
        [InlineData(360 + 180)]
        [InlineData(360 + 270)]
        public void CosInt(int degrees)
        {
            var result = MathRotation.CosInt(degrees);

            var expected = (int)Math.Cos(Math.PI / 180 * degrees);
            result.Should().Be(expected);
        }

        [Fact]
        public void SinInt_Not_a_multiple_of_90()
        {
            Action action = () => MathRotation.SinInt(90 + 1);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CosInt_Not_a_multiple_of_90()
        {
            Action action = () => MathRotation.CosInt(180 + 1);
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(   0,   0)]
        [InlineData(  90,  90)]
        [InlineData( -90, 270)]
        [InlineData(-180, 180)]
        [InlineData(-270,  90)]
        [InlineData(-360,   0)]
        [InlineData( 360+180, 180)]
        [InlineData(-360-180, 180)]
        public void Modulo_always_positive(int degrees, int expected)
        {
            var result = MathRotation.Modulo(degrees, 360);

            result.Should().Be(expected);
        }

        //todo: move
        [Theory]
        [InlineData(  0,   0,   0, "[[1,0,0],[0,1,0],[0,0,1]]")]
        [InlineData(  0,   0,  90, "[[0,-1,0],[1,0,0],[0,0,1]]")]
        [InlineData(  0,   0, 180, "[[-1,0,0],[0,-1,0],[0,0,1]]")]
        [InlineData(  0,   0, 270, "[[0,1,0],[-1,0,0],[0,0,1]]")]
        [InlineData(  0,  90,   0, "[[0,0,1],[0,1,0],[-1,0,0]]")]
        [InlineData(  0,  90,  90, "[[0,-1,0],[0,0,1],[-1,0,0]]")]
        [InlineData(  0,  90, 180, "[[0,0,-1],[0,-1,0],[-1,0,0]]")]
        [InlineData(  0,  90, 270, "[[0,1,0],[0,0,-1],[-1,0,0]]")]
        [InlineData(  0, 180,   0, "[[-1,0,0],[0,1,0],[0,0,-1]]")]
        [InlineData(  0, 180,  90, "[[0,-1,0],[-1,0,0],[0,0,-1]]")]
        [InlineData(  0, 180, 180, "[[1,0,0],[0,-1,0],[0,0,-1]]")]
        [InlineData(  0, 180, 270, "[[0,1,0],[1,0,0],[0,0,-1]]")]
        [InlineData(  0, 270,   0, "[[0,0,-1],[0,1,0],[1,0,0]]")]
        [InlineData(  0, 270,  90, "[[0,-1,0],[0,0,-1],[1,0,0]]")]
        [InlineData(  0, 270, 180, "[[0,0,1],[0,-1,0],[1,0,0]]")]
        [InlineData(  0, 270, 270, "[[0,1,0],[0,0,1],[1,0,0]]")]
        [InlineData( 90,   0,   0, "[[1,0,0],[0,0,-1],[0,1,0]]")]
        [InlineData( 90,   0,  90, "[[0,0,1],[1,0,0],[0,1,0]]")]
        [InlineData( 90,   0, 180, "[[-1,0,0],[0,0,1],[0,1,0]]")]
        [InlineData( 90,   0, 270, "[[0,0,-1],[-1,0,0],[0,1,0]]")]
        [InlineData( 90,  90,   0, "[[0,1,0],[0,0,-1],[-1,0,0]]")]
        [InlineData( 90,  90,  90, "[[0,0,1],[0,1,0],[-1,0,0]]")]
        [InlineData( 90,  90, 180, "[[0,-1,0],[0,0,1],[-1,0,0]]")]
        [InlineData( 90,  90, 270, "[[0,0,-1],[0,-1,0],[-1,0,0]]")]
        [InlineData( 90, 180,   0, "[[-1,0,0],[0,0,-1],[0,-1,0]]")]
        [InlineData( 90, 180,  90, "[[0,0,1],[-1,0,0],[0,-1,0]]")]
        [InlineData( 90, 180, 180, "[[1,0,0],[0,0,1],[0,-1,0]]")]
        [InlineData( 90, 180, 270, "[[0,0,-1],[1,0,0],[0,-1,0]]")]
        [InlineData( 90, 270,   0, "[[0,-1,0],[0,0,-1],[1,0,0]]")]
        [InlineData( 90, 270,  90, "[[0,0,1],[0,-1,0],[1,0,0]]")]
        [InlineData( 90, 270, 180, "[[0,1,0],[0,0,1],[1,0,0]]")]
        [InlineData( 90, 270, 270, "[[0,0,-1],[0,1,0],[1,0,0]]")]
        [InlineData(180,   0,   0, "[[1,0,0],[0,-1,0],[0,0,-1]]")]
        [InlineData(180,   0,  90, "[[0,1,0],[1,0,0],[0,0,-1]]")]
        [InlineData(180,   0, 180, "[[-1,0,0],[0,1,0],[0,0,-1]]")]
        [InlineData(180,   0, 270, "[[0,-1,0],[-1,0,0],[0,0,-1]]")]
        [InlineData(180,  90,   0, "[[0,0,-1],[0,-1,0],[-1,0,0]]")]
        [InlineData(180,  90,  90, "[[0,1,0],[0,0,-1],[-1,0,0]]")]
        [InlineData(180,  90, 180, "[[0,0,1],[0,1,0],[-1,0,0]]")]
        [InlineData(180,  90, 270, "[[0,-1,0],[0,0,1],[-1,0,0]]")]
        [InlineData(180, 180,   0, "[[-1,0,0],[0,-1,0],[0,0,1]]")]
        [InlineData(180, 180,  90, "[[0,1,0],[-1,0,0],[0,0,1]]")]
        [InlineData(180, 180, 180, "[[1,0,0],[0,1,0],[0,0,1]]")]
        [InlineData(180, 180, 270, "[[0,-1,0],[1,0,0],[0,0,1]]")]
        [InlineData(180, 270,   0, "[[0,0,1],[0,-1,0],[1,0,0]]")]
        [InlineData(180, 270,  90, "[[0,1,0],[0,0,1],[1,0,0]]")]
        [InlineData(180, 270, 180, "[[0,0,-1],[0,1,0],[1,0,0]]")]
        [InlineData(180, 270, 270, "[[0,-1,0],[0,0,-1],[1,0,0]]")]
        [InlineData(270,   0,   0, "[[1,0,0],[0,0,1],[0,-1,0]]")]
        [InlineData(270,   0,  90, "[[0,0,-1],[1,0,0],[0,-1,0]]")]
        [InlineData(270,   0, 180, "[[-1,0,0],[0,0,-1],[0,-1,0]]")]
        [InlineData(270,   0, 270, "[[0,0,1],[-1,0,0],[0,-1,0]]")]
        [InlineData(270,  90,   0, "[[0,-1,0],[0,0,1],[-1,0,0]]")]
        [InlineData(270,  90,  90, "[[0,0,-1],[0,-1,0],[-1,0,0]]")]
        [InlineData(270,  90, 180, "[[0,1,0],[0,0,-1],[-1,0,0]]")]
        [InlineData(270,  90, 270, "[[0,0,1],[0,1,0],[-1,0,0]]")]
        [InlineData(270, 180,   0, "[[-1,0,0],[0,0,1],[0,1,0]]")]
        [InlineData(270, 180,  90, "[[0,0,-1],[-1,0,0],[0,1,0]]")]
        [InlineData(270, 180, 180, "[[1,0,0],[0,0,-1],[0,1,0]]")]
        [InlineData(270, 180, 270, "[[0,0,1],[1,0,0],[0,1,0]]")]
        [InlineData(270, 270,   0, "[[0,1,0],[0,0,1],[1,0,0]]")]
        [InlineData(270, 270,  90, "[[0,0,-1],[0,1,0],[1,0,0]]")]
        [InlineData(270, 270, 180, "[[0,-1,0],[0,0,-1],[1,0,0]]")]
        [InlineData(270, 270, 270, "[[0,0,1],[0,-1,0],[1,0,0]]")]
        public void Get_RotationMatrix(int rx, int ry, int rz, string expected)
        {
            var degrees = new Vector(rx, ry, rz);
            var result = MathRotation
                .GetRotationMatrix(degrees)
                .ToJson();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(  0,   0,   0)]
        [InlineData(  0,   0,  90)]
        [InlineData(  0,   0, 180)]
        [InlineData(  0,   0, 270)]
        [InlineData(  0,  90,   0)]
        [InlineData(  0,  90,  90)]
        [InlineData(  0,  90, 180)]
        [InlineData(  0,  90, 270)]
        [InlineData(  0, 180,   0)]
        [InlineData(  0, 180,  90)]
        [InlineData(  0, 180, 180)]
        [InlineData(  0, 180, 270)]
        [InlineData(  0, 270,   0)]
        [InlineData(  0, 270,  90)]
        [InlineData(  0, 270, 180)]
        [InlineData(  0, 270, 270)]
        [InlineData( 90,   0,   0)]
        [InlineData( 90,   0,  90)]
        [InlineData( 90,   0, 180)]
        [InlineData( 90,   0, 270)]
        [InlineData( 90,  90,   0)]
        [InlineData( 90,  90,  90)]
        [InlineData( 90,  90, 180)]
        [InlineData( 90,  90, 270)]
        [InlineData( 90, 180,   0)]
        [InlineData( 90, 180,  90)]
        [InlineData( 90, 180, 180)]
        [InlineData( 90, 180, 270)]
        [InlineData( 90, 270,   0)]
        [InlineData( 90, 270,  90)]
        [InlineData( 90, 270, 180)]
        [InlineData( 90, 270, 270)]
        [InlineData(180,   0,   0)]
        [InlineData(180,   0,  90)]
        [InlineData(180,   0, 180)]
        [InlineData(180,   0, 270)]
        [InlineData(180,  90,   0)]
        [InlineData(180,  90,  90)]
        [InlineData(180,  90, 180)]
        [InlineData(180,  90, 270)]
        [InlineData(180, 180,   0)]
        [InlineData(180, 180,  90)]
        [InlineData(180, 180, 180)]
        [InlineData(180, 180, 270)]
        [InlineData(180, 270,   0)]
        [InlineData(180, 270,  90)]
        [InlineData(180, 270, 180)]
        [InlineData(180, 270, 270)]
        [InlineData(270,   0,   0)]
        [InlineData(270,   0,  90)]
        [InlineData(270,   0, 180)]
        [InlineData(270,   0, 270)]
        [InlineData(270,  90,   0)]
        [InlineData(270,  90,  90)]
        [InlineData(270,  90, 180)]
        [InlineData(270,  90, 270)]
        [InlineData(270, 180,   0)]
        [InlineData(270, 180,  90)]
        [InlineData(270, 180, 180)]
        [InlineData(270, 180, 270)]
        [InlineData(270, 270,   0)]
        [InlineData(270, 270,  90)]
        [InlineData(270, 270, 180)]
        [InlineData(270, 270, 270)]
        public void Compare_4x4_RotationMatrix(int rx, int ry, int rz)
        {
            var degrees = new Vector(rx, ry, rz);
            var resultAug = MathRotation.GetRotationMatrixAugmented(degrees);
            //look at upper 3x3
            var result3x3 = new int[,] {
                { resultAug[0, 0], resultAug[0, 1], resultAug[0, 2], },
                { resultAug[1, 0], resultAug[1, 1], resultAug[1, 2], },
                { resultAug[2, 0], resultAug[2, 1], resultAug[2, 2], },
            }.ToJson();

            //should be the same as this method
            var expected = MathRotation
                .GetRotationMatrix(degrees)
                .ToJson();

            result3x3.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Rotation_Matrices_Unique_Count()
        {
            var rotations = MathRotation.GetUniqueRotationMatrices();

            rotations.Count().Should().Be(24);
        }

        [Theory]
        //single cubie 1x1x1
        [InlineData(@"
S", 1)]
        //cube 2x2x2
        [InlineData(@"
BB
BB

BB
BB", 1)]
        //line 1x1x3
        [InlineData(@"
IIII", 3)]
        //2-layer 2x2x4
        [InlineData(@"
IIII
IIII

IIII
IIII", 3)]
        //square 1x2x2
        [InlineData(@"
SS
SS
", 3)]
        //rectangle 1x2x3
        [InlineData(@"
RRR
RRR", 6)]
        //3-layer rectangular cuboid 3x2x4
        [InlineData(@"
RRRR
RRRR

RRRR
RRRR

RRRR
RRRR", 6)]
        //tripod
        [InlineData(@"
TT
T

T", 8)]
        //V
        [InlineData(@"
VV
V", 12)]
        //L
        [InlineData(@"
LLL
L", 24)]
        public void Rotations_Unique(string piece, int expectedUnique)
        {
            var rotations = piece.ToPoints('-').GetUniqueRotations();

            rotations.Count().Should().Be(expectedUnique);
        }

        [Theory]
        [InlineData(  0,   0,   0,  2,  3,  4)]
        [InlineData(  0,   0,  90, -3,  2,  4)]
        [InlineData(  0,   0, 180, -2, -3,  4)]
        [InlineData(  0,   0, 270,  3, -2,  4)]
        [InlineData(  0,  90,   0,  4,  3, -2)]
        [InlineData(  0,  90,  90, -3,  4, -2)]
        [InlineData(  0,  90, 180, -4, -3, -2)]
        [InlineData(  0,  90, 270,  3, -4, -2)]
        [InlineData(  0, 180,   0, -2,  3, -4)]
        [InlineData(  0, 180,  90, -3, -2, -4)]
        [InlineData(  0, 180, 180,  2, -3, -4)]
        [InlineData(  0, 180, 270,  3,  2, -4)]
        [InlineData(  0, 270,   0, -4,  3,  2)]
        [InlineData(  0, 270,  90, -3, -4,  2)]
        [InlineData(  0, 270, 180,  4, -3,  2)]
        [InlineData(  0, 270, 270,  3,  4,  2)]
        [InlineData( 90,   0,   0,  2, -4,  3)]
        [InlineData( 90,   0,  90,  4,  2,  3)]
        [InlineData( 90,   0, 180, -2,  4,  3)]
        [InlineData( 90,   0, 270, -4, -2,  3)]
        [InlineData( 90,  90,   0,  3, -4, -2)]
        [InlineData( 90,  90,  90,  4,  3, -2)]
        [InlineData( 90,  90, 180, -3,  4, -2)]
        [InlineData( 90,  90, 270, -4, -3, -2)]
        [InlineData( 90, 180,   0, -2, -4, -3)]
        [InlineData( 90, 180,  90,  4, -2, -3)]
        [InlineData( 90, 180, 180,  2,  4, -3)]
        [InlineData( 90, 180, 270, -4,  2, -3)]
        [InlineData( 90, 270,   0, -3, -4,  2)]
        [InlineData( 90, 270,  90,  4, -3,  2)]
        [InlineData( 90, 270, 180,  3,  4,  2)]
        [InlineData( 90, 270, 270, -4,  3,  2)]
        [InlineData(180,   0,   0,  2, -3, -4)]
        [InlineData(180,   0,  90,  3,  2, -4)]
        [InlineData(180,   0, 180, -2,  3, -4)]
        [InlineData(180,   0, 270, -3, -2, -4)]
        [InlineData(180,  90,   0, -4, -3, -2)]
        [InlineData(180,  90,  90,  3, -4, -2)]
        [InlineData(180,  90, 180,  4,  3, -2)]
        [InlineData(180,  90, 270, -3,  4, -2)]
        [InlineData(180, 180,   0, -2, -3,  4)]
        [InlineData(180, 180,  90,  3, -2,  4)]
        [InlineData(180, 180, 180,  2,  3,  4)]
        [InlineData(180, 180, 270, -3,  2,  4)]
        [InlineData(180, 270,   0,  4, -3,  2)]
        [InlineData(180, 270,  90,  3,  4,  2)]
        [InlineData(180, 270, 180, -4,  3,  2)]
        [InlineData(180, 270, 270, -3, -4,  2)]
        [InlineData(270,   0,   0,  2,  4, -3)]
        [InlineData(270,   0,  90, -4,  2, -3)]
        [InlineData(270,   0, 180, -2, -4, -3)]
        [InlineData(270,   0, 270,  4, -2, -3)]
        [InlineData(270,  90,   0, -3,  4, -2)]
        [InlineData(270,  90,  90, -4, -3, -2)]
        [InlineData(270,  90, 180,  3, -4, -2)]
        [InlineData(270,  90, 270,  4,  3, -2)]
        [InlineData(270, 180,   0, -2,  4,  3)]
        [InlineData(270, 180,  90, -4, -2,  3)]
        [InlineData(270, 180, 180,  2, -4,  3)]
        [InlineData(270, 180, 270,  4,  2,  3)]
        [InlineData(270, 270,   0,  3,  4,  2)]
        [InlineData(270, 270,  90, -4,  3,  2)]
        [InlineData(270, 270, 180, -3, -4,  2)]
        [InlineData(270, 270, 270,  4, -3,  2)]
        public void RotatePoint(int xr, int yr, int zr, int xe, int ye, int ze)
        {
            var degrees = new Vector(xr, yr, zr);
            var point = new Vector(2, 3, 4);
            var rotated = point.Rotate(degrees);

            var expected = new Vector(xe, ye, ze);

            rotated.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TranslatePoint()
        {
            var point = new Vector(2, 3, 4);
            var offset = new Vector(10, -20, 30);
            var translated = point + offset;

            var expected = new Vector(2 + 10, 3 - 20, 4 + 30);

            translated.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Compare_TranslationMatrix()
        {
            var offset = new Vector(10, -20, 30);

            var translate3D = MathRotation.GetTranslationMatrix(offset);
            var translateXD = MathRotation.GetTranslationMatrixTest(offset);

            translateXD.Should().BeEquivalentTo(translate3D);
        }

        [Fact]
        public void TranslationMatrix_2D()
        {
            var offset = new Vector(10, -20);
            var expected = new int[,]
            {
                { 1, 0,  10 },
                { 0, 1, -20 },
              //{ 0, 0,   1 },
            };

            var result = MathRotation.GetTranslationMatrixTest(offset);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TranslationMatrix_3D()
        {
            var offset = new Vector(10, -20, 30);

            var result = MathRotation.GetTranslationMatrixTest(offset);

            var expected = new int[,]
            {
                { 1, 0, 0,  10 },
                { 0, 1, 0, -20 },
                { 0, 0, 1,  30 },
              //{ 0, 0, 0,   1 },
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TranslatePoints()
        {
            var points = new List<Vector> {
                new Vector( 1,  2,  3),
                new Vector(-4, -5, -6)
            };

            var offset = new Vector(-2, 3, 4);
            var rotatedInOrigo = points.Select(point => point + offset);

            var expected = new List<Vector>
            {
                new Vector( -1,  5,  7),
                new Vector( -6, -2, -2),
            };

            rotatedInOrigo.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetAxesMinValues_2D_point()
        {
            var points = new List<Vector> {
                new Vector( 0,  2),
                new Vector( 1,  0),
                new Vector( 0, -2),
                new Vector(-1,  0)
            };

            var min = points.GetAxesMinValues();

            min.X.Should().Be(-1);
            min.Y.Should().Be(-2);
        }

        [Fact]
        public void GetAxesMaxValues_2D_point()
        {
            var points = new List<Vector> {
                new Vector( -1,  0),
                new Vector(  0,  2),
                new Vector(  1,  0),
                new Vector(  0, -2),
            };

            var max = points.GetAxesMaxValues();
            
            max.Length.Should().Be(2);

            max.X.Should().Be(1);
            max[0].Should().Be(1);
            max[0, 0].Should().Be(1);
            
            max.Y.Should().Be(2);
            max[1].Should().Be(2);
            max[1, 0].Should().Be(2);
        }
 
        [Fact]
        public void GetAxesMinValues_3D_point()
        {
            var points = new List<Vector> {
                new Vector(-1,  0,  0),
                new Vector( 0, -2,  0),
                new Vector( 1,  0, -3),
                new Vector( 0,  2,  0),
                new Vector( 0,  0,  3)
            };

            var min = points.GetAxesMinValues();

            min.Length.Should().Be(3);
            min.X.Should().Be(-1);
            min.Y.Should().Be(-2);
            min.Z.Should().Be(-3);
        }

        [Fact]
        public void TranslateToOrigo_Test()
        {
            var points = new List<Vector> {
                new Vector(-1,  0,  0),
                new Vector( 0, -2,  0),
                new Vector( 1,  0, -3),
                new Vector( 0,  2,  0),
                new Vector( 0,  0,  3),
            };

            var origo = points.TranslateToOrigo();

            var expected = new List<Vector> {
                new Vector( 0,  2,  3),
                new Vector( 1,  0,  3),
                new Vector( 2,  2,  0),
                new Vector( 1,  4,  3),
                new Vector( 1,  2,  6),
            };

            origo.Should().BeEquivalentTo(expected);
        }
    }
}
