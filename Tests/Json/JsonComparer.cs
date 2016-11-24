using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepComparison;
using Newtonsoft.Json.Linq;

namespace Tests
{
    internal class JsonComparer
    {
        public ComparisonResult Compare(JObject j, object a)
        {
            var properties = a.GetType().GetProperties();
            foreach (var property in j.Properties())
            {
                var match = properties.FirstOrDefault(p => p.Name == property.Name);
                if (match == null)
                {
                    var subject = CheckIfAnonymousType(a.GetType())
                        ? "properties of an anonymous object:\r\n"
                        : $"properties of type {a.GetType().FullName}:\r\n";
                    return new ComparisonResult(
                        $"property {property.Name} is not found among {subject}" +
                        string.Join(", ", properties.Select(p => p.Name)));
                }
            }
            return ComparisonResult.True;
        }
        private static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}