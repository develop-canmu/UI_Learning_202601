using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.Gacha
{
    public class GachaChoiceData
    {
        private long gachaChoiceId = 0;
        /// <summary>m_gacha_choice_id</summary>
        public long GachaChoiceId { get { return gachaChoiceId; } }
        
        private DateTime releasedRecently = DateTime.MinValue;
        /// <summary>新しい要素が追加された日時</summary>
        public DateTime ReleasedRecently { get { return releasedRecently; } }
        
        private bool isSelected = false;
        /// <summary>ピックアップが選択されてるか</summary>
        public bool IsSelected { get { return isSelected; } }
        
        public GachaChoiceData(TopChoice choice)
        {
            gachaChoiceId = choice.id;
            if( !string.IsNullOrEmpty(choice.releasedRecently) ) {
                releasedRecently = DateTime.Parse(choice.releasedRecently);
            }
            isSelected = choice.isSelected > 0;
        }

        public void UpdateSelectState(bool isSelected)
        {
            this.isSelected = isSelected;
        }
    }
}