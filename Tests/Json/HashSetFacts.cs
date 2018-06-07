using System.Collections.Generic;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class HashSetFacts
    {

        private readonly DeepComparer
            _comparer = new DeepComparerBuilder()
                .GoDeepFor(Collections.HashSet).Build();

        [Fact]
        public void Eq()
        {
            _comparer.Compare(
                new HashSet<int> {1, 2},
                new HashSet<int> {2, 1}).Message.Should().BeNull();

        }
        [Fact]
        public void Neq()
        {
            _comparer.Compare(
                new HashSet<int> {1},
                new HashSet<int> {2, 1}).Message.Should().Be("HashSets are not equal");

        }
    }
}