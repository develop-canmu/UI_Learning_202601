using UnityEngine;
using TMPro;

namespace Pjfb
{
    public class SuccessCharacterSortFilterRankToggle : MonoBehaviour
    {
        [SerializeField]
        private UIToggle toggle;
        public bool IsOn => toggle.isOn;
        
        [SerializeField]
        private TextMeshProUGUI rankNameText;
        
        /// <summary>トグルの状態を設定</summary>
        public void SetIsOnWithoutNotify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }
        
        /// <summary>表示名設定</summary>
        public void SetName(string rankName)
        {
            rankNameText.text = rankName;
#if UNITY_EDITOR
            gameObject.name = rankName;
#endif
        }
    }
}