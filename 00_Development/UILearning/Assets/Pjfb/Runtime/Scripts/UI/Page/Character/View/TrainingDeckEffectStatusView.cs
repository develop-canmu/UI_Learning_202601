using System.Collections;
using System.Collections.Generic;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb
{
    public class TrainingDeckEffectStatusView : MonoBehaviour
    {
        [SerializeField] private CancellableImage iconImage;
        // %の表示テキスト
        [SerializeField] private TMP_Text textMagnification;
        // 実数の表示
        [SerializeField] private TMP_Text textActualValue;
        
        public void SetStatus(BuffIconData iconData, Color realColor, Color percentColor)
        {
            // アイコン
            iconImage.SetTexture( ResourcePathManager.GetPath("TrainingStatusTypeDetailBuffIcon", iconData.BuffIconId) );
            
            // 効果値が0以下なら表示しない
            textMagnification.gameObject.SetActive(iconData.PercentValue > 0);
            textActualValue.gameObject.SetActive(iconData.RealValue > 0);
            
            textMagnification.text = string.Format(StringValueAssetLoader.Instance["training.deckEnhance.buffStatus"], iconData.PercentValueString);
            textMagnification.color = percentColor; 
            
            textActualValue.text = string.Format(StringValueAssetLoader.Instance["training.deckEnhance.buffStatus"], string.Format("{0:#,0}",iconData.RealValue));
            textActualValue.color = realColor;
        }
    }
}