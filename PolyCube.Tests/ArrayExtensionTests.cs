using Xunit;
using FluentAssertions;

namespace Polycube.Tests
{
    public class ArrayExtensionTests
    {
        [Fact]
        public void GetLengths_2D()
        {
            var matrix = new int[5, 7];

            var dimensions = matrix.GetLengths();

            dimensions.X.Should().Be(7);
            dimensions.Y.Should().Be(5);
        }

        [Fact]
        public void GetLengths_3D()
        {
            var matrix = new int[5, 7, 11];

            var dimensions = matrix.GetLengths();

            dimensions.X.Should().Be(7);
            dimensions.Y.Should().Be(5);
            dimensions.Z.Should().Be(11);
        }

        [Fact]
        public void MultiDimensional_Array_ToJson()
        {
            var array2D = new int[,] { { 1, 2 }, { 3, 4 } };

            var result = array2D.ToJson();

            var expected = "[[1,2],[3,4]]";
            result.Should().Be(expected);
        }
    }
}
