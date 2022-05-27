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
            _maxRowDigits = _rows.NumberOfDigits();
            _maxColDigits = _cols.NumberOfDigits();
            _columnRoot = CreateToroidalLinkedList(_constraintMatrix);
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

        private readonly Stack<Node> _guesses = new Stack<Node>();
        public List<List<Node>> Solutions { get; set; } = new List<List<Node>>();
        private readonly Node _columnRoot;
        private int _maxSolutions;
        private Stopwatch _stopwatch = new Stopwatch();
        public List<List<Node>> AllGuesses { get; set; } = new List<List<Node>>();

        public void Solve(int maxSolutions)
        {
            _maxSolutions = maxSolutions;
            Solutions.Clear();
            _guesses.Clear();
            AllGuesses.Clear();
            _stopwatch.Restart();
#if STATS && DEBUG
            Stats.Counters.Clear();
#endif
            Search();
#if STATS && DEBUG
            //don't forget to inspect the stats
            Debugger.Break();
#endif
        }

        //this is DLX, Algorithm X implemented with Dancing Links
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Search()
        {
#if DEBUG
            //var hest = 1337;
#endif
#if STATS && DEBUG
            Stats.IncrementCounter(nameof(Search));
#endif
#if ASCII && DEBUG
            var toroidalAsciiArt = ToStringThing();
#endif
#if ALL_GUESSES && DEBUG
            AllGuesses.Add(new List<Node>(_guesses));
#endif

            //if column list is empty we have found a solution
            if (_columnRoot.Right == _columnRoot)
            {
                //todo: _guesses can be reduced to row ids, benchmark
                var solution = new List<Node>(_guesses);
                Solutions.Add(solution);

                return;
            }

            var colHead = GetColumnWithFewestRows();
            //if we have columns without data rows it's unsolvable
            if (colHead.Count == 0)
            {
#if STATS && DEBUG
                Stats.IncrementCounter("unsolvable");
#endif
                return;
            }

            //cover column
            colHead.CoverColumnAndRows();

            //loop vertical nodes
            for (var dataRow = colHead.Down; dataRow != colHead; dataRow = dataRow.Down)
            {
#if STATS && DEBUG
                Stats.IncrementCounter(nameof(_guesses));
#endif
                //guess row is part of the solution
                _guesses.Push(dataRow);

                //cover columns with intersecting rows
                for (var dataCol = dataRow.Right; dataCol != dataRow; dataCol = dataCol.Right)
                {
#if STATS && DEBUG
                    Stats.IncrementCounter("dataCol cover");
#endif
                    dataCol.ColumnHeader.CoverColumnAndRows();
                }

                //todo: redo benchmark, performance for 1-N vs all solutions
                //conditional search vs conditional return
                Search();
#if TESTKOSSA || true
                if (Solutions.Count >= _maxSolutions)
                    return;
#endif
                //backtrack current guess
                for (var dataCol = dataRow.Left; dataCol != dataRow; dataCol = dataCol.Left)
                {
                    dataCol.ColumnHeader.UncoverColumnAndRows();
                }

                //undo guess
                _guesses.Pop();
            }

            //backtrack
            colHead.UncoverColumnAndRows();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Node GetColumnWithFewestRows()
        {
            var min = _columnRoot.Right;
            for (var colHead = min.Right; min.Count > 0 && colHead != _columnRoot; colHead = colHead.Right)
            {
                //todo: benchmark traversal count without .Count
#if STATS && DEBUG
                Stats.IncrementCounter("min.Count");
#endif
                if (colHead.Count < min.Count)
                    min = colHead;
            }

            return min;
        }

        private string ToStringThing()
        {
            if (_columnRoot == _columnRoot.Right)
                return string.Empty;

            var coordinateSet = new HashSet<(int row, int col)>();
            var rowSet = new HashSet<int>();
            var colSet = new HashSet<int>();

            var minRow = _rows;
            var maxRow = 0;
            var minCol = _columnRoot.Left.ColId;
            var maxCol = 0;
            for (var col = _columnRoot.Right; col != _columnRoot; col = col.Right)
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
