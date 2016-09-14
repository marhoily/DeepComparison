using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public class MemberFacts
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(x => x == typeof(X))
                .Build();

        public MemberFacts()
        {
            _comparer
                .Compare(new X(3, 4), new X(3, 4))
                .Should().Be(True);
        }

        [Fact]
        public void Should_Not_See_Fields() => _comparer
            .Compare(new X(3, 4), new X(3, 5))
            .Should().Be(True);

        [Fact]
        public void Should_See_Private_Properties() => _comparer
            .Compare(new X(3, 4), new X(5, 4)).Message
            .Should().Be("object.Equals(3, 5)");

        // ReSharper disable once MemberCanBePrivate.Global
        public class X
        {
            // ReSharper disable once NotAccessedField.Local
            private readonly int _y;
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private int I { get; }

            public X(int i, int y)
            {
                _y = y;
                I = i;
            }
        }
    }
}