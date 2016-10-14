using System;
using DeepComparison;
using Xunit;

namespace Tests
{
    public sealed class CustomRules
    {
        [Fact]
        public void FactMethodName()
        {
            var deepComparer = new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(int))
                .GoDeepFor(t => t == typeof(int))
                .Build();
            deepComparer.Compare(1, 2);
        }
    }
}