using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SudokuSolver.Tests
{
    public class SerializerTests
    {
        [Fact]
        public void Serializer_default_offset_negative_one_Using_array()
        {
            var boardArray = new int[,] { { 1 } };
            var expectedBoard = new int[,] { { 0 } };
            
            var serializer = new SudokuSerializer();
            var result = serializer.DeserializeBoard(boardArray);

            result.Should().Equal(expectedBoard);
        }

        [Fact]
        public void Serializer_default_offset_negative_one_Using_string()
        {
            var boardString = "1";
            var expectedBoard = new int[,] { { 0 } };

            var serializer = new SudokuSerializer();
            var result = serializer.DeserializeBoard(boardString);

            result.Should().Equal(expectedBoard);
        }

        [Fact]
        public void Serializer_custom_offset_Using_array()
        {
            var boardArray = new int[,] { { 1 } };
            var expectedBoard = new int[,] { { 1 } };
            
            var serializer = new SudokuSerializer() { Offset = 0 };
            var result = serializer.DeserializeBoard(boardArray);

            result.Should().Equal(expectedBoard);
        }

        [Fact]
        public void Serializer_custom_offset_Using_string()
        {
            var boardString = "1";
            var expectedBoard = new int[,] { { 1 } };

            var serializer = new SudokuSerializer() { Offset = 0 };
            var result = serializer.DeserializeBoard(boardString);

            result.Should().BeEquivalentTo(expectedBoard);
        }
    }
}
