using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameBallUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image ballImage;
        [SerializeField] private Sprite[] ballTypeSprites;
        [SerializeField] private GameObject coverImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private ClubRoyalInGameBallSkillRecoveringController skillRecoveringController;

        private const string NormalTrigger = "Normal";
        private const string IncreaseTrigger = "Increase";
        private const string DecreaseTrigger = "Decrease";
        private const string SkillIncreaseTrigger = "Restoration";

        private BallType lastBallType = BallType.Normal;
        private BallType lastFillBallType = BallType.Normal;
        private bool lastIsCovered = false;

        public enum BallType
        {
            Normal,
            Gold,
            Rainbow,
        }
        
        public void SetBlackCoverActive(bool isActive)
        {
            coverImage.SetActive(isActive);
        }

        public void SetFillRate(float fillRate, bool isRecoveringEffect)
        {
            fillImage.fillAmount = fillRate;
            skillRecoveringController.SetState(isRecoveringEffect);
        }
        
        public void SetBallTypeSprite(BallType ballType)
        {
            ballImage.sprite = ballTypeSprites[(int)ballType];
        }
        
        public void SetFillBallTypeSprite(BallType ballType)
        {
            fillImage.sprite = ballTypeSprites[(int)ballType];
        }
        
        public void PlayAnimation(BallType ballType, BallType fillBallType, bool isCovered, bool isDecreased, bool isSkillIncrease)
        {
            if (ballType != lastBallType || fillBallType != lastFillBallType || isCovered != lastIsCovered)
            {
                animator.SetTrigger(isDecreased ? DecreaseTrigger : (isSkillIncrease ? SkillIncreaseTrigger : IncreaseTrigger));
            }
        }

        public void SetSyncedValue(BallType ballType, BallType fillBallType, bool isCovered, bool isDecreased)
        {
            lastBallType = ballType;
            lastFillBallType = fillBallType;
            lastIsCovered = isCovered;
        }

        public void SetOutlineColor(GuildBattleCommonConst.GuildBattleTeamSide viewSide)
        {
            var key = viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? "clubroyalingame.ally_leaderui_outline" : "clubroyalingame.enemy_leaderui_outline";
            outlineImage.color = ColorValueAssetLoader.Instance[key];
        }
    }
}