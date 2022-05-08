using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DancingLinks
{
    public class ToroidalLinkedList
    {
        public ToroidalLinkedList(bool[,] constraintMatrix)
        {
            columnRoot = CreateToroidalLinkedList(constraintMatrix);
        }

        private Node CreateToroidalLinkedList(bool[,] constraintMatrix)
        {
            //create column root
            var colRoot = new Node(NodeConstants.ROW_HEADER, NodeConstants.COL_HEADER);

            //create column header row
            var colLength = constraintMatrix.GetLength(1);
            for (var col = 0; col < colLength; col++)
            {
                var colHead = new Node(NodeConstants.ROW_HEADER, col);
                colRoot.AddHorizontalNode(colHead);
            }

            //create data node rows based on constraint matrix
            var rowLength = constraintMatrix.GetLength(0);
            for (var row = 0; row < rowLength; row++)
            {
                //pretend first existing dataNode on each row is a row header, so we can append nodes horizontally
                Node dataHead = null;
                //create data nodes rowwise
                for (var colHead = colRoot.Right; colHead != colRoot; colHead = colHead.Right)
                {
                    //create data node if constraint matrix says so
                    if (constraintMatrix[row, colHead.ColId])
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

        private Node CreateToroidalLinkedList(bool[][] constraintMatrix)
        {
            //create column root
            var colRoot = new Node(NodeConstants.ROW_HEADER, NodeConstants.COL_HEADER);

            //create column header row
            var colLength = constraintMatrix.GetLength(1);
            for (var col = 0; col < colLength; col++)
            {
                var colHead = new Node(NodeConstants.ROW_HEADER, col);
                colRoot.AddHorizontalNode(colHead);
            }

            //create data node rows based on constraint matrix
            var rowLength = constraintMatrix.GetLength(0);
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

        public void Solve(int maxSolutions)
        {
            Solutions.Clear();
            guesses.Clear();
            Search(maxSolutions);
        }

        //this is DLX, Algorithm X implemented with Dancing Links
        private void Search(int maxSolutions)
        {
            //if column list is empty we have found a solution
            if (columnRoot.Right == columnRoot)
            {
                var solution = new List<Node>(guesses);
                Solutions.Add(solution);
                return;
            }

            var colHead = GetColumnWithFewestRows();
            //if we have columns without data rows it's unsolvable
            if (colHead.Count == 0)
                return;

            //cover column
            colHead.CoverColumnAndRows();

            //loop vertical nodes
            for (var dataRow = colHead.Down; dataRow != colHead; dataRow = dataRow.Down)
            {
                //guess row is part of the solution
                guesses.Push(dataRow);

                //cover columns with intersecting rows
                for (var dataCol = dataRow.Right; dataCol != dataRow; dataCol = dataCol.Right)
                    dataCol.ColumnHeader.CoverColumnAndRows();

                //if (Solutions.Count < maxSolutions)
                //    Search(maxSolutions);
                //50% faster when searching for a single solution, 1-2% faster when searching for all
                Search(maxSolutions);
                if (Solutions.Count >= maxSolutions)
                    return;

                //backtrack current guess
                for (var dataCol = dataRow.Left; dataCol != dataRow; dataCol = dataCol.Left)
                    dataCol.ColumnHeader.UncoverColumnAndRows();

                //undo guess
                guesses.Pop();
            }

            //backtrack
            colHead.UncoverColumnAndRows();
        }

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
                if (colHead.Count < min.Count)
                    min = colHead;
            }

            return min;
        }
    }
}
