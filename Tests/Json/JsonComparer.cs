using System;
using System.Collections;
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

        public ComparisonResult Compare(JToken j, object a)
        {
            return CompareToken(j, a, "$root");
        }
        private ComparisonResult CompareToken(JToken j, object a, string context)
        {
            switch (j.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Object:
                    return CompareObject(j.As<JObject>(), a, context);
                case JTokenType.Array:
                    if (!(a is IEnumerable))
                        return new ComparisonResult("not an array");
                    return CompareArray(j.As<JArray>(), (IEnumerable)a, context);
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
                    if (!Types.TryGetValue(j.Type, out type))
                        throw new ArgumentOutOfRangeException(
                            "property.Value.Type", j.Type.ToString());
                    if (type != a.GetType())
                    {
                        return new ComparisonResult(
                            $"JSON property {context} is of type {j.Type} " +
                            $"and we expected CLR object's property type to be {type}, " +
                            $"but found {a.GetType()}");
                    }
                    if (!Equals(j.ToObject(a.GetType()), a))
                    {
                        return new ComparisonResult($"{context}: {j} != {a}");
                    }

                    break;
            }
            return ComparisonResult.True;
        }

        private ComparisonResult CompareArray(JArray j, IEnumerable a, string context)
        {
            var counter = 0;
            return j.SequenceEqual(a, 
                (jj, aa) => CompareToken((JToken) jj, aa,
                    $"{context}[{counter++}]"));
        }

        private ComparisonResult CompareObject(JObject j, object a, string context)
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
                var result = CompareToken(
                    property.Value, match.GetValue(a),
                    context + "." + property.Name);
                if (result != ComparisonResult.True)
                    return result;
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