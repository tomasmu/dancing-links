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
        //todo: class Grid
        public Polycube(bool[,,] grid, IEnumerable<Piece> pieces)
        {
            Grid = grid;
            Pieces = pieces.ToArray();

            _gridCubieCount = CountGridCubies(grid);
            _pieceCubieCount = pieces.Sum(p => p.Points.Count());
            ThrowIfExactCoverIsImpossible();

            _pieceCount = pieces.Count();

            _constraintColumnCount = _pieceCount + _gridCubieCount;

            CreateConstraintMatrix(pieces);
        }

        public void ThrowIfExactCoverIsImpossible()
        {
            if (_gridCubieCount != _pieceCubieCount)
            {
                var pieceCounts = Pieces.Select(p => p.Points.Count().ToString()).StringJoin("+");
                throw new ArgumentException($"{_gridCubieCount} grid cubies != {_pieceCubieCount} piece cubies ({pieceCounts})");
            }
        }

        private bool[][] _constraintMatrix;
        private List<Piece> _constraintPieces;
        private List<(int x, int y, int z)> _constraintOffsets;
        private readonly Piece[][] _allPieceRotations;
        private readonly int _gridCubieCount;
        private readonly int _pieceCubieCount;
        private readonly int _pieceCount;
        private readonly int _constraintColumnCount;

        public int CountGridCubies(bool[,,] grid)
        {
            var gridCubies = 0;
            var len = grid.GetLengths();
            for (int y = 0; y < len.y; y++)
            {
                for (int x = 0; x < len.x; x++)
                {
                    for (int z = 0; z < len.z; z++)
                    {
                        if (!grid[y, x, z])
                            gridCubies++;
                    }
                }
            }

            return gridCubies;
        }

        public void CreateConstraintMatrix(IEnumerable<Piece> pieces)
        {
            var constraintRows = new List<bool[]>();
            var constraintPieces = new List<Piece>();
            var constraintOffsets = new List<(int x, int y, int z)>();

            var gridLen = Grid.GetLengths();
            for (int pieceIndex = 0; pieceIndex < pieces.Count(); pieceIndex++)
            {
                var pieceRotations = Pieces[pieceIndex].GetRotations();
                foreach (var rotatedPiece in pieceRotations)
                {
                    var pieceLen = rotatedPiece.GetDimension();
                    for (int yGrid = 0; yGrid <= gridLen.y - pieceLen.y; yGrid++)
                    {
                        for (int xGrid = 0; xGrid <= gridLen.x - pieceLen.x; xGrid++)
                        {
                            for (int zGrid = 0; zGrid <= gridLen.z - pieceLen.z; zGrid++)
                            {
                                var gridPoint = (xGrid, yGrid, zGrid);
                                var constraintRow = CreateConstraintRow(pieceIndex, rotatedPiece, gridPoint, gridLen);
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

        private bool[] CreateConstraintRow(int pieceIndex, Piece piece, (int x, int y, int z) grid, (int x, int y, int z) gridLen)
        {
            //todo: in order to handle blocked cubies in the grid
            //i need to create an index map of some sort i think?
            //if (0,0,1) is blocked, then 0,1,2,... = (0,0,0),(0,0,2),(0,0,3),...
            var constraintRow = new bool[_constraintColumnCount];
            constraintRow[pieceIndex] = true;
            foreach (var point in piece.Points)
            {
                var (x, y, z) = (grid.x + point[0, 0], grid.y + point[1, 0], grid.z + point[2, 0]);
                if (Grid[y, x, z])
                    return null;

                var constraintColumn = (y * gridLen.x * gridLen.z) + (x * gridLen.z) + z;
                constraintRow[_pieceCount + constraintColumn] = true;
            }

            return constraintRow;
        }

        public List<char[,,]> Solutions { get; set; }

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

            var (xLen, yLen, zLen) = Grid.GetLengths();
            var grid = new char[yLen, xLen, zLen];
            for (int i = 0; i < solutionPieces.Count; i++)
            {
                foreach (var point in solutionPieces[i].Points)
                {
                    var (dx, dy, dz) = solutionOffsets[i];
                    var (x, y, z) = (point[0, 0] + dx, point[1, 0] + dy, point[2, 0] + dz);
                    grid[y, x, z] = solutionPieces[i].Name;
                }
            }

            var hest = GridToString(grid);
            Debug.WriteLine(hest);
            return grid;
        }

        private string GridToString(char[,,] grid)
        {
            var sb = new StringBuilder();
            var (xLen, yLen, zLen) = Grid.GetLengths();
            for (int y = 0; y < yLen; y++)
            {
                var indent = new string(' ', y);
                for (int x = 0; x < xLen; x++)
                {
                    sb.Append($"{indent}[");
                    for (int z = 0; z < zLen; z++)
                    {
                        sb.Append(grid[y, x, z]);
                    }

                    sb.AppendLine("]");
                }
            }

            var result = sb.ToString();
            return result;
        }

        public bool[,,] Grid { get; }
        public Piece[] Pieces { get; }
    }
}
