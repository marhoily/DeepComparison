using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepComparison
{
    /// <summary>
    /// Has default collection selectors to be use with 
    /// <see cref="DeepComparerBuilder.GoDeepFor(System.Func{System.Type,bool})"/>
    /// </summary>
    public static class Collections
    {
        /// <summary>Tells comparer how to find arrays. 
        /// Suggests to compare them sequentially </summary>
        public static readonly CollectionPredicate Array =
            t => !t.IsArray
                ? null
                : new TreatObjectAs.Collection(
                    CollectionComparison.Sequential,
                    t.GetElementType(), x => (IEnumerable) x);

        /// <summary>Tells comparer how to find <see cref="List"/>s.
        /// Suggests to compare them sequentially </summary>
        public static readonly CollectionPredicate List = t =>
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(List<>)) return null;
            return new TreatObjectAs.Collection(
                CollectionComparison.Sequential,
                t.GetGenericArguments()[0],
                x => (IEnumerable) x);
        };

        /// <summary>Tells comparer how to find <see cref="HashSet"/>s. 
        /// Suggests to compare them by equivalency </summary>
        public static readonly CollectionPredicate HashSet = t =>
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(HashSet<>)) return null;
            return new TreatObjectAs.Collection(
                CollectionComparison.Equivalency,
                t.GetGenericArguments()[0],
                x => (IEnumerable) x);
        };

        /// <summary>Tells comparer how to find <see cref="IEnumerable{T}"/>s.
        /// Suggests to compare them sequentially </summary>
        public static readonly CollectionPredicate Enumerable = t =>
        {
            var interfaces = t
                .GetInterfaces()
                .Where(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .ToArray();
            if (interfaces.Length > 1)
                throw new InvalidOperationException(
                    $"It is not clear how to enumerate {t.FullName} " +
                    "because it implements more than one variant of IEnumerable of: " +
                    string.Join(", ", interfaces.Select(i => i.GetGenericArguments()[0].FullName)));

            if (interfaces.Length == 0)
                return null;
            var ifc = interfaces[0];
            return new TreatObjectAs.Collection(
                CollectionComparison.Sequential,
                ifc.GetGenericArguments()[0],
                x => (IEnumerable) x);
        };
    }
}