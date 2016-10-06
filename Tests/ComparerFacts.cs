using System;
using System.Linq;
using System.Reflection;
using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public sealed class ComparerFacts
    {
        private readonly DeepComparerBuilder _builder =
            new DeepComparerBuilder();

        [Fact]
        public void Null_And_NonNull()
        {
            var notNull = new X { I = 3 };
            _builder.Build().Compare(null, notNull).Message.Should()
                .Be("object.Equals(<null>, Tests.ComparerFacts+X)");
        }
        [Fact]
        public void Null_Should_Equal_Null()
        {
            _builder.Build().Compare<X>(null, null).Should().Be(True);
        }
        [Fact]
        public void Do_Not_Expand_Objects_By_Default()
        {
            _builder.Build()
                .Compare(new X { I = 3 }, new X { I = 3 }).Message.Should()
                .Be("object.Equals(Tests.ComparerFacts+X, Tests.ComparerFacts+X)");
        }
        [Fact]
        public void ExpandObject_Example()
        {
            _builder
                .GoDeepFor(t => t == typeof(X))
                .Build()
                .Compare(new X { I = 3 }, new X { I = 3 })
                .Should().Be(True);
        }
        [Fact]
        public void When_Different_Types_Should_Throw()
        {
            _builder.GoDeepFor(t => t == typeof(X));
            Assert.Throws<TargetException>(()
                => _builder.Build().Compare(typeof(X), new X(), new Y()));
        }
        [Fact]
        public void Compare_Collections()
        {
            _builder.GoDeepFor(Collections.Enumerable)
                .Build().Compare(null, new Guid[0]).Message.Should()
                .Be("compareCollections(<null>, System.Guid[])");
        }

        public class X { public int I { get; set; } }
        public class Y { public int I { get; set; } }
    }
}