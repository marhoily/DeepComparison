using System.Runtime.Serialization;
using DeepComparison;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;
using static DeepComparison.ComparisonResult;

namespace Tests
{
    public sealed class ExpandObjectFacts
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(t => t.HasAttribute<DataContractAttribute>())
                .FilterProperties(p => p.HasAttribute<DataMemberAttribute>())
                .Build();

        [Fact]
        public void Should_Compare_Filtered_Properties()
        {
            var a = new X { In = 2 };
            var b = new X { In = 3 };
            _comparer.Compare(a, b).Message
                .Should().Be("object.Equals(2, 3)");
        }
        [Fact]
        public void ShouldNot_Compare_Ignored_Properties()
        {
            var a = new X { Out = 3 };
            var b = new X { Out = 2 };
            _comparer.Compare(a, b).Should().Be(True);
        }
        [Fact]
        public void Should_GoDeepFor_Objects()
        {
            var a = new X { Px = new X { In = 3 } };
            var b = new X { Px = new X { In = 3 } };
            _comparer.Compare(a, b).Should().Be(True);
        }
        [Fact]
        public void Deep_Different()
        {
            var a = new X { Px = new X { In = 3 } };
            var b = new X { Px = new X { In = 4 } };
            _comparer.Compare(a, b).Message
                .Should().Be("object.Equals(3, 4)");
        }

        [DataContract]
        public class X
        {
            [DataMember]
            public X Px { get; set; }
            [DataMember]
            public int In { get; set; }
            public int Out { get; set; }
        }
    }
}