using DeepComparison;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Tests
{
    public sealed class JsonFacts
    {
        private readonly JsonComparerBuilder _builder =
            new JsonComparerBuilder();

        [Fact]
        public void Property_Name_Of_AnonymousObject_Does_Not_Match()
        {
            _builder.Build().Compare(
                    new JObject(new JProperty("Age", 18)),
                    new {Blah = 18})
                .Message.Should().Be(
                    "property Age is not found among properties of an anonymous object:\r\n" +
                    "Blah");
        }
        [Fact]
        public void Property_Name_Of_Tangible_Object_Does_Not_Match()
        {
            _builder.Build().Compare(new JObject(new JProperty("Age", 18)), "")
                .Message.Should().Be(
                    "property Age is not found among properties of type System.String:\r\n" +
                    "Chars, Length");
        }
        [Fact]
        public void Property_Matches()
        {
            _builder.Build().Compare(
                    new JObject(new JProperty("Age", 18)),
                    new {Age = 18})
                .Should().Be(ComparisonResult.True);
        }
    }
}