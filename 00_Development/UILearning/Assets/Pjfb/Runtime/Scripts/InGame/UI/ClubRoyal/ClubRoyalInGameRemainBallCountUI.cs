using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameRemainBallCountUI : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGameBallUI[] ballUIs;
        [SerializeField] private TMP_Text remainBallCountText;
        [SerializeField] private Transform absorbBallTargetTransform;

        private int lastSyncedValue = -1;

        public void UpdateUI(bool playAnimation, bool isSkillIncrease)
        {
            if (!PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData.TryGetValue(PjfbGuildBattleDataMediator.Instance.PlayerIndex, out var playerData))
            {
                return;
            }

            var isOverHeal = playerData.AvailableMilitaryStrength > playerData.MaxMilitaryStrength;
            var color = isOverHeal ? ColorValueAssetLoader.Instance["clubroyalingame.overheal_ball_count_text"] : Color.white;
            var colorCode = ColorUtility.ToHtmlStringRGB(color);
            remainBallCountText.text = $"<color=#{colorCode}>{playerData.AvailableMilitaryStrength}</color>/{playerData.MaxMilitaryStrength}";
            var currentValue = playerData.AvailableMilitaryStrength;
            SetBallCountUI((int)currentValue, playerData.RemainTurnToRecoveryBallCount, playAnimation, isSkillIncrease);
        }

        public void UpdateUI(int maxValue, int currentValue)
        {
            for (var i = 0; i < ballUIs.Length; i++)
            {
                ballUIs[i].gameObject.SetActive(i < maxValue);
            }
            SetBallCountUI(currentValue, -1, false, false);
        }

        private void SetBallCountUI(int currentValue, int remainTurnToRecovery, bool playAnimation, bool isSkillIncrease)
        {
            GuildBattlePlayerData playerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayerData(PjfbGuildBattleDataMediator.Instance.PlayerIndex);
            bool isRecoveringEffect = playerData.AvailableMilitaryStrength < playerData.MaxMilitaryStrength && GuildBattleAbilityLogic.IsActivatingBallRecoveryUpPerTurn(playerData);

            var maxBallUICount = ballUIs.Length;
            var isDecreased = lastSyncedValue > currentValue;
            for (var i = 0; i < maxBallUICount; i++)
            {
                ballUIs[i].SetBlackCoverActive(currentValue <= i);
                var currentBallSpriteType = Math.Clamp((currentValue - 1) / maxBallUICount, (int)ClubRoyalInGameBallUI.BallType.Normal, (int)ClubRoyalInGameBallUI.BallType.Rainbow);
                if ((currentBallSpriteType * maxBallUICount + i + 1) > currentValue && currentBallSpriteType > 0)
                {
                    currentBallSpriteType--;
                }
                ballUIs[i].SetBallTypeSprite((ClubRoyalInGameBallUI.BallType)currentBallSpriteType);
                var fillImageBallSpriteType = Math.Clamp(currentValue < maxBallUICount ? currentBallSpriteType : currentBallSpriteType + 1, (int)ClubRoyalInGameBallUI.BallType.Normal, (int)ClubRoyalInGameBallUI.BallType.Rainbow);
                if (fillImageBallSpriteType == 0 && currentValue > i)
                {
                    fillImageBallSpriteType++;
                }
                
                ballUIs[i].SetFillBallTypeSprite((ClubRoyalInGameBallUI.BallType)fillImageBallSpriteType);
                var isFillingBall = fillImageBallSpriteType * maxBallUICount + i == currentValue;
                if (isFillingBall && remainTurnToRecovery > 0)
                {
                    ballUIs[i].SetFillRate(1 - (float)remainTurnToRecovery / PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleRequiredTurnToRecoveryMilitaryStrength, isRecoveringEffect);
                }
                else
                {
                    ballUIs[i].SetFillRate(0, false);
                }

                if (playAnimation && lastSyncedValue != -1)
                {
                    ballUIs[i].PlayAnimation((ClubRoyalInGameBallUI.BallType)currentBallSpriteType, (ClubRoyalInGameBallUI.BallType)fillImageBallSpriteType, currentValue <= i, isDecreased, isSkillIncrease);
                }
                ballUIs[i].SetSyncedValue((ClubRoyalInGameBallUI.BallType)currentBallSpriteType, (ClubRoyalInGameBallUI.BallType)fillImageBallSpriteType, currentValue <= i, isDecreased);
            }
            
            lastSyncedValue = currentValue;
        }

        public Vector3 GetBallCountTextPosition()
        {
            return absorbBallTargetTransform.position;
        }
    }
}