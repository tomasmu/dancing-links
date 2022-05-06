using DancingLinks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pentominoes
{
    public class Pentomino
    {
        public Pentomino(IEnumerable<string> pieces, int boardRows, int boardColumns, IEnumerable<string> blocked = null) {
            pieceNames = pieces.ToList();
            rows = boardRows;
            columns = boardColumns;
            numberOfPieces = pieceNames.Count();
        }

        private int[,] Grid { get; set; }
        private readonly List<string> pieceNames;
        private const int FORBIDDEN_VALUE = -1;
        private readonly int rows = 2;
        private readonly int columns = 3;
        private readonly int numberOfPieces = 2;

        public IEnumerable<IEnumerable<(int pieceId, IEnumerable<(int row, int col)> coordinates)>>
            Solutions { get; private set; }

        public void Solve(int maxSolutions)
        {
            ConstraintMatrix = GetConstraintList(); //TestConstraintMatrix();
            var toroidalLinkedList = new ToroidalLinkedList(ConstraintMatrix);
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList
                .Solutions
                .Select(ParseNodeListSolution);
        }

        public bool[,] GetConstraintList() {
            var constraintList = new List<bool[]>();
            var constraintColumns = numberOfPieces + rows * columns;
            
            for(var pieceIndex = 0; pieceIndex<numberOfPieces; pieceIndex++)
            {
                var possibleVariations = GetPossibleVariations(pieceNames[pieceIndex]);
                foreach(var possibleVariation in possibleVariations) {
                    var size = GetSize(possibleVariation);
                    for(var row = 0; row <= rows - size.rows; row++)
                    {
                        for(var col = 0; col <= columns - size.columns; col++)
                        {
                            if (CanPlacePiece(row, col, possibleVariation, null))
                            {
                                var constraintRow = new bool[constraintColumns];
                                constraintRow[pieceIndex] = true;
                                for (int pieceRow = 0; pieceRow < size.rows; pieceRow++)
                                {
                                    for (int pieceCol = 0; pieceCol < size.columns; pieceCol++)
                                    {
                                        if (possibleVariation[pieceRow, pieceCol])
                                        {
                                            var boardIndex = (row + pieceRow) * columns + (col + pieceCol);
                                            var constraintIndex = numberOfPieces + boardIndex;
                                            constraintRow[constraintIndex] = true;
                                        }
                                    }
                                }

                                constraintList.Add(constraintRow);
                            }
                        }
                    }
                }
            }

            var constraintMatrix = new bool[constraintList.Count(), constraintColumns];
            for (int row = 0; row < constraintList.Count(); row++)
            {
                for (int col = 0; col < constraintColumns; col++)
                {
                    constraintMatrix[row, col] = constraintList[row][col];
                }
            }

            return constraintMatrix;
        }

        private bool CanPlacePiece(int row, int col, bool[,] possibleVariation, int[] forbiddenIndices = null)
        {
            return true;
        }

        public (int columns, int rows) GetSize(bool[,] variation) {
            var columns = variation.GetLength(1);
            var rows = variation.GetLength(0);
            return (columns, rows);
        }

        public bool[][,] GetPossibleVariations(string pieceLiteral) =>
            pieceLiteral
                .ToRectangleMatrix()
                .GetUniqueRotationsAndMirrors()
                .ToArrayMatrix();

        // public void Solve(bool[,] constraintMatrix, int maxSolutions = 42)
        // {
        //     var toroidalLinkedList = new ToroidalLinkedList(constraintMatrix);
        //     toroidalLinkedList.Solve(maxSolutions);
        //     Solutions = toroidalLinkedList
        //         .Solutions
        //         .Select(ParseNodeListSolution)
        //         .ToList();
        // }

        private IEnumerable<(int pieceId, IEnumerable<(int row, int col)> coordinates)>
            ParseNodeListSolution(List<Node> nodes)
        {
            var solutionRows = nodes
                .Select(node => ConstraintMatrix.GetRow(node.RowId));

            var boardRows = solutionRows
                .Select(row =>
                {
                    var piece = row.Take(numberOfPieces);
                    var boardRowWithPiece = row.Skip(numberOfPieces);

                    var pieceId = piece
                        .ToList()
                        .FindIndex(b => b == true);
                    var pieceIndices = boardRowWithPiece
                        .Select((boolean, index) => (boolean, index))
                        .Where(bi => bi.boolean == true)
                        .Select(bi => bi.index);

                    var coordinates = pieceIndices
                        .Select(index => (row: index / columns, col: index % columns));
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

                public void InitGrid()
        {
            //Grid = new int[BOARD_SIZE, BOARD_SIZE];
            //Grid[3, 3] = FORBIDDEN_VALUE;
            //Grid[3, 4] = FORBIDDEN_VALUE;
            //Grid[4, 3] = FORBIDDEN_VALUE;
            //Grid[4, 4] = FORBIDDEN_VALUE;

            //todo: hål kan ju implementeras som enstaka rader i constraint matrixen
            //är det så här man vill att det ska se ut?
            var pentominoes = new Dictionary<string, List<int[][]>>
            {
                //Box
                ["B"] = new List<int[][]>
                {
                    new int[][]
                    {
                        new int[] { 1, 1 },
                        new int[] { 1, 1 },
                    },
                },
                //I
                ["I"] = new List<int[][]>
                {
                    new int[][]
                    {
                        new int[] { 1, 1 },
                    },
                    new int[][]
                    {
                        new int[] { 1 },
                        new int[] { 1 },
                    },
                },
                //L
                ["L"] = new List<int[][]>
                {
                    //rotations
                    new int[][]
                    {
                        new int[] { 1 },
                        new int[] { 1, 1, 1 },
                    },
                    new int[][]
                    {
                        new int[] { 1, 1 },
                        new int[] { 1 },
                        new int[] { 1 },
                    },
                    new int[][]
                    {
                        new int[] { 1, 1, 1 },
                        new int[] { 0, 0, 1 },
                    },
                    new int[][]
                    {
                        new int[] { 0, 1 },
                        new int[] { 0, 1 },
                        new int[] { 1, 1 },
                    },
                    //mirrors
                    new int[][]
                    {
                        new int[] { 1, 1, 1 },
                        new int[] { 1 },
                    },
                    new int[][]
                    {
                        new int[] { 1, 1 },
                        new int[] { 0, 1 },
                        new int[] { 0, 1 },
                    },
                    new int[][]
                    {
                        new int[] { 0, 0, 1 },
                        new int[] { 1, 1, 1 },
                    },
                    new int[][]
                    {
                        new int[] { 1, 0 },
                        new int[] { 1, 0 },
                        new int[] { 1, 1 },
                    },
                }
            };
        }

    }
}
