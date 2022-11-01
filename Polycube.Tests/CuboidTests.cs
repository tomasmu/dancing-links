﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Polycube.Tests
{
    public class CuboidTests
    {
        [Fact]
        public void CuboidProperties()
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
        public void CanParseString()
        {
            var parsedGrid = @"
-..
...
...
...

...
...
...
...".ToGrid('-');

            var (xLen, yLen, zLen) = parsedGrid.GetLengths();

            xLen.Should().Be(4);
            yLen.Should().Be(2);
            zLen.Should().Be(3);
            parsedGrid[0, 0, 0].Should().BeTrue();  //should be the only one
            //parsedGrid[all, the, others].Should().BeFalse();
        }

        //todo: should probably not be done in the Cuboid class
        [Fact]
        public void CuboidCanParseString()
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
        }
    }
}
