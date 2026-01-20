using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameBallCountUI : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGameBallUI[] ballUIs;

        public void SetActiveBallCount(int ballCount)
        {
            for (var i = 0; i < ballUIs.Length; i++)
            {
                ballUIs[i].gameObject.SetActive(ballCount > i);
                ballUIs[i].SetBlackCoverActive(false);
            }
        }
        
        public void SetActiveBallCount(int startBallCount, int currentBallCount)
        {
            for (var i = 0; i < ballUIs.Length; i++)
            {
                ballUIs[i].gameObject.SetActive(startBallCount > i);
                ballUIs[i].SetBlackCoverActive(currentBallCount < i);
            }
        }

        public void SetOutlineColor(GuildBattleCommonConst.GuildBattleTeamSide viewSide)
        {
            foreach (var ui in ballUIs)
            {
                ui.SetOutlineColor(viewSide);
            }
        }
    }
}