using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class GrowthLiberationTabSheetLiberation : GrowthLiberationTabSheetBase
    {
        private const int LiberationItemCount = 1;
        
        [SerializeField] protected CharacterPieceIcon characterPieceIcon;
        
        private Dictionary<long, EnhanceLevelMasterObject> enhanceLevelCache;

        private long possessionValue = 0;
        private long requiredValue = 0;
        
        protected override long currentLv => uChara.newLiberationLevel;

        public override void Initialize(long id)
        {
            base.Initialize(id);
            mElementId = MChara.mEnhanceIdLiberation;
        }


        protected override void SetDictionary()
        {
            enhanceLevelCache = uChara.MEnhanceLevelList.ToDictionary(o => o.level);
        }

        protected override void SetMaxLevel()
        {
            maxLevel = enhanceLevelCache.Count > 0 ? enhanceLevelCache.Keys.Max() : 0;
        }
        
        protected override void SetRequiredItem()
        {
            var currentMEnhanceLevel = enhanceLevelCache[currentLv];
            var afterMEnhanceLevel = enhanceLevelCache[afterLv];
            if (currentMEnhanceLevel == null || afterMEnhanceLevel == null || IsMaxLevel)
            {
                characterPieceIcon.gameObject.SetActive(false);
                return;
            }

            characterPieceIcon.gameObject.SetActive(true);
            var charaPiece = UserDataManager.Instance.charaPiece.data.FirstOrDefault(data => data.Key == uChara.charaId).Value;
            possessionValue = charaPiece?.value ?? 0;
            requiredValue = afterMEnhanceLevel.totalExp - currentMEnhanceLevel.totalExp;
            characterPieceIcon.SetIconId(uChara.charaId);
            characterPieceIcon.SetCountDigitString(possessionValue, requiredValue);
            canLevelUp = CanLevelUp(afterLv);
        }
        
        /// <summary>
        /// 必要アイテムと所持アイテムを比較して強化できるかを確認する
        /// </summary>
        protected override bool CanLevelUp(long lv)
        {
            if (IsMaxLevel) return false;
            if (!enhanceLevelCache.ContainsKey(currentLv))
            {
                Debug.LogError($"Not found level {currentLv} EnhanceLevelMasterObject data");
                return false;
            }
            if (!enhanceLevelCache.ContainsKey(lv))
            {
                Debug.LogError($"Not found level {lv} EnhanceLevelMasterObject data");
                return false;
            }
            EnhanceLevelMasterObject currentMEnhanceLevel = enhanceLevelCache[currentLv];
            EnhanceLevelMasterObject afterMEnhanceLevel = enhanceLevelCache[lv];
            if (currentMEnhanceLevel == null || afterMEnhanceLevel == null) return false;
            var charaPiece = UserDataManager.Instance.charaPiece.data.FirstOrDefault(data => data.Key == uChara.charaId).Value;
            possessionValue = charaPiece?.value ?? 0;
            requiredValue = afterMEnhanceLevel.totalExp - currentMEnhanceLevel.totalExp;
            if (possessionValue < requiredValue) return false;
            return true;
        }
        

        public override async void OnClickConfirmButton()
        {
            var confirmWindow = await CharacterLiberationConfirmModal.OpenAsync(
                ModalType.CharacterLiberationConfirm
                ,new CharacterLiberationConfirmModal.LiberationData(
                    userCharacterId: userCharacterId,
                    mCharaId: MChara.id,
                    currentLv : currentLv,
                    afterLv : afterLv,
                    currentMaxGrowthLv:uChara.GetMaxGrowthLevel(),
                    afterMaxGrowthLv:uChara.GetMaxGrowthLevel(afterLv),
                    possessionValue,
                    requiredValue),
                this.GetCancellationTokenOnDestroy());

            var response = (CharaLiberationAPIResponse) await confirmWindow.WaitCloseAsync();
            
            if (response != null)
            {
                OnPlayEffect?.Invoke(GrowthLiberationTabSheetType.Liberation, MChara.id, currentLv, afterLv,
                    response.autoSell, new List<CombinationManager.CollectionProgressData>());
                OnLevelUp?.Invoke();
            }
        }
    }
}


