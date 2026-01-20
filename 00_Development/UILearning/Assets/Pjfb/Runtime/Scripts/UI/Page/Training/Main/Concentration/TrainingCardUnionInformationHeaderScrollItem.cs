using CruFramework.UI;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingCardUnionInformationHeaderScrollItem : ScrollDynamicItem
    {
        /// <summary>
        /// カードユニオン情報見出しのスクロール用データ
        /// </summary>
        public class Argument : TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem
        {
            private string headerTitle = null;
            /// <summary>ヘッダータイトル</summary>
            public string HeaderTitle { get {return headerTitle;} }

            public Argument(string headerTitle)
            {
                this.headerTitle = headerTitle;
            }
        }
         
        // カテゴリ別名称用テキスト(ヘッダーテキスト)
        [SerializeField] 
        private TextMeshProUGUI headerTitleText = null;
        
        protected override void OnSetView(object value)
        {
            Argument data = (Argument)value;
            // ヘッダーのタイトルを設定する
            headerTitleText.text = data.HeaderTitle;
        }
    }
}