using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.UserData;
using TMPro;
using Pjfb.Master;

namespace Pjfb.Colosseum
{
    public class ColosseumStatusUi : MonoBehaviour
    {
        [SerializeField] private ColosseumRoomImage roomImage;
        [SerializeField] private ColosseumRankImage rankImage;
        [SerializeField] private TMP_Text userName;
        [SerializeField] private TMP_Text userRanking;
        [SerializeField] private DeckRankImage deckRankImage;
        [SerializeField] private TMP_Text combatPower;
        [SerializeField] private OmissionTextSetter omissionTextSetter;

        public void SetView(ColosseumSeasonData colosseumSeasonData)
        {
            var userData = UserDataManager.Instance.user;
            
            userName.text = userData.name;
            combatPower.text = userData.maxCombatPower.Value.ToDisplayString(omissionTextSetter.GetOmissionData());
            deckRankImage.SetTexture(userData.maxDeckRank.Value);

            var seasonStatus = colosseumSeasonData.UserSeasonStatus;
            
            userRanking.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],seasonStatus.ranking);

            var gradeNumber = seasonStatus.gradeAfter != 0 ? seasonStatus.gradeAfter : seasonStatus.gradeNumber;

            roomImage.SetTexture(gradeNumber);
            
            var rankNumber = MasterManager.Instance.colosseumGradeRankLabelMaster.GetRankNumber(seasonStatus.mColosseumEventId, gradeNumber, seasonStatus.ranking);
            rankImage.SetTexture(rankNumber);
        }
        
    }
}