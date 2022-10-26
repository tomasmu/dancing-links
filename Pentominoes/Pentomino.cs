using DancingLinks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pentominoes
{
    public class Pentomino
    {
        public Pentomino(
            int[][] board,
            IEnumerable<bool[][]> pieces)
        {
            _board = board;
            _rows = _board.Length;
            _cols = _board[0].Length;
            _cellCount = CellCount(_board);

            _pieceList = pieces.ToList();
            _numberOfPieces = _pieceList.Count();

            _constraintCount = _numberOfPieces + _cellCount;

            SanityCheck();

            CreateIndexMapping();
        }

        private void SanityCheck()
        {
            //check if it's theoretically possible to fill the board with the given pieces
            //the size of all pieces must equal the board size in order to fill it exactly
            var pieceCells = _pieceList.Sum(CellCount);
            if (_cellCount != pieceCells)
            {
                var pieceCounts = _pieceList.Select(CellCount).StringJoin("+");
                throw new ArgumentException($"{_cellCount} board cells != {pieceCells} piece cells ({pieceCounts})");
            }
        }

        private readonly int[][] _board;
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _cellCount;

        private readonly List<bool[][]> _pieceList;
        private readonly int _numberOfPieces;

        private readonly int _constraintCount;

        private Dictionary<(int row, int col), int> _mapRowColToIndex;
        private Dictionary<int, (int row, int col)> _mapIndexToRowCol;

        public bool[][] ConstraintMatrix { get; set; }
        public int[][][] Solutions { get; private set; }
        public int[][][] AllGuesses { get; private set; }
        public Stopwatch Stopwatch { get; private set; } = new();

        public void Solve(int maxSolutions)
        {
            Stopwatch.Restart();

            ConstraintMatrix = CreateConstraintMatrix();
            var toroidalLinkedList = new ToroidalLinkedList(ConstraintMatrix);
#if ASCII && DEBUG
            var temp = ConstraintMatrix.ToStringThing();
#endif
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList
                .Solutions
                .Select(ParseNodeListSolution)
                .ToArray();
            AllGuesses = toroidalLinkedList
                .AllGuesses
                .Select(ParseNodeListSolution)
                .ToArray();

            Stopwatch.Stop();
        }

        public bool[][] CreateConstraintMatrix()
        {
            //create a constraint list since we don't know the number of constraints yet
            var constraintList = new List<bool[]>();

            //add piece constraints with all piece rotations and positions
            for (var pieceIndex = 0; pieceIndex < _numberOfPieces; pieceIndex++)
            {
                var pieceRotations = _pieceList[pieceIndex].GetUniqueRotationsAndMirrors();
                foreach (var piece in pieceRotations)
                {
                    var (pieceCols, pieceRows) = piece.GetDimension();
                    for (var row = 0; row <= _rows - pieceRows; row++)
                    {
                        for (var col = 0; col <= _cols - pieceCols; col++)
                        {
                            var constraintRow = CreateConstraintRow(pieceIndex, piece, pieceRows, pieceCols, row, col);

                            if (constraintRow != null)
                                constraintList.Add(constraintRow);
                        }
                    }
                }
            }

            var constraintMatrix = constraintList.ToArray();
            return constraintMatrix;
        }

        private bool[] CreateConstraintRow(int pieceIndex, bool[][] piece, int pieceRows, int pieceCols, int row, int col)
        {
            var constraintRow = new bool[_constraintCount];
            constraintRow[pieceIndex] = true;

            for (var pieceRow = 0; pieceRow < pieceRows; pieceRow++)
            {
                for (var pieceCol = 0; pieceCol < pieceCols; pieceCol++)
                {
                    if (piece[pieceRow][pieceCol])
                    {
                        var (boardRow, boardCol) = (row + pieceRow, col + pieceCol);
                        if (!IsBlank(_board[boardRow][boardCol]))
                            return null;

                        var constraintIndex = _mapRowColToIndex[(boardRow, boardCol)];
                        constraintRow[constraintIndex] = true;
                    }
                }
            }

            return constraintRow;
        }

        private static bool IsBlank(int cellValue) => cellValue == default;
        private static bool IsBlank(bool cellValue) => cellValue == default;

        private string ToSolutionString(int[][] board)
        {
            var sb = new StringBuilder();
            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _cols; col++)
                {
                    if (col != 0)
                        sb.Append(' ');

                    sb.Append(board[row][col].ToBase36Digit());
                }

                if (row != _rows - 1)
                    sb.AppendLine();
            }

            var result = sb.ToString();
            return result;
        }

        public IEnumerable<string> SolutionStrings() => Solutions.Select(ToSolutionString);
        public IEnumerable<string> AllGuessesStrings() => AllGuesses.Select(ToSolutionString);

        private int[][] ParseNodeListSolution(int[] rowIds)
        {
            var solutionRows = rowIds
                .Select(rowId => ConstraintMatrix[rowId]);

            var board = new int[_rows][];
            for (var row = 0; row < _rows; row++)
                board[row] = new int[_cols];

            foreach (var constraintRow in solutionRows)
            {
                for (var row = 0; row < _rows; row++)
                {
                    for (var col = 0; col < _cols; col++)
                    {
                        if (!IsBlank(_board[row][col]))
                            continue;

                        var constraintIndex = _mapRowColToIndex[(row, col)];
                        if (constraintRow[constraintIndex])
                        {
                            var pieceIndex = GetPieceIndex(constraintRow);
                            board[row][col] = pieceIndex + 1;
                        }
                    }
                }
            }

            return board;
        }

        private void CreateIndexMapping()
        {
            _mapRowColToIndex = new();
            _mapIndexToRowCol = new();

            var cellIndex = 0;
            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _cols; col++)
                {
                    if (!IsBlank(_board[row][col]))
                        continue;

                    var constraintIndex = _numberOfPieces + cellIndex;
                    _mapRowColToIndex[(row, col)] = constraintIndex;
                    _mapIndexToRowCol[constraintIndex] = (row, col);

                    cellIndex++;
                }
            }
        }

        private static int CellCount(int[][] matrix)
        {
            var count = 0;
            var (rows, cols) = (matrix.Length, matrix[0].Length);
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (IsBlank(matrix[row][col]))
                        count++;
                }
            }

            return count;
        }

        private static int CellCount(bool[][] matrix)
        {
            var count = 0;
            var (rows, cols) = (matrix.Length, matrix[0].Length);
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (!IsBlank(matrix[row][col]))
                        count++;
                }
            }

            return count;
        }

        private static int GetPieceIndex(bool[] constraintRow)
        {
            var pieceIndex = 0;
            while (!constraintRow[pieceIndex])
                pieceIndex++;
            return pieceIndex;
        }
    }
}
