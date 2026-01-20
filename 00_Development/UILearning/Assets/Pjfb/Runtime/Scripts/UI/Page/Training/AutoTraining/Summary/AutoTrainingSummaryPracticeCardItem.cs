using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using TMPro;

using Pjfb.Common;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryPracticeCardItem : ScrollGridItem
    {
        
        public class Arguments
        {
            private long cardId = 0;
            /// <summary>カードId</summary>
            public long CardId{get{return cardId;}}

            private long characterId = 0;
            /// <summary>キャラId</summary>
            public long CharacterId{get{return characterId;}}
    
            private long count = 0;
            /// <summary>回数</summary>
            public long Count{get{return count;}}
    
            private long enhanceLevel = 0;
            /// <summary>強化レベル</summary>
            public long EnhanceLevel{get{return enhanceLevel;}}
    
            private long cardCharaId = 0;
            /// <summary>カードキャラID</summary>
            public long CardCharaId{get{return cardCharaId;}}
    
            public Arguments(long characterId, long cardId, long count, long enhanceLevel, long cardCharaId)
            {
                this.characterId = characterId;
                this.cardId = cardId;
                this.count = count;
                this.enhanceLevel = enhanceLevel;
                this.cardCharaId = cardCharaId;
            }
        }
        
        [SerializeField]
        private PracticeCardView practiceCardView = null;
        [SerializeField]
        private IconImage characterIcon = null;
        [SerializeField]
        private TMP_Text countText = null;
        
        [SerializeField]
        private GameObject characterIconRoot = null;
        
        
        Arguments args = null;
        
        protected override void OnSetView(object value)
        {
            args = (Arguments)value;
            
            practiceCardView.SetCard(
                args.CardId,
                args.CardCharaId,
                args.CharacterId,
                -1,
                PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.DetailLabel);

            // キャラアイコン
            if(args.CharacterId > 0)
            {
                characterIconRoot.SetActive(true);
                characterIcon.SetTexture(args.CharacterId);
            }
            else
            {
                characterIconRoot.SetActive(false);
            }
            // カウント
            countText.text = string.Format(StringValueAssetLoader.Instance["auto_training.summary.count"], args.Count);
        }
 
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnLongTap()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingCardReward, new TrainingCardRewardModal.Arguments(practiceCardView.MCard, args.CharacterId, -1, practiceCardView.CardChara, true, false));
        }
    }
}