

///////////////////////////////
// ScrollDynamicへ生成するItemプレハブを選択する処理を書く
// サンプルではインスペクタでアイテムリストを作成してその中から返す
//////////////////////////////

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using UnityEngine;

namespace CruFramework.Sample
{
    public class SampleScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        
        [SerializeField]
        private ScrollDynamicItem[] items = null;
        
        /// <summary>
        /// ScrollDyminac::SetItemsに渡した値が入ってるのでそれをプレハブを選択する
        /// </summary>
        public override ScrollDynamicItem GetItem(object item)
        {
            int index = (int)item;
            return items[index % items.Length];
        }
    }
}

#endif