using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public class ExpandObjectWithNoProperties
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(X))
                .Build();

        [Fact]
        public void Should_Equal()
        {
            var a = new X();
            var b = new X();
            _comparer.Compare(a, b).Should().Be(True);
        }

        public class X
        {
        }
    }
}