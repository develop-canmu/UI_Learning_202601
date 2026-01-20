using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public static class BadgeUtility
    {
        public static bool IsBaseCharacterBadge => IsBaseCharacterGrowthOrLiberationBadge || IsBaseCharacterPieceToCharaBadge || IsAdviserBadge;

        public static bool IsBaseCharacterGrowthOrLiberationBadge => CanGrowthOrLiberation(UserDataManager.Instance.GetUserDataCharaListByType(CardType.Character));

        public static bool IsBaseCharacterPieceToCharaBadge => CanPieceToChara(CardType.Character);

        public static bool IsSpecialSupportCardBadge => CanGrowthOrLiberation(UserDataManager.Instance.GetUserDataCharaListByType(CardType.SpecialSupportCharacter));

        /// <summary> アドバイザー強化、解放またはアドバイザー一覧のどちらかにバッジが表示されているか </summary>
        public static bool IsAdviserBadge => IsAdviserGrowthOrLiberationBadge || IsAdviserPieceToCharaBadge;
        
        /// <summary> アドバイザー強化、解放バッジ表示 </summary>
        public static bool IsAdviserGrowthOrLiberationBadge => CanGrowthOrLiberation(UserDataManager.Instance.GetUserDataCharaListByType(CardType.Adviser));

        /// <summary> アドバイザー一覧バッジ表示 </summary>
        public static bool IsAdviserPieceToCharaBadge => CanPieceToChara(CardType.Adviser);
        
        private static bool CanGrowthOrLiberation(UserDataChara[] charaList)
        {
            var enhanceLevelPointDictionary = MasterManager.Instance.enhanceLevelPointMaster.values
                    .GroupBy(x => x.mEnhanceId)
                    .ToDictionary(g => g.Key, g => g.GroupBy(x => x.level)
                        .ToDictionary(x  => x.Key, x=> x.ToList()));

                foreach (var chara in charaList)
                {
                    if(chara.IsMaxGrowthLevel)  continue;
                    var growthLevelPointDic = enhanceLevelPointDictionary.GetValueOrDefault(chara.MChara.mEnhanceIdGrowth, null);
                    if(growthLevelPointDic is null)  continue;

                    long nextLevel = chara.level + 1;
                    if (!growthLevelPointDic.ContainsKey(nextLevel))
                    {
                        Logger.LogError($"Not found level {nextLevel} EnhanceLevelMasterObject data");
                        continue;
                    }
                    
                    List<EnhanceLevelPointMasterObject> nextLevelRequiredPointList = growthLevelPointDic[nextLevel];
                    if (nextLevelRequiredPointList.Any(x =>
                            ((UserDataManager.Instance.point.Find(x.mPointId)?.value) ?? 0) < x.value)) continue;

                    return true;
                }

                var enhanceLevelDictionary = MasterManager.Instance.enhanceLevelMaster.values
                    .GroupBy(x => x.mEnhanceId)
                    .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.level, x => x.totalExp));

                foreach (var chara in charaList)
                {
                    var liberationLevelDic =
                        enhanceLevelDictionary.GetValueOrDefault(chara.MChara.mEnhanceIdLiberation, null);
                    if(liberationLevelDic is null || liberationLevelDic.Count == 0) continue;
                    if(chara.newLiberationLevel >= liberationLevelDic.Keys.Max())   continue;

                    long currentLevel = chara.newLiberationLevel;
                    long nextLevel = currentLevel + 1;

                    if (!liberationLevelDic.ContainsKey(currentLevel) || !liberationLevelDic.ContainsKey(nextLevel))
                    {
                        Logger.LogError($"Not found level {currentLevel} or {nextLevel} EnhanceLevelMasterObject data");
                        continue;
                    }

                    long possessionCount = UserDataManager.Instance.charaPiece.Find(chara.charaId)?.value ?? 0;
                    long requiredCount = liberationLevelDic[nextLevel] - liberationLevelDic[currentLevel];
                    if(requiredCount > possessionCount) continue;

                    return true;
                }
                return false;
        }

        private static bool CanPieceToChara(CardType type)
        {
            foreach(CharaMasterObject c in MasterManager.Instance.charaMaster.values)
            {
                // ユーザーが所持している
                if(UserDataManager.Instance.chara.data.Values.Any(data => data.charaId == c.id))continue;
                // タイプをチェック
                if(c.cardType != type) continue;
                // chara.priceFromPiece = -1のキャラは解放できない
                if(c.priceFromPiece < 0) continue;
                var possessionCharaPieceValue = UserDataManager.Instance.charaPiece.data.Values.FirstOrDefault(data => data.charaId == c.id)?.value ?? 0;
                var requiredCharaPieceValue = c.priceFromPiece;
                if (possessionCharaPieceValue < requiredCharaPieceValue) continue;
                return true;
            }
            return false;
        }
        
        
        
        public static bool IsCharacterBadge => IsBaseCharacterBadge || IsSpecialSupportCardBadge ||
                                               UserDataManager.Instance.hasTrustPrize || 
                                               CombinationManager.HasCombinationBadge ||
                                               SupportEquipmentManager.HasSupportEquipmentBadge ||
                                               TrainingDeckEnhanceUtility.IsTrainingDeckEnhanceBadge;
    }
}