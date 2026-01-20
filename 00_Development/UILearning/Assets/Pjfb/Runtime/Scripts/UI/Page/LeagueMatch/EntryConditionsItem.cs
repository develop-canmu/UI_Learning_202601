using CruFramework.UI;
using UnityEngine;
using TMPro;

namespace Pjfb.LeagueMatchTournament
{
    public class EntryConditionsItem : ScrollGridItem
    {
        public class EntryConditionItemData
        {
            // 条件タイプ
            private string conditionKey = string.Empty;
            public string ConditionKey => conditionKey;
            // クラブの状況
            private string clubStatus = string.Empty;
            public string ClubStatus => clubStatus;
            // 達成状況
            private bool isComplete = false;
            public bool IsComplete => isComplete;
            public EntryConditionItemData(string conditionKey, string clubStatus, bool isComplete)
            {
                this.conditionKey = conditionKey;
                this.clubStatus = clubStatus;
                this.isComplete = isComplete;
            }
        }
        
        // 参加条件
        [SerializeField] 
        private TextMeshProUGUI entryConditionNameText = null;
        // クラブの状況
        [SerializeField]
        private TextMeshProUGUI clubStatus = null;
        // エントリー条件の達成状況
        [SerializeField]
        private UIToggle entryConditionToggle = null;
        // エントリー条件省略表記
        [SerializeField]
        private OmissionTextSetter entryConditionOmissionTextSetter = null;
        public OmissionTextSetter EntryConditionOmissionTextSetter => entryConditionOmissionTextSetter;
        // クラブ状況の省略表記
        [SerializeField]
        private OmissionTextSetter clubStatusOmissionTextSetter = null;
        public OmissionTextSetter ClubStatusOmissionTextSetter => clubStatusOmissionTextSetter;
        
        /// <summary>エントリー条件の達成状況表示</summary>
        private void UpdateView(EntryConditionItemData data)
        {
            entryConditionNameText.text = data.ConditionKey;
            clubStatus.text = data.ClubStatus;
            entryConditionToggle.isOn = data.IsComplete;
        }

        protected override void OnSetView(object value)
        {
            EntryConditionItemData data = (EntryConditionItemData)value;
            UpdateView(data);
        }
    }
}