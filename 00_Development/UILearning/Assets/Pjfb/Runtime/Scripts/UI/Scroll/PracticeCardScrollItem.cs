using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Training;
using UnityEngine;

namespace Pjfb.Character
{
    public class PracticeCardScrollItem : ScrollGridItem
    {
    
        public  class Arguments
        {
            
            private long cardId = 0;
            /// <summary>カードId</summary>
            public long CardId{get{return cardId;}}
            
            private long cardCharacterId = 0;
            /// <summary>TrainingCardCharaId</summary>
            public  long CardCharacterId{get{return cardCharacterId;}}
        
            private long characterId = 0;
            /// <summary>キャラId</summary>
            public  long CharacterId{get{return characterId;}}
            
            private long characterLv = 0;
            /// <summary>キャラのLv</summary>
            public long CharacterLv{get{return characterLv;}}
            
            private long count = 0;
            /// <summary>回数</summary>
            public long Count{get{return count;}}

            private PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags = PracticeCardView.DisplayEnhanceUIFlags.None;
            /// <summary>表示する強化UIのフラグ</summary>
            public PracticeCardView.DisplayEnhanceUIFlags DisplayEnhanceUIFlags{get{return displayEnhanceUIFlags;}}
            
            public Arguments(long cardCharacterId, long characterId, long cardId, long characterLv, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
            {
                this.cardId = cardId;
                this.cardCharacterId = cardCharacterId;
                this.characterId = characterId;
                this.characterLv = characterLv;
                this.displayEnhanceUIFlags = displayEnhanceUIFlags;
            }
            
            public Arguments(long characterId, long cardId, long count)
            {
                this.characterId = characterId;
                this.cardId = cardId;
                this.count = count;
                cardCharacterId = -1;
                characterLv = -1;
                displayEnhanceUIFlags =  PracticeCardView.DisplayEnhanceUIFlags.None;
            }
        }
    
        [SerializeField] private PracticeCardView practiceCardView;

        protected override void OnSetView(object value)
        {
            Arguments args = (Arguments)value;
            
            practiceCardView.SetCard(
                args.CardId,
                args.CardCharacterId,
                args.CharacterId,
                args.CharacterLv,
                args.DisplayEnhanceUIFlags
            );
        }

        public void SetCardData(object value)
        {
            OnSetView(value);
        }
    }
}


