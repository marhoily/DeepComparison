using System.Collections.Generic;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class ClrTypesFacts
    {
        private readonly DeepComparerBuilder _builder =
            new DeepComparerBuilder();

        [Fact]
        public void String()
        {
            ClrTypes.AllExceptBasic(typeof(int)).Should().BeFalse();
            ClrTypes.AllExceptBasic(typeof(string)).Should().BeFalse();
        }

        [Fact]
        public void FactMethodName()
        {
            var cmp = _builder
                .GoDeepFor(Collections.List)
                .GoDeepFor(t => Collections.List(t) == null && ClrTypes.AllExceptBasic(t))
                .Build();
            cmp.Compare("xx", "xy").Message.Should().Be("object.Equals(xx, xy)");
            cmp.Compare(new List<int>{1, 2},new List<int>{1, 1})
                .Message.Should().Be("object.Equals(2, 1)");
            cmp.Compare(new List<int> {1, 2}, new List<int> {1, 2})
                .Message.Should().BeNull();

        }
    }
}