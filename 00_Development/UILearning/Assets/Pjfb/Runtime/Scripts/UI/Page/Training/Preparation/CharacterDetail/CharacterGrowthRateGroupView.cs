using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{
    public class CharacterGrowthRateGroupView : MonoBehaviour
    {
        [SerializeField]
        private CharacterGrowthRateView staminaRate = null;
        [SerializeField]
        private CharacterGrowthRateView speedRate = null;
        [SerializeField]
        private CharacterGrowthRateView physicsRate = null;
        [SerializeField]
        private CharacterGrowthRateView techniqueRate = null;
        [SerializeField]
        private CharacterGrowthRateView intelligenceRate = null;
        [SerializeField]
        private CharacterGrowthRateView kickRate = null;
        
        /// <summary>キャラをセット</summary>
        public void SetCharacter(long mCharId, long level)
        {
            CharaTrainingCorrectionMasterObject mGrowth = MasterManager.Instance.charaTrainingCorrectionMaster.FindByMCharId(mCharId, level);
            if (mGrowth == null)
            {
                CruFramework.Logger.LogError($"error CharacterGrowthRateGroupView SetCharacter  mGrowth = null : mCharId = {mCharId} ");
                return;
            }
            // ステータス取得
            CharacterStatus status = mGrowth.GetCharacterStatus();
            
            staminaRate.SetValue(CastPercentDouble(status.Stamina));
            speedRate.SetValue(CastPercentDouble(status.Speed));
            physicsRate.SetValue(CastPercentDouble(status.Physical));
            techniqueRate.SetValue(CastPercentDouble(status.Technique));
            intelligenceRate.SetValue(CastPercentDouble(status.Intelligence));
            kickRate.SetValue(CastPercentDouble(status.Kick));
        }

        public void SetGrowthResultPreview(long mCharId, long beforeLevel, long afterLevel, string statusColorId, string statusHighlightColorId)
        {
            CharaTrainingCorrectionMasterObject mBeforeGrouth = MasterManager.Instance.charaTrainingCorrectionMaster.FindByMCharId(mCharId, beforeLevel);
            CharaTrainingCorrectionMasterObject mAfterGrouth = MasterManager.Instance.charaTrainingCorrectionMaster.FindByMCharId(mCharId, afterLevel);

            if (mBeforeGrouth == null || mAfterGrouth == null)
            {
                CruFramework.Logger.LogError($"error CharacterGrowthRateGroupView SetCharacter  mGrowth = null : mCharId = {mCharId} ");
                return;
            }

            // レベルアップ前ステータス取得
            CharacterStatus beforeStatus = mBeforeGrouth.GetCharacterStatus();
            // レベルアップ後ステータス取得
            CharacterStatus afterStatus = mAfterGrouth.GetCharacterStatus();

            bool isStaminaChanged = beforeStatus.Stamina < afterStatus.Stamina;
            bool isSpeedChanged = beforeStatus.Speed < afterStatus.Speed;
            bool isPhysicalChanged = beforeStatus.Physical < afterStatus.Physical;
            bool isTechniqueChanged = beforeStatus.Technique < afterStatus.Technique;
            bool isIntelligenceChanged = beforeStatus.Intelligence < afterStatus.Intelligence;
            bool isKickChanged = beforeStatus.Kick < afterStatus.Kick;

            // カラー取得
            Color defaultColor = ColorValueAssetLoader.Instance[statusColorId];
            Color highlightColor = ColorValueAssetLoader.Instance[statusHighlightColorId];

            staminaRate.SetValue(CastPercentDouble(afterStatus.Stamina), isStaminaChanged ? highlightColor : defaultColor);
            speedRate.SetValue(CastPercentDouble(afterStatus.Speed), isSpeedChanged ? highlightColor : defaultColor);
            physicsRate.SetValue(CastPercentDouble(afterStatus.Physical), isPhysicalChanged ? highlightColor : defaultColor);
            techniqueRate.SetValue(CastPercentDouble(afterStatus.Technique), isTechniqueChanged ? highlightColor : defaultColor);
            intelligenceRate.SetValue(CastPercentDouble(afterStatus.Intelligence), isIntelligenceChanged ? highlightColor : defaultColor);
            kickRate.SetValue(CastPercentDouble(afterStatus.Kick), isKickChanged ? highlightColor : defaultColor);
        }

        private double CastPercentDouble(BigValue value)
        {
            // ÷100で%に変換
            return (double)value.Value * 0.01;
        }
    }
}