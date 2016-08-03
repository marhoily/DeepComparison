using System;
using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public class CustomFacts
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(X))
                .CustomRuleFor<int>((x, y) => Math.Abs(x - y) < 2)
                .Build();

        [Fact]
        public void Custom_False()
        {
            _comparer
                .Compare(new X { I = 3 }, new X { I = 4 })
                .Should().Be(True);
        }
        [Fact]
        public void Custom_True()
        {
            _comparer
                .Compare(new X { I = 3 }, new X { I = 40 })
                .Should().Be(False);
        }

        public class X
        {
            public int I { get; set; }
        }
    }
}