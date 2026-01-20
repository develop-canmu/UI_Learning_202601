using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CharacterDetailEventScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        // 表示するアイテムを共通化するためのインターフェース
        public interface IEventScrollDynamicItem
        {
            
        }
        
        // イベント表示Item
        [SerializeField] private ScrollDynamicItem eventViewItem;
        // イベント名表示ラベル
        [SerializeField] private ScrollDynamicItem nameLabelItem;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            // わたってきたデータによってセットするアイテムを変える
            // イベント名表示データなら
            if (item is CharacterDetailEventScrollDynamicItem.Param)
            {
                return eventViewItem;
            }
            else if(item is CharacterDetailEventNameScrollDynamicItem.Param)
            {
                return nameLabelItem;
            }
            
            // 見つからないタイプならエラーを出す
            CruFramework.Logger.LogError($"Not Find Type : {item.GetType()}");
            return null;
        }
    }
}