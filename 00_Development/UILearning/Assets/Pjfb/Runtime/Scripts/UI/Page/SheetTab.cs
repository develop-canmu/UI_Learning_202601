using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using UnityEngine.UI;

namespace Pjfb
{
    
    public class SheetTab : MonoBehaviour
    {
        [SerializeField]
        private Image activeImage = null;
        [SerializeField]
        private Image deactiveImage = null;
        [SerializeField]
        private TMPro.TMP_Text text = null;
                
        [SerializeField]
        private SE se = SE.se_common_icon_tap;
        /// <summary>シートを開いたときの効果音</summary>
        public SE Se{get{return se;}}
        
        // アクティブ状態
        public void SetActiveState(bool active)
        {
            // TODO 共通部分修正でエラーが出るので一旦エラー回避
            if(activeImage != null && deactiveImage != null)
            {
                activeImage.gameObject.SetActive(active);
                deactiveImage.gameObject.SetActive(!active);
            }

            string key = active ? "tab.selected_text" : "tab.deselect_text";
            text.color = ColorValueAssetLoader.Instance[key];
        }
    }
}
