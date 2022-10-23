using FluentAssertions;
using Pentominoes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pentominoes.Tests
{
    public class PentominoTests
    {
        [Fact]
        public void CanFindAllSolutionsWithBlockedPieces()
        {
            //arrange
            var L = @"
o
ooo";
            var single = @"
o";
            var pieceList = new[] { L, single }.ToRectangleMatrix('o');
            var board = @"
...
.o.".ToRectangleMatrix('o').ToIntMatrix();

            var solver = new Pentomino(board, pieceList);
            var expectedSolutions = @"
1 1 1
2 0 1

1 1 1
1 0 2".Trim();
            var expectedConstraintMatrix = @"
1 0 1 1 1 0 1
1 0 1 1 1 1 0
0 1 1 0 0 0 0
0 1 0 1 0 0 0
0 1 0 0 1 0 0
0 1 0 0 0 1 0
0 1 0 0 0 0 1".Trim();

            //act
            solver.Solve(3);
            var solutions = solver
                .SolutionStrings()
                .StringJoin(Environment.NewLine + Environment.NewLine);
            var constraintMatrix = solver.ConstraintMatrix.ToStringThing();

            //dessert
            solutions.Should().Be(expectedSolutions);
            constraintMatrix.Should().Be(expectedConstraintMatrix);
        }

        [Fact]
        public void SolveFindsAllPermutationsWithIdenticalPieces()
        {
            //arrange
            var B = @"
oo
oo";
            var board = new int[4][]
            {
                new int[6] { 0, 0, 0, 0, 0, 0, },
                new int[6] { 0, 0, 0, 0, 0, 0, },
                new int[6] { 0, 0, 0, 0, 0, 0, },
                new int[6] { 0, 0, 0, 0, 0, 0, },
            };
            var pieces = new[] { B, B, B, B, B, B }.ToRectangleMatrix('o');

            var expectedNumberOfPermutations = Factorial(6);

            //act
            var solver = new Pentomino(board, pieces);
            solver.Solve(1000);
            var numberOfSolutions = solver.Solutions.Length;

            //assumptions
            numberOfSolutions.Should().Be(expectedNumberOfPermutations);
        }

        static int Factorial(int n, int acc = 1) => n <= 1 ? acc : Factorial(n - 1, n * acc);

        [Fact]
        public void SolveCalendar()
        {
            //arrange
            var C = @"
oo
o
oo";
            var Z = @"
 oo
oo";
            var Z_ = @"
  oo
ooo";
            var S = @"
 oo
 o
oo";
            var L4 = @"
o
o
o
oo";
            var L3 = @"
o
o
oo";
            var V = @"
o
o
ooo";
            var d = @"
 o
oo
oo";
            var I = @"
oooo";
            var T = @"
ooo
 o
 o";
            var date = @"
---o--x
------x
-------
o------
-------
-------
-------
xxxxo--".ToRectangleMatrix('o', 'x').ToIntMatrix();
            var pieceList = new string[] { C, Z, Z_, S, L4, L3, V, d, I, T }.ToRectangleMatrix('o');
            var expected = @"
7 7 7 0 6 6 0
7 9 9 9 9 6 0
7 5 5 5 5 6 A
0 5 3 3 A A A
8 4 4 3 3 3 A
8 8 4 2 2 1 1
8 8 4 4 2 2 1
0 0 0 0 0 1 1

6 6 A 0 4 4 0
6 7 A A A 4 0
6 7 A 3 3 4 4
0 7 7 7 3 3 3
8 8 8 9 9 9 9
5 8 8 2 2 1 1
5 5 5 5 2 2 1
0 0 0 0 0 1 1".Trim();

            //act
            var solver = new Pentomino(date, pieceList);
            solver.Solve(10_000);

            var firstTwoSolutions = solver
                .SolutionStrings()
                .Take(2)
                .StringJoin(Environment.NewLine + Environment.NewLine);

            var numberOfConstraintColumns = solver.ConstraintMatrix[0].Length;
            var expectedNumberOfConstraintColumns = pieceList.Length + (8 * 7) - (2 + 4) - (1 + 1 + 1);

            //azerty
            firstTwoSolutions.Should().Be(expected);
            numberOfConstraintColumns.Should().Be(expectedNumberOfConstraintColumns);
        }

        private bool[][][] GetPentominoPieces()
        {
            var F = @"
 oo
oo
 o";
            var I = @"ooooo";
            var L = @"
o
o
o
oo";
            var N = @"
oo
 ooo";
            var P = @"
oo
oo
o";
            var T = @"
ooo
 o
 o";
            var U = @"
o o
ooo";
            var V = @"
o
o
ooo";
            var W = @"
o
oo
 oo";
            var X = @"
 o
ooo
 o";
            var Y = @"
  o
oooo";
            var Z = @"
oo
 o
 oo";

            var pentominoes = new[] { F, I, L, N, P, T, U, V, W, X, Y, Z }.ToRectangleMatrix('o');
            return pentominoes;
        }

        [Fact]
        public void Solve_Scotts_Pentomino_Problem()
        {
            //arrange
            var board = @"
--------
--------
--------
---oo---
---oo---
--------
--------
--------".ToRectangleMatrix('o').ToIntMatrix();
            var pentominoes = GetPentominoPieces();
            var correctNumberOfSolutions = 520;
            var correctNumberOfConstraintRows = 1568;
            var correctNumberOfConstraintCols = pentominoes.Length + (8 * 8 - 4);

            //act
            var solver = new Pentomino(board, pentominoes);
            solver.Solve(1000);

            //var allGuesses = solver
            //    .AllGuessesStrings()
            //    .StringJoin(Environment.NewLine + Environment.NewLine);

            var numberOfSolutions = solver.Solutions.Length;
            var numberOfConstraintRows = solver.ConstraintMatrix.Length;
            var numberOfConstraintCols = solver.ConstraintMatrix[0].Length;

            //assert
            numberOfSolutions.Should().Be(correctNumberOfSolutions);
            numberOfConstraintRows.Should().Be(correctNumberOfConstraintRows);
            numberOfConstraintCols.Should().Be(correctNumberOfConstraintCols);
        }

        [Fact]
        public void Solve_1337_tetris()
        {
            //arrange
            var L = @"
o
o
oo";
            var N = @"
oo
 oo";
            var T = @"
ooo
 o";
            var V = @"
o
oo";
            var boards = new Dictionary<string, string>
            {
                ["stair"] = @"
-xxxxxx
--xxxxx
---xxxx
----xxx
-----xx
-x----x
-------",
                ["9x3"] = @"
---
---
---
---
---
---
---
---
---",
                ["remove_square"] = @"
------
------
--xxx-
--xxx-
--xxx-
------",
                ["temp"] = @"
-------
-------
-------
-------
-------",
            };
            var board = boards["stair"].ToRectangleMatrix('x').ToIntMatrix();
            var pentominoes = new[] { L, L, L, L, N, T, V, }.ToRectangleMatrix('o');

            var solver = new Pentomino(board, pentominoes);
            solver.Solve(10_000);

            var solutions = solver
                .SolutionStrings()
                .StringJoin(Environment.NewLine + Environment.NewLine);
            var permutationsAndMirrors = Factorial(4) * 2d;
            var solutionCount = solver.Solutions.Length / permutationsAndMirrors;

            solver.Solutions.Length.Should().BeGreaterThan(0);
        }
    }
}
