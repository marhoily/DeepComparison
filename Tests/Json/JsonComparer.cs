using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepComparison;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Tests
{
    internal class JsonComparer
    {
        private static readonly Dictionary<JTokenType, Type> Types = 
            new Dictionary<JTokenType, Type>()
            {
                { JTokenType.String, typeof(string) },
                { JTokenType.Integer, typeof(int) }
            };

        public ComparisonResult Compare(JObject j, object a)
        {
            return CompareInner(j, a, "<root>");
        }
        public ComparisonResult CompareInner(JObject j, object a, string context)
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
                switch (property.Value.Type)
                {
                    case JTokenType.None:
                        break;
                    case JTokenType.Object:
                        var result = CompareInner(
                            property.Value.As<JObject>(), 
                            match.GetValue(a),
                            $"{context}.{property.Name}");
                        if (result != ComparisonResult.True)
                            return result;
                        break;
                    case JTokenType.Array:
                        break;
                    case JTokenType.Constructor:
                        break;
                    case JTokenType.Property:

                        break;
                    case JTokenType.Comment:
                        break;
                    case JTokenType.Float:
                        break;
                    case JTokenType.Boolean:
                        break;
                    case JTokenType.Null:
                        break;
                    case JTokenType.Undefined:
                        break;
                    case JTokenType.Date:
                        break;
                    case JTokenType.Raw:
                        break;
                    case JTokenType.Bytes:
                        break;
                    case JTokenType.Guid:
                        break;
                    case JTokenType.Uri:
                        break;
                    case JTokenType.TimeSpan:
                        break;
                    default:
                        Type type;
                        if (!Types.TryGetValue(property.Value.Type, out type))
                            throw new ArgumentOutOfRangeException(
                                "property.Value.Type", property.Value.Type.ToString());
                        if (type != match.PropertyType)
                            return new ComparisonResult(
                                $"JSON property {property.Name} is of type {property.Value.Type} " +
                                $"and we expected CLR object's property type to be {type}, " +
                                $"but found {match.PropertyType}");
                        if (!Equals(property.Value.ToObject(match.PropertyType),
                            match.GetValue(a)))
                        {
                            return new ComparisonResult(
                                $"[{context}.{property.Name}]: {property.Value} != {match.GetValue(a)}");
                        }

                        break;
                }
                //match.PropertyType
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