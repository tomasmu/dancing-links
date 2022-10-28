using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace Polycube.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void RegexReplaceTest()
        {
            var result = "hello 42 hello".RegexReplace(@"\d|llo", "he");
            result.Should().Be("hehe hehe hehe");
        }

        [Fact]
        public void RegexRemoveTest()
        {
            var result = "hejsan".RegexRemove("[an-s]");
            result.Should().Be("hej");
        }

        [Fact]
        public void TrimNewLine()
        {
            var result = @"

    leading and trailing
    newlines should be trimmed

".TrimNewLine();

            var expected = @"    leading and trailing
    newlines should be trimmed";

            result.Should().Be(expected);
        }

        [Fact]
        public void StringJoin()
        {
            var result = new string[] { "a", "b" }.StringJoin(", ");

            result.Should().Be("a, b");
        }

        [Fact]
        public void RegexSplit()
        {
            var result = "first, second".RegexSplit(@",\s");

            var expected = new string[] { "first", "second" };
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void RegexSplit_Captured_Group_Unexpected()
        {
            var result = @"first, second".RegexSplit(@"(,\s)");

            //captured groups are included in the result by default
            //this is by design, but unexpected
            var unexpected = new string[] { "first", ", ", "second" };
            result.Should().BeEquivalentTo(unexpected);
        }

        [Fact]
        public void RegexSplit_Non_Captured_Groups()
        {
            var result_A = @"first, second".RegexSplit(@"(?:,\s)");
            var result_B = @"first, second".RegexSplit(@"(,\s)", RegexOptions.ExplicitCapture);

            //non captured groups are not included
            //A and B are equivalent, but B has cleaner regex syntax
            result_A.Should().BeEquivalentTo(result_B);
        }

        [Fact]
        public void RegexSplit_Named_Captured_Group_Unexpected2()
        {
            var result = @"first, second".RegexSplit(@"(?<named_group>,\s)", RegexOptions.ExplicitCapture);

            //named captured groups are always included in the result
            //this is by design, but unexpected
            var unexpected = new string[] { "first", ", ", "second" };
            result.Should().BeEquivalentTo(unexpected);
        }

        //todo: move
        [Fact]
        public void StringToPieceCoordinates()
        {
            //this is a 3D piece
            //a "T" in the bottom layer
            //and an I on top of the T, half of it hanging in the air
            //can be inscribed in a 3x2x4 cuboid

            // |....
            // |TTTT
            // |..T
            // |_____

            // |.I..
            // |.I..
            // |....
            // |_____
            var piece = @"
-
TTTT
--T

-I
-I"
.ToCoordinates('-')
.ToJsonArray()
.StringJoin(",");

            var newLine_Whitespace_Comment_Pattern = @"\r|\n|\s|/\*.*?\*/";
            var expected = @"
/* y=0 first layer */
                                                        /*      */
[[1],[0],[0]],[[1],[0],[1]],[[1],[0],[2]],[[1],[0],[3]],/* TTTT */
                            [[2],[0],[2]],              /*   T  */
/* y=1 second layer */
              [[0],[1],[1]],                            /*  I   */
              [[1],[1],[1]]                             /*  I   */
                                                        /*      */
"
.RegexRemove(newLine_Whitespace_Comment_Pattern);

            piece.Should().BeEquivalentTo(expected);
        }
    }
}
