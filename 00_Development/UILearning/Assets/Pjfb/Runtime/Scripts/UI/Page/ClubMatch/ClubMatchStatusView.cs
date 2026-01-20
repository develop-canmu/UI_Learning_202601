using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.UserData;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using Pjfb.Networking.App.Request;
using Pjfb.Extensions;

namespace Pjfb.ClubMatch
{
    public enum ClubMatchStatusViewType
    {
        Club,
        Personal,
    }
    public class ClubMatchStatusView : MonoBehaviour
    {
        [SerializeField] private GameObject personalRoot;
        [SerializeField] private GameObject clubRoot;
        
        [SerializeField] private DeckRankImage deckRankImage;
        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text teamTotalPowerText;
        [SerializeField] private OmissionTextSetter teamTotalPowerOmissionTextSetter;
        [SerializeField] private TMP_Text personalRankingText;
        [SerializeField] private TMP_Text personalScoreText;
        [SerializeField] private OmissionTextSetter personalScoreOmissionTextSetter;
        [SerializeField] private TMP_Text personalScoreRankingText;

        [SerializeField] private Image clubEmblemImage;
        [SerializeField] private Image clubRankImage;
        [SerializeField] private TMP_Text clubNameText;
        [SerializeField] private TMP_Text clubRankingText;
        [SerializeField] private TMP_Text clubScoreText;
        [SerializeField] private OmissionTextSetter clubScoreOmissionTextSetter;

        private bool isClubViewInitialized;
        private bool isPersonalViewInitialized;
        
        public void SetPersonalView(ColosseumUserSeasonStatus seasonStatus, long selfCumulativeRanking, BigValue currentTotalCombatPower)
        {
            personalRoot.SetActive(true);
            clubRoot.SetActive(false);

            if (isPersonalViewInitialized)
            {
                return;
            }
            
            var totalCombatPower = ColosseumManager.GetClubMatchTotalCombatPower(seasonStatus.defenseCount, currentTotalCombatPower);
            deckRankImage.SetTexture(StatusUtility.GetPartyRank(totalCombatPower));
            userNameText.text = UserDataManager.Instance.user.name;
            teamTotalPowerText.text = totalCombatPower.ToDisplayString(teamTotalPowerOmissionTextSetter.GetOmissionData());
            personalRankingText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], seasonStatus.ranking);
            personalScoreText.text =  new BigValue(seasonStatus.score).ToDisplayString(personalScoreOmissionTextSetter.GetOmissionData());
            personalScoreRankingText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], selfCumulativeRanking > 0 ? selfCumulativeRanking : seasonStatus.ranking);
            isPersonalViewInitialized = true;
        }
        
        public async UniTask SetClubViewAsync(ColosseumRankingGroup rankingGroup, long gradeNumber)
        {
            if (!isClubViewInitialized)
            {
                var clubData = await ClubMatchUtility.GetClubData();
                clubNameText.text = rankingGroup.name;
                clubRankingText.text = rankingGroup.ranking.ToString();
                clubScoreText.text = new BigValue(rankingGroup.score).ToDisplayString(clubScoreOmissionTextSetter.GetOmissionData());
                await ClubUtility.LoadAndSetEmblemIcon(clubEmblemImage, clubData.emblemId);
                await ClubUtility.LoadAndSetRankIcon(clubRankImage, gradeNumber);
                isClubViewInitialized = true;
            }
            personalRoot.SetActive(false);
            clubRoot.SetActive(true);
        }

    }
}