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
        public void Nested_Object_Property_Value_Does_Not_Match()
        {
            _builder.Build().Compare(
                    new JObject(new JProperty("Nested", new JObject(new JProperty("Age", 18)))),
                    new { Nested = new { Age = 19}})
                .Message.Should().Be("[<root>.Nested.Age]: 18 != 19");
        }
        [Fact]
        public void Property_Value_Does_Not_Match()
        {
            _builder.Build().Compare(
                    new JObject(new JProperty("Age", 18)),
                    new { Age = 19})
                .Message.Should().Be("[<root>.Age]: 18 != 19");
        }

        [Fact]
        public void Property_Type_Does_Not_Match()
        {
            _builder.Build().Compare(
                    new JObject(new JProperty("Age", "blah")),
                    new { Age = 18})
                .Message.Should().Be(
                    "JSON property Age is of type String and " +
                    "we expected CLR object's property type to be System.String, " +
                    "but found System.Int32");
        }
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