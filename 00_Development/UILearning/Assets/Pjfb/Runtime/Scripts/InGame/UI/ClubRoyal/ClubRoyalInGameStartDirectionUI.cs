using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameStartDirectionUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text[] guildNameTexts;
        [SerializeField] private ClubEmblemImage[] emblemImages;
        [SerializeField] private GameObject[] hideObjects;

        private const string OpenTrigger = "Open";
        private const string CloseTrigger = "Close";

        public void Open()
        {
            gameObject.SetActive(true);
            foreach (var obj in hideObjects)
            {
                obj.SetActive(false);
            }
            animator.SetTrigger(OpenTrigger);
        }

        public async UniTask StartAnimation(BattleV2ClientData battleV2ClientData)
        {
            var allyGuildData = battleV2ClientData.groupList[(int)PjfbGuildBattleDataMediator.Instance.PlayerSide];
            var enemyGuildData = battleV2ClientData.groupList[(int)GuildBattleCommonLogic.GetOtherTeamSide(PjfbGuildBattleDataMediator.Instance.PlayerSide)];
            guildNameTexts[(int)GuildBattleCommonConst.GuildBattleTeamSide.Left].text = allyGuildData.name;
            guildNameTexts[(int)GuildBattleCommonConst.GuildBattleTeamSide.Right].text = enemyGuildData.name;
            emblemImages[(int)GuildBattleCommonConst.GuildBattleTeamSide.Left].SetTextureAsync(allyGuildData.mGuildEmblemId).Forget();
            emblemImages[(int)GuildBattleCommonConst.GuildBattleTeamSide.Right].SetTextureAsync(enemyGuildData.mGuildEmblemId).Forget();
            
            foreach (var obj in hideObjects)
            {
                obj.SetActive(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: destroyCancellationToken);
            animator.SetTrigger(CloseTrigger);
        }
    }
}