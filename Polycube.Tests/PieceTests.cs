using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace Polycube.Tests
{
    public class PieceTests
    {
        [Fact]
        public void GridCubies_equal_PieceCubies()
        {
            var piece_20cubies = new string('a', 20);
            var piece_3cubies = new string('b',  3);

            var pieces = new string[] {
                piece_20cubies,
                piece_3cubies
            }.Select(str => new Piece(str));
            //20+3=23

            var grid_23cubies = new bool[2, 3, 4];
            grid_23cubies[0, 0, 0] = true;
            //2*3*4 - 1 = 23
            var cuboid = new Cuboid(grid_23cubies);

            //assert: does not throw
            var polycube = new Polycube(cuboid, pieces);
        }

        [Fact]
        public void GridCubies_not_equal_PieceCubies()
        {
            var piece_19cubies = new string('a', 19);
            var piece_3cubies = new string('b', 3);

            var pieces = new string[] {
                piece_19cubies,
                piece_3cubies
            }.Select(str => new Piece(str));

            var grid_23cubies = new bool[2, 3, 4];
            grid_23cubies[1, 2, 3] = true;
            var cuboid = new Cuboid(grid_23cubies);

            Action create = () => new Polycube(cuboid, pieces);
            create.Should().Throw<ArgumentException>().WithMessage("23 grid cubies != 22 piece cubies (19+3)");
        }

        [Fact]
        public void NewPiece_TakesNameFromInputString()
        {
            var str = @" - ABC";
            var piece = new Piece(str);
            piece.Name.Should().Be('A');
        }

        [Fact]
        public void NewPiece_DefaultName_IfInputStringWhiteSpaceOrEmptyChar()
        {
            var str = @" - ";
            var piece = new Piece(str);
            piece.Name.Should().Be('Ø');
        }
    }
}
