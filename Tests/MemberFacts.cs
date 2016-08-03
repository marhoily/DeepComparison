using DeepComparison;
using FluentAssertions;
using Xunit;

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
                .Should().BeTrue();
        }

        [Fact]
        public void Should_Not_See_Fields() => _comparer
            .Compare(new X(3, 4), new X(3, 5))
            .Should().BeTrue();

        [Fact]
        public void Should_See_Private_Properties() => _comparer
            .Compare(new X(3, 4), new X(5, 4))
            .Should().BeFalse();

        public class X
        {
            private readonly int _y;
            private int I { get; }

            public X(int i, int y)
            {
                _y = y;
                I = i;
            }
        }
    }
}