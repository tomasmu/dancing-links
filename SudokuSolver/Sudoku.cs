﻿using DancingLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class Sudoku
    {
        public Sudoku() => Board = new int[0, 0];

        public Sudoku(int[,] board, ISudokuSerializer serializer = null)
        {
            if (serializer != null)
                Serializer = serializer;

            Board = DeserializeBoard(board);
        }

        public Sudoku(string board, ISudokuSerializer serializer = null)
        {
            if (string.IsNullOrEmpty(board))
                throw new Exception($"Board string must have at least one cell");

            if (serializer != null)
                Serializer = serializer;

            Board = DeserializeBoard(board);
        }

        public int[,] Board { get; }
        public int[,] Solution => Solutions.FirstOrDefault();
        public IEnumerable<int[,]> Solutions { get; private set; } = Enumerable.Empty<int[,]>();
        public ISudokuSerializer Serializer { get; set; } = new SudokuSerializer();
        public int[,] DeserializeBoard(string board) => Serializer.DeserializeBoard(board);
        public int[,] DeserializeBoard(int[,] board) => Serializer.DeserializeBoard(board);
        public string SerializedBoard => Serializer.SerializeBoard(Board);
        public string PrintableBoard => Serializer.SerializePrintFormat(Board);
        public string SerializedSolution => SerializedSolutions.FirstOrDefault();
        public IEnumerable<string> SerializedSolutions => Solutions.Select(Serializer.SerializeBoard);
        public string PrintableSolution => PrintableSolutions.FirstOrDefault();
        public IEnumerable<string> PrintableSolutions => Solutions.Select(Serializer.SerializePrintFormat);

        public void Solve(int maxSolutions)
        {
            ConstraintMatrix = CreateConstraintMatrix(Board);
            var toroidalLinkedList = new ToroidalLinkedList(ConstraintMatrix);
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList.Solutions.Select(ParseNodeListSolution);
        }

        //todo: datatype sudoku board
        private bool[][] CreateConstraintMatrix(int[,] board)
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
            var constraintMatrix = new bool[candidateRowCount][];
            for (var row = 0; row < rowLength; row++)
            {
                for (var col = 0; col < colLength; col++)
                {
                    for (var digit = 0; digit < digitCount; digit++)
                    {
                        //with 9 digits the index is in base 9 here, ie row*9^2 + col*9^1 + digit*9^0
                        var candidateIndex = (row * digitCount * digitCount) + (col * digitCount) + digit;
                        constraintMatrix[candidateIndex] = new bool[constraintColCount];

                        //add clues only if we have a blank, or when we have the correct clue
                        if (board[row, col] == SudokuConstants.BlankCellValue || board[row, col] == digit)
                        {
                            //map each constraint to a unique column value (offset + 0..cellCount-1)
                            var cellIndex = cellOffset + row * digitCount + col;
                            var rowIndex = rowOffset + row * digitCount + digit;
                            var colIndex = colOffset + col * digitCount + digit;
                            var box = row - (row % boxLength) + (col / boxLength);
                            var boxIndex = boxOffset + box * digitCount + digit;

                            //four constraints per row
                            constraintMatrix[candidateIndex][cellIndex] = true;
                            constraintMatrix[candidateIndex][rowIndex]  = true;
                            constraintMatrix[candidateIndex][colIndex]  = true;
                            constraintMatrix[candidateIndex][boxIndex]  = true;
                        }
                    }
                }
            }

            return constraintMatrix;
        }

        private int[,] ParseNodeListSolution(int[] rowIds)
        {
            var cellCount = rowIds.Length;
            int dimension = (int)Math.Sqrt(cellCount);

            var board = new int[dimension, dimension];
            for (var i = 0; i < cellCount; i++)
            {
                int rowId = rowIds[i];
                int row = rowId / (dimension * dimension);
                int col = rowId / dimension % dimension;
                int digit = rowId % dimension;

                board[row, col] = digit;
            }

            //todo: parse constraint rows instead of parsing rowId
            //can we make the constraint matrix smaller?
            //var board = new int[Board.GetLength(0), Board.GetLength(1)];
            //var constraintRows = nodes.Select(node => ConstraintMatrix[node.RowId]);

            return board;
        }

        public bool IsUnsolved => Solutions.Count() == 0;
        public bool IsSolved => Solutions.Count() == 1;
        public bool IsSolvedAmbiguous => Solutions.Count() >= 2;

        public bool[][] ConstraintMatrix { get; set; }
    }
}
