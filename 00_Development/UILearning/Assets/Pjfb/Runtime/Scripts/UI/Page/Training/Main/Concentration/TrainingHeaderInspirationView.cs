using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingHeaderInspirationView : MonoBehaviour
    {
    
        [SerializeField]
        private Slider expSlider = null;
        [SerializeField]
        private TMPro.TMP_Text lvText = null;

        /// <summary>表示</summary>
        public void SetView(TrainingMainArguments arguments)
        {
            // レベル
            TrainingCardInspireLevelMasterObject currentLv = arguments.GetInspirationLv(out TrainingCardInspireLevelMasterObject nextLv);
            
            // 現在のレベル表示
            lvText.text = string.Format( StringValueAssetLoader.Instance["training.inspiration_lv"], currentLv.level);
            
            // 経験値
            // 最大レベル
            if(nextLv == null)
            {
                expSlider.value = 1.0f;
            }
            // 次のレベルまでの経験値
            else
            {
                expSlider.value = (float)(arguments.Pending.inspireExp - currentLv.exp) / (float)(nextLv.exp - currentLv.exp);
            }
        }
    }
}