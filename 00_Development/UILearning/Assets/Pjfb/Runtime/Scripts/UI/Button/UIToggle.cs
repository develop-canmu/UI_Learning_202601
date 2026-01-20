using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class UIToggle : UnityEngine.UI.Toggle
    {
        [SerializeField]
        private SE onSoundType = SE.se_common_icon_tap;
        /// <summary>押下時の鳴らす音</summary>
        public SE OnSoundType{get{return onSoundType;}set{onSoundType = value;}}
        
        [SerializeField]
        private SE offSoundType = SE.se_common_cancel;
        /// <summary>押下時の鳴らす音</summary>
        public SE OffSoundType{get{return offSoundType;}set{offSoundType = value;}}

        protected override void Start()
        {
            base.Start();
            // SE再生イベント登録
            onValueChanged.AddListener(
                value =>
                {
                    SE se = value ? onSoundType : offSoundType;
                    SEManager.PlaySE(se);
                } 
            );
        }
    }
}
