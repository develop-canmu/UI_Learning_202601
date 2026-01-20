using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.ClubMatch;
using Pjfb.UserData;

namespace Pjfb
{
    public class UIFooterClubButton : MonoBehaviour
    {
        
        [SerializeField] private UIBadgeBalloon announcementResultBalloon = null;
        [SerializeField] private UIBadgeBalloon inSeasonBalloon = null;
        [SerializeField] private UIBadgeBalloon aggregationBalloon = null;

        public void UpdateClubMatchBalloon()
        {
            inSeasonBalloon.SetActive(false);
            announcementResultBalloon.SetActive(false);
            aggregationBalloon.SetActive(false);
            if(UserDataManager.Instance.user.gMasterId == 0) return;
            ClubMatchBanner.Data clubMatchData = ClubMatchUtility.GetClubMatchBannerData();
            if(clubMatchData == null) return;
            if (clubMatchData.HasSeasonHome && clubMatchData.SeasonData.IsOnSeason)
            {
                inSeasonBalloon.SetActive(true);
            }
        }
        
    }
}