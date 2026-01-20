using CruFramework.UI;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{ 
    public class SupportCharacterIconScrollData
    {
        public long MCharaId { get; }
        public long Level { get; }
        public long LiberationLevel { get; }
        public bool IsSpecialAttack { get; }
        public SwipeableParams<CharacterDetailData> SwipeableParams { get; }
        
        public SupportCharacterIconScrollData(long mCharaId, long level, long liberationLevel, bool isSpecialAttack, SwipeableParams<CharacterDetailData> swipeableParams)
        {
            MCharaId = mCharaId;
            Level = level;
            LiberationLevel = liberationLevel;
            IsSpecialAttack = isSpecialAttack;
            SwipeableParams = swipeableParams;
        }
    }
    
    public class SupportCharacterIconScrollItem : ScrollGridItem
    {
        [SerializeField]
        private CharacterIcon icon = null;
        
        [SerializeField]
        private GameObject specialAttack = null;
        
        private SupportCharacterIconScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (SupportCharacterIconScrollData)value;
            // アイコン
            icon.SetIcon(scrollData.MCharaId, scrollData.Level, scrollData.LiberationLevel);
            icon.SwipeableParams = scrollData.SwipeableParams;
            // 特攻
            specialAttack.SetActive(scrollData.IsSpecialAttack);
        }
    }
}