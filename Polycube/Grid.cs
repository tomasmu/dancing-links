using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PolycubeSolver
{
    public class Cuboid
    {
        public bool[,,] Grid { get; set; }
        public Vector Length { private set; get; }
        public int CubieCount { private set; get; }

        public Cuboid(bool[,,] grid)
        {
            Grid = grid;

            Init();
        }

        public Cuboid(string grid)
        {
            var blockedChar = '-';
            Grid = ToGrid(grid, blockedChar);

            Init();
        }

        private void Init()
        {
            Length = GetLengths();
            CubieCount = CountCubies();
            MapPoints(Grid);
        }

        private Vector GetLengths() =>
            new(Grid.GetLength(1), Grid.GetLength(0), Grid.GetLength(2));

        private int CountCubies()
        {
            var cubieCount = 0;
            for (int y = 0; y < Length.Y; y++)
            {
                for (int x = 0; x < Length.X; x++)
                {
                    for (int z = 0; z < Length.Z; z++)
                    {
                        if (!Grid[y, x, z])
                            cubieCount++;
                    }
                }
            }

            return cubieCount;
        }

        public int GetMapped(Vector point) => MapPointToIndex[point];
        public bool this[int x, int y, int z] => Grid[x, y, z];

        public Dictionary<Vector, int> MapPointToIndex;
        public List<Vector> BlockedPoints;
        private void MapPoints(bool[,,] grid)
        {
            MapPointToIndex = new();
            BlockedPoints = new();
            var index = 0;
            for (int y = 0; y < Length.Y; y++)
            {
                for (int x = 0; x < Length.X; x++)
                {
                    for (int z = 0; z < Length.Z; z++)
                    {
                        var coordinate = new Vector(x, y, z);
                        //if (0,0,1) is blocked, then 0,1,2,... maps to (0,0,0),(0,0,2),(0,0,3),...
                        if (grid[y, x, z])
                        {
                            BlockedPoints.Add(coordinate);
                        }
                        else
                        {
                            MapPointToIndex[coordinate] = index;
                            index++;
                        }
                    }
                }
            }
        }

        public static bool[,,] ToGrid(string gridString, char blockedChar)
        {
            var doubleNewLine = @"(\r\n){2}|\r{2}|\n{2}";
            var singleNewLine = @"(\r\n){1}|\r{1}|\n{1}";

            var gridArray = gridString
                .Trim()
                .RegexSplit(doubleNewLine, RegexOptions.ExplicitCapture)
                .Select(y => y
                    .RegexSplit(singleNewLine, RegexOptions.ExplicitCapture)
                        .Select(x => x.ToArray())
                    .ToArray())
                .ToArray();

            var yLength = gridArray.Length;
            var xLength = gridArray.Max(y => y.Length);
            var zLength = gridArray.Max(y => y.Max(x => x.Length));

            var grid = new bool[yLength, xLength, zLength];
            for (int y = 0; y < gridArray.Length; y++)
            {
                for (int x = 0; x < gridArray[y].Length; x++)
                {
                    for (int z = 0; z < gridArray[y][x].Length; z++)
                    {
                        if (gridArray[y][x][z] == blockedChar)
                            grid[y, x, z] = true;
                    }
                }
            }

            return grid;
        }
    }
}
