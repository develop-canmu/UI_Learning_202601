using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Pjfb
{
    public class UIButton : CruFramework.UI.UIButton
    {
        [SerializeField]
        private SE soundType = SE.None;
        /// <summary>押下時の鳴らす音</summary>
        public SE SoundType{get{return soundType;}set{soundType = value;}}
        
        /// <summary>ページを戻る</summary>
        public void BackPage()
        {
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }
        
        protected override void OnTriggerLongTap()
        {
            bool isInteractable = IsInteractable();
            // イベントなし
            if(!isInteractable || GetLongTapEventCount() <= 0)return;
            SEManager.PlaySE(soundType);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            bool isInteractable = IsInteractable();
            base.OnPointerClick(eventData);
            if(!isInteractable || GetClickEventCount() <= 0)return;
            SEManager.PlaySE(soundType);
        }
    }
}