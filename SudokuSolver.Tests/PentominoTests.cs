using Pentominoes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SudokuSolver.Tests
{
    public class PentominoTests
    {
        [Fact]
        public void Test()
        {
            var pentomino = new Pentomino(new[] {"B","I"},2,3);
            pentomino.Solve(42);
            var solutions = pentomino.Solutions.Count();
        }

        [Fact]
        public void TestConstraintGeneration() {
            var expected = Pentomino.TestConstraintMatrix().ToStringThing();
            var B = @"
..
..";
            var I = @"
..";
            var pentomino = new Pentomino(new[] { B, I }, 2, 3);
            var result = pentomino.GetConstraintList().ToStringThing();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestSolve()
        {
            var expected = Pentomino.TestConstraintMatrix().ToStringThing();
            var B = @"
..
..";
            var I = @"
..";
            var L = @"
.
.
..";
            var l = @"
.
..";
            var fnutt = @".";

            var rows = 6;
            var cols = 6;
            //var pentomino = new Pentomino(new[] { B, I }, rows, cols);
            //var pentomino = new Pentomino(new[] { L, L, l, B, fnutt, }, rows, cols);
            var pentomino = new Pentomino(new[] { B, B, B, B, B, B, B, B, B }, rows, cols);
            var result = pentomino.GetConstraintList().ToStringThing();

            pentomino.Solve(int.MaxValue);
            var solutions = pentomino.Solutions.ToList();

            var sb = new StringBuilder();
            var board = new int[rows, cols];
            var count = 0;
            foreach (var solution in solutions)
            {
                foreach (var (pieceId, coordinates) in solution)
                {
                    foreach (var (row, col) in coordinates)
                    {
                        board[row, col] = pieceId + 1;
                    }
                }

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (col != 0)
                            sb.Append(" ");
                        sb.Append(board[row, col] == 0 ? "." : board[row, col].ToString());
                    }

                    sb.AppendLine();
                }
                sb.AppendLine("--- " + ++count);
            }

            Debug.WriteLine(sb.ToString());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCalendar()
        {
            var expected = Pentomino.TestConstraintMatrix().ToStringThing();
            var C = @"
..
.
..";
            var Z = @"
 ..
..";
            var Zlong = @"
  ..
...";
            var S = @"
 ..
 .
..";
            var L4 = @"
.
.
.
..";
            var L3 = @"
.
.
..";
            var Lbig = @"
.
.
...";
            var d = @"
 .
..
..";
            var I = @"....";
            var T = @"
...
 .
 .";
            var blocked = @"
      .
      .





....";
            var date = @"
    .
      .




       .";

            var rows = 8;
            var cols = 7;
            var pieceList = new[] { blocked, date, C, Z, Zlong, S, L4, L3, Lbig, d, I, T };
            var blockedList = new[] { blocked, date };
            var pentomino = new Pentomino(
                pieceList,
                rows,
                cols,
                blockedList);
            //var pentomino = new Pentomino(new[] { B, B, B, B, B, B, B, B, B }, 6, 6);
            var result = pentomino.GetConstraintList().ToStringThing();

            pentomino.Solve(10);
            var solutions = pentomino.Solutions.ToList();

            var sb = new StringBuilder();
            var board = new int[rows, cols];
            var count = 0;
            foreach (var solution in solutions)
            {
                foreach (var (pieceId, coordinates) in solution)
                {
                    foreach (var (row, col) in coordinates)
                    {
                        board[row, col] = pieceId;
                    }
                }

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (col != 0)
                            sb.Append(" ");
                        sb.Append(GetChar(board[row, col]));
                    }

                    sb.AppendLine();
                }
                sb.AppendLine("--- " + ++count);
            }

            Debug.WriteLine(sb.ToString());

            Assert.Equal(expected, result);
        }

        private string GetChar(int v) =>
            v <= 9
            ? v.ToString()
            : ((char)(v - 10 + 'A')).ToString();

        [Theory]
        [InlineData(@"
..
..")]
        [InlineData(@"
..
.
.")]
        [InlineData(@"
....")]
        [InlineData(@"
...
 .
 .")]
        public void Test2(string input)
        {
            var result = input.ToRectangleMatrix().GetUniqueRotationsAndMirrors();

            foreach (var item in result)
            {
                Debug.WriteLine(item.ToStringThing());
                Debug.WriteLine("");
            }


        }
    }
}
