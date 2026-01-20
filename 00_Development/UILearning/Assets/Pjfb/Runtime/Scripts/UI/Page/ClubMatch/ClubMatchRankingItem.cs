using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using Pjfb.Colosseum;
using Pjfb.UserData;
using UnityEngine.UI;
using Pjfb.Master;
using Pjfb.Extensions;
using Pjfb.Club;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.ClubMatch
{
    public class ClubMatchRankingItem : ColosseumRankingItem
    {
        public class RankingData : Data
        {
            public object backArgs;
        }
        private const int BaseBonusMultiplier = 10000;
        private const string CurrentScoreTitle = "clubmatch.bonus.score";
        private const string CumulativeScoreTitle = "clubmatch.cumulative.score";

        [SerializeField] private Image frameImage;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text clubName;
        [SerializeField] private TMP_Text scoreTitle;
        [SerializeField] private TMP_Text expectedScore;
        [SerializeField] private OmissionTextSetter expectedScoreOmissionTextSetter;
        [SerializeField] private TMP_Text totaleScore;
        [SerializeField] private OmissionTextSetter totalOmissionTextSetter;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private GameObject bonusActivatedObject;
        [SerializeField] private GameObject bonusActivatedOccured;
        [SerializeField] private GameObject consecutivePenaltyObject;
        [SerializeField] private GameObject consecutivePenaltyOccured;
        [SerializeField] private TMP_Text consecutivePenaltyValue;
        [SerializeField] private GameObject allBadgesRoot;
        [SerializeField] private GameObject defenseCountObject;
        [SerializeField] private TMP_Text defenseCountValue;
        private RankingData param;

        public override void Init(ItemParamsBase value)
        {
            base.Init(value);

            param = value as RankingData;
            
            playerName.text = data.userData.name;
            clubName.text = data.userData.groupName;
            scoreTitle.text = string.Format(StringValueAssetLoader.Instance[data.scoreType == ScoreType.CurrentScore ? CurrentScoreTitle : CumulativeScoreTitle]);

            var scoreBattleTurn = param.colosseumSeasonData.ScoreBattleTurn;
            long bonusMultiplier = BaseBonusMultiplier; // baseが1.0
            if (scoreBattleTurn.bonusRankList != null)
            {
                foreach (var bonus in scoreBattleTurn.bonusRankList)
                {
                    if (bonus.l[0] == data.userData.ranking)
                    {
                        bonusMultiplier += bonus.l[1];
                    }
                }
            }

            // 表示
            var penaltyValue = (int)(ColosseumManager.GetPenaltyCoefficient(param.userData.defenseCount) * 100);
            var isBonusRankActivated = data.scoreType == ScoreType.CurrentScore && bonusMultiplier > BaseBonusMultiplier;
            var isPenaltyActivated = penaltyValue > 0;
            var isSelf = data.userData.uMasterId == UserDataManager.Instance.user.uMasterId && data.userData.userType == 1;
            string penaltyValueString = string.Format(StringValueAssetLoader.Instance["common.percent_value"], penaltyValue);
            var score = ColosseumManager.GetExpectedScoreData(data.userData, scoreBattleTurn).totalScore;
            expectedScore.text = "+" + new BigValue(score).ToDisplayString(expectedScoreOmissionTextSetter.GetOmissionData()) + "/" + param.colosseumSeasonData.MColosseumEvent.turnUnitMinute + "min";
            totaleScore.text = new BigValue(data.userData.score).ToDisplayString(totalOmissionTextSetter.GetOmissionData());
            allBadgesRoot.SetActive(isPenaltyActivated || isBonusRankActivated);
            consecutivePenaltyObject.SetActive(isPenaltyActivated);
            consecutivePenaltyOccured.SetActive(isPenaltyActivated && !isBonusRankActivated);
            bonusActivatedObject.SetActive(isBonusRankActivated);
            bonusActivatedOccured.SetActive(isBonusRankActivated && !isPenaltyActivated);
            defenseCountObject.SetActive(isPenaltyActivated);
            selfObject.SetActive(isSelf && !isPenaltyActivated);
            consecutivePenaltyValue.text = penaltyValueString;
            defenseCountValue.text = param.userData.defenseCount.ToString();
            frameImage.color = ColorValueAssetLoader.Instance[isSelf ? "clubmatch.my.rank" : "pvp.rank"];
            totalPowerText.color = ColorValueAssetLoader.Instance[isPenaltyActivated ? "warning" : "default"];
            if (isPenaltyActivated)
            {
                totalPowerText.text = (new BigValue(data.userData.combatPower) * (100 - penaltyValue) / 100).ToDisplayString(omissionTextSetter.GetOmissionData());
            }
        }

        protected override async UniTask OnClickBannerActionAsync()
        {
            // デッキ情報をまだ取得していなければここでとる
            await UpdateDeckAsync();
            var topModal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
            if (topModal != null)
            {
                // モーダルを閉じる
                await topModal.CloseAsync();
            }
            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.TeamConfirm, 
                true, 
                new TeamConfirmPage.PageParams(
                    PageType.ClubMatch, 
                     param.backArgs, 
                    colosseumDeck,
                    data.userData,
                    data.colosseumSeasonData)
            );
        }

        public async void OnLongTap()
        {
            await ClubMatchUtility.OpenClubInfo(data.userData.groupId, UserDataManager.Instance.user.uMasterId, data.userData.groupType);
        }

        #region EventListeners
        public void OnClickBonusDetail()
        {
            ClubMatchScoreBreakdownModalWindow.Open(new ClubMatchScoreBreakdownModalWindow.WindowParams{userSeasonStatus = data.userData, seasonData = data.colosseumSeasonData});
        }

        public void OnLogTapPlayerInfo()
        {
            ClubMatchUtility.OpenProfile(param.userData.uMasterId, (ColosseumPlayerType)param.userData.userType);
        }
        #endregion
    }
}
