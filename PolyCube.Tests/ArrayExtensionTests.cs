using Xunit;
using FluentAssertions;

namespace PolycubeSolver.Tests
{
    public class ArrayExtensionTests
    {
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
