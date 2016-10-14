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
                if (!yEr.MoveNext())
                    return new ComparisonResult("Second collection lacks an item: "+ xEr.Current);
                var c = compare(xEr.Current, yEr.Current);
                if (!c.AreEqual)
                    return c;
            }
            if (yEr.MoveNext())
                return new ComparisonResult("First collection lacks an item " + yEr.Current);
            return True;
        }
    }
}