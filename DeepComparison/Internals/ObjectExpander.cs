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
        public ComparisonResult CompareProperties(object x, object y, Type formalType, Func<object, object, Type, ComparisonResult> comparer, Formatting formatting)
        {
            if (x == null && y == null) return True;
            if (x == null || y == null)
                return formatting.Explain(x, y, "comparePropertiesOf");
            foreach (var p in formalType.GetProperties(Instance | Public | NonPublic).Where(_propSelector))
            {
                var c = comparer(
                    p.GetValue(x, null), p.GetValue(y, null), 
                    p.PropertyType);

                if (!c.AreEqual)
                {
                    return c.WithPath(p);
                }
            }
            return True;
        }
    }
}