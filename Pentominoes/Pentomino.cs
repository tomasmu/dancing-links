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
            bool[][][] pieces)
        {
            _board = board;
            _rows = _board.Length;
            _cols = _board[0].Length;
            _cellCount = CellCount(_board);

            _pieceList = pieces;
            _numberOfPieces = _pieceList.Length;

            _constraintCount = _numberOfPieces + _cellCount;

            InitMaps();

            var pieceCells = (count: _pieceList.Sum(CellCount), str: _pieceList.Select(CellCount).StringJoin("+"));
            if (_cellCount != pieceCells.count)
                throw new ArgumentException($"Number of empty board cells ({_cellCount}) does not match the total piece size ({pieceCells.count}={pieceCells.str})");
        }

        private readonly int[][] _board;
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _cellCount;

        private readonly bool[][][] _pieceList;
        private readonly int _numberOfPieces;

        private readonly int _constraintCount;

        private Dictionary<(int row, int col), int> _mapRowColToIndex;
        private Dictionary<int, (int row, int col)> _mapIndexToRowCol;

        public bool[][] ConstraintMatrix { get; set; }
        public int[][][] Solutions { get; private set; }
        public Stopwatch Stopwatch { get; private set; } = new();

        public void Solve(int maxSolutions)
        {
            Stopwatch.Reset();
            Stopwatch.Start();

            ConstraintMatrix = CreateConstraintMatrix();
            var toroidalLinkedList = new ToroidalLinkedList(ConstraintMatrix);
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList
                .Solutions
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
                    var (pieceRows, pieceCols) = piece.GetSize();
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
                        if (IsBlocked(_board[boardRow][boardCol]))
                            return null;

                        var constraintIndex = _mapRowColToIndex[(boardRow, boardCol)];
                        constraintRow[constraintIndex] = true;
                    }
                }
            }

            return constraintRow;
        }

        private static bool IsBlocked(int cellValue) => cellValue != default;

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

        private int[][] ParseNodeListSolution(List<Node> nodes)
        {
            var solutionRows = nodes
                .Select(node => ConstraintMatrix[node.RowId]);

            var board = new int[_rows][];
            for (var row = 0; row < _rows; row++)
                board[row] = new int[_cols];

            foreach (var constraintRow in solutionRows)
            {
                for (var row = 0; row < _rows; row++)
                {
                    for (var col = 0; col < _cols; col++)
                    {
                        if (IsBlocked(_board[row][col]))
                            continue;

                        var constraintIndex = _mapRowColToIndex[(row, col)];
                        if (constraintRow[constraintIndex])
                        {
                            var pieceIndex = GetPieceIndex(constraintRow);
                            board[row][col] = pieceIndex + 1;
                        }

                        constraintIndex++;
                    }
                }
            }

            return board;
        }

        private void InitMaps()
        {
            _mapRowColToIndex = new();
            _mapIndexToRowCol = new();

            var cellIndex = 0;
            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _cols; col++)
                {
                    if (IsBlocked(_board[row][col]))
                        continue;

                    var constraintIndex = _numberOfPieces + cellIndex;
                    _mapRowColToIndex[(row, col)] = constraintIndex;
                    _mapIndexToRowCol[constraintIndex] = (row, col);

                    cellIndex++;
                }
            }
        }

        private int CellCount(int[][] matrix)
        {
            var count = 0;
            var (rows, cols) = (matrix.Length, matrix[0].Length);
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (!IsBlocked(matrix[row][col]))
                        count++;
                }
            }

            return count;
        }

        private int CellCount(bool[][] matrix)
        {
            var count = 0;
            var (rows, cols) = (matrix.Length, matrix[0].Length);
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (matrix[row][col])
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
