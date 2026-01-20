using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.Gacha
{
    public class GachaRequiredPointView : MonoBehaviour
    {
        [SerializeField] private IconImage pointIconImage;
        [SerializeField] private TextMeshProUGUI prizeCountText;

        public void SetView(long pointId, long value)
        {
            pointIconImage.SetTexture(pointId);
            prizeCountText.text = value.ToString();
        }
    }
}