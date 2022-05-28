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
    //    //rowId as the only data?
    //    //cover header?
    //}

    public static class NodeConstants
    {
        public static int ROW_HEADER = -1;
        public static int COL_HEADER = -1;
    }

    public class Node
    {
        public Node Left;
        public Node Right;
        public Node Up;
        public Node Down;

        public Node ColumnHeader;
#if STATS && DEBUG
        private int _count;
        public int Count
        {
            get
            {
                Stats.IncrementCounter("get_Count");
                return _count;
            }
            set
            {
                Stats.IncrementCounter("set_Count");
                _count = value;
            }
        }
#else
        public int Count;
#endif

        public int RowId;
        public int ColId;

        public Node()
        {
            Left = this;
            Right = this;
            Up = this;
            Down = this;
        }

        public Node(int row, int col) : this()
        {
            RowId = row;
            ColId = col;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddHorizontalNode(Node node)
        {
            node.Right = this;
            node.Left = this.Left;
            this.Left.Right = node;
            this.Left = node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVerticalDataNode(Node node)
        {
            node.Down = this;
            node.Up = this.Up;
            this.Up.Down = node;
            this.Up = node;

            node.ColumnHeader = this;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CoverColumnAndRows()
        {
            CoverHorizontalNode();
            for (var data = this.Down; data != this; data = data.Down)
                data.CoverRow();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UncoverColumnAndRows()
        {
            for (var data = this.Up; data != this; data = data.Up)
                data.UncoverRow();
            UncoverHorizontalNode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoverHorizontalNode()
        {
            this.Right.Left = this.Left;
            this.Left.Right = this.Right;

#if STATS && false
            Stats.IncrementCounter(nameof(CoverHorizontalNode));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UncoverHorizontalNode()
        {
            this.Right.Left = this;
            this.Left.Right = this;

#if STATS && false
            Stats.IncrementCounter(nameof(UncoverHorizontalNode));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoverVerticalNode()
        {
            this.Down.Up = this.Up;
            this.Up.Down = this.Down;

            ColumnHeader.Count--;

#if STATS && false
            Stats.IncrementCounter(nameof(CoverVerticalNode));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UncoverVerticalNode()
        {
            this.Down.Up = this;
            this.Up.Down = this;

            ColumnHeader.Count++;

#if STATS && false
            Stats.IncrementCounter(nameof(UncoverVerticalNode));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CoverRow()
        {
            for (var data = this.Right; data != this; data = data.Right)
                data.CoverVerticalNode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UncoverRow()
        {
            for (var data = this.Left; data != this; data = data.Left)
                data.UncoverVerticalNode();
        }
    }
}
