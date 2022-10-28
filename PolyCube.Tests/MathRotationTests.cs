using FluentAssertions;
using System;
using System.Collections.Generic;
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
            var result = MathRotation
                .GetRotationMatrix(rx, ry, rz)
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
            var result4x4 = MathRotation.GetRotationMatrix_4x4(rx, ry, rz);
            //look at upper 3x3
            var result3x3 = new int[,] {
                { result4x4[0, 0], result4x4[0, 1], result4x4[0, 2], },
                { result4x4[1, 0], result4x4[1, 1], result4x4[1, 2], },
                { result4x4[2, 0], result4x4[2, 1], result4x4[2, 2], },
            }.ToJson();

            var expected = MathRotation
                .GetRotationMatrix(rx, ry, rz)
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
            var point = new int[,] { { 2 }, { 3 }, { 4 } };
            var rotated = point.Rotate(xr, yr, zr);

            var expected = new int[,] { { xe }, { ye }, { ze } };

            rotated.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TranslateMatrix()
        {
            var point = new int[,] { { 2 }, { 3 }, { 4 } };
            var translated = point.Translate(10, -20, 30);

            var expected = new int[,] { { 2+10 }, { 3-20 }, { 4+30 } };

            translated.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TranslatePoints()
        {
            var points = new List<int[,]> {
                new int[,] { {  1 }, {  2 }, {  3 } },
                new int[,] { { -4 }, { -5 }, { -6 } },
            };

            var rotatedInOrigo = points
                .Select(x => x.Translate(-2, 3, 4));

            var expected = new List<int[,]>
            {
                new int[,] { { -1 }, {  5 }, {  7 } },
                new int[,] { { -6 }, { -2 }, { -2 } },
            };

            rotatedInOrigo.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetAxesMinValues_2D_point()
        {
            var points = new List<int[,]> {
                new int[,] { {  0 }, {  2 } },
                new int[,] { {  1 }, {  0 } },
                new int[,] { {  0 }, { -2 } },
                new int[,] { { -1 }, {  0 } },
            };

            var min = points.GetAxesMinValues();

            min[0].Should().Be(-1);
            min[1].Should().Be(-2);
        }

        [Fact]
        public void GetAxesMaxValues_2D_point()
        {
            var points = new List<int[,]> {
                new int[,] { { -1 }, {  0 } },
                new int[,] { {  0 }, {  2 } },
                new int[,] { {  1 }, {  0 } },
                new int[,] { {  0 }, { -2 } },
            };

            var min = points.GetAxesMaxValues();

            min[0].Should().Be(1);
            min[1].Should().Be(2);
        }
 
        [Fact]
        public void GetAxesMinValues_3D_point()
        {
            var points = new List<int[,]> {
                new int[,] { { -1 }, {  0 }, {  0 } },
                new int[,] { {  0 }, { -2 }, {  0 } },
                new int[,] { {  1 }, {  0 }, { -3 } },
                new int[,] { {  0 }, {  2 }, {  0 } },
                new int[,] { {  0 }, {  0 }, {  3 } },
            };

            var min = points.GetAxesMinValues();

            min[0].Should().Be(-1);
            min[1].Should().Be(-2);
            min[2].Should().Be(-3);
        }

        [Fact]
        public void TranslateToOrigo()
        {
            var points = new List<int[,]> {
                new int[,] { { -1 }, {  0 }, {  0 } },
                new int[,] { {  0 }, { -2 }, {  0 } },
                new int[,] { {  1 }, {  0 }, { -3 } },
                new int[,] { {  0 }, {  2 }, {  0 } },
                new int[,] { {  0 }, {  0 }, {  3 } },
            };

            var origo = points.TranslateToOrigo();

            var expected = new List<int[,]> {
                new int[,] { {  0 }, {  2 }, {  3 } },
                new int[,] { {  1 }, {  0 }, {  3 } },
                new int[,] { {  2 }, {  2 }, {  0 } },
                new int[,] { {  1 }, {  4 }, {  3 } },
                new int[,] { {  1 }, {  2 }, {  6 } },
            };

            origo.Should().BeEquivalentTo(expected);
        }
    }
}
