using FluentAssertions;
using Xunit;

namespace Polycube.Tests
{
    public class MatrixMultiplicationTests
    {
        [Fact]
        public void Multiplication()
        {
            var a = new int[,]
            {
                { 11, 12, 13 },
                { 21, 22, 23 },
                { 31, 32, 33 },
                { 41, 42, 43 },
            };
            var b = new int[,]
            {
                { 111, 112 },
                { 121, 122 },
                { 131, 132 },
            };

            var expected = new int[,]
            {
                { (11*111 + 12*121 + 13*131), (11*112 + 12*122 + 13*132) },
                { (21*111 + 22*121 + 23*131), (21*112 + 22*122 + 23*132) },
                { (31*111 + 32*121 + 33*131), (31*112 + 32*122 + 33*132) },
                { (41*111 + 42*121 + 43*131), (41*112 + 42*122 + 43*132) },
            };

            var result = a.Multiply(b);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Multiplication_MissingRows()
        {
            var a = new int[,]
            {
                { 11, 12, 13 },
                { 21, 22, 23 },
                { 31, 32, 33 },
                { 41, 42, 43 },
            };
            var b = new int[,]
            {
                { 111, 112 },
                { 121, 122 },
              //{ 131, 132 }, //if these are missing { 1, 1 } is assumed
            };

            var expected = new int[,]
            {
                { (11*111 + 12*121 + 13*1), (11*112 + 12*122 + 13*1) },
                { (21*111 + 22*121 + 23*1), (21*112 + 22*122 + 23*1) },
                { (31*111 + 32*121 + 33*1), (31*112 + 32*122 + 33*1) },
                { (41*111 + 42*121 + 43*1), (41*112 + 42*122 + 43*1) },
            };

            var result = a.Multiply(b);

            result.Should().BeEquivalentTo(expected);
        }
    }
}
