using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DancingLinks
{
    public class ToroidalLinkedList
    {
        public ToroidalLinkedList(bool[][] constraintMatrix)
        {
            _constraintMatrix = constraintMatrix;
            _rows = _constraintMatrix.Length;
            _cols = _constraintMatrix[0].Length;
            _maxRowDigits = _rows.GetWidth();
            _maxColDigits = _cols.GetWidth();
            columnRoot = CreateToroidalLinkedList(_constraintMatrix);
        }

        private bool[][] _constraintMatrix;
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _maxRowDigits;
        private readonly int _maxColDigits;

        private Node CreateToroidalLinkedList(bool[][] constraintMatrix)
        {
            //create column root
            var colRoot = new Node(NodeConstants.ROW_HEADER, NodeConstants.COL_HEADER);

            //create column header row
            var colLength = constraintMatrix.Length > 0
                ? constraintMatrix[0].Length
                : 0;
            for (var col = 0; col < colLength; col++)
            {
                var colHead = new Node(NodeConstants.ROW_HEADER, col);
                colRoot.AddHorizontalNode(colHead);
            }

            //create data node rows based on constraint matrix
            var rowLength = constraintMatrix.Length;
            for (var row = 0; row < rowLength; row++)
            {
                //pretend first existing dataNode on each row is a row header, so we can append nodes horizontally
                Node dataHead = null;
                //create data nodes rowwise
                for (var colHead = colRoot.Right; colHead != colRoot; colHead = colHead.Right)
                {
                    //create data node if constraint matrix says so
                    if (constraintMatrix[row][colHead.ColId])
                    {
                        var data = new Node(row, colHead.ColId);
                        if (dataHead == null)
                            dataHead = data;
                        else
                            dataHead.AddHorizontalNode(data);

                        colHead.AddVerticalDataNode(data);
                    }
                }
            }

            return colRoot;
        }

        private readonly Stack<Node> guesses = new Stack<Node>();
        public List<List<Node>> Solutions { get; set; } = new List<List<Node>>(); //todo: List<Solution>?
        private readonly Node columnRoot;
        private int _maxSolutions;
        private Dictionary<string, long> _counters;
        private Stopwatch _stopwatch;

        public void Solve(int maxSolutions)
        {
            _maxSolutions = maxSolutions;
            Solutions.Clear();
            guesses.Clear();
            _counters = new Dictionary<string, long>();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            Search();
        }

        //this is DLX, Algorithm X implemented with Dancing Links
        private void Search()
        {
            _counters.Increment(nameof(Search));
            var temp = ToStringThing();
            //if column list is empty we have found a solution
            if (columnRoot.Right == columnRoot)
            {
                var solution = new List<Node>(guesses);
                Solutions.Add(solution);
                _counters["solution_" + Solutions.Count] = _stopwatch.ElapsedMilliseconds;
                return;
            }

            var colHead = GetColumnWithFewestRows();
            //if we have columns without data rows it's unsolvable
            if (colHead.Count == 0)
            {
                _counters.Increment("unsolvable");
                return;
            }

            //cover column
            colHead.CoverColumnAndRows();

            //loop vertical nodes
            for (var dataRow = colHead.Down; dataRow != colHead; dataRow = dataRow.Down)
            {
                _counters.Increment(nameof(guesses));
                //guess row is part of the solution
                guesses.Push(dataRow);

                //cover columns with intersecting rows
                for (var dataCol = dataRow.Right; dataCol != dataRow; dataCol = dataCol.Right)
                {
                    _counters.Increment("dataCol cover");
                    dataCol.ColumnHeader.CoverColumnAndRows();
                }

                //if (Solutions.Count < maxSolutions)
                //    Search(maxSolutions);
                //old benchmark: 50% faster when searching for a single solution, 1-2% faster when searching for all
                Search();
                if (Solutions.Count >= _maxSolutions)
                    return;

                //backtrack current guess
                for (var dataCol = dataRow.Left; dataCol != dataRow; dataCol = dataCol.Left)
                {
                    dataCol.ColumnHeader.UncoverColumnAndRows();
                }

                //undo guess
                guesses.Pop();
            }

            //backtrack
            colHead.UncoverColumnAndRows();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Node GetColumnWithFewestRows()
        {
            //todo:
            //needs profiling/benchmark first!
            //if i remember correctly, this loop runs a lot timewise (only in debug?)
            //it is also crucial for minimizing solving time (random column is very slow)
            //can columnRoot keep track of min column so we get O(1) retrieval instead of O(N)?
            //maybe with a stack on every column hide, so we can backtrack, etc
            var min = columnRoot.Right;
            for (var colHead = min.Right; min.Count > 0 && colHead != columnRoot; colHead = colHead.Right)
            {
                _counters.Increment("min.Count");
                if (colHead.Count < min.Count)
                    min = colHead;
            }

            return min;
        }

        private string ToStringThing()
        {
            if (columnRoot == columnRoot.Right)
                return string.Empty;

            var coordinateSet = new HashSet<(int row, int col)>();
            var rowSet = new HashSet<int>();
            var colSet = new HashSet<int>();

            var minRow = _rows;
            var maxRow = 0;
            var minCol = columnRoot.Left.ColId;
            var maxCol = 0;
            for (var col = columnRoot.Right; col != columnRoot; col = col.Right)
            {
                coordinateSet.Add((col.RowId, col.ColId));
                rowSet.Add(col.RowId);
                colSet.Add(col.ColId);
                for (var row = col.Down; row != col; row = row.Down)
                {
                    if (row.RowId < minRow)
                        minRow = row.RowId;
                    else if (row.RowId > maxRow)
                        maxRow = row.RowId;

                    if (row.ColId < minCol)
                        minCol = row.ColId;
                    else if (row.ColId > maxCol)
                        maxCol = row.ColId;

                    coordinateSet.Add((row.RowId, row.ColId));
                    rowSet.Add(row.RowId);
                    colSet.Add(col.ColId);
                }
            }

            var maxWidth = _maxRowDigits + _maxColDigits + 2;
            var capacity =
                (maxRow - minRow + 1)
                * (maxCol - minCol + 1)
                * maxWidth
                + (maxRow + 1) * 2;
            var rowFormat = "D" + _maxRowDigits;
            var colFormat = "D" + _maxColDigits;
            var sb = new StringBuilder(capacity);

            for (var row = minRow; row <= maxRow; row++)
            {
                if (!rowSet.Contains(row))
                    continue;

                //ugly grid
                //sb.Append(' ');
                //for (var col = minCol; col <= maxCol; col++)
                //{
                //    sb.Append(new string(' ', _maxRowDigits));
                //    sb.Append('|');
                //    sb.Append(new string(' ', _maxColDigits));
                //    sb.Append(' ');
                //}
                //sb.AppendLine();
                for (var col = minCol; col <= maxCol; col++)
                {
                    if (!colSet.Contains(col))
                        continue;

                    if (coordinateSet.Contains((row, col)))
                    {
                        sb.Append($" {row.ToString(rowFormat)},{col.ToString(colFormat)}");
                    }
                    else
                    {
                        sb.Append(new string(' ', maxWidth));
                    }
                }

                sb.AppendLine();
            }

            var result = sb.ToString();
            return result;
        }
    }
}
