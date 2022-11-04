using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PolycubeSolver.Tests
{
    public class PentominoTests
    {
        [Fact]
        public void CanSolve2D()
        {
            var grid = @"
...
...";
            var V = @"
V
VVV";
            var I = @"II";

            var pieces = new List<Piece> { new Piece(V), new Piece(I) };
            var cuboid = new Cuboid(grid);
            var polycube = new Polycube(cuboid, pieces);

            polycube.Solve(100);

            polycube.Solutions.Count.Should().Be(4);
        }

        [Fact]
        public void Solve_Scotts_Pentomino_Problem()
        {
            //arrange
            var F = @"
-FF
FF
-F";
            var I = @"IIIII";
            var L = @"
L
L
L
LL";
            var N = @"
NN
-NNN";
            var P = @"
PP
PP
P";
            var T = @"
TTT
-T
-T";
            var U = @"
U-U
UUU";
            var V = @"
V
V
VVV";
            var W = @"
W
WW
-WW";
            var X = @"
-X
XXX
-X";
            var Y = @"
--Y
YYYY";
            var Z = @"
ZZ
-Z
-ZZ";

            var pentominoes = new[] { F, I, L, N, P, T, U, V, W, X, Y, Z }
                .Select(p => new Piece(p));

            var board = @"
........
........
........
...--...
...--...
........
........
........";
            var cuboid = new Cuboid(board);
            var polycube = new Polycube(cuboid, pentominoes);

            //act
            polycube.Solve(1000);

            //assert
            polycube.Solutions.Count.Should().Be(520);
        }
    }
}
