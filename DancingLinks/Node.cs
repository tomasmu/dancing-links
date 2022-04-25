using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DancingLinks
{
    //generic node that acts as columnRoot, columnHeader, and dataNodes
    //used for doubly linked list that is circular in two dimensions (up/down and left/right)
    //should perhaps be split into Root, Header, Data; inheriting from some Node

    //random thoughts:
    //class BaseNode {
    //    //left, right
    //}
    //class RootNode : BaseNode
    //
    //    //keep track of column counts?
    //}
    //class HeaderNode : BaseNode
    //{
    //    //up, down
    //    //count   //todo: who's counting?
    //}
    //class DataNode : BaseNode
    //{
    //    //up, down
    //    //T data;
    //    //rowId, colId or data?
    //    //cover header?
    //}

    public static class NodeConstants
    {
        //todo: remove
        public static int ROW_HEADER = -1;
        public static int COL_HEADER = -1;
    }

    public class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Up { get; set; }
        public Node Down { get; set; }

        public Node ColumnHeader { get; set; }

        public int RowId { get; set; }
        public int ColId { get; set; }
        public int Count { get; set; }

        public Node(int row, int col)
        {
            Left = this;
            Right = this;
            Up = this;
            Down = this;

            RowId = row;
            ColId = col;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]    //todo: benchmark
        public void AddHorizontalNode(Node node)
        {
            node.Right = this;
            node.Left = this.Left;
            this.Left.Right = node;
            this.Left = node;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVerticalDataNode(Node node)
        {
            node.Down = this;
            node.Up = this.Up;
            this.Up.Down = node;
            this.Up = node;

            node.ColumnHeader = this;
            Count++;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CoverColumnAndRows()
        {
            CoverHorizontalNode();
            for (var data = this.Down; data != this; data = data.Down)
                data.CoverRow();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UncoverColumnAndRows()
        {
            for (var data = this.Up; data != this; data = data.Up)
                data.UncoverRow();
            UncoverHorizontalNode();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoverHorizontalNode()
        {
            this.Right.Left = this.Left;
            this.Left.Right = this.Right;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UncoverHorizontalNode()
        {
            this.Right.Left = this;
            this.Left.Right = this;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoverVerticalNode()
        {
            this.Down.Up = this.Up;
            this.Up.Down = this.Down;

            ColumnHeader.Count--;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UncoverVerticalNode()
        {
            this.Down.Up = this;
            this.Up.Down = this;

            ColumnHeader.Count++;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoverRow()
        {
            for (var data = this.Right; data != this; data = data.Right)
                data.CoverVerticalNode();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UncoverRow()
        {
            for (var data = this.Left; data != this; data = data.Left)
                data.UncoverVerticalNode();
        }
    }
}
