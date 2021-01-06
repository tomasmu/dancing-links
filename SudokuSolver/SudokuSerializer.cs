using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class SudokuSerializer : ISudokuSerializer
    {
        public int Offset { get; set; } = -1;

        public int[,] DeserializeBoard(int[,] board)
        {
            var rowLength = board.GetLength(0);
            var colLength = board.GetLength(1);
            var boardOffset = new int[rowLength, colLength];
            for (int row = 0; row < rowLength; row++)
                for (int col = 0; col < colLength; col++)
                    boardOffset[row, col] = board[row, col] + Offset;

            return boardOffset;
        }

        public int[,] DeserializeBoard(string board)
        {
            var cellCount = board.Length;
            var boxLength = (int)Math.Pow(cellCount, 1 / 4.0);
            var rowLength = boxLength * boxLength;
            var colLength = rowLength;

            if (Math.Pow(boxLength, 4) != cellCount)
            {
                var below = Math.Pow(boxLength, 4);
                var above = Math.Pow(boxLength + 1, 4);
                throw new Exception($"Input string length cannot form a valid sudoku. Length {cellCount} is not on the form (n^2)^2, closest are {below} and {above}.");
            }

            var result = new int[rowLength, colLength];
            for (int row = 0; row < rowLength; row++)
            {
                for (int col = 0; col < colLength; col++)
                {
                    int stringIndex = row * rowLength + col;
                    result[row, col] = CharToCellValue(board[stringIndex]);
                }
            }

            return result;
        }

        public string SerializeBoard(int[,] board)
        {
            var result = new StringBuilder();
            var rowLength = board.GetLength(0);
            var colLength = board.GetLength(1);
            for (int row = 0; row < rowLength; row++)
            {
                for (int col = 0; col < colLength; col++)
                {
                    var chr = CellValueToSerializeChar(board[row, col]);
                    result.Append(chr);
                }
            }

            return result.ToString();
        }

        public string SerializePrintFormat(int[,] board)
        {
            var cellCount = board.Length;
            var rowLength = board.GetLength(0);
            var colLength = board.GetLength(1);
            var boxLength = (int)Math.Pow(cellCount, 1 / 4.0);

            var charWidth = 2 * boxLength * (boxLength + 1) + 1;
            var line = new string('-', charWidth);

            var result = new StringBuilder();
            for (int row = 0; row < rowLength; row++)
            {
                if (row % boxLength == 0)
                    result.AppendLine(line);

                for (int col = 0; col < colLength; col++)
                {
                    if (col % boxLength == 0)
                        result.Append("| ");

                    var chr = CellValueToPrintChar(board[row, col]);
                    result.Append($"{chr} ");
                }

                result.AppendLine("|");
            }

            result.AppendLine(line);

            return result.ToString();
        }

        //todo: DeserializeBoard
        public string RawCellValueToString(int digit, int width) =>
            $"{digit}".PadLeft(width, ' ');

        //todo: two digit support?
        private char CellValueToPrintChar(int digit)
        {
            if (digit == SudokuConstants.BlankCellValue)
                return SudokuConstants.BlankCellPrintChar;

            digit -= Offset;
            if (digit >= 0 && digit <= 9)
                return (char)(digit + '0');
            else if (digit >= 10 && digit <= 'Z' - 'A' + 10)
                return (char)(digit - 10 + 'A');

            return '?';
        }

        private char CellValueToSerializeChar(int digit)
        {
            if (digit == SudokuConstants.BlankCellValue)
                return SudokuConstants.BlankCellSerializeChar;

            digit -= Offset;
            if (digit >= 0 && digit <= 9)
                return (char)(digit + '0');
            else if (digit >= 10 && digit <= 'Z' - 'A' + 10)
                return (char)(digit - 10 + 'A');

            return '?';
        }

        private int CharToCellValue(char chr)
        {
            int cellValue;
            if (chr == SudokuConstants.BlankCellSerializeChar)
                cellValue = SudokuConstants.BlankCellValue;
            else if (chr >= '0' && chr <= '9')
                cellValue = chr - '0' + Offset;
            else if (chr >= 'A' && chr <= 'Z')
                cellValue = chr + 10 - 'A' + Offset;
            else
                cellValue = '?';

            return cellValue;
        }
    }
}
