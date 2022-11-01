using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Polycube
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

    //todo: make it work for both 2D and 3D
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
            Points.GetUniqueRotations().Select(p => new Piece(p, Name));

        public Vector GetDimension()
        {
            //todo: works when min is (0,0,0), which is how i use it
            //should perhaps be max-min?
            var max = Points.GetAxesMaxValues();
            return max + 1;
        }

        private string PointsToString(IEnumerable<Vector> points)
        {
            var pieceChar = Name;
            var max = points.GetAxesMaxValues();
            var (xLen, yLen, zLen) = (max[0] + 1, max[1] + 1, max[2] + 1);
            var pieceArray = new char[yLen, xLen, zLen];
            foreach (var point in points)
            {
                var (x, y, z) = (point[0, 0], point[1, 0], point[2, 0]);
                pieceArray[y, x, z] = pieceChar;
            }

            var sb = new StringBuilder();
            for (int y = 0; y < yLen; y++)
            {
                var indent = new string(_indentChar, y);
                for (int x = 0; x < xLen; x++)
                {
                    sb.Append($"{indent}[");
                    for (int z = 0; z < zLen; z++)
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
