using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaPageArgs
    {
        private long focusGachaSettingId = 0;
        /// <summary>最初に表示するガチャId</summary>
        public long FocusGachaSettingId 
        { 
            get { return focusGachaSettingId; }
            set { focusGachaSettingId = value; } 
        }

        private bool focusTicketGacha = false;
        /// <summary>focusGachaSettingIdが見つからない場合にチケットガチャをフォーカスするか。</summary>
        public bool FocusTicketGacha 
        { 
            get { return focusTicketGacha; }
            set { focusTicketGacha = value; } 
        }
        
        public GachaPageArgs() {}

        public GachaPageArgs(long gachaSettingId)
        {
            focusGachaSettingId = gachaSettingId;
        }
    }
}
