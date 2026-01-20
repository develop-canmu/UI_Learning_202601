using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{
    public class StoryScenarioItem : ScrollGridItem
    {
        
        [SerializeField]
        private TMPro.TMP_Text titleText = null;
        
        [SerializeField]
        private TMPro.TMP_Text subTitleText = null;
        
        private long storyId = 0;
        
        protected override void OnSetView(object value)
        {
            storyId = (long)value;
            // マスタ
            HuntEnemyMasterObject mStory = MasterManager.Instance.huntEnemyMaster.FindData(storyId);
            // 名前
            titleText.text = mStory.name;
            subTitleText.text = mStory.subName;
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelected()
        {
            TriggerEvent(storyId);
        }
    }
}