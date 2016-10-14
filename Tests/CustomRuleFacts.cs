using System;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class CustomRuleFacts
    {
        [Fact]
        public void Double_GoDeepFor()
        {
            new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(int))
                .GoDeepFor(t => t == typeof(int))
                .Build().Compare(1, 2);
        }
        [Fact]
        public void Double_Custom()
        {
            var deepComparer = new DeepComparerBuilder()
                .CustomRuleFor<int>((a, b) => a == b)
                .CustomRuleFor<int>((a, b) => a == b)
                .Build();
            Assert.Throws<InvalidOperationException>(
                    () => deepComparer.Compare(1, 2))
                .Message.Should().Be(
                "More than one custom rule for 'System.Int32':\r\n" +
                "Custom((a, b) => (a == b))\r\n" +
                "Custom((a, b) => (a == b))");
        }
    }
}