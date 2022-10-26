using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Environment;

namespace Pentominoes.Tests
{
    public class RotationTests
    {
        //trying some 3D rotation stuff even though it doesn't belong in this project
        //this project will probably be scrapped since it's only 2D
        [Fact]
        public void RotationMatrices()
        {
            var rotationDegrees = new List<int> { 0, 90, 180, 270 };
            var rotationMatrices = new Dictionary<string, List<(int, int, int)>>();
            foreach (var x in rotationDegrees)
            {
                foreach (var y in rotationDegrees)
                {
                    foreach (var z in rotationDegrees)
                    {
                        var degrees = (x, y, z);
                        var temp = ExtensionMethods
                            .GetRotationMatrix(degrees)
                            .ToStringThing(", ");

                        var matrix = $"[[{temp.Split("\r\n").StringJoin("], [")}]]";
                        if (!rotationMatrices.ContainsKey(matrix))
                            rotationMatrices[matrix] = new List<(int, int, int)>();

                        rotationMatrices[matrix].Add((x, y, z));
                    }
                }
            }

            //taken from my python project, assuming it's correct :-)
            var all24UniqueRotationMatricesSorted = @"
[[-1, 0, 0], [0, -1, 0], [0, 0, 1]]
[[-1, 0, 0], [0, 0, -1], [0, -1, 0]]
[[-1, 0, 0], [0, 0, 1], [0, 1, 0]]
[[-1, 0, 0], [0, 1, 0], [0, 0, -1]]
[[0, -1, 0], [-1, 0, 0], [0, 0, -1]]
[[0, -1, 0], [0, 0, -1], [1, 0, 0]]
[[0, -1, 0], [0, 0, 1], [-1, 0, 0]]
[[0, -1, 0], [1, 0, 0], [0, 0, 1]]
[[0, 0, -1], [-1, 0, 0], [0, 1, 0]]
[[0, 0, -1], [0, -1, 0], [-1, 0, 0]]
[[0, 0, -1], [0, 1, 0], [1, 0, 0]]
[[0, 0, -1], [1, 0, 0], [0, -1, 0]]
[[0, 0, 1], [-1, 0, 0], [0, -1, 0]]
[[0, 0, 1], [0, -1, 0], [1, 0, 0]]
[[0, 0, 1], [0, 1, 0], [-1, 0, 0]]
[[0, 0, 1], [1, 0, 0], [0, 1, 0]]
[[0, 1, 0], [-1, 0, 0], [0, 0, 1]]
[[0, 1, 0], [0, 0, -1], [-1, 0, 0]]
[[0, 1, 0], [0, 0, 1], [1, 0, 0]]
[[0, 1, 0], [1, 0, 0], [0, 0, -1]]
[[1, 0, 0], [0, -1, 0], [0, 0, -1]]
[[1, 0, 0], [0, 0, -1], [0, 1, 0]]
[[1, 0, 0], [0, 0, 1], [0, -1, 0]]
[[1, 0, 0], [0, 1, 0], [0, 0, 1]]";

            var rotationsString = Environment.NewLine +
                rotationMatrices
                    .Keys
                    .OrderBy(key => key)
                    .Select(str => str)
                    .StringJoin(Environment.NewLine);

            Assert.Equal(all24UniqueRotationMatricesSorted, rotationsString);

            /*
            all 24 unique rotation matrices and which (x,y,z) degree rotations create them
            (0,0,0),(180,180,180)                             [ 1,  0,  0]
                                                              [ 0,  1,  0]
                                                              [ 0,  0,  1]

            (0,0,90),(180,180,270)                            [ 0, -1,  0]
                                                              [ 1,  0,  0]
                                                              [ 0,  0,  1]

            (0,0,180),(180,180,0)                             [-1,  0,  0]
                                                              [ 0, -1,  0]
                                                              [ 0,  0,  1]

            (0,0,270),(180,180,90)                            [ 0,  1,  0]
                                                              [-1,  0,  0]
                                                              [ 0,  0,  1]

            (0,90,0),(90,90,90),(180,90,180),(270,90,270)     [ 0,  0,  1]
                                                              [ 0,  1,  0]
                                                              [-1,  0,  0]

            (0,90,90),(90,90,180),(180,90,270),(270,90,0)     [ 0, -1,  0]
                                                              [ 0,  0,  1]
                                                              [-1,  0,  0]

            (0,90,180),(90,90,270),(180,90,0),(270,90,90)     [ 0,  0, -1]
                                                              [ 0, -1,  0]
                                                              [-1,  0,  0]

            (0,90,270),(90,90,0),(180,90,90),(270,90,180)     [ 0,  1,  0]
                                                              [ 0,  0, -1]
                                                              [-1,  0,  0]

            (0,180,0),(180,0,180)                             [-1,  0,  0]
                                                              [ 0,  1,  0]
                                                              [ 0,  0, -1]

            (0,180,90),(180,0,270)                            [ 0, -1,  0]
                                                              [-1,  0,  0]
                                                              [ 0,  0, -1]

            (0,180,180),(180,0,0)                             [ 1,  0,  0]
                                                              [ 0, -1,  0]
                                                              [ 0,  0, -1]

            (0,180,270),(180,0,90)                            [ 0,  1,  0]
                                                              [ 1,  0,  0]
                                                              [ 0,  0, -1]

            (0,270,0),(90,270,270),(180,270,180),(270,270,90) [ 0,  0, -1]
                                                              [ 0,  1,  0]
                                                              [ 1,  0,  0]

            (0,270,90),(90,270,0),(180,270,270),(270,270,180) [ 0, -1,  0]
                                                              [ 0,  0, -1]
                                                              [ 1,  0,  0]

            (0,270,180),(90,270,90),(180,270,0),(270,270,270) [ 0,  0,  1]
                                                              [ 0, -1,  0]
                                                              [ 1,  0,  0]

            (0,270,270),(90,270,180),(180,270,90),(270,270,0) [ 0,  1,  0]
                                                              [ 0,  0,  1]
                                                              [ 1,  0,  0]

            (90,0,0),(270,180,180)                            [ 1,  0,  0]
                                                              [ 0,  0, -1]
                                                              [ 0,  1,  0]

            (90,0,90),(270,180,270)                           [ 0,  0,  1]
                                                              [ 1,  0,  0]
                                                              [ 0,  1,  0]

            (90,0,180),(270,180,0)                            [-1,  0,  0]
                                                              [ 0,  0,  1]
                                                              [ 0,  1,  0]

            (90,0,270),(270,180,90)                           [ 0,  0, -1]
                                                              [-1,  0,  0]
                                                              [ 0,  1,  0]

            (90,180,0),(270,0,180)                            [-1,  0,  0]
                                                              [ 0,  0, -1]
                                                              [ 0, -1,  0]

            (90,180,90),(270,0,270)                           [ 0,  0,  1]
                                                              [-1,  0,  0]
                                                              [ 0, -1,  0]

            (90,180,180),(270,0,0)                            [ 1,  0,  0]
                                                              [ 0,  0,  1]
                                                              [ 0, -1,  0]

            (90,180,270),(270,0,90)                           [ 0,  0, -1]
                                                              [ 1,  0,  0]
                                                              [ 0, -1,  0]
            */
        }

        [Fact]
        public void RotatePoint()
        {
            var degreeRotations = new List<int> { 0, 90, 180, 270 };
            var point = (2, 3, 4);
            var rotatedPoints = new Dictionary<(int, int, int), List<(int, int, int)>>();
            foreach (var x in degreeRotations)
            {
                foreach (var y in degreeRotations)
                {
                    foreach (var z in degreeRotations)
                    {
                        var degrees = (x, y, z);
                        var point2 = ExtensionMethods.RotatePoint(point, degrees);

                        if (!rotatedPoints.ContainsKey(point2))
                            rotatedPoints[point2] = new List<(int, int, int)>();

                        rotatedPoints[point2].Add(degrees);
                    }
                }
            }

            var result = rotatedPoints
                .OrderBy(kv => kv.Value.First())
                .Select(kv => {
                    var key = kv.Key
                        .ToString()
                        .Split(new char[] { '(', ')', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .StringJoin("], [");
                    var value = kv.Value.StringJoin(", ");
                    var assertFormat = $"[[{key}]] : [{value}]";

                    return assertFormat;
                })
                .StringJoin(Environment.NewLine);

            //taken from my python project, assuming it's correct :-)
            var all24PointRotations = @"
[[2], [3], [4]] : [(0, 0, 0), (180, 180, 180)]
[[-3], [2], [4]] : [(0, 0, 90), (180, 180, 270)]
[[-2], [-3], [4]] : [(0, 0, 180), (180, 180, 0)]
[[3], [-2], [4]] : [(0, 0, 270), (180, 180, 90)]
[[4], [3], [-2]] : [(0, 90, 0), (90, 90, 90), (180, 90, 180), (270, 90, 270)]
[[-3], [4], [-2]] : [(0, 90, 90), (90, 90, 180), (180, 90, 270), (270, 90, 0)]
[[-4], [-3], [-2]] : [(0, 90, 180), (90, 90, 270), (180, 90, 0), (270, 90, 90)]
[[3], [-4], [-2]] : [(0, 90, 270), (90, 90, 0), (180, 90, 90), (270, 90, 180)]
[[-2], [3], [-4]] : [(0, 180, 0), (180, 0, 180)]
[[-3], [-2], [-4]] : [(0, 180, 90), (180, 0, 270)]
[[2], [-3], [-4]] : [(0, 180, 180), (180, 0, 0)]
[[3], [2], [-4]] : [(0, 180, 270), (180, 0, 90)]
[[-4], [3], [2]] : [(0, 270, 0), (90, 270, 270), (180, 270, 180), (270, 270, 90)]
[[-3], [-4], [2]] : [(0, 270, 90), (90, 270, 0), (180, 270, 270), (270, 270, 180)]
[[4], [-3], [2]] : [(0, 270, 180), (90, 270, 90), (180, 270, 0), (270, 270, 270)]
[[3], [4], [2]] : [(0, 270, 270), (90, 270, 180), (180, 270, 90), (270, 270, 0)]
[[2], [-4], [3]] : [(90, 0, 0), (270, 180, 180)]
[[4], [2], [3]] : [(90, 0, 90), (270, 180, 270)]
[[-2], [4], [3]] : [(90, 0, 180), (270, 180, 0)]
[[-4], [-2], [3]] : [(90, 0, 270), (270, 180, 90)]
[[-2], [-4], [-3]] : [(90, 180, 0), (270, 0, 180)]
[[4], [-2], [-3]] : [(90, 180, 90), (270, 0, 270)]
[[2], [4], [-3]] : [(90, 180, 180), (270, 0, 0)]
[[-4], [2], [-3]] : [(90, 180, 270), (270, 0, 90)]
".Trim();

            Assert.Equal(all24PointRotations, result);
        }

        [Fact]
        public void Rotate3dPiece()
        {
            /*  one piece taking up 4 width/x, 3 depth/z, 2 height/y
            y=0
                |----       empty row
                |XXXX       T-shape in the bottom layer of the xz plane
                |-X--
               z|_____
                 x
            y=1
                |-X--
                |-X--       I-shape in the second layer, half of it on top of the "T"-piece
                |----       empty row
               z|_____
                 x
            */

            var piece = @"
-
XXXX
-X

-X
-X
".ToCuboidMatrix('-');

            var uniquePieceRotations = new Dictionary<string, List<(int, int, int)>>();
            var allPieceRotations = new Dictionary<(int, int, int), string>();
            var rotations = new List<int>() { 0, 90, 180, 270 };
            foreach (var x in rotations)
            {
                foreach (var y in rotations)
                {
                    foreach (var z in rotations)
                    {
                        var degrees = (x, y, z);
                        var rotatedPiece = piece.Rotate(degrees);

                        var rotatedPieceString = rotatedPiece.ToStringThing();

                        if (!uniquePieceRotations.ContainsKey(rotatedPieceString))
                            uniquePieceRotations[rotatedPieceString] = new List<(int, int, int)>();

                        uniquePieceRotations[rotatedPieceString].Add(degrees);
                        allPieceRotations[degrees] = rotatedPieceString;
                    }
                }
            }

            foreach (var (key, value) in uniquePieceRotations)
            {
                Debug.WriteLine($"{value.StringJoin(",")}{NewLine}{key}");
            }

            var not_rotated_0_0_0 = @"
[         ]
[ X X X X ]
[   X     ]
 [   X     ]
 [   X     ]
 [         ]".Trim();

            var x_rotated_90_0_0 = @"
[     ]
[ X   ]
[     ]
 [     ]
 [ X   ]
 [     ]
  [   X ]
  [ X X ]
  [ X   ]
   [     ]
   [ X   ]
   [     ]".Trim();
            var x2_rotated_180_0_0 = @"
[     X   ]
[     X   ]
[         ]
 [         ]
 [ X X X X ]
 [     X   ]".Trim();

            var y_rotated_0_90_0 = @"
[   X   ]
[ X X   ]
[   X   ]
[   X   ]
 [       ]
 [   X X ]
 [       ]
 [       ]".Trim();
            var y2_rotated_0_180_0 = @"
[     X   ]
[ X X X X ]
[         ]
 [         ]
 [     X   ]
 [     X   ]".Trim();

            var z_rotated_0_0_90 = @"
[   X     ]
[         ]
 [   X     ]
 [ X X X X ]
  [         ]
  [   X     ]".Trim();
            var z2_rotated_0_0_180 = @"
[         ]
[   X     ]
[   X     ]
 [   X     ]
 [ X X X X ]
 [         ]".Trim();

            var xyz_rotated_90_180_270 = @"
[     ]
[     ]
[ X   ]
[     ]
 [   X ]
 [   X ]
 [ X X ]
 [   X ]
  [     ]
  [     ]
  [   X ]
  [     ]".Trim();

            Assert.Equal(not_rotated_0_0_0, allPieceRotations[(0, 0, 0)]);

            Assert.Equal(x_rotated_90_0_0, allPieceRotations[(90, 0, 0)]);
            Assert.Equal(x2_rotated_180_0_0, allPieceRotations[(180, 0, 0)]);

            Assert.Equal(y_rotated_0_90_0, allPieceRotations[(0, 90, 0)]);
            Assert.Equal(y2_rotated_0_180_0, allPieceRotations[(0, 180, 0)]);

            Assert.Equal(z_rotated_0_0_90, allPieceRotations[(0, 0, 90)]);
            Assert.Equal(z2_rotated_0_0_180, allPieceRotations[(0, 0, 180)]);

            Assert.Equal(xyz_rotated_90_180_270, allPieceRotations[(90, 180, 270)]);

            Assert.Equal(24, uniquePieceRotations.Count);
        }
    }
}
