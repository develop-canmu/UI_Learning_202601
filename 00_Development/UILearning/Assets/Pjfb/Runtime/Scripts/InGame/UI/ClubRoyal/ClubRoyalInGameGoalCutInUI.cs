using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameGoalCutInUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterCardImage[] characterImages;
        [SerializeField] private TMP_Text[] goalNameTexts;
        [SerializeField] private TMP_Text[] goalRemainHPTexts;
        [SerializeField] private TMP_Text[] goalMaxHPTexts;
        [SerializeField] private TMP_Text[] goalCountTexts;
        [SerializeField] private TMP_Text[] bonusGoalCountTexts;
        [SerializeField] private GameObject[] bonusGoalCountRoots;
        [SerializeField] private TMP_Text[] playerNameTexts;
        [SerializeField] private Slider[] goalHPSliders;
        [SerializeField] private Image[] hpSliderGaugeImage;

        private const string AllyGoalTrigger = "GoalYou";
        private const string EnemyGoalTrigger = "GoalOpponent";

        public bool IsPlaying => isPlaying || queuedData.Count > 0;
        private bool isPlaying;
        private Queue<Tuple<GuildBattlePartyModel, GuildBattleCommonMapSpotModel, int, int>> queuedData = new Queue<Tuple<GuildBattlePartyModel, GuildBattleCommonMapSpotModel, int, int>>();

        public void PlayCutIn(GuildBattlePartyModel scoredParty, GuildBattleCommonMapSpotModel spot, int ballCount, int damageDealt)
        {
            if (isPlaying)
            {
                queuedData.Enqueue(new Tuple<GuildBattlePartyModel, GuildBattleCommonMapSpotModel, int, int>(scoredParty, spot, ballCount, damageDealt));
                return;
            }
            
            isPlaying = true;
            gameObject.SetActive(true);
            var viewSide = scoredParty.Side == PjfbGuildBattleDataMediator.Instance.PlayerSide
                ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right;
            var leaderCharacter = scoredParty.GetLeaderCharacterData();
            var playerData = PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayers[scoredParty.PlayerId];
            var index = (int)viewSide;
            characterImages[index].SetTextureAsync(leaderCharacter.mCharaId).Forget();
            var spotName = PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary[spot.Id].GetSpotName();
            goalNameTexts[index].text = $"{spotName}";
            var maxHp = PjfbGuildBattleDataMediator.Instance.BattleField.CommonMapSpotsDictionary[spot.Id].MaxHP;
            goalRemainHPTexts[index].text = spot.RemainHP.ToString();
            goalMaxHPTexts[index].text = maxHp.ToString();
            goalCountTexts[index].text = StringValueAssetLoader.Instance["clubroyalingame.goal_cutin_goal_count"].Format(ballCount);
            var hasBonusDamage = damageDealt > ballCount;
            bonusGoalCountRoots[index].SetActive(hasBonusDamage);
            if (hasBonusDamage)
            {
                bonusGoalCountTexts[index].text = StringValueAssetLoader.Instance["clubroyalingame.goal_cutin_bonus_goal_count"].Format(damageDealt - ballCount);
            }
            playerNameTexts[index].text = playerData.Name;
            var ratio = (float)spot.RemainHP / maxHp;
            goalHPSliders[index].value = ratio;
            if (ratio > BattleConst.HpColorThresholdUnder50)
            {
                hpSliderGaugeImage[index].color = ColorValueAssetLoader.Instance["clubroyalingame.over_50_hp_gauge_color"];
            }
            else if (ratio > BattleConst.HpColorThresholdUnder20)
            {
                hpSliderGaugeImage[index].color = ColorValueAssetLoader.Instance["clubroyalingame.over_20_hp_gauge_color"];
            }
            else
            {
                hpSliderGaugeImage[index].color = ColorValueAssetLoader.Instance["clubroyalingame.under_20_hp_gauge_color"];
            }
            
            animator.SetTrigger(viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? AllyGoalTrigger : EnemyGoalTrigger);
        }

        /// <summary>
        /// called by animation event.
        /// </summary>
        public void OnEndAnimation()
        {
            isPlaying = false;
            if (queuedData.Count > 0)
            {
                var data = queuedData.Dequeue();
                PlayCutIn(data.Item1, data.Item2, data.Item3, data.Item4);
                return;
            }
            
            ClubRoyalInGameUIMediator.Instance.OccupySpotUI.OnEndGoalCutIn();
        }
    }
}