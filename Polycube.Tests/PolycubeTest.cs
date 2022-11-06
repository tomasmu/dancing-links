using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PolycubeSolver.Tests
{
    public class PolycubeTest
    {
        [Fact]
        public void Solve_Simple()
        {
            var grid = new bool[2, 2, 3];
            var cuboid = new Cuboid(grid);
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

            var polycube = new Polycube(cuboid, pieces);
            polycube.Solve(100);

            //box can be in 2 places, S and V in 4
            polycube.Solutions.Count.Should().Be(2 * 4);
        }

        [Fact]
        public void Solve_Simple_WithBlockedCubies()
        {
            var grid = new bool[2, 2, 3];
            grid[0, 0, 0] = true;   //blocked
            var cuboid = new Cuboid(grid);

            var pieces = new string[]
            {
                @"
-B
BB

BB
BB",
                @"II",
                @"JJ",
            }.Select(str => new Piece(str));

            var polycube = new Polycube(cuboid, pieces);
            polycube.Solve(100);

            //B-piece in front (1 way): I,J can be IJ, JI, [IJ], [JI] = 1*4
            //B-piece in back (2 ways): I,J can be IJ, JI = 2*2
            //4+4=8
            polycube.Solutions.Count.Should().Be(8);
        }

        [Fact]
        public void Solve_SomaCube()
        {
            var grid = new bool[3, 3, 3];
            var cuboid = new Cuboid(grid);
            var pieces = GetSomaPieces();
            var polycube = new Polycube(cuboid, pieces);
            polycube.Solve(20000);

            //soma cube has 240 unique solutions
            //240*24*2 = 11520 in total
            polycube.Solutions.Count.Should().Be(11520);
        }

        [Fact(
            //Skip = "this test takes a few seconds to run"
        )]
        public void Solve_Wooden_puzzle()
        {
            var grid = new bool[3, 3, 3];
            var pieces = GetWoodenPieces();
            var cuboid = new Cuboid(grid);
            var polycube = new Polycube(cuboid, pieces);

            polycube.Solve(330000);

            polycube.Solutions.Count.Should().Be(324864);
        }

        [Fact]
        public void Solve_Wooden_puzzle_Another_shape()
        {
            var grid = @"
...
...
...

...
.-.
...

...
...
...

---
-.-
---";
            var pieces = GetWoodenPieces();
            var cuboid = new Cuboid(grid);
            var polycube = new Polycube(cuboid, pieces);

            polycube.Solve(400000);

            polycube.Solutions.Count.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact(
            Skip = "this test takes an hour to run"
        )]
        public void Solve_Bedlam_cube()
        {
            var grid = new bool[4, 4, 4];
            var pieces = GetBedlamPieces();
            var cuboid = new Cuboid(grid);
            var polycube = new Polycube(cuboid, pieces);

            polycube.Solve(470000);
            //test run:
            //solutions: 460464
            //elapsed time: 45m
            polycube.Solutions.Count.Should().Be(24 * 19186);
        }

        [Fact(
            Skip = "this test takes two hours to run"
        )]
        public void Solve_Tetris_cube()
        {
            var grid = new bool[4, 4, 4];
            var pieces = GetTetrisCubePieces();
            var cuboid = new Cuboid(grid);
            var polycube = new Polycube(cuboid, pieces);

            polycube.Solve(240000);
            //test run:
            //solutions: 236136
            //elapsed time: 1h 50m
            polycube.Solutions.Count.Should().Be(24 * 9839);
        }

        public static IEnumerable<Piece> GetSomaPieces()
        {
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
            return pieces;
        }

        public static IEnumerable<Piece> GetWoodenPieces()
        {
            var pieces = new string[]
            {
                @"
111
1",
                @"
222
2",
                @"
333
3",
                @"
444
4",
                @"
V
VV",
                @"
TTT
-T-",
                @"
ZZ
-ZZ"
            }.Select(str => new Piece(str));
            return pieces;
        }

        public static IEnumerable<Piece> GetBedlamPieces()
        {
            var pieces = new string[]
            {
@"
--A
AAA
-A",

@"
-B
BBB
-B",

@"
C
CC
-CC",

@"
D
DD
-D

D",

@"
E
EE
E

-
E",

@"
F
FF
-F

-
F",

@"
G
G
G

-
GG",

@"
H
H
HH

-
-
H",

@"
I
I
II

I",

@"
JJ
J
J

-J",

@"
-
KK
-K

K
K",

@"
L
L
LL

-
L",

@"
M
MM

-
-M"
            }.Select(str => new Piece(str));
            return pieces;
        }

        public static IEnumerable<Piece> GetTetrisCubePieces()
        {
            var pieces = new string[]
            {
@"
AAA
--A
--A

A",

@"
BBB
--B
--B

-B",

@"
-CC
--C

CC",

@"
D
D
DD
-D",

@"
E
EEE
-E

E",

@"
FF
FF

F",

@"
GGG
G
G",

@"
H
HH
H
H",

@"
I
I
II

-
-
-I",

@"
-
JJ
J

-J
-J",

@"
K
KK
K

-
-K",

@"
-
LLL
-L

-L
-L"
            }.Select(str => new Piece(str));
            return pieces;
        }
    }
}
