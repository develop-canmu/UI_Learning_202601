using System.Collections.Generic;
using CruFramework.UI;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingCardUnionInformationScrollItem : ScrollDynamicItem
    {
        /// <summary>
        /// カードユニオン情報のスクロール用データ
        /// </summary>
        public class Argument : TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem
        {            
            private long mTrainingCardId = 0;
            /// <summary>$mCharaId カードを取得する mChara のID</summary>
            public long MTrainingCardId { get {return mTrainingCardId;} }
            
            private long cardCharacterId = 0; 
            /// <summary>カードキャラID</summary>
            public long CardCharacterId { get {return cardCharacterId;} }
            
            private long mCharaId = 0;
            /// <summary>$mTrainingCardId 取得する練習カードのID</summary>
            public long MCharaId { get {return mCharaId;} }
            
            private List<long> specialLectureMembers = new List<long>();
            /// <summary>スペシャルレクチャー参加者リスト</summary>
            public List<long> SpecialLectureMembers { get {return specialLectureMembers;} }
            
            private Color backgroundColor = new Color();
            /// <summary>カード情報のベースの色</summary>
            public Color BackgroundColor { get {return backgroundColor;} }
            
            private bool isSelected = false;
            /// <summary>選出された練習メニューカードかどうか</summary>
            public bool IsSelected { get {return isSelected;} }
            
            private PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags = PracticeCardView.DisplayEnhanceUIFlags.None;
            /// <summary>表示する強化UIのフラグ</summary>
            public PracticeCardView.DisplayEnhanceUIFlags DisplayEnhanceUIFlags{ get {return displayEnhanceUIFlags;} }
            
            public Argument(long mTrainingCardId, long cardCharacterId, long mCharaId, List<long> specialLectureMembers, Color backgroundColor, bool isSelected, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
            {
                this.mTrainingCardId = mTrainingCardId;
                this.cardCharacterId = cardCharacterId;
                this.mCharaId = mCharaId;
                this.specialLectureMembers = specialLectureMembers;
                this.backgroundColor = backgroundColor;
                this.isSelected = isSelected;
                this.displayEnhanceUIFlags = displayEnhanceUIFlags;
            }
        }
        
        // カードユニオン情報のアイテム
        [SerializeField]
        private TrainingCardUnionInformationView trainingCardUnionInformationView = null;
        
        protected override void OnSetView(object value)
        {
            Argument data = (Argument)value;
            // Itemのデータをセットして表示
            trainingCardUnionInformationView.SetViewData(data.MTrainingCardId, data.CardCharacterId, data.MCharaId, data.SpecialLectureMembers, data.BackgroundColor, data.IsSelected, data.DisplayEnhanceUIFlags);
        }
    }
}


