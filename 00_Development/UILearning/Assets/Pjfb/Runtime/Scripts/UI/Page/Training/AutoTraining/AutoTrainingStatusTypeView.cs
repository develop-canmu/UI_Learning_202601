using System.Collections;
using System.Collections.Generic;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class AutoTrainingStatusTypeView : MonoBehaviour
    {
        [SerializeField]
        private CancellableImage iconImage = null;
        [SerializeField]
        private TMP_Text nameText = null;
        
        /// <summary>タイプを設定</summary>
        public void SetType(long type)
        {
            // アイコン
            iconImage.SetTexture( ResourcePathManager.GetPath("AutoTrainingStatusIcon", type) );
            // 名前
            nameText.text = StringValueAssetLoader.Instance["auto_training.status_type_name" + type];
        }
    }
}