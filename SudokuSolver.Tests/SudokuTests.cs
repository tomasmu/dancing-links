using System;
using System.Linq;
using Xunit;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;

namespace SudokuSolver.Tests
{
    public class SudokuTests
    {
        [Fact]
        public void Can_generate_correct_number_of_solutions_to_a_blank_4x4_sudoku()
        {
            var sudoku_4x4_no_clues = new string('.', 16);
            var expectedNumberOfSolutions = 288;

            var sudoku = new Sudoku(sudoku_4x4_no_clues);
            sudoku.Solve(int.MaxValue);
            var numberOfSolutions = sudoku.Solutions.Count();

            numberOfSolutions.Should().Be(expectedNumberOfSolutions);
        }

        [Fact]
        public void Can_find_both_solutions_to_ambiguous_9x9()
        {
            var sudoku_ambiguous = @"
835 617 942
612 495 738
7.. 238 651

1.. 763 825
367 582 194
258 149 367

571 926 483
486 351 279
923 874 516".RemoveWhiteSpace();
            var expectedNumberOfSolutions = 2;

            var sudoku = new Sudoku(sudoku_ambiguous);
            sudoku.Solve(int.MaxValue);
            var numberOfSolutions = sudoku.Solutions.Count();

            sudoku.IsSolved.Should().BeFalse();
            sudoku.IsSolvedAmbiguous.Should().BeTrue();
            numberOfSolutions.Should().Be(expectedNumberOfSolutions);
        }

        [Fact]
        public void Can_solve_9x9_sudoku()
        {
            //highest number of givens for a minimal puzzle
            //by dobrichev, source: http://forum.enjoysudoku.com/high-clue-tamagotchis-t30020-135.html
            var sudoku_9x9_40clues = @"..........12.34567.345.6182..1.582.6..86....1.2...7.5...37.5.28.8..6.7..2.7.83615";
            var expectedSolution   = @"576821349812934567934576182741358296358692471629147853463715928185269734297483615";

            var sudoku = new Sudoku(sudoku_9x9_40clues);
            sudoku.Solve(int.MaxValue);

            sudoku.IsSolved.Should().BeTrue();
            sudoku.SerializedSolution.Should().Be(expectedSolution);
        }

        [Fact]
        public void Can_solve_16x16_sudoku()
        {
            var sudoku_16x16_55clues = "...9.....3.....2....F..CG....A.8.4.5.....9.............A..D....F..8............G.....5..........A.F............C.....D9..4....7.....G..E.........5.4.....7.B1D9....3.....1..5.4.....A..F........F.G.....8.A....E.....14.....2.5.8.......C.G..........973......1.";
            var expectedSolution     = "DFC958G463EA71B21E72F6DCGB459A3864A57B31F928ECGD3GB89E2A1CD7456F578B4FC63A12D9EG4D9C25AB7G6E83F1A1F63GE7589DB42CE23G1D98B4CFA675786FG31E4D59C2ABG5E46C82A7FB1D93CA23D7B9E18G5F469B1DA45F263CGE87F9G1B26D85A437CEB6DEC14G9F73285A8347EAF5C2G16BD92C5A8973DEB6FG14";

            var sudoku = new Sudoku(sudoku_16x16_55clues);
            sudoku.Solve(1);

            sudoku.IsSolved.Should().BeTrue();
            sudoku.SerializedSolution.Should().Be(expectedSolution);
        }

        [Fact]
        public void Can_solve_16x16_sudoku_without_offset()
        {
            var sudoku_16x16_55clues = "...8.....2.....1....E..BF....9.7.3.4.....8.............9..C....E..7............F.....4..........9.E............B.....C8..3....6.....F..D.........4.3.....6.A0C8....2.....0..4.3.....9..E........E.F.....7.9....D.....03.....1.4.7.......B.F..........862......0.";
            var expectedSolution     = "CEB847F352D960A10D61E5CBFA34892753946A20E817DBFC2FA78D190BC6345E467A3EB52901C8DF3C8B149A6F5D72E090E52FD6478CA31BD12F0C87A3BE9564675EF20D3C48B19AF4D35B7196EA0C82B912C6A8D07F4E358A0C934E152BFD76E8F0A15C749326BDA5CDB03F8E6217497236D9E4B1F05AC81B497862CDA5EF03";

            var serializer = new SudokuSerializer() { Offset = 0 };
            var sudoku = new Sudoku(sudoku_16x16_55clues, serializer);
            sudoku.Solve(1);

            sudoku.IsSolved.Should().BeTrue();
            sudoku.SerializedSolution.Should().Be(expectedSolution);
        }

        [Fact]
        public void Can_solve_25x25_sudoku()
        {
            //by qiuyanzhe, source: http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html#p269747
            var sudoku_25x25_156clues = "123456789ABC.............6789ABC..................BC.............................................ONM.................ONMLKJIH23456789ABC..............789ABC...................C.............................................ONML................ONMLKJIHG3456789ABC...............89ABC.................................................................ONMLK...............ONMLKJIHGF456789ABC................9ABC.............................................O...................ONMLKJ..............ONMLKJIHGFE56789ABC.................ABC.............................................ON..................ONMLKJI.............ONMLKJIHGFED";
            var expectedSolution      = "123456789ABCONMLKJIHGFEDP6789ABCONMLKJIHGFEDP12345BCONMLKJIHGFEDP123456789ALKJIHGFEDP123456789ABCONMGFEDP123456789ABCONMLKJIH23456789ABCONMLKJIHGFEDP1789ABCONMLKJIHGFEDP123456CONMLKJIHGFEDP123456789ABKJIHGFEDP123456789ABCONMLFEDP123456789ABCONMLKJIHG3456789ABCONMLKJIHGFEDP1289ABCONMLKJIHGFEDP1234567ONMLKJIHGFEDP123456789ABCJIHGFEDP123456789ABCONMLKEDP123456789ABCONMLKJIHGF456789ABCONMLKJIHGFEDP1239ABCONMLKJIHGFEDP12345678NMLKJIHGFEDP123456789ABCOIHGFEDP123456789ABCONMLKJDP123456789ABCONMLKJIHGFE56789ABCONMLKJIHGFEDP1234ABCONMLKJIHGFEDP123456789MLKJIHGFEDP123456789ABCONHGFEDP123456789ABCONMLKJIP123456789ABCONMLKJIHGFED";

            var sudoku = new Sudoku(sudoku_25x25_156clues);
            sudoku.Solve(2);

            sudoku.IsSolved.Should().BeTrue();
            sudoku.SerializedSolution.Should().Be(expectedSolution);
        }

        [Fact]

        public void Solve_1x1_sudoku()
        {
            var puzzle = ".";
            var sudoku = new Sudoku(puzzle);
            sudoku.Solve(int.MaxValue);

            sudoku.IsSolved.Should().BeTrue();
            sudoku.SerializedSolution.Should().Be("1");
        }

        [Fact]
        public void Can_uniquely_solve_9x9_sudoku()
        {
            var puzzle_worst_backtrack_case_17clues = @"
.........
.....3.85
..1.2....
...5.7...
..4...1..
.9.......
5......73
..2.1....
....4...9".RemoveWhiteSpace();
            var expectedResult = @"
987654321
246173985
351928746
128537694
634892157
795461832
519286473
472319568
863745219".RemoveWhiteSpace();

            var sudoku = new Sudoku(puzzle_worst_backtrack_case_17clues);
            sudoku.Solve(int.MaxValue);
            var actual = sudoku.SerializedSolution;

            actual.Should().Be(expectedResult);
            sudoku.IsSolved.Should().BeTrue();
        }

        [Theory]
        //credit goes to: https://github.com/Emerentius/sudoku/tree/master/sudokus
        [InlineData(@"../../../Files/Emerentius/easy_sudokus.txt")]
        [InlineData(@"../../../Files/Emerentius/medium_sudokus.txt")]
        [InlineData(@"../../../Files/Emerentius/hard_sudokus.txt")]
        public void Files_from_emerent(string fileName)
        {
            foreach (var puzzleAndAnswer in ParsePuzzleAnswerFile(fileName))
            {
                var puzzle = puzzleAndAnswer[0];
                var expectedAnswer = puzzleAndAnswer[1];
                var sudoku = new Sudoku(puzzle);
                sudoku.Solve(int.MaxValue);
                var actual = sudoku.SerializedSolution;

                actual.Should().Be(expectedAnswer);
                sudoku.IsSolved.Should().BeTrue();
                sudoku.Solutions.Should().ContainSingle();
            }
        }

        [Theory]
        [InlineData(@"../../../Files/Emerentius/invalid_sudokus.txt")]
        public void Cannot_solve_invalid_sudokus(string fileName)
        {
            foreach (var puzzleAndComment in ParsePuzzleAnswerFile(fileName))
            {
                var puzzle = puzzleAndComment[0];
                //var comment = puzzleAndComment[1];
                var sudoku = new Sudoku(puzzle);
                sudoku.Solve(int.MaxValue);

                sudoku.IsUnsolved.Should().BeTrue();
            }
        }

        public IEnumerable<string[]> ParsePuzzleAnswerFile(string fileName)
        {
            var commentAndEmptyPattern = new Regex($"^#|^$", RegexOptions.Compiled);
            var puzzleAnswerPair = File.ReadAllLines(fileName)
                .Where(line => !commentAndEmptyPattern.IsMatch(line))
                .Select(line => line.Split(";"));

            return puzzleAnswerPair;
        }
    }
}
