using System.Collections.Generic;

namespace PolyQA.Extensions
{
    public static class ListExtensions
    {
        public static bool AddUnique<T>(this List<T> list, T item)
        {
            if (list.Contains(item)) return false;
            list.Add(item);
            return true;
        }
    }
}