using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    public class TrainingGetInspirationScrollItem : ScrollGridItem
    {
    
        public class Argument
        {
            private long mTrainingCardInspireid = 0;
            /// <summary>Id</summary>
            public long MTrainingCardInspireid{get{return mTrainingCardInspireid;}}
            
            private bool isNew = false;
            /// <summary>新規</summary>
            public bool IsNew{get{return isNew;}}
            
            private long count = 0;
            /// <summary>個数表示</summary>
            public long Count{get{return count;}}
            
            public Argument(long mTrainingCardInspireid, bool isNew, long count = 0)
            {
                this.mTrainingCardInspireid = mTrainingCardInspireid;
                this.isNew = isNew;
                this.count = count;
            }
        }
    
        [SerializeField]
        private TrainingGetInspirationView view = null;
        
        [SerializeField]
        private GameObject newBadge = null;

        protected override void OnSetView(object value)
        {
            Argument arg = (Argument)value;
            
            if(newBadge != null)
            {
                newBadge.SetActive(arg.IsNew);
            }
            
            view.SetInspiration(arg.MTrainingCardInspireid, arg.IsNew);
            // 数の表示
            view.SetCount(arg.Count);
            // 効果値テキストの非表示
            view.SetEnableValueText(false);
        }
    }
}