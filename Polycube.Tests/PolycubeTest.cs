using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Polycube.Tests
{
    public class PolycubeTest
    {
        [Fact]
        public void Solve_Test()
        {
            var grid = new bool[2, 2, 3];
            //grid[0, 0, 0] = true;     //todo: not implemented yet
            var pieces = new string[]
            {
@"
BB
BB

BB
BB",
@"
V
VV",
@"S",
            }.Select(str => new Piece(str));

            var poly = new Polycube(grid, pieces);
            poly.Solve(100);

            //box can be in 2 places, S and V in 4
            poly.Solutions.Count.Should().Be(2 * 4);
        }

        [Fact]
        public void Solve_SomaCube()
        {
            var grid = new bool[3, 3, 3];
            var pieces = new string[]
            {
@"
VV
V",
@"
LLL
L",
@"
TTT
-T",
@"
-ZZ
ZZ",
@"
A
AA

A",
@"
BB
-B

B",
@"
PP
P

P",
            }.Select(str => new Piece(str));

            var polycube = new Polycube(grid, pieces);
            polycube.Solve(20000);

            //soma cube has 240 unique solutions
            //240*24*2 = 11520 in total
            polycube.Solutions.Count.Should().Be(11520);
        }
    }
}
