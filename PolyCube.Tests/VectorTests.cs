using FluentAssertions;
using PolycubeSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PolycubeSolver.Tests
{
    public class VectorTests
    {
        [Fact]
        public void Test_OrderBy()
        {
            var unsorted = new List<Vector>
            {
                new Vector(0, 1, 1),
                new Vector(1, 0, 1),
                new Vector(1, 0, 0),
                new Vector(1, 1, 1),
                new Vector(0, 1, 1),
                new Vector(1, 1, 0),
                default,
                new Vector(0, 1, 0),
                new Vector(0, 1, 1),
                new Vector(0, 0, 1),
                new Vector(1, 1, 0),
                new Vector(0, 0, 0),
                new Vector(1, 1, 0),
                new Vector(0, 1),
            };
            var sorted = unsorted.OrderBy(v => v);

            var expected = new List<Vector>
            {
                default,
                new Vector(0, 0, 0),
                new Vector(0, 0, 1),
                new Vector(0, 1),       //(0, 1) before (0, 1, 0)
                new Vector(0, 1, 0),
                new Vector(0, 1, 1),
                new Vector(0, 1, 1),
                new Vector(0, 1, 1),
                new Vector(1, 0, 0),
                new Vector(1, 0, 1),
                new Vector(1, 1, 0),
                new Vector(1, 1, 0),
                new Vector(1, 1, 0),
                new Vector(1, 1, 1),
            };
            sorted.Should().Equal(expected);
        }

        [Fact]
        public void Test_Equals()
        {
            var equal1 = new Vector(1, 1, 1);
            var equal2 = new Vector(1, 1, 1);

            (equal1 == equal2).Should().BeTrue();
            (equal1 != equal2).Should().BeFalse();
        }

        [Fact]
        public void Test_NotEquals()
        {
            var first = new Vector(1, 1, 1);
            var notfirst = new Vector(1, 1, 42);

            (first == notfirst).Should().BeFalse();
            (first != notfirst).Should().BeTrue();
        }

        [Fact]
        public void Test_NotEquals_Different_Lengths()
        {
            var @short = new Vector(1, 1);
            var @long = new Vector(1, 1, 1);

            (@short != @long).Should().BeTrue();
            (@short == @long).Should().BeFalse();
        }

        [Fact]
        public void Equals_BothNull()
        {
            var left = default(Vector);
            var right = default(Vector);

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        [Fact]
        public void Equals_LeftNull()
        {
            var left = default(Vector);
            var right = new Vector(1, 1, 1);

            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [Fact]
        public void Equals_RightNull()
        {
            var left = new Vector(1, 2, 3);
            var right = default(Vector);

            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(0, 0, 2)]
        [InlineData(0, 0, 3)]
        [InlineData(0, 1, 3)]
        [InlineData(0, 2, 3)]
        [InlineData(0, 3, 3)]
        [InlineData(1, 2, 2)]
        [InlineData(1, 3, 3)]
        [InlineData(2, 2, 1)]
        public void Test_GreaterThan(int x, int y, int z)
        {
            var great = new Vector(2, 2, 2);
            var less = new Vector(x, y, z);

            var result = great > less;
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 1, true)]
        [InlineData(0, 0, 2, true)]
        [InlineData(0, 0, 3, true)]
        [InlineData(0, 1, 3, true)]
        [InlineData(0, 2, 3, true)]
        [InlineData(0, 3, 3, true)]
        [InlineData(1, 2, 2, true)]
        [InlineData(1, 3, 3, true)]
        [InlineData(2, 2, 1, true)]
        [InlineData(2, 2, 2, false)]
        [InlineData(3, 2, 2, false)]
        [InlineData(3, 4, 5, false)]
        public void Test_LessThan(int x, int y, int z, bool expected)
        {
            var great = new Vector(2, 2, 2);
            var less = new Vector(x, y, z);

            var result = less < great;
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, 2, 2, true)]
        [InlineData(2, 1, 2, true)]
        [InlineData(2, 2, 1, true)]
        [InlineData(2, 2, 2, true)]
        [InlineData(3, 2, 2, false)]
        [InlineData(3, 4, 5, false)]
        public void Test_GreaterOrEqual(int x, int y, int z, bool expected)
        {
            var great = new Vector(2, 2, 2);
            var less = new Vector(x, y, z);

            var result = less <= great;
            result.Should().Be(expected);
        }

        [Fact]
        public void Five_guarantees_of_equivalence_Vector()
        {
            //1. reflexive
            var x = new Vector(1, 2, 3);
            x.Equals(x).Should().BeTrue();

            //2. symmetric
            var y = new Vector(1, 2, 3);
            x.Equals(y).Should().Be(y.Equals(x));

            //3. transitive
            var z = new Vector(1, 2, 3);
            (x.Equals(y) && y.Equals(z)).Should().BeTrue();
            x.Equals(z).Should().BeTrue();

            //4. successive invocations return the same value
            //not implemented here

            //5. non-null isn't equal to null
            //x.Equals() //throws if x is null, which breaks both 1 and 2

            //5.Should().Be(3);
        }

        [Fact]
        public void Five_guarantees_of_equivalence_Matrix()
        {
            //1. reflexive
            var x = new Matrix(new int[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            x.Equals(x).Should().BeTrue();

            //2. symmetric
            var y = new Matrix(new int[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            x.Equals(y).Should().Be(y.Equals(x));

            //3. transitive
            var z = new Matrix(new int[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            (x.Equals(y) && y.Equals(z)).Should().BeTrue();
            x.Equals(z).Should().BeTrue();

            //4. successive invocations return the same value
            //not implemented here

            //5. non-null isn't equal to null
            //x.Equals() //throws if x is null, which breaks both 1 and 2

            //5.Should().Be(3);
        }


        [Fact]
        public void Test_Matrix_Equals()
        {
            var equal1 = new Matrix(new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            });
            var equal2 = new Matrix(new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            });

            equal1.Equals(equal2).Should().BeTrue();
        }

        [Fact]
        public void Test_Matrix_NotEquals()
        {
            var equal1 = new Matrix(new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            });
            var equal2 = new Matrix(new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1337 },
            });

            equal1.Equals(equal2).Should().BeFalse();
        }

        [Fact]
        public void Test_Distinct_Vector()
        {
            var list = new List<Vector>
            {
                new Vector(1, 2, 3),
                new Vector(4, 5, 6),
                new Vector(1, 2, 3),
                new Vector(4, 5, 6),
                new Vector(1, 2, 3),
            }.Distinct();

            var expected = new List<Vector>
            {
                new Vector(1, 2, 3),
                new Vector(4, 5, 6),
            };
            list.Should().Equal(expected);
        }

        [Fact]
        public void Test_Distinct_Matrix()
        {
            var list = new List<Matrix>
            {
                new Matrix(new int[,] { { 1, 2, 3 }, { 1, 2, 3 } }),
                new Matrix(new int[,] { { 4, 5, 6 }, { 4, 5, 6 } }),
                new Matrix(new int[,] { { 1, 2, 3 }, { 1, 2, 3 } }),
                new Matrix(new int[,] { { 4, 5, 6 }, { 4, 5, 6 } }),
                new Matrix(new int[,] { { 1, 2, 3 }, { 1, 2, 3 } }),
            }.Distinct();

            var expected = new List<Matrix>
            {
                new Matrix(new int[,] { { 1, 2, 3 }, { 1, 2, 3 } }),
                new Matrix(new int[,] { { 4, 5, 6 }, { 4, 5, 6 } }),
            };
            list.Should().Equal(expected);
        }
    }
}
