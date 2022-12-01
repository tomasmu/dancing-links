using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolycubeSolver
{
    //todo: options
    [Flags]
    public enum RotationOptions
    {
        None = 0,
        X    = 1 << 0,
        Y    = 1 << 1,
        Z    = 1 << 2,
        All  = X | Y | Z,
    }

    public class Piece
    {
        public char Name { get; set; }
        //todo: options
        private const char _emptyChar = '-';
        private const char _indentChar = ' ';
        private const char _defaultPieceChar = 'Ø';
        //public RotationOptions Options { get; set; }

        public Piece(string piece)
        {
            var name = piece.FirstOrDefault(chr => !chr.IsEmptyOrWhiteSpace() && chr != _emptyChar);
            Name = name.IsEmptyOrWhiteSpace() ? _defaultPieceChar : name;
            Points = piece.ToPoints(_emptyChar);
        }

        public Piece(string piece, char name)
        {
            Name = name;
            Points = piece.ToPoints(_emptyChar);
        }

        public Piece(IEnumerable<Vector> points, char name)
        {
            Name = name;
            Points = points;
        }

        public IEnumerable<Vector> Points { get; }
        
        public IEnumerable<Piece> GetRotations() =>
            Points
                .GetUniqueRotations()
                .Select(p => new Piece(p, Name));

        public Vector GetDimension()
        {
            //todo: works when min is (0,0,0), which is how i use it
            //should perhaps be max-min+1?
            var max = Points.GetAxesMaxValues();
            return max + 1;
        }

        public Piece Translate(Vector offset) =>
            new(Points.Select(p => p + offset), Name);

        private string PointsToString(IEnumerable<Vector> points)
        {
            var pieceChar = Name;
            var maxLen = points.GetAxesMaxValues() + 1;
            var pieceArray = new char[maxLen.Y, maxLen.X, maxLen.Z];
            foreach (var point in points)
            {
                var (x, y, z) = point;
                pieceArray[y, x, z] = pieceChar;
            }

            var sb = new StringBuilder();
            for (int y = 0; y < maxLen.Y; y++)
            {
                var indent = new string(_indentChar, y);
                for (int x = 0; x < maxLen.X; x++)
                {
                    sb.Append($"{indent}[");
                    for (int z = 0; z < maxLen.Z; z++)
                    {
                        var chr = pieceArray[y, x, z] == 0 ? _emptyChar : pieceChar;
                        sb.Append(chr);
                    }

                    sb.AppendLine("]");
                }
            }

            var result = sb.ToString();
            return result;
        }

        public override string ToString() => PointsToString(Points);
    }
}
