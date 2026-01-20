using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Ranking;
using CruFramework.UI;

namespace Pjfb.Ranking
{
    public class RankingOwnAffiliate : MonoBehaviour
    {
        [SerializeField]
        private RankingOwnInfoView ownInfoView = null;
        
        [SerializeField]
        private RankingClubInfoView clubInfoView = null;

        /// <summary>
        /// 自分の情報を表示する
        /// </summary>
        public void SetMyInfo(long userId, long rank, long userIconId, string userName, BigValue value, bool isCurrent, bool isPointOmission)
        {
            ownInfoView.gameObject.SetActive(true);
            // 各情報のセット
            ownInfoView.SetRank(rank, isCurrent);
            ownInfoView.SetUserIcon(userIconId);
            ownInfoView.SetName(userName);
            ownInfoView.SetPoint(value, isPointOmission);
            ownInfoView.SetUserId(userId);
        }
        
        /// <summary>
        /// 自分の所属するクラブの情報を表示する
        /// </summary>
        public void SetMyClubInfo(long emblemId, long rank, string guildName, BigValue value, bool isCurrent, bool isPointOmission)
        {
            // 各情報のセット
            clubInfoView.SetRank(rank, isCurrent);
            clubInfoView.SetEmblem(emblemId);
            clubInfoView.SetName(guildName);
            clubInfoView.SetPoint(value, isPointOmission);
        }

        /// <summary>
        /// クラブに所属していない場合の表示
        /// </summary>
        public void NoJoinClub()
        {
            clubInfoView.gameObject.SetActive(true);
            clubInfoView.SetRank(0, false);
            clubInfoView.ShowNoJoinClub();
        }

        /// <summary>
        /// クラブに所属している場合の表示
        /// </summary>
        public void JoinClub()
        {
            clubInfoView.gameObject.SetActive(true);
            clubInfoView.ShowJoinClub();
        }
        
        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            clubInfoView.gameObject.SetActive(false);
            ownInfoView.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}