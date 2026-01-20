using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MiniJSON;

namespace Pjfb
{
    /// <summary>
    /// オブジェクトを JSON 形式にシリアライズしたり、逆に、JSON 形式の文字列を Dictionary 形式にデシリアライズする Utility
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// オブジェクトを JSON 形式の文字列に変換する
        /// </summary>
        /// <returns>JSON 形式の文字列</returns>
        /// <param name="obj">対象のオブジェクト</param>
        public static string ToJson(object obj)
        {
            return Json.Serialize(obj);
        }

        /// <summary>
        /// JSON 形式の文字列をオブジェクトに変換する\n
        /// 変換できなかった場合、null を返す
        /// </summary>
        /// <returns>結果のオブジェクト</returns>
        /// <param name="json">対象の JSON 文字列</param>
        public static object Parse(string json)
        {
           if (String.IsNullOrEmpty(json))
           {
                Debug.LogWarning("requested json is null");
                return null;
           }
           try 
           { 
               return Json.Deserialize(json);
           } 
           catch (Exception e)
           {
               Debug.LogWarning(e + "\njson:\n" + json); 
           }
           return null;
        }

        /// <summary>
        /// JSON 形式の文字列を、与えられた型のオブジェクトに変換する\n
        /// 変換できなかった場合、null を返す
        /// </summary>
        /// <returns>変換後のオブジェクト</returns>
        /// <param name="json">変換対象のJSON形式の文字列</param>
        /// <typeparam name="T">変換したい型</typeparam>
        public static T Parse<T>(string json) where T : class
        {
            return Parse(json) as T;
        }

        /// <summary>
        /// JSON 形式の文字列を、与えられた型の配列に変換する\n
        /// 変換できなかった場合、null を返す\n
        /// 配列の各成分に対して、与えられた型に変換できなかった場合、その成分は null となります。
        /// </summary>
        /// <returns>変換後の配列</returns>
        /// <param name="json">変換したいJSON形式の文字列</param>
        /// <typeparam name="T">各成分の型</typeparam>
        public static T[] ParseList<T>(string json) where T : class
        {
            T[] res = null;

            if (json != null)
            {
                var list = Parse<List<object>>(json);

                if (list != null)
                {
                    res = new T[list.Count];

                    for (int i = 0; i < list.Count; i++)
                    {
                        res[i] = list[i] as T;
                    }
                }
            }
            
            return res;
        }

        /// <summary>
        /// JSON 形式の文字列を、与えられた型の配列に変換する\n
        /// 変換できなかった場合、null を返す\n
        /// 配列の各成分に対して、与えられた型に変換できなかった場合、その成分は null となります。
        /// </summary>
        /// <returns>変換後の配列</returns>
        /// <param name="json">変換したいJSON形式の文字列</param>
        /// <typeparam name="T">各成分の型</typeparam>
        public static int[] ParseIntList(string json)
        {
            int[] res = null;

            var list = Parse<List<object>>(json);

            if (list != null)
            {
                res = new int[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    res[i] = (int)list[i];
                }
            }

            return res;
        }

        /// <summary>
        /// JSON 形式の文字列を、文字列の配列に変換する\n
        /// 変換できない場合 null を返す
        /// </summary>
        /// <returns>変換した配列</returns>
        /// <param name="json">変換対象の JSON 形式の文字列</param>
        public static string[] ParseStringList(string json)
        {
            return ParseList<string>(json);
        }

        public struct CustomParsePropertyParam
        {
            public string propertyName;
            public Func<PropertyInfo, object, object> parseFunc;

            public CustomParsePropertyParam(string _propertyName, Func<PropertyInfo, object, object> _parseFunc)
            {
                this.propertyName = _propertyName;
                this.parseFunc = _parseFunc;
            }
        }

        public static T[] FromJson<T>(string json)
        {
            var jsonStr = "{\"list\": " + json + "}";
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonStr);

            return wrapper.list;
        }

        [Serializable]
        public class Wrapper<T>
        {
            public T[] list;
        }
    }   
}

