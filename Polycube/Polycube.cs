using DancingLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolycubeSolver
{
    public class Polycube
    {
        public Cuboid Cuboid { get; }
        public Piece[] Pieces { get; }
        public Piece[] Optional { get; }

        public Polycube(Cuboid cuboid, IEnumerable<Piece> pieces, IEnumerable<Piece> optional = null)
        {
            Cuboid = cuboid;
            Pieces = pieces.ToArray();
            Optional = optional?.ToArray() ?? Array.Empty<Piece>();

            _pieceCubieCount = pieces.Sum(p => p.Points.Count());
            _optionalCubieCount = optional?.Sum(p => p.Points.Count()) ?? 0;
            ThrowIfExactCoverIsImpossible();

            _pieceCount = pieces.Count();

            _constraintColumnCount = _pieceCount + Cuboid.CubieCount;

            CreateConstraintMatrix(Pieces, Optional);
        }

        public void ThrowIfExactCoverIsImpossible()
        {
            var pieceCounts = Pieces.Select(p => p.Points.Count().ToString()).StringJoin("+");
            if (_optionalCubieCount == 0)
            {
                if (Cuboid.CubieCount != _pieceCubieCount)
                {
                    throw new ArgumentException(
                        $"{Cuboid.CubieCount} grid cubies != {_pieceCubieCount} piece cubies ({pieceCounts})");
                }
            }
            else
            {
                var optionalCounts = Optional.Select(p => p.Points.Count().ToString()).StringJoin("+");
                if (Cuboid.CubieCount > _pieceCubieCount + _optionalCubieCount)
                {
                    throw new ArgumentException(
                        $"{Cuboid.CubieCount} grid cubies < {_pieceCubieCount} piece cubies ({pieceCounts})" +
                        $" + {_optionalCubieCount} optional cubies ({optionalCounts})");
                }
            }

        }

        private bool[][] _constraintMatrix;
        private List<Piece> _constraintPiecesWithOffset;
        private readonly int _pieceCubieCount;
        private readonly int _optionalCubieCount;
        private readonly int _pieceCount;
        private readonly int _constraintColumnCount;

        public void CreateConstraintMatrix(Piece[] pieces, Piece[] optional)
        {
            var (constraints, piecesWithOffset) = CreateConstraintRows(pieces, false);
            var (constraintsOptional, optionalWithOffset) = CreateConstraintRows(optional, true);

            _constraintMatrix = constraints.Concat(constraintsOptional).ToArray();
            _constraintPiecesWithOffset = piecesWithOffset.Concat(optionalWithOffset).ToList();
        }

        public (IEnumerable<bool[]> constraintRows, IEnumerable<Piece> piecesWithOffset) CreateConstraintRows(Piece[] pieces, bool isOptional)
        {
            var constraintRows = new List<bool[]>();
            var constraintPiecesWithOffset = new List<Piece>();

            for (int pieceIndex = 0; pieceIndex < pieces.Length; pieceIndex++)
            {
                var pieceRotations = pieces[pieceIndex].GetRotations();
                foreach (var rotatedPiece in pieceRotations)
                {
                    var pieceLength = rotatedPiece.GetDimension();
                    for (int yGrid = 0; yGrid <= Cuboid.Length.Y - pieceLength.Y; yGrid++)
                    {
                        for (int xGrid = 0; xGrid <= Cuboid.Length.X - pieceLength.X; xGrid++)
                        {
                            for (int zGrid = 0; zGrid <= Cuboid.Length.Z - pieceLength.Z; zGrid++)
                            {
                                var fromPoint = new Vector(xGrid, yGrid, zGrid);
                                var pieceWithOffset = rotatedPiece.Translate(fromPoint);
                                var constraintRow = CreateConstraintRow(pieceIndex, pieceWithOffset, isOptional);
                                if (constraintRow != null)
                                {
                                    constraintRows.Add(constraintRow);
                                    constraintPiecesWithOffset.Add(pieceWithOffset);
                                }
                            }
                        }
                    }
                }
            }

            return (constraintRows, constraintPiecesWithOffset);
        }

        private bool[] CreateConstraintRow(int pieceIndex, Piece piece, bool isOptional = false)
        {
            var constraintRow = new bool[_constraintColumnCount];
            constraintRow[pieceIndex] = !isOptional;
            foreach (var point in piece.Points)
            {
                if (Cuboid[point.Y, point.X, point.Z])
                    return null;

                var constraintColumn = Cuboid.GetMapped(point);
                constraintRow[_pieceCount + constraintColumn] = true;
            }

            return constraintRow;
        }

        public List<char[,,]> Solutions { get; set; }
        public IEnumerable<string> SolutionsAsStrings => Solutions
            .Select(grid => GridToString(grid));

        public void Solve(int maxSolutions)
        {
            var toroidalLinkedList = new ToroidalLinkedList(_constraintMatrix);
            toroidalLinkedList.Solve(maxSolutions);
            Solutions = toroidalLinkedList
                .Solutions
                .Select(ParseNodeListSolution)
                .ToList();
        }

        public char[,,] ParseNodeListSolution(int[] rowIds)
        {
            var blockedChar = '-';
            
            var grid = new char[Cuboid.Length.Y, Cuboid.Length.X, Cuboid.Length.Z];
            foreach (var (x, y, z) in Cuboid.BlockedPoints)
                grid[y, x, z] = blockedChar;

            var solutionPiecesWithOffset = rowIds.Select(rowId => _constraintPiecesWithOffset[rowId]).ToList();
            foreach (var piece in solutionPiecesWithOffset)
                foreach (var (x, y, z) in piece.Points)
                    grid[y, x, z] = piece.Name;

            return grid;
        }

        public string GridToString(char[,,] grid)
        {
            var sb = new StringBuilder();
            for (int y = 0; y < Cuboid.Length.Y; y++)
            {
                var indent = new string(' ', y);
                for (int x = 0; x < Cuboid.Length.X; x++)
                {
                    sb.Append($"{indent}[");
                    for (int z = 0; z < Cuboid.Length.Z; z++)
                        sb.Append(grid[y, x, z]);

                    sb.AppendLine("]");
                }
            }

            var result = sb.ToString();
            return result;
        }
    }
}
