using CruFramework.UI;
using TMPro;
using UnityEngine;


namespace Pjfb
{
    public class RarityLimitationScrollItem : ScrollGridItem
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
     
        // 受け渡しクラス
        public class Data
        {
            // タイトル
            public string displayName { get; }
            // 内容
            public string description { get; }
            
            public Data(string displayName, string description)
            {
                this.displayName = displayName;
                this.description = description;
            }
        }

        private Data data = null;
        
        protected override void OnSetView(object value)
        {
            data = (Data)value;
            titleText.text = data.displayName;
            bodyText.text = data.description;
        }
    }
}