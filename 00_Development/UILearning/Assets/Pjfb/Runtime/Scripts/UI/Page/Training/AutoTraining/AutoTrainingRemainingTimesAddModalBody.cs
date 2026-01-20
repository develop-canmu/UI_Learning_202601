using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Common;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingRemainingTimesAddModalBody : MonoBehaviour
    {
        [SerializeField] private PossessionItemUi requiredItemUi;
        [SerializeField] private TextMeshProUGUI staminaBeforeText;
        [SerializeField] private TextMeshProUGUI staminaAfterText;
        [SerializeField] private TextMeshProUGUI textBody;
        [SerializeField] [ColorValue] private string colorKey;

        public void SetItemView(long pointId, long beforeValue, long afterValue)
        {
            requiredItemUi.SetAfterCount(pointId, beforeValue, afterValue);
            Color color = ColorValueAssetLoader.Instance[colorKey];
            requiredItemUi.SetColor(color);
        }

        public void SetStaminaView(long beforeValue, long afterValue)
        {
            staminaBeforeText.text = beforeValue.ToString();
            staminaAfterText.text = afterValue.ToString();
        }

        public void SetTextBody(string text)
        {
            textBody.text = text;
        }
        
    }
}