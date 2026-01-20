using System;
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Extensions
{
    public static class ListExtensions
    {
        public static T PopLast<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty()) throw new InvalidOperationException();
            var t = list[^1];
            list.RemoveAt(list.Count - 1);

            return t;
        }

        /// <summary>
        /// 条件によって排除する
        /// </summary>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, Func<T, bool> condition)
        {
            return enumerable.Where(aData => !condition(aData));
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var aData in enumerable) action(aData);
        }

        public static string ToCsv<T>(this List<T> list, string separator = ",")
        {
            var retVal = string.Empty;
            list?.ForEach(aData => retVal += aData + separator );
            if (retVal.Length > 0) retVal = retVal[..^separator.Length];
            return retVal;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// リスト順番をずらす
        /// 参考: <see cref="ListExtensionTest.ShiftOrder"/>
        /// </summary>
        public static List<T> ShiftOder<T>(this List<T> list, int startIndex)
        {
            if (list.IsNullOrEmpty()) return list;
            
            if (startIndex < 0)  startIndex += (int) MathF.Ceiling((float)-startIndex / list.Count) * list.Count;
            startIndex %= list.Count;
            
            list.AddRange(list.GetRange(0, startIndex));
            list.RemoveRange(0, startIndex);
            return list;
        }

        public static Dictionary<T, List<Y>> ToDictionaryOfList<T, Y>(this IEnumerable <Y> list, Func<Y, T> key)
        {
            return list.GroupBy(key).ToDictionary(anItem => anItem.Key, anItem => anItem.ToList());
        }
    }
}