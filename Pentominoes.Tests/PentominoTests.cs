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
            var blocked = @"
---
-o-";
            var (rows, cols) = (2, 3);
            var pieceList = new[] { L, single }.ToRectangleMatrix('o');
            var blockedList = new[] { blocked }.ToRectangleMatrix('o');
            var solver = new Pentomino(
                rows,
                cols,
                pieceList,
                blockedList);

            var expectedSolutions = @"
0 0 0
1 2 0

0 0 0
0 2 1"[2..];

            //act
            solver.Solve(3);
            var solutions = solver
                .SolutionStrings()
                .StringJoin(Environment.NewLine + Environment.NewLine);

            //dessert
            solutions.Should().Be(expectedSolutions);
        }

        [Fact]
        public void SolveFindsAllPermutationsWithIdenticalPieces()
        {
            var B = @"
oo
oo";
            var (rows, cols) = (6, 4);
            var pieces = new[] { B, B, B, B, B, B }.ToRectangleMatrix('o');
            var solver = new Pentomino(rows, cols, pieces);
            static int factorial(int n, int acc = 1) => n <= 1 ? acc : factorial(n - 1, n * acc);
            var expectedNumberOfPermutations = factorial(6);

            solver.Solve(1000);
            var numberOfSolutions = solver.Solutions.Count();

            numberOfSolutions.Should().Be(expectedNumberOfPermutations);
        }

        [Fact]
        public void SolveCalendar()
        {
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
            var I = @"oooo";
            var T = @"
ooo
 o
 o";
            var blocked = @"
------o
------o
-
-
-
-
-
oooo";
            var date = @"
---o---x
-------x
--------
o-------
--------
--------
--------
xxxxo---";
            var (rows, cols) = (8, 7);
            var pieceList = new string[] { C, Z, Z_, S, L4, L3, V, d, I, T }.ToRectangleMatrix('o');
            var blockedList = new string[] { blocked, date }.ToRectangleMatrix('o');
            var solver = new Pentomino(
                rows,
                cols,
                pieceList,
                blockedList);
            var expected = @"
6 6 6 B 5 5 A
6 8 8 8 8 5 A
6 4 4 4 4 5 9
B 4 2 2 9 9 9
7 3 3 2 2 2 9
7 7 3 1 1 0 0
7 7 3 3 1 1 0
A A A A B 0 0

5 5 9 B 3 3 A
5 6 9 9 9 3 A
5 6 9 2 2 3 3
B 6 6 6 2 2 2
7 7 7 8 8 8 8
4 7 7 1 1 0 0
4 4 4 4 1 1 0
A A A A B 0 0"[2..];

            solver.Solve(10_000);
            var firstTwoSolutions = solver
                .SolutionStrings()
                .Take(2)
                .StringJoin(Environment.NewLine + Environment.NewLine);

            firstTwoSolutions.Should().Be(expected);

            Console.WriteLine("cbenchmark: " + solver.stopwatch.ElapsedMilliseconds);
            Debug.WriteLine("dbenchmark: " + solver.stopwatch.ElapsedMilliseconds);
        }

        [Fact]
        public void SolvePentominoProblem()
        {
            //arrange
            var F = @"
 oo
oo
 o";
            var I = @"
ooooo";
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
            var holes = @"
--------
--------
--------
---oo---
---oo---
--------
--------
--------";
            var pentominoes = new[] { F, I, L, N, P, T, U, V, W, X, Y, Z }.ToRectangleMatrix('o');
            var blocked = new[] { holes }.ToRectangleMatrix('o');
            var (rows, cols) = (8, 8);
            var solver = new Pentomino(rows, cols, pentominoes, blocked);
            var correctNumberOfSolutions = 520;

            var correctNumberOfConstraints = 1568;
            var myNumberOfConstraints = correctNumberOfConstraints + blocked.Count();

            //act
            solver.Solve(10_000);

            var numberOfSolutions = solver.Solutions.Count();
            var numberOfConstraints = solver.ConstraintMatrix.GetLength(0);

            //assert
            numberOfSolutions.Should().Be(correctNumberOfSolutions);
            numberOfConstraints.Should().Be(myNumberOfConstraints);

            //temporary benchmark-ish things
            //solver.stopwatch.ElapsedMilliseconds.Should().BeLessOrEqualTo(6200);
            //solver.stopwatch.ElapsedMilliseconds.Should().BeGreaterThan(6200);
        }
    }
}
