using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DeepComparison
{
    internal sealed class RulesContainer
    {
        private readonly List<Func<Type, TreatObjectAs>>
            _byFunc = new List<Func<Type, TreatObjectAs>>();
        public void GoDeepFor(Func<Type, bool> func)
        {
            _byFunc.Add(t => func(t)
                ? TreatObjectAs.PropertiesBag
                : TreatObjectAs.Simple);
        }
        public void TreatAsCollection(CollectionPredicate func)
        {
            _byFunc.Add(x => func(x) ?? TreatObjectAs.Simple);
        }
        public void RuleFor<T>(Expression<Func<T, T, bool>> func)
        {
            var compile = func.Compile();
            _byFunc.Add(t => t != typeof(T)
                ? TreatObjectAs.Simple
                : new TreatObjectAs.Custom(func.ToString(), (x, y) =>
                {
                    if (x == null && y == null) return true;
                    if (x == null || y == null) return false;
                    return compile.Invoke((T) x, (T) y);
                }));
        }

        public TreatObjectAs this[Type propertyType]
        {
            get
            {
                var rules = _byFunc
                    .Select(predicate => predicate(propertyType))
                    .Where(rule => rule != TreatObjectAs.Simple)
                    .Distinct()
                    .ToList();
                if (rules.Count ==1) return rules[0];
                if (rules.Count > 1)
                    throw new InvalidOperationException(
                        $"More than one custom rule for '{propertyType}':\r\n" + 
                        string.Join("\r\n", rules));
                return TreatObjectAs.Simple;
            }
        }
    }
}