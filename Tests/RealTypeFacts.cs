using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public class RealTypeFacts
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(X))
                .Build();

        public RealTypeFacts()
        {
            _comparer
                .Compare<object>(new X { I = 3 }, new X { I = 3 })
                .Should().Be(True);
        }

        [Fact]
        public void Should_Compare_Real_Types()
        {
            _comparer
                .Compare<object>(new X { I = 3 }, new X { I = 4 })
                .Should().NotBe(True);
        }

        public class X { public int I { get; set; } }
    }
}