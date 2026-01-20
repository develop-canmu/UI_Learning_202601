using TMPro;
using UnityEngine;

namespace Pjfb
{
    /// <summary>
    /// 絞り込み/ソートモーダルにおける各カテゴリ内に表示するトグルのベースクラス
    /// </summary>
    public class SortFilterToggle : MonoBehaviour
    {
        /// <summary>トグル</summary>
        [SerializeField]
        private UIToggle toggle;

        /// <summary>トグルボタンの隣に表示する項目名</summary>
        [SerializeField]
        private TextMeshProUGUI toggleName;
        
        /// <summary>トグルのON/OFFを取得するgetter</summary>
        public bool IsOn => toggle.isOn;

        /// <summary>トグルのON/OFFをセット</summary>
        public void SetIsOnWithoutNotify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }

        /// <summary>トグルの名前をセット</summary>
        public void SetName(string toggleName)
        {
            this.toggleName.text = toggleName;

#if UNITY_EDITOR
            // エディタ上で確認しやすいように設定
            gameObject.name = toggleName;
#endif
        }
    }
}