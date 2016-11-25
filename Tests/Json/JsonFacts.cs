using DeepComparison;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Tests
{
    public sealed class JsonFacts
    {
        private readonly JsonComparer _comparer;

        public JsonFacts()
        {
            _comparer = new JsonComparerBuilder().Build();
        }

        [Fact]
        public void Property_Value_Does_Not_Match()
        {
            _comparer.Compare(
                    new JObject(new JProperty("Age", 18)),
                    new { Age = 19})
                .Message.Should().Be("$root.Age: 18 != 19");
        }

        [Fact]
        public void Property_Type_Does_Not_Match()
        {
            _comparer.Compare(
                    new JObject(new JProperty("Age", "blah")),
                    new { Age = 18})
                .Message.Should().Be(
                    "JSON property $root.Age is of type String and " +
                    "we expected CLR object's property type to be System.String, " +
                    "but found System.Int32");
        }
        [Fact]
        public void Property_Name_Of_AnonymousObject_Does_Not_Match()
        {
            _comparer.Compare(new JObject(new JProperty("Age", 18)), new {Blah = 18})
                .Message.Should().Be(
                    "property Age is not found among properties of an anonymous object:\r\n" +
                    "Blah");
        }
        [Fact]
        public void Property_Name_Of_Tangible_Object_Does_Not_Match()
        {
            _comparer.Compare(new JObject(new JProperty("Age", 18)), "")
                .Message.Should().Be(
                    "property Age is not found among properties of type System.String:\r\n" +
                    "Chars, Length");
        }
        [Fact]
        public void Property_Matches()
        {
            _comparer.Compare(new JObject(new JProperty("Age", 18)), new {Age = 18})
                .Should().Be(ComparisonResult.True);
        }
        [Fact]
        public void Nested_Object_Property_Value_Does_Not_Match()
        {
            _comparer.Compare(
                    new JObject(new JProperty("Nested", new JObject(new JProperty("Age", 18)))),
                    new { Nested = new { Age = 19 } })
                .Message.Should().Be("$root.Nested.Age: 18 != 19");
        }
        [Fact]
        public void Not_An_Array()
        {
            _comparer.Compare(new JArray(1, 2, 3), new  { })
                .Message.Should().Be("not an array");
        }
        [Fact]
        public void Array_Comparison()
        {
            _comparer.Compare(new JArray(1, 2, 3), new [] { 1, 3, 2})
                .Message.Should().Be("$root[1]: 2 != 3");
        }
        [Fact]
        public void Different_Array_Size()
        {
            _comparer.Compare(new JArray(1, 3), new [] { 1, 3, 2}).Message.Should()
                .Be("First collection lacks an item 2, and 0 more; First 2 items matched though");
        }
    }
}