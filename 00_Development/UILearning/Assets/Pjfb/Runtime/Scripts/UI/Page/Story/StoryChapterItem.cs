using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{
    public class StoryChapterItem : ScrollGridItem
    {
        
        // private StoryChapterData chapterData = null;
        
        protected override void OnSetView(object value)
        {
            // chapterData = (StoryChapterData)value;
            //
            // HuntStageMasterObject mChapter = MasterManager.Instance.huntStageMaster.FindData(chapterData.StageId);

            // 名前
            // chapterNameText.text = mChapter.name;
            // // サブタイトル
            // subNameText.text = mChapter.subName;
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelected()
        {
            // TriggerEvent(chapterData);
        }
    }
}