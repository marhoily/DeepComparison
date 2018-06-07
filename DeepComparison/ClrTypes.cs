using System;
using System.Collections.Generic;

namespace DeepComparison
{
    /// <summary>Contains lists of CLR types</summary>
    public static class ClrTypes
    {
        private static readonly HashSet<Type> Basic = new HashSet<Type>
        {
            typeof(string),
            typeof(Guid),
            typeof(Uri),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan)
        };

        /// <summary>
        /// List of types that have "compare by value" semantic + reasonable ToString.
        /// It makes sense to not GoDeepFor them
        /// </summary>
        public static Func<Type, bool> AllExceptBasic => t => !t.IsPrimitive && !Basic.Contains(t);
    }
}