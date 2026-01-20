using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    /// <summary>
    /// <see cref="JsonUtility"/> に不足している機能を提供します。
    /// JsonUtilityだと名前がかぶるためJsonHelperにする
    /// https://takap-tech.com/entry/2021/02/02/222406
    /// </summary>
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string dummy_json = $"{{\"{DummyNode<T>.RootName}\": {json}}}";

            // ダミーのルートにデシリアライズしてから中身の配列を返す
            var obj = JsonUtility.FromJson<DummyNode<T>>(dummy_json);
            return obj.Array;
        }

        // 内部で使用するダミーのルート要素
        [Serializable]
        private struct DummyNode<T>
        {
            public static readonly string RootName = nameof(array);
            
            [SerializeField]
            private T[] array;
            
            public T[] Array 
            {
                get
                {
                    return array;
                }
            }
        }
    }
}