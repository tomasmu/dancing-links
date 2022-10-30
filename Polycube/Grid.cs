using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Polycube
{
    public class Cuboid
    {
        public bool[,,] Grid { get; set; }
        public int XLength { private set; get; }
        public int YLength { private set; get; }
        public int ZLength { private set; get; }
        public int XMax { private set; get; }
        public int YMax { private set; get; }
        public int ZMax { private set; get; }
        public int CubieCount { private set; get; }

        public Cuboid(bool[,,] grid)
        {
            Grid = grid;

            Init();
        }

        private void Init()
        {
            (XLength, YLength, ZLength) = Grid.GetLengths();
            (XMax, YMax, ZMax) = (XLength - 1, YLength - 1, ZLength - 1);
            CubieCount = CountCubies(Grid);
            MapCoordinates(Grid);
        }

        public Cuboid(string stringLiteral)
        {
            var blockedChar = '-';
            Grid = stringLiteral.ToGrid(blockedChar);

            Init();
        }

        private int CountCubies(bool[,,] grid)
        {
            var cubieCount = 0;
            var len = grid.GetLengths();
            for (int y = 0; y < len.y; y++)
            {
                for (int x = 0; x < len.x; x++)
                {
                    for (int z = 0; z < len.z; z++)
                    {
                        if (!grid[y, x, z])
                            cubieCount++;
                    }
                }
            }

            return cubieCount;
        }

        public (int x, int y, int z) GetMapped(int index) => MapIndexToXYZ[index];
        public int GetMapped(int x, int y, int z) => MapXYZToIndex[(x, y, z)];
        public bool this[int x, int y, int z] => Grid[x, y, z];

        public Dictionary<int, (int x, int y, int z)> MapIndexToXYZ;
        public Dictionary<(int x, int y, int z), int> MapXYZToIndex;
        private void MapCoordinates(bool[,,] grid)
        {
            MapIndexToXYZ = new();
            MapXYZToIndex = new();
            var index = 0;
            for (int y = 0; y < YLength; y++)
            {
                for (int x = 0; x < XLength; x++)
                {
                    for (int z = 0; z < ZLength; z++)
                    {
                        if (grid[y, x, z])
                            continue;

                        MapIndexToXYZ[index] = (x, y, z);
                        MapXYZToIndex[(x, y, z)] = index;
                        index++;
                    }
                }
            }
        }
    }
}
