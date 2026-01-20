using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameHeaderUI : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGameHeaderTeamInfoUI allyTeamInfoUI;
        [SerializeField] private ClubRoyalInGameHeaderTeamInfoUI enemyTeamInfoUI;
        [SerializeField] private TMP_Text remainTurnText;
        [SerializeField] private Slider situationGaugeSlider;
        [SerializeField] private Image situationImage;
        [SerializeField] private Sprite[] situationSprites;
        [SerializeField] private GameObject beforeFightTimerRoot;
        [SerializeField] private GameObject inFightTimerRoot;
        
        private const float AdvantageThreshold = 0.60f;
        private const float DisadvantageThreshold = 0.40f;

        public void InitializeUI(BattleV2ClientData clientData)
        {
            var playerSide = PjfbGuildBattleDataMediator.Instance.PlayerSide;
            var allyClubData = playerSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? clientData.groupList[0] : clientData.groupList[1];
            var enemyClubData = playerSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? clientData.groupList[1] : clientData.groupList[0];
            var players = PjfbGuildBattleDataMediator.Instance.Players;
            var allyPlayers = new List<BattlePlayerModel>();
            var enemyPlayers = new List<BattlePlayerModel>();
            foreach (var player in players)
            {
                if ((GuildBattleCommonConst.GuildBattleTeamSide)player.Side == playerSide)
                {
                    allyPlayers.Add(player);
                }
                else
                {
                    enemyPlayers.Add(player);
                }
            }
            
            allyTeamInfoUI.InitializeUI(allyClubData.mGuildEmblemId, allyClubData.name, allyPlayers);
            enemyTeamInfoUI.InitializeUI(enemyClubData.mGuildEmblemId, enemyClubData.name, enemyPlayers);
            
            inFightTimerRoot.SetActive(false);
            beforeFightTimerRoot.SetActive(true);
        }

        public void OnBattleStateChanged()
        {
            var isInFight = PjfbGuildBattleDataMediator.Instance.GameState == GuildBattleCommonConst.GuildBattleGameState.InFight;
            inFightTimerRoot.SetActive(isInFight);
            beforeFightTimerRoot.SetActive(!isInFight);
        }

        public void UpdateRemainTurn(int remainTurn)
        {
            remainTurnText.text = remainTurn.ToString();
        }

        public void UpdateSituationGaugeSlider(int allyPoint, int enemyPoint)
        {
            var total = allyPoint + enemyPoint;
            var ratio = total != 0 ? Mathf.Clamp((float)allyPoint / total, 0, 1.0f) : 0.5f;
            situationGaugeSlider.value = ratio;
            var situationSprite = situationSprites[0];
            if(ratio >= AdvantageThreshold)
            {
                situationSprite = situationSprites[2];
            }
            else if(ratio <= DisadvantageThreshold)
            {
                situationSprite = situationSprites[1];
            }

            situationImage.sprite = situationSprite;
        }

        public void UpdateActivePlayerView(List<long> activePlayerIds)
        {
            foreach (var activePlayerId in activePlayerIds)
            {
                // 所属していないプレイヤーの分は中で弾いているので両方取っちゃう.
                allyTeamInfoUI.UpdateActivePlayerView(activePlayerId, true);
                enemyTeamInfoUI.UpdateActivePlayerView(activePlayerId, true);
            }
        }
    }
}