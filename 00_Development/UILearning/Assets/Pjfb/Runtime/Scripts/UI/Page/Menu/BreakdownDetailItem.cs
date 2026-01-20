using System;
using UnityEngine;
using Pjfb.Extensions;
using TMPro;

namespace Pjfb.Menu
{
    public class BreakdownDetailItem : MonoBehaviour
    {
        public class BreakdownDetailItemData
        {
            private string date;
            public string Date => date;
            private string use;
            public string Use => use;
            private string num;
            public string Num => num;

            public BreakdownDetailItemData(string date, string num, string use = null)
            {
                this.date = date;
                this.use = use;
                this.num = num;
            }
        }
        
        [SerializeField] private TextMeshProUGUI dateText = default;
        [SerializeField] private TextMeshProUGUI useText = default;
        [SerializeField] private TextMeshProUGUI numText = default;
        [SerializeField] private GameObject line = default;

        private readonly string colorDefault = "default";
        private readonly string colorWarning = "warning";
        
        public void SetItem(BreakdownDetailItemData data, bool isIncome = true)
        {
            dateText.text = data.Date;
            numText.text = data.Num;
            numText.color = isIncome ? ColorValueAssetLoader.Instance[colorDefault] : ColorValueAssetLoader.Instance[colorWarning];
            if(data.Use != null)
            {
                useText.text = data.Use;
                useText.gameObject.SetActive(true);
            }
            else
            {
                useText.gameObject.SetActive(false);
            }
            line.SetActive(true);
        }

        public void HideLine()
        {
            line.SetActive(false);
        }
    }
}