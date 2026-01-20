using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    public class TrainingInspirationCardList
    {
        public class InspirationData
        {
            private long id = 0;
            /// <summary>Id</summary>
            public long Id{get{return id;}}
            
            private bool isNew = false;
            /// <summary>新規獲得</summary>
            public bool IsNew{get{return isNew;}}
            
            public InspirationData(long id, bool isNew)
            {
                this.id = id;
                this.isNew = isNew;
            }
        }
    
        private long mTrainingCardId = 0;
        /// <summary>練習カードId</summary>
        public  long MTrainingCardId{get{return mTrainingCardId;}}
        
        private long mCharId = 0;
        /// <summary>キャラId</summary>
        public  long MCharId{get{return mCharId;}}
        
        private long mTrainingCardCharaId = 0;
        /// <summary>カードキャラId</summary>
        public  long MTrainingCardCharaId{get{return mTrainingCardCharaId;}}
        
        private InspirationData[] inspirations = null;
        /// <summary>インスピレーション</summary>
        public InspirationData[] Inspirations{get{return inspirations;}}
        
        private PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags = PracticeCardView.DisplayEnhanceUIFlags.None;
        /// <summary>表示する強化UIのフラグ</summary>
        public PracticeCardView.DisplayEnhanceUIFlags DisplayEnhanceUIFlags{get{return displayEnhanceUIFlags;}}
        
        public TrainingInspirationCardList(long mTrainingCardId, long mCharId, long mTrainingCardCharaId, InspirationData[] inspirations, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            this.mTrainingCardId = mTrainingCardId;
            this.mCharId = mCharId;
            this.mTrainingCardCharaId = mTrainingCardCharaId;
            this.inspirations = inspirations;
            this.displayEnhanceUIFlags = displayEnhanceUIFlags;
        }
    }
}