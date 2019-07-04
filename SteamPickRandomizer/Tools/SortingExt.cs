using System;
using System.Collections.Generic;

namespace RandomizedSteamPick.Tools
{
    public static class SortingExt
    {
        public static int InsertSorted<T>(this List<T> @this, T item) where T : IComparable<T>
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return 0;
            }
            if (@this[@this.Count - 1].CompareTo(item) <= 0)
            {
                @this.Add(item);
                return @this.Count - 1;
            }
            if (@this[0].CompareTo(item) >= 0)
            {
                @this.Insert(0, item);
                return 0;
            }
            int index = @this.BinarySearch(item);
            if (index < 0)
                index = ~index;
            @this.Insert(index, item);
            return index;
        }

        public static int InsertSorted<T>(this List<T> @this, T item, IComparer<T> comparator)
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return 0;
            }
            if (comparator.Compare(@this[@this.Count - 1], item) <= 0)
            {
                @this.Add(item);
                return @this.Count - 1;
            }
            if (comparator.Compare(@this[0], item) >= 0)
            {
                @this.Insert(0, item);
                return 0;
            }
            int index = @this.BinarySearch(item, comparator);
            if (index < 0)
                index = ~index;
            @this.Insert(index, item);
            return index;
        }
    }
}
