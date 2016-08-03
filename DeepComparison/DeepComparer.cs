using System;
using static DeepComparison.ComparisonResult;

namespace DeepComparison
{
    using FCompare = Func<object, object, ComparisonResult>;

    /// <summary>A class with Compare() method</summary>
    public sealed class DeepComparer
    {
        private readonly ObjectExpander _objectExpander;
        private readonly RulesContainer _rulesContainer;
        private readonly CachingDictionary<Type, FCompare> _cache;
        private readonly Formatting _formatting = new Formatting();

        internal DeepComparer(
            ObjectExpander objectExpander,
            RulesContainer rulesContainer)
        {
            _rulesContainer = rulesContainer;
            _objectExpander = objectExpander;
            _cache = new CachingDictionary<Type, FCompare>(GetComparer);
        }

        /// <summary>Compares two objects deeply</summary>
        /// <typeparam name="T">objects formal type. 
        ///     By default comparer doesn't care about runtime types 
        ///     of the arguments</typeparam>
        public ComparisonResult Compare<T>(T x, T y)
        {
            return _cache.Get(typeof(T))(x, y);
        }
        /// <summary>Compares two objects deeply</summary>
        /// <param name="x">an object to compare</param>
        /// <param name="y">an object to compare</param>
        /// <param name="formalType">objects formal type. 
        ///     By default comparer doesn't care about runtime types 
        ///     of the arguments</param>
        /// <returns>true when equal</returns>
        public ComparisonResult Compare(object x, object y, Type formalType)
        {
            return _cache.Get(formalType)(x, y);
        }
        private FCompare GetComparer(Type formalType)
        {
            var compareOption = _rulesContainer[formalType];
            if (compareOption == TreatObjectAs.PropertiesBag)
                return (x, y) => _objectExpander.CompareProperties(x, y, formalType, Compare);
            var collection = compareOption as TreatObjectAs.Collection;
            if (collection != null)
                return (x, y) => CompareCollection(x, y, collection);
            var custom = compareOption as TreatObjectAs.Custom;
            if (custom != null)
                return custom.Comparer;
            return ObjEquals;
        }

        private ComparisonResult ObjEquals(object x, object y)
        {
            if (Equals(x, y)) return True;
            var xText = _formatting.Format(x);
            var yText = _formatting.Format(y);
            return $"object.Equals({xText}, {yText})";
        }

        private ComparisonResult CompareCollection(object x, object y, TreatObjectAs.Collection collection)
        {
            if (x == null && y == null) return True;
            if (x == null || y == null) return False;
            var xE = collection.ToEnumerable(x);
            var yE = collection.ToEnumerable(y);
            if (collection.Comparison == CollectionComparison.Sequential)
                return xE.SequenceEqual(yE, _cache.Get(collection.ItemType));
            throw new NotImplementedException();
        }
    }

    internal sealed class Formatting
    {
        public string Format(object obj)
        {
            if (obj == null) return "<null>";
            return obj.ToString();
        }
    }
}