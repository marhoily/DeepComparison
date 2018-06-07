﻿using System;
using System.Collections.Generic;
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
        public ComparisonResult Compare<T>(T x, T y) => 
            _cache.Get(typeof(T))(x, y);

        /// <summary>Compares two objects deeply</summary>
        /// <param name="formalType">objects formal type. 
        ///     By default comparer doesn't care about runtime types 
        ///     of the arguments</param>
        /// <param name="x">an object to compare</param>
        /// <param name="y">an object to compare</param>
        /// <returns>true when equal</returns>
        public ComparisonResult Compare(Type formalType, object x, object y) =>
            _cache.Get(formalType)(x, y);

        private FCompare GetComparer(Type formalType)
        {
            var compareOption = _rulesContainer[formalType];
            if (compareOption == TreatObjectAs.PropertiesBag)
                return (x, y) => _objectExpander.CompareProperties(
                    x, y, formalType, Compare, _formatting);
            var collection = compareOption as TreatObjectAs.Collection;
            if (collection != null)
                return (x, y) => CompareCollection(x, y, collection);
            var custom = compareOption as TreatObjectAs.Custom;
            return custom != null ? Custom(custom.Comparer) 
                : (a, b) => Cmp(a, b, "object.Equals", Equals);
        }

        private FCompare Custom(Func<object, object, bool> comparer) =>
            (x, y) => Cmp(x, y, "customCompare", comparer);

        private ComparisonResult Cmp(object x, object y, string tag, Func<object, object, bool> comparer) 
            => comparer(x, y) ? True : _formatting.Explain(x, y, tag);


        private ComparisonResult CompareCollection(object x, object y, TreatObjectAs.Collection collection)
        {
            if (x == null && y == null) return True;
            if (x == null || y == null)
                return _formatting.Explain(x, y, "compareCollections");

            var xE = collection.ToEnumerable(x);
            var yE = collection.ToEnumerable(y);
            if (collection.Comparison == CollectionComparison.Sequential)
                return xE.SequenceEqual(yE, _cache.Get(collection.ItemType));
            if (collection.Comparison == CollectionComparison.Equivalency)
            {
                var eq = (bool) x.GetType()
                    .GetMethod(nameof(HashSet<int>.SetEquals))
                    .Invoke(x, new[] {y});
                if (eq) return True;
                return new ComparisonResult("HashSets are not equal");
            }
            throw new NotImplementedException();
        }
    }
}