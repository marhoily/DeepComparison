using System;
using System.Linq;
using System.Reflection;
using static System.Reflection.BindingFlags;
using static DeepComparison.ComparisonResult;

namespace DeepComparison
{
    internal sealed class ObjectExpander
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;

        public void SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
        }
        public ComparisonResult CompareProperties(object x, object y, 
            Type formalType, Func<object, object, Type, ComparisonResult> comparer)
        {
            if (x == null && y == null) return True;
            if (x == null || y == null) return False;
            return formalType
                .GetProperties(Instance | Public | NonPublic) 
                .Where(_propSelector)
                .Select(p => comparer(
                    p.GetValue(x, null),
                    p.GetValue(y, null),
                    p.PropertyType))
                .FirstOrDefault(c => !c.AreEqual) ?? True;
        }
    }
}