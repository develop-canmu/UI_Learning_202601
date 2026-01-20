using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    // イベント名表示アイテム
    public class CharacterDetailEventNameScrollDynamicItem : ScrollDynamicItem
    {
        public class Param : CharacterDetailEventScrollDynamicItemSelector.IEventScrollDynamicItem
        {
            private string name;
            public string Name => name;
            
            public Param(string name)
            {
                this.name = name;
            }
        }
        
        // 名前表示ラベル
        [SerializeField]
        private CharacterEventNameLabel nameLabelPrefab = null;
        
        protected override void OnSetView(object value)
        {
            Param param = (Param) value;
            nameLabelPrefab.SetName(param.Name);
        }
    }
}