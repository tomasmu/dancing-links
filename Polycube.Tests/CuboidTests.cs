using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PolycubeSolver.Tests
{
    public class CuboidTests
    {
        [Fact]
        public void CuboidConstructorArray()
        {
            var grid = new bool[3, 5, 7];
            grid[0, 0, 0] = true;   //block one
            var cuboid = new Cuboid(grid);

            cuboid.Length.X.Should().Be(5);
            cuboid.Length.Y.Should().Be(3);
            cuboid.Length.Z.Should().Be(7);
            cuboid.CubieCount.Should().Be(5 * 3 * 7 - 1);
        }

        [Fact]
        public void CuboidConstructorCanParseString()
        {
            var gridOneBlocked = @"
-..
...
...
...

...
...
...
...";
            var cuboid = new Cuboid(gridOneBlocked);

            cuboid.Length.X.Should().Be(4);
            cuboid.Length.Y.Should().Be(2);
            cuboid.Length.Z.Should().Be(3);
            cuboid.CubieCount.Should().Be(4 * 2 * 3 - 1);
            cuboid.Grid[0, 0, 0].Should().BeTrue();  //should be the only one
        }
    }
}
