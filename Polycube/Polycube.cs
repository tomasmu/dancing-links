using DancingLinks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polycube
{
    public class Polycube
    {
        public Cuboid Cuboid { get; }
        public Piece[] Pieces { get; }

        public Polycube(Cuboid cuboid, IEnumerable<Piece> pieces)
        {
            Cuboid = cuboid;
            Pieces = pieces.ToArray();

            _pieceCubieCount = pieces.Sum(p => p.Points.Count());
            ThrowIfExactCoverIsImpossible();

            _pieceCount = pieces.Count();

            _constraintColumnCount = _pieceCount + Cuboid.CubieCount;

            CreateConstraintMatrix(pieces);
        }

        public void ThrowIfExactCoverIsImpossible()
        {
            if (Cuboid.CubieCount != _pieceCubieCount)
            {
                var pieceCounts = Pieces.Select(p => p.Points.Count().ToString()).StringJoin("+");
                throw new ArgumentException($"{Cuboid.CubieCount} grid cubies != {_pieceCubieCount} piece cubies ({pieceCounts})");
            }
        }

        private bool[][] _constraintMatrix;
        private List<Piece> _constraintPieces;
        private List<(int x, int y, int z)> _constraintOffsets;
        private readonly int _pieceCubieCount;
        private readonly int _pieceCount;
        private readonly int _constraintColumnCount;

        public void CreateConstraintMatrix(IEnumerable<Piece> pieces)
        {
            var constraintRows = new List<bool[]>();
            var constraintPieces = new List<Piece>();
            var constraintOffsets = new List<(int x, int y, int z)>();

            for (int pieceIndex = 0; pieceIndex < pieces.Count(); pieceIndex++)
            {
                var pieceRotations = Pieces[pieceIndex].GetRotations();
                foreach (var rotatedPiece in pieceRotations)
                {
                    var pieceLen = rotatedPiece.GetDimension();
                    for (int yGrid = 0; yGrid <= Cuboid.Length.Y - pieceLen.Y; yGrid++)
                    {
                        for (int xGrid = 0; xGrid <= Cuboid.Length.X - pieceLen.X; xGrid++)
                        {
                            for (int zGrid = 0; zGrid <= Cuboid.Length.Z - pieceLen.Z; zGrid++)
                            {
                                var gridPoint = (xGrid, yGrid, zGrid);
                                var constraintRow = CreateConstraintRow(pieceIndex, rotatedPiece, gridPoint);
                                if (constraintRow != null)
                                {
                                    constraintRows.Add(constraintRow);
                                    constraintPieces.Add(rotatedPiece);
                                    constraintOffsets.Add((xGrid, yGrid, zGrid));
                                }
                            }
                        }
                    }
                }
            }

            _constraintMatrix = constraintRows.ToArray();
            _constraintPieces = constraintPieces;
            _constraintOffsets = constraintOffsets;
        }

        private bool[] CreateConstraintRow(int pieceIndex, Piece piece, (int x, int y, int z) grid)
        {
            //todo: in order to handle blocked cubies in the grid
            //i need to create an index map of some sort i think?
            //if (0,0,1) is blocked, then 0,1,2,... = (0,0,0),(0,0,2),(0,0,3),...
            var constraintRow = new bool[_constraintColumnCount];
            constraintRow[pieceIndex] = true;
            foreach (var point in piece.Points)
            {
                var (x, y, z) = (grid.x + point[0, 0], grid.y + point[1, 0], grid.z + point[2, 0]);
                if (Cuboid[y, x, z])
                    return null;

                var constraintColumn = Cuboid.GetMapped(x, y, z);
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
            //todo: is there a better way?
            var solutionRows = rowIds.Select(rowId => _constraintMatrix[rowId]).ToList();
            var solutionPieces = rowIds.Select(rowId => _constraintPieces[rowId]).ToList();
            var solutionOffsets = rowIds.Select(rowId => _constraintOffsets[rowId]).ToList();

            var grid = new char[Cuboid.Length.Y, Cuboid.Length.X, Cuboid.Length.Z];
            var blockedChar = '-';
            for (int y = 0; y < Cuboid.Length.Y; y++)
                for (int x = 0; x < Cuboid.Length.X; x++)
                    for (int z = 0; z < Cuboid.Length.Z; z++)
                        if (Cuboid.Grid[y, x, z])
                            grid[y, x, z] = blockedChar;
            
            for (int i = 0; i < solutionPieces.Count; i++)
            {
                foreach (var point in solutionPieces[i].Points)
                {
                    var (dx, dy, dz) = solutionOffsets[i];
                    var (x, y, z) = (point[0, 0] + dx, point[1, 0] + dy, point[2, 0] + dz);
                    grid[y, x, z] = solutionPieces[i].Name;
                }
            }

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
                    {
                        sb.Append(grid[y, x, z]);
                    }

                    sb.AppendLine("]");
                }
            }

            var result = sb.ToString();
            return result;
        }
    }
}
