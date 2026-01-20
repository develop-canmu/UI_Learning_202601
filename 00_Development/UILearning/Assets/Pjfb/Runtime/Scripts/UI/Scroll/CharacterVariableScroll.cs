using System;
using System.Collections.Generic;
using Pjfb.Character;
using Pjfb.UserData;

namespace Pjfb
{
    public enum DeckBadgeType
    {
        None,
        Formatting,
        CurrentEditing,
        AssignedByOtherTeam,
        ReachMCharaLimit,
        CannotEdit,
    }
    public class CharacterVariableScrollData
    {
        public long id = 0;
        private long mCharaId = 0;
        public long MCharaId => mCharaId;
    

        public readonly long MasterId;
        public bool IsFavorite;
        public DeckBadgeType DeckBadgeType;
        public bool IsSelecting;
        
        public readonly Func<float> GetSelectEffectNormalizedTime;
        public SwipeableParams<CharacterVariableDetailData> SwipeableParams;
        public CharacterVariableScrollData(UserDataCharaVariable chara, Func<float> getSelectEffectNormalizedTime,
            SwipeableParams<CharacterVariableDetailData> swipeableParams,
            DeckBadgeType deckBadgeType = DeckBadgeType.None)
        {
            id = chara.id;
            mCharaId = chara.charaId;
            MasterId = chara.masterId;
            IsFavorite = chara.isLocked;
            DeckBadgeType = deckBadgeType;
            SwipeableParams = swipeableParams;
            GetSelectEffectNormalizedTime = getSelectEffectNormalizedTime;
        }
    }
    
    public abstract class CharacterVariableScroll : CharacterScrollBase<CharacterVariableScrollData>
    {

    }
}