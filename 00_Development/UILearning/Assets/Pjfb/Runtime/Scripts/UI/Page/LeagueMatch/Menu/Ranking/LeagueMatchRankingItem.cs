using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRankingItem : ScrollGridItem
    {
        // ランキングのパラメータ
        public class Param
        {
            private bool isAffiliated;
            public bool IsAffiliated
            {
                get { return isAffiliated; }
                set { isAffiliated = value; }
            }
            private long ranking;
            public long Ranking
            {
                get { return ranking; }
                set { ranking = value; }
            }
            private long mGuildEmblemId;
            public long MGuildEmblemId
            {
                get { return mGuildEmblemId; }
                set { mGuildEmblemId = value; }
            }
            private string name;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            private long winCount;
            public long WinCount
            {
                get { return winCount; }
                set { winCount = value; }
            }
            private long score;
            public long Score
            {
                get { return score; }
                set { score = value; }
            }
            private long winCountSub;
            public long WinCountSub
            {
                get { return winCountSub; }
                set { winCountSub = value; }
            }
        }

        [SerializeField] private GameObject frameAffiliated;
        [SerializeField] private Sprite[] rankingImageList;
        [SerializeField] private TextMeshProUGUI textClubRank;
        [SerializeField] private Image iconClubRank;
        [SerializeField] private Image clubEmblem;
        [SerializeField] private TextMeshProUGUI clubName;
        [SerializeField] private TextMeshProUGUI clubWinPoint;
        [SerializeField] private TextMeshProUGUI point;
        [SerializeField] private TextMeshProUGUI matchWins;
        
        // 各要素のセット
        protected override void OnSetView(object value)
        {
            Param param = (Param)value;
            
            // 所属クラブの場合は枠を表示
            frameAffiliated.SetActive(param.IsAffiliated);
            
            // ランキングによって表示を変える
            if (param.Ranking <= rankingImageList.Length)
            {
                iconClubRank.gameObject.SetActive(true);
                textClubRank.gameObject.SetActive(false);
                iconClubRank.sprite = rankingImageList[param.Ranking - 1];
            }
            else
            {
                iconClubRank.gameObject.SetActive(false);
                textClubRank.gameObject.SetActive(true);
                textClubRank.text = param.Ranking.ToString();
            }
            
            clubWinPoint.text = param.WinCount.ToString();
            point.text = param.Score.ToString();
            matchWins.text = param.WinCountSub.ToString();
            // エンブレムのロード
            ClubUtility.LoadAndSetEmblemIcon( clubEmblem, param.MGuildEmblemId).Forget();
            
            clubName.text = param.Name;
        }        
    }
}