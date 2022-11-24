using FluentAssertions;
using Xunit;

namespace PolycubeSolver.Tests
{
    public class MatrixMultiplicationTests
    {
        [Fact]
        public void Multiplication()
        {
            var a = new Matrix(new int[,]
            {
                { 11, 12, 13 },
                { 21, 22, 23 },
                { 31, 32, 33 },
                { 41, 42, 43 },
            });
            var b = new Matrix(new int[,]
            {
                { 111, 112 },
                { 121, 122 },
                { 131, 132 },
            });
            var result = a * b;

            var expected = new Matrix(new int[,]
            {
                { (11*111 + 12*121 + 13*131), (11*112 + 12*122 + 13*132) },
                { (21*111 + 22*121 + 23*131), (21*112 + 22*122 + 23*132) },
                { (31*111 + 32*121 + 33*131), (31*112 + 32*122 + 33*132) },
                { (41*111 + 42*121 + 43*131), (41*112 + 42*122 + 43*132) },
            });
            result.Should().Be(expected);
        }

        [Fact]
        public void Multiplication_MissingRows()
        {
            var a = new Matrix(new int[,]
            {
                { 11, 12, 13 },
                { 21, 22, 23 },
                { 31, 32, 33 },
                { 41, 42, 43 },
            });
            var b = new Matrix(new int[,]
            {
                { 111, 112 },
                { 121, 122 },
              //{ 131, 132 }, //if these are missing { 1, 1 } is assumed
            });
            var result = a * b;

            var expected = new Matrix(new int[,]
            {
                { (11*111 + 12*121 + 13*1), (11*112 + 12*122 + 13*1) },
                { (21*111 + 22*121 + 23*1), (21*112 + 22*122 + 23*1) },
                { (31*111 + 32*121 + 33*1), (31*112 + 32*122 + 33*1) },
                { (41*111 + 42*121 + 43*1), (41*112 + 42*122 + 43*1) },
            });
            result.Should().Be(expected);
        }

        [Fact]
        public void Multiplication_Vector()
        {
            var a = new Matrix(new int[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 9, 8, 7 },
                { 6, 5, 4 },
            });
            var b = new Vector(100, 10);
            var result = a * b;

            var expected = new Vector(4);
            //multiply assumes 1 here -----v
            expected[0] = 1*100 + 2*10 + 3*1;
            expected[1] = 4*100 + 5*10 + 6*1;
            expected[2] = 9*100 + 8*10 + 7*1;
            expected[3] = 6*100 + 5*10 + 4*1;
            result.Should().Be(expected);
        }

        [Fact]
        public void MultiplicationLeft()
        {
            var degrees = new Vector(90, 90, 90);
            var matrix = MathRotation.GetRotationMatrix(degrees);
            var point = new Vector(2, 3, 4);
            var result = matrix * point;

            var expected = new Vector(4, 3, -2);
            result.Should().Be(expected);
        }


        [Fact]
        public void MatrixMultiplication()
        {
            var a = new Matrix(new int[,]
            {
                { 11, 12, 13 },
                { 21, 22, 23 },
                { 31, 32, 33 },
                { 41, 42, 43 },
            });
            var b = new Matrix(new int[,]
            {
                { 111, 112 },
                { 121, 122 },
                { 131, 132 },
            });
            var result = a * b;

            var expected = new Matrix(new int[,]
            {
                { (11*111 + 12*121 + 13*131), (11*112 + 12*122 + 13*132) },
                { (21*111 + 22*121 + 23*131), (21*112 + 22*122 + 23*132) },
                { (31*111 + 32*121 + 33*131), (31*112 + 32*122 + 33*132) },
                { (41*111 + 42*121 + 43*131), (41*112 + 42*122 + 43*132) },
            });
            result.Should().Be(expected);
        }
    }
}
