using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public interface ISudokuSerializer
    {
        int[,] DeserializeBoard(string board);
        int[,] DeserializeBoard(int[,] board);
        string SerializeBoard(int[,] board);
        string SerializePrintFormat(int[,] board);
    }
}
