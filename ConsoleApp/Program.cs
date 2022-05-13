using Pentominoes;
using SudokuSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //            //todo: command line arguments
            //            //benchmark
            //            //solve puzzle and file
            //            //arguments: --solve string, --file string, --stats, --verbose
            //            var fileName = @"../../../SudokuSolver.Tests/Files/t-dillon/puzzles5_forum_hardest_1905_11+";
            //            //var fileName = @"../../../SudokuSolver.Tests/Files/t-dillon/puzzles2_17_clue";
            //            var commentsAndEmptyPattern = new Regex($"^#|^$", RegexOptions.Compiled);

            //            var numberOfSudokus = 5555;   //Release
            //#if DEBUG
            //            numberOfSudokus = 1111;       //Debug: without debugger attached (ctrl+f5)
            //#endif
            //            if (Debugger.IsAttached)
            //                numberOfSudokus = 165;    //Debug: with debugger attached (f5)

            //            var sudokus = File.ReadAllLines(fileName)
            //                .Where(line => !commentsAndEmptyPattern.IsMatch(line))
            //                .Take(numberOfSudokus)
            //                .ToList();
            //            var solutions = new StringBuilder();

            //            ////from ton at http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801-15.html#p276828
            //            //var sudoku_16x16_55clues = "...9.....3.....2....F..CG....A.8.4.5.....9.............A..D....F..8............G.....5..........A.F............C.....D9..4....7.....G..E.........5.4.....7.B1D9....3.....1..5.4.....A..F........F.G.....8.A....E.....14.....2.5.8.......C.G..........973......1.";
            //            //var sudoku_25x25_146clues = ".C...........9...F..M..........9.J...AB............4.M.............C..KF1..G1KF...........E.4.MCP.........72B.N.J8....D......D.8.2.......7N6.9.JB.........N....G..........1....7...A3......9J..D.N....5......F...M..........PK........C.E1P.......3..G4F........J9....D7....5....NA.M.PH..........C.K........KCG..........EFM1.P......F....B..........G.....9....1..A.N.....I.........8A...8.D.5.......J.BN...6....H7.........1.....4M.......B.N.......K...2.E....J.N.5.8.9..L....A.7.......3........P4..C.....F1G...........F.C..P1.M..3....N...J.2...........A....7B...I.......K......................4EF..M........A.B...9..................J.";
            //            //sudokus = new List<string> {
            //            //    sudoku_25x25_146clues
            //            //    //sudoku_16x16_55clues,
            //            //    //string.Join("", sudoku_16x16_55clues.Reverse()),
            //            //};

            //            var totalTime = new Stopwatch();
            //            totalTime.Start();
            //            foreach (var sudoku in sudokus)
            //            {
            //                var solver = new Sudoku(sudoku);
            //                solver.Solve(1);
            //            }
            //            totalTime.Stop();

            //            //Console.WriteLine(solver.SerializedBoard);
            //            //Console.WriteLine(solver.PrintableBoard);
            //            //if (maxSolveTime > solveTime.Elapsed.TotalMilliseconds)
            //            //{
            //            //    maxSolveTime = solveTime.Elapsed.TotalMilliseconds;
            //            //    maxSudoku = sudoku;
            //            //}
            //            //Console.WriteLine(solver.PrintableSolution());
            //            //Console.WriteLine(solver.SerializedBoard);
            //            //Console.WriteLine(solver.SerializedSolution);
            //            //Console.WriteLine(solver.SerializedSolutions.Count());
            //            //solutions.AppendLine(solver.SerializedSolution);

            //            //File.WriteAllText($"{fileName}_solutions.txt", solutions.ToString());
            //            //Console.WriteLine(maxSolveTime + ":" + maxSudoku);
            //            //Console.WriteLine($"filename          : {fileName}");
            //            //Console.WriteLine($"number of puzzles : {sudokus.Count()}");
            //            //Console.WriteLine($"total seconds     : {totalTime.Elapsed.TotalSeconds}");
            //            //Console.WriteLine($"time per puzzle   : {totalTime.Elapsed.TotalMilliseconds / sudokus.Count()} ms");
            //            //Console.WriteLine($"puzzles per second: {sudokus.Count() / totalTime.Elapsed.TotalSeconds}");

            //            //puzzles5_forum_hardest_1905_11+
            //            //release:               5555 st, ca 5000 ms +- 50 / 1%
            //            //debug: profiling        165 st, ca 5000 ms
            //            //debug: no debugging    1111 st, ca 5000 ms
            //            Console.WriteLine($"number of puzzles : {sudokus.Count()}");
            //            Console.WriteLine($"total milliseconds: {totalTime.Elapsed.TotalMilliseconds}");
            //            Console.ReadKey();
        }
    }
}
