﻿using DancingLinks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pentominoes
{
    public class Pentomino
    {
        public Pentomino(
            int boardRows,
            int boardColumns,
            IEnumerable<string> pieces,
            IEnumerable<string> blocked = null)
        {
            _pieceList = pieces.ToList();
            _rows = boardRows;
            _cols = boardColumns;
            _numberOfPieces = _pieceList.Count;
            _blockedList = blocked?.ToList();
            _numberOfBlocked = blocked?.Count() ?? 0;
            _totalNumberOfPieces = _numberOfPieces + _numberOfBlocked;
            _constraintColumnCount = _totalNumberOfPieces + _rows * _cols;
            _blockedGrid = CreateBlockedGrid(_rows, _cols, _blockedList);
        }

        public char[] ActiveCellCharacters { get; set; } = new[] { 'o' };

        private bool[,] CreateBlockedGrid(int rows, int cols, List<string> blockedList)
        {
            var blockedGrid = new bool[rows, cols];
            if (blockedList == null)
                return blockedGrid;

            foreach (var blockedString in blockedList)
            {
                var blocked = blockedString.ToRectangleMatrix(ActiveCellCharacters);
                for (var row = 0; row < rows; row++)
                {
                    for (var col = 0; col < cols; col++)
                    {
                        if (blocked[row][col])
                            blockedGrid[row, col] = true;
                    }
                }
            }

            return blockedGrid;
        }

        private readonly List<string> _pieceList;
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _numberOfPieces;
        private readonly int _numberOfBlocked;
        private readonly int _totalNumberOfPieces;
        private readonly int _constraintColumnCount;
        private readonly bool[,] _blockedGrid;
        private readonly List<string> _blockedList;

        public readonly Stopwatch stopwatch = new();

        public IEnumerable<IEnumerable<(int pieceId, IEnumerable<(int row, int col)> coordinates)>> Solutions { get; private set; }

        public void Solve(int maxSolutions)
        {
            stopwatch.Reset();
            stopwatch.Start();

            ConstraintMatrix = CreateConstraintMatrix();
            var toroidalLinkedList = new ToroidalLinkedList(ConstraintMatrix);
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList
                .Solutions
                .Select(ParseNodeListSolution);

            stopwatch.Stop();
        }

        //todo: bool[][]
        //todo: create constraints with holes instead of treating them as blocking pieces
        //this reduces the number of columns
        public bool[,] CreateConstraintMatrix()
        {
            //create a constraint list since we don't know the number of constraints yet
            var constraintList = new List<bool[]>();

            //add piece constraints with all piece rotations and positions
            for (var pieceIndex = 0; pieceIndex < _numberOfPieces; pieceIndex++)
            {
                var pieceRotations = GetAllUniquePieceRotations(_pieceList[pieceIndex]);
                foreach (var piece in pieceRotations) {
                    var (pieceRows, pieceCols) = piece.GetSize();
                    for (var row = 0; row <= _rows - pieceRows; row++)
                    {
                        for (var col = 0; col <= _cols - pieceCols; col++)
                        {
                            var constraintRow = CreateConstraintRow(pieceIndex, piece, row, col, pieceRows, pieceCols);

                            if (constraintRow != null)
                                constraintList.Add(constraintRow);
                        }
                    }
                }
            }

            //add blocked constraints as static blocks
            for (var blockedIndex = 0; blockedIndex < _numberOfBlocked; blockedIndex++)
            {
                var pieceIndex = _numberOfPieces + blockedIndex;
                var blocked = _blockedList[blockedIndex].ToRectangleMatrix(ActiveCellCharacters);
                var (rows, cols) = blocked.GetSize();
                var constraintRow = new bool[_constraintColumnCount];
                constraintRow[pieceIndex] = true;

                for (var row = 0; row < rows; row++)
                {
                    for (var col = 0; col < cols; col++)
                    {
                        if (blocked[row][col])
                        {
                            var boardIndex = row * _cols + col;
                            var constraintIndex = _totalNumberOfPieces + boardIndex;
                            constraintRow[constraintIndex] = true;
                        }
                    }
                }

                constraintList.Add(constraintRow);
            }

            //create constraintMatrix array from list
            var constraintMatrix = new bool[constraintList.Count, _constraintColumnCount];
            for (var row = 0; row < constraintList.Count; row++)
            {
                for (var col = 0; col < _constraintColumnCount; col++)
                {
                    constraintMatrix[row, col] = constraintList[row][col];
                }
            }

            return constraintMatrix;
        }

        private bool[] CreateConstraintRow(int pieceIndex, bool[,] piece, int row, int col, int pieceRows, int pieceCols)
        {
            var constraintRow = new bool[_constraintColumnCount];
            constraintRow[pieceIndex] = true;

            for (var pieceRow = 0; pieceRow < pieceRows; pieceRow++)
            {
                for (var pieceCol = 0; pieceCol < pieceCols; pieceCol++)
                {
                    var boardRow = row + pieceRow;
                    var boardCol = col + pieceCol;

                    if (piece[pieceRow, pieceCol])
                    {
                        if (_blockedGrid[boardRow, boardCol])
                            return null;

                        var boardIndex = boardRow * _cols + boardCol;
                        var constraintIndex = _totalNumberOfPieces + boardIndex;
                        constraintRow[constraintIndex] = true;
                    }
                }
            }

            return constraintRow;
        }

        public bool[][,] GetAllUniquePieceRotations(string pieceLiteral) =>
            pieceLiteral
                .ToRectangleMatrix(ActiveCellCharacters)
                .GetUniqueRotationsAndMirrors()
                .ToArrayMatrixArray();

        private string ToSolutionString(IEnumerable<(int pieceId, IEnumerable<(int row, int col)> coordinates)> solution)
        {
            var sb = new StringBuilder();
            var board = new int[_rows, _cols];
            foreach (var (pieceId, coordinates) in solution)
            {
                foreach (var (row, col) in coordinates)
                {
                    board[row, col] = pieceId;
                }
            }

            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _cols; col++)
                {
                    if (col != 0)
                        sb.Append(' ');
                    sb.Append(board[row, col].ToBase36Digit());
                }

                if (row != _rows - 1)
                    sb.AppendLine();
            }

            var result = sb.ToString();
            return result;
        }

        public IEnumerable<string> SolutionStrings() =>
            Solutions.Select(ToSolutionString);

        private IEnumerable<(int pieceId, IEnumerable<(int row, int col)> coordinates)>
            ParseNodeListSolution(List<Node> nodes)
        {
            var solutionRows = nodes
                .Select(node => ConstraintMatrix.GetRow(node.RowId));

            var boardRows = solutionRows
                .Select(row =>
                {
                    var piece = row.Take(_totalNumberOfPieces);
                    var boardRowWithPiece = row.Skip(_totalNumberOfPieces);

                    var pieceId = piece
                        .ToList()
                        .FindIndex(b => b == true);
                    var pieceIndices = boardRowWithPiece
                        .Select((boolean, index) => (boolean, index))
                        .Where(bi => bi.boolean == true)
                        .Select(bi => bi.index);

                    var coordinates = pieceIndices
                        .Select(index => (row: index / _cols, col: index % _cols));
                    return (pieceId, coordinates);
                });

            return boardRows;
        }

        public bool[,] ConstraintMatrix { get; set; }

        public static bool[,] TestConstraintMatrix()
        {
            //pentominoes:
            //måste man dra av 4 och försöka trixa bort dem om det är hål?
            //kan man inte bara ha 8*8 och aldrig placera ut nåt på hålen?
            //dvs
            //const int COLUMNS = NUMBER_OF_PENTOMINOES + BOARD_LENGTH - 4;
            //eller
            //const int COLUMNS = NUMBER_OF_PENTOMINOES + BOARD_LENGTH;

            var numberOfPieces = 2;
            //board dimensions
            var rows = 2;
            var columns = 3;

            var boardLength = rows * columns;

            var ROWS = 2+(3+4);     //rows är egentligen okänd här men exakt antal krävs, list.Count() innan man skapar constraint matrixen?
            var COLUMNS = numberOfPieces + boardLength;
            //ROWS upper bound: 8*8 * 12 * 4 = 3072; //starta i 8x8-griden, 12 bitar, 4 rotationer (inga speglingar)
            //1568 är antalet enligt https://arxiv.org/pdf/cs/0011047.pdf
            var constraintMatrix = new bool[ROWS, COLUMNS];

            int rowNumber;
            int pieceIndex;

            //set piece
            //on a 2x3 board we have..

            //B: square piece
            //B B x //indices 3*row + col
            //B B x
            rowNumber = 0;
            pieceIndex = 0;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 0] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 1] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 0] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 1] = true;

            //x B B
            //x B B
            rowNumber = 1;
            pieceIndex = 0;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 1] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 2] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 1] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 2] = true;

            //I: I piece
            //vertical
            //I x x
            //I x x
            rowNumber = 2;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 0] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 0] = true;

            //x I x
            //x I x
            rowNumber = 3;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 1] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 1] = true;

            //x x I
            //x x I
            rowNumber = 4;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 2] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 2] = true;

            //horizontal
            //I I x
            //x x x
            rowNumber = 5;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 0] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 1] = true;

            //x I I
            //x x x
            rowNumber = 6;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 1] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 0 + 2] = true;

            //x x x
            //I I x
            rowNumber = 7;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 0] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 1] = true;

            //x x x
            //x I I
            rowNumber = 8;
            pieceIndex = 1;
            constraintMatrix[rowNumber, pieceIndex] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 1] = true;
            constraintMatrix[rowNumber, numberOfPieces + columns * 1 + 2] = true;

            return constraintMatrix;
        }
    }
}
