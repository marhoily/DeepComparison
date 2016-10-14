using System;
using System.Collections;
using System.Collections.Generic;
using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.CollectionComparison;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public sealed class ExpandCollectionFacts
    {
        private readonly DeepComparerBuilder
            _builder = new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(X));

        private readonly X _x1 = new X("x1");
        private readonly X _x2 = new X { S = new HashSet<X>() };

        [Fact]
        public void DoNot_GoDeepFor_Collections_ByDefault()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _builder.Build().Compare(a, b).Message.Should()
                .Be("object.Equals(Tests.ExpandCollectionFacts+X[], Tests.ExpandCollectionFacts+X[])");
        }

        [Fact]
        public void GoDeepFor_Collections_Example()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _builder.GoDeepFor(Collections.Array)
                .Build().Compare(a, b).Message.Should()
                .Be("object.Equals(<null>, System.Collections.Generic.HashSet`1[Tests.ExpandCollectionFacts+X])");
        }
        [Fact]
        public void Full_Syntax()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _builder
                .GoDeepFor(t => !t.IsArray ? null :
                    new TreatObjectAs.Collection(
                        Sequential,
                        t.GetElementType(),
                        x => (IEnumerable)x))
                .Build()
                .Compare(a, b).Should().Be(True);
        }
        [Fact]
        public void Expand_Two()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _builder
                .GoDeepFor(Collections.Array)
                .GoDeepFor(Collections.List)
                .Build()
                .Compare(a, b).Should().Be(True);
        }
        [Fact]
        public void Expand_Enumerable()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _builder.GoDeepFor(Collections.Enumerable)
                .Build()
                .Compare(a, b).Should().Be(True);
        }
        [Fact]
        public void Expand_Ambiguity()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            Assert.Throws<InvalidOperationException>(() => _builder
                .GoDeepFor(Collections.List)
                .GoDeepFor(Collections.Enumerable)
                .Build()
                .Compare(a, b));
        }

        class MultiList : IEnumerable<int>, IEnumerable<string>
        {
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
        [Fact]
        public void Two_Enumerable_Variants()
        {
            Assert.Throws<InvalidOperationException>(() => _builder
                .GoDeepFor(Collections.Enumerable)
                .Build()
                .Compare(new MultiList(), new MultiList())).Message.Should().Be(
                "It is not clear how to enumerate Tests.ExpandCollectionFacts+MultiList " +
                "because it implements more than one variant of IEnumerable of: " +
                "System.Int32, System.String");
        }

        [Fact]
        public void Collection_First_Has_More_Elements()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new X[0]  };
            _builder.GoDeepFor(Collections.Array)
                .Build().Compare(a, b).Message.Should()
                .Be("Second collection lacks an item: x1");
        }
        [Fact]
        public void Collection_Second_Has_More_Elements()
        {
            var a = new X { A = new X[0]  };
            var b = new X { A = new[] { _x1 } };
            _builder.GoDeepFor(Collections.Array)
                .Build().Compare(a, b).Message.Should()
                .Be("First collection lacks an item x1");
        }
        [Fact]
        public void Collection_Null_Element()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new X[] { null } };
            _builder.GoDeepFor(Collections.Array)
                .Build().Compare(a, b).Message.Should()
                .Be("comparePropertiesOf(x1, <null>)");
        }

        [Fact]
        public void Collection_Null_Elements_Equal()
        {
            var a = new X { A = new X[] { null } };
            var b = new X { A = new X[] { null } };
            _builder.GoDeepFor(Collections.Array)
                .Build()
                .Compare(a, b).Should().Be(True);
        }

        public class X
        {
            private string _name;

            public X(string name = "")
            {
                _name = name;
            }

            public override string ToString() => _name;

            public X[] A { get; set; }
            public List<X> L { get; set; }
            public HashSet<X> S { get; set; }
        }
    }
}