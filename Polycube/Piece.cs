using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolycubeSolver
{
    public class PieceOptions
    {
        public PieceOptions()
        {
        }

        public PieceOptions(IEnumerable<int> x, IEnumerable<int> y, IEnumerable<int> z) => (X, Y, Z) = (x, y, z);

        public IEnumerable<int> X { get; set; } = Enumerable.Range(0, 4).Select(n => 90 * n);
        public IEnumerable<int> Y { get; set; } = Enumerable.Range(0, 4).Select(n => 90 * n);
        public IEnumerable<int> Z { get; set; } = Enumerable.Range(0, 4).Select(n => 90 * n);

        public IEnumerable<Matrix> GetAllRotationMatrices()
        {
            var degrees = X.Select(x => Y.Select(y => Z.Select(z => new Vector(x, y, z))).SelectMany(v => v)).SelectMany(v => v);
            var rotations = MathRotation.GetUniqueRotationMatrices(degrees);
            return rotations;
        }
    }

    public class Piece
    {
        public char Name { get; set; }
        private const char _emptyChar = '-';
        private const char _indentChar = ' ';
        private const char _defaultPieceChar = 'Ø';
        public PieceOptions Options { get; set; } = new();

        public Piece(string piece)
        {
            var name = piece.FirstOrDefault(chr => !chr.IsEmptyOrWhiteSpace() && chr != _emptyChar);
            Name = name.IsEmptyOrWhiteSpace() ? _defaultPieceChar : name;
            Points = piece.ToPoints(_emptyChar).ToList();
        }

        public Piece(string piece, char name)
        {
            Name = name;
            Points = piece.ToPoints(_emptyChar).ToList();
        }

        public Piece(IEnumerable<Vector> points, char name)
        {
            Name = name;
            Points = points.ToList();
        }

        public List<Vector> Points { get; }

        public IEnumerable<Piece> GetRotations() => Options
            .GetAllRotationMatrices()
            .Select(rotation => Points
                .Select(point => rotation * point)
                .TranslateToOrigo()
                .OrderBy(p => p))
            .GroupBy(points => points.Select(point => point.ToString()).StringJoin(";"))
            .Select(g => g.First())
            .Select(points => new Piece(points, Name));

        public Vector GetDimension()
        {
            var (min, max) = Points.GetAxesMinMaxValues();
            return max - min + 1;
        }

        private string PointsToString(IEnumerable<Vector> points)
        {
            var pieceChar = Name;
            var dim = GetDimension();
            var pieceArray = new char[dim.Y, dim.X, dim.Z];
            foreach (var (x, y, z) in points)
                pieceArray[y, x, z] = pieceChar;

            var sb = new StringBuilder();
            for (int y = 0; y < dim.Y; y++)
            {
                var indent = new string(_indentChar, y);
                for (int x = 0; x < dim.X; x++)
                {
                    sb.Append($"{indent}[");
                    for (int z = 0; z < dim.Z; z++)
                        sb.Append(pieceArray[y, x, z] == 0 ? _emptyChar : pieceChar);

                    sb.AppendLine("]");
                }
            }

            return sb.ToString();
        }

        public override string ToString() => PointsToString(Points);
    }
}
