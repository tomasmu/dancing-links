using DancingLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class Sudoku
    {
        public Sudoku() =>
            Board = new int[0, 0];
        
        public Sudoku(int[,] board, ISudokuSerializer serializer = null)
        {
            if (serializer != null)
                Serializer = serializer;

            Board = DeserializeBoard(board);
        }
        
        public Sudoku(string board, ISudokuSerializer serializer = null)
        {
            if (serializer != null)
                Serializer = serializer;

            Board = DeserializeBoard(board);
        }

        public int[,] Board { get; }
        public int[,] Solution =>
            Solutions.FirstOrDefault();
        public IEnumerable<int[,]> Solutions { get; private set; } = Enumerable.Empty<int[,]>();

        public ISudokuSerializer Serializer { get; set; } = new SudokuSerializer();

        public int[,] DeserializeBoard(string board) =>
            Serializer.DeserializeBoard(board);
        public int[,] DeserializeBoard(int[,] board) =>
            Serializer.DeserializeBoard(board);
        public string SerializeBoard() =>
            Serializer.SerializeBoard(Board);
        public string PrintableBoard() =>
            Serializer.SerializePrintFormat(Board);

        public string SerializedSolution() =>
            SerializedSolutions().FirstOrDefault();
        public IEnumerable<string> SerializedSolutions() =>
            Solutions.Select(Serializer.SerializeBoard);

        public string PrintableSolution() =>
            PrintableSolutions().FirstOrDefault();
        public IEnumerable<string> PrintableSolutions() =>
            Solutions.Select(Serializer.SerializePrintFormat);

        public void Solve(int maxSolutions)
        {
            var constraintMatrix = CreateConstraintMatrix(Board);
            //cpu time: 23% -> 36%
            var toroidalLinkedList = new ToroidalLinkedList(constraintMatrix);
            //cpu time: 75% -> 60%
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList.Solutions.Select(ParseNodeListSolution);
        }

        //todo: datatype sudoku board
        private bool[,] CreateConstraintMatrix(int[,] board)
        {
            //most of these variables are for pedagogic reasons
            //e.g.: rowLength, colLength, and digitCount will always be equal; but makes the code easier to understand
            var cellCount = board.Length;
            var rowLength = board.GetLength(0);
            var colLength = board.GetLength(1);
            var boxLength = (int)Math.Pow(cellCount, 1 / 4.0);
            var digitCount = rowLength;

            var sudokuConstraintCount = 4;
            var cellOffset = 0 * cellCount;
            var rowOffset  = 1 * cellCount;
            var colOffset  = 2 * cellCount;
            var boxOffset  = 3 * cellCount;

            var candidateRowCount = digitCount * cellCount;
            var constraintColCount = sudokuConstraintCount * cellCount;

            //add constraints
            var constraintMatrix = new bool[candidateRowCount, constraintColCount];
            for (var row = 0; row < rowLength; row++)
            {
                for (var col = 0; col < colLength; col++)
                {
                    for (var digit = 0; digit < digitCount; digit++)
                    {
                        //add clues only if we have a blank, or when we have the correct clue
                        if (board[row, col] == SudokuConstants.BlankCellValue || board[row, col] == digit)
                        {
                            //map each constraint to a unique value (offset + 0..cellCount-1)
                            var cellIndex = cellOffset + row * digitCount + col;
                            var rowIndex = rowOffset + row * digitCount + digit;
                            var colIndex = colOffset + col * digitCount + digit;
                            var box = row - (row % boxLength) + (col / boxLength);
                            var boxIndex = boxOffset + box * digitCount + digit;

                            //four constraints per row
                            var candidateIndex = (row * digitCount * digitCount) + (col * digitCount) + digit;

                            constraintMatrix[candidateIndex, cellIndex] = true;
                            constraintMatrix[candidateIndex, rowIndex]  = true;
                            constraintMatrix[candidateIndex, colIndex]  = true;
                            constraintMatrix[candidateIndex, boxIndex]  = true;
                        }
                    }
                }
            }

            return constraintMatrix;
        }

        private int[,] ParseNodeListSolution(List<Node> nodes)
        {
            var cellCount = nodes.Count;
            int dimension = (int)Math.Sqrt(cellCount);

            var board = new int[dimension, dimension];
            for (int i = 0; i < cellCount; i++)
            {
                int rowId = nodes[i].RowId;
                int row = rowId / (dimension * dimension);
                int col = rowId / dimension % dimension;
                int digit = rowId % dimension;

                board[row, col] = digit;
            }

            return board;
        }

        //todo: perhaps not the best of checks :-)
        public bool IsSolved =>
            Solutions.Count() == 1;
        public bool IsUnsolved =>
            Solutions.Count() == 0;
        public bool IsSolvedAmbiguous =>
            Solutions.Count() >= 2;
    }
}
