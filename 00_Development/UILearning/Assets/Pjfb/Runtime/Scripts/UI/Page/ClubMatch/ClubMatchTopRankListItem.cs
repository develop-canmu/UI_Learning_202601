using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.ClubMatch
{
    public class ClubMatchTopRankListItem : MonoBehaviour
    {
        private const int TOP_RANKING_BORDER = 3;
        
        [SerializeField] private Image CellOutline;
        [SerializeField] private Image ClubRankIcon;
        [SerializeField] private TMP_Text ClubRankText;
        [SerializeField] private TMP_Text ClubName;
        [SerializeField] private TMP_Text ClubStrength;
        [SerializeField] private OmissionTextSetter clubStrengthOmissionTextSetter;
        [SerializeField] private TMP_Text ClubScore;
        [SerializeField] private OmissionTextSetter clubScoreOmissionTextSetter;
        [SerializeField] private Image ClubRankChangeIcon;
        [SerializeField] private Sprite[] rankingImageList;
        [SerializeField] private Sprite[] rankingChangeImageList;
        [SerializeField] private ClubMatchRankingItem itemPrefab;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private GameObject listContainer;
        [SerializeField] private List<GameObject> rankChangeFeatureOnOffGroup;
        [SerializeField] private Image clubEmblem;

        private ColosseumRankingUser[] guildMemberList;

        private List<ClubMatchRankingItem> activeMemberList = new List<ClubMatchRankingItem>();

        private bool isOpen = false;
        private ColosseumRankingGroup curUser;
        private ColosseumSeasonData curSeasonData;
        private bool isSelf;

        private void CleanUp()
        {
            activeMemberList.RemoveAll(x => x == null);
            if (activeMemberList.Count > 0)
            {
                foreach (var item in activeMemberList)
                {
                    item.gameObject.SetActive(false);
                    Destroy(item.gameObject);
                }
            }
        }
        public void Init(ColosseumSeasonData seasonData, ColosseumRankingGroup user, ColosseumRankingUser[] member, bool isSelf = false, bool isShowRankingChange = false)
        {
            CleanUp();
            guildMemberList = member;
            curSeasonData = seasonData;
            curUser = user;
            this.isSelf = isSelf;
            var ranking = user.ranking;
            var isTopUser = ranking <= TOP_RANKING_BORDER;
            ClubRankIcon.gameObject.SetActive(isTopUser);
            ClubRankText.gameObject.SetActive(!isTopUser);
            ClubUtility.LoadAndSetEmblemIcon( clubEmblem, user.mGuildEmblemId).Forget();

            if (isTopUser)
            {
                ClubRankIcon.sprite = rankingImageList[ranking - 1];
            }
            else
            {
                ClubRankText.text = ranking.ToString();
            }

            BigValue expectedScore = BigValue.Zero;
            member.ForEach(m => expectedScore += ColosseumManager.GetExpectedScoreData(m, seasonData.ScoreBattleTurn).totalScore);

            CellOutline.enabled = isSelf;
            ClubName.text = user.name;
            ClubStrength.text = "+" + expectedScore.ToDisplayString(clubStrengthOmissionTextSetter.GetOmissionData()) + "\n/" + seasonData.MColosseumEvent.turnUnitMinute + "min";
            ClubScore.text = new BigValue(user.score).ToDisplayString(clubScoreOmissionTextSetter.GetOmissionData());
            
            //仕様:ランキング変動区分(1 => なし、 2 => 上昇、 3 => 下降、 4 => 変動なし)
            ClubRankChangeIcon.sprite = null;
            long changeID = user.rankingChangeType;
            foreach (var o in rankChangeFeatureOnOffGroup)
            {
                o.SetActive(isShowRankingChange);
            }
            if (isShowRankingChange)
            {
                ClubRankChangeIcon.enabled = changeID > 1;
                if (changeID > 1)
                {
                    ClubRankChangeIcon.sprite = rankingChangeImageList[changeID - 2];
                }
            }
            
        }

        public void OnClick()
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                PopulateMemberList(curSeasonData, guildMemberList, isSelf);
                listContainer.gameObject.SetActive(true);
                arrow.transform.localRotation = Quaternion.Euler(0,0,180f);
            }
            else
            {
                listContainer.gameObject.SetActive(false);
                arrow.transform.localRotation = Quaternion.Euler(0,0,0f);
                CleanUp();
            }
        }
        
        public void OpenClubInfo()
        {
            ClubMatchUtility.OpenClubInfo(curUser.groupId, curUser.id, curUser.groupType).Forget();
        }

        private void PopulateMemberList(ColosseumSeasonData SeasonData, ColosseumRankingUser[] member, bool isSelf)
        {
            foreach (var user in member)
            {
                var item = Instantiate(itemPrefab, itemPrefab.transform.parent, false);
                item.Init(new ClubMatchRankingItem.RankingData
                {
                    userData = user, colosseumSeasonData = SeasonData, disableOnClickAction = isSelf,
                    borderLineType = BorderLineType.Non, scoreType = ScoreType.CurrentScore,
                    backArgs = new ClubMatchTopPage.Data(curSeasonData)
                });
                activeMemberList.Add(item);
                item.gameObject.SetActive(true);
            }
        }
    }
}