using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameResultUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ClubRoyalInGameRankingResultItem[] killCountRankingItems;
        [SerializeField] private ClubRoyalInGameRankingResultItem[] defenceCountRankingItems;
        [SerializeField] private ClubRoyalInGameRankingResultItem[] goalCountRankingItems;
        [SerializeField] private ClubRoyalInGameRankingResultItem[] winStreakCountRankingItems;
        [SerializeField] private UIButton closeButton;

        private const string winTrigger = "Win";
        private const string loseTrigger = "Lose";
        private const string closeTrigger = "Close";

        public async UniTask PlayAnimation(GuildBattleResultData resultData)
        {
            // ゴール演出開始まではとりあえず待ち
            await Task.Delay(TimeSpan.FromSeconds(ClubRoyalInGameFieldUI.BattleDirectionDuration), cancellationToken: destroyCancellationToken);
            // ゴール演出が全て終わるまで待ち
            await UniTask.WaitUntil(() =>
                !ClubRoyalInGameUIMediator.Instance.GoalCutInUI.IsPlaying && !ClubRoyalInGameUIMediator.Instance.OccupySpotUI.IsPlaying);
            var totalCombatPowerDictionary = new Dictionary<long, BigValue>();
            foreach (var kvp in PjfbGuildBattleDataMediator.Instance.BattleCharaDataList)
            {
                var player = PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData[kvp.Key];
                totalCombatPowerDictionary.Add(player.PlayerId, BigValue.Zero);

                foreach (var chara in kvp.Value)
                {
                    totalCombatPowerDictionary[player.PlayerId] += chara.combatPower;
                }
            }
            
            var isWin = resultData.WinTeam == PjfbGuildBattleDataMediator.Instance.PlayerSide;
            var trigger = isWin ? winTrigger : loseTrigger;
            SetRankingView(killCountRankingItems, resultData.WinFightCount, resultData.WinTeam, totalCombatPowerDictionary);
            SetRankingView(defenceCountRankingItems, resultData.SpotDefenceCount, resultData.WinTeam, totalCombatPowerDictionary);
            SetRankingView(goalCountRankingItems, resultData.TotalDamageCount, resultData.WinTeam, totalCombatPowerDictionary);
            SetRankingView(winStreakCountRankingItems, resultData.MaxWinStreakCount, resultData.WinTeam, totalCombatPowerDictionary);
            
            animator.SetTrigger(trigger);
        }

        private void SetRankingView(ClubRoyalInGameRankingResultItem[] rankingItems, Dictionary<long, int> resultData, GuildBattleCommonConst.GuildBattleTeamSide winTeam, Dictionary<long, BigValue> totalCombatPowerDictionary)
        {
            var orderedData = resultData
                .OrderByDescending(kvp => kvp.Value)
                .ThenByDescending(kvp => (int)PjfbGuildBattleDataMediator.Instance.GetBattlePlayer(kvp.Key).Side == (int)winTeam)
                .ThenByDescending(kvp =>
                {
                    totalCombatPowerDictionary.TryGetValue(kvp.Key, out var val);
                    return val;
                })
                .ToList();
            
            var i = 0;
            for (i = 0; i < rankingItems.Length; i++)
            {
                rankingItems[i].gameObject.SetActive(false);
            }

            i = 0;
            foreach (var kvp in orderedData)
            {
                if (i >= rankingItems.Length)
                {
                    break;
                }

                var playerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayer(kvp.Key);
                rankingItems[i].SetUI(playerData.IconId, playerData.Name, kvp.Value, i, (int)playerData.Side == (int)PjfbGuildBattleDataMediator.Instance.PlayerSide);
                rankingItems[i].gameObject.SetActive(true);
                i++;
            }
        }

        public void OnClickClose()
        {
            closeButton.interactable = false;
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null);
        }
    }
}