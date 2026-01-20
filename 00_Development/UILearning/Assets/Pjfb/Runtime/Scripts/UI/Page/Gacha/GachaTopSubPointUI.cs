using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaTopSubPointUI : MonoBehaviour
    {
        [SerializeField]
        private IconImage subPointIconImage = null;
        
        [SerializeField]
        private TextMeshProUGUI subPointPossessionCountText = null;

        /// <summary>データセット</summary>
        public void SetData(GachaSubPointData data)
        {
            // 画像セット
            subPointIconImage.SetTexture(data.subPointId);
            // ポイント名
            // 所持数
            subPointPossessionCountText.text = data.value.ToString();
        }
    }
}
