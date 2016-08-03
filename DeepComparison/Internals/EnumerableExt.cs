using System;
using System.Collections;
using static DeepComparison.ComparisonResult;

namespace DeepComparison
{
    using FCompare = Func<object, object, ComparisonResult>;

    internal static class EnumerableExt
    {
        public static ComparisonResult SequenceEqual(this IEnumerable xE, IEnumerable yE, FCompare compare)
        {
            var xEr = xE.GetEnumerator();
            var yEr = yE.GetEnumerator();
            while (xEr.MoveNext())
            {
                if (!yEr.MoveNext()) return new ComparisonResult("y collection lacks an item");
                if (!compare(xEr.Current, yEr.Current).AreEqual) return False;
            }
            return !yEr.MoveNext() ? True : False;
        }
    }
}