using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;


using TMPro;

namespace Pjfb
{
    public class HowToPlayScrollItem : ScrollGridItem
    {
        [SerializeField]
        private RubyTextMeshProUGUI descriptionText = null;
        [SerializeField]
        private CancellableRawImage bannerImage = null;
    
        protected override void OnSetView(object value)
        {
            HowToPlayModal.DescriptionData data = (HowToPlayModal.DescriptionData)value;
            // アクティブをONの状態にしないとルビがバグる
            gameObject.SetActive(true);
            // 説明
            descriptionText.UnditedText = data.Description;
            // バナー
            bannerImage.SetTexture(data.ImageAddress);
        }
    }
}