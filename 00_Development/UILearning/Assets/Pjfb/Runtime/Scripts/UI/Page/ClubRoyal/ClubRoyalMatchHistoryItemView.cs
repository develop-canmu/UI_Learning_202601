using System;
using Pjfb.Extensions;
using UnityEngine;
using TMPro;

namespace Pjfb.ClubRoyal
{
    /// <summary>試合履歴モーダルのView</summary>
    public class ClubRoyalMatchHistoryItemView : MonoBehaviour
    {
        /// <summary>試合結果の種別</summary>
        public enum MatchResultType
        {
            Win = 1,
            Lose = 2,
        }
        
        /// <summary>試合履歴の情報</summary>
        public class MatchHistoryInfo
        {
            /// <summary>試合結果</summary>
            private MatchResultType matchResult;
            /// <summary>試合結果のGetter</summary>
            public MatchResultType MatchResult => matchResult;
            
            /// <summary>試合日時</summary>
            private string matchDate;
            /// <summary>試合日時のGetter</summary>
            public string MatchDate => matchDate;

            /// <summary>自身のクラブ情報</summary>
            private ClubRoyalMatchHistoryClubInfoView.ClubInfo ownClubInfo;
            /// <summary>自身のクラブ情報のGetter</summary>
            public ClubRoyalMatchHistoryClubInfoView.ClubInfo OwnClubInfo => ownClubInfo;
            
            /// <summary>相手のクラブ情報</summary>
            private ClubRoyalMatchHistoryClubInfoView.ClubInfo opponentClubInfo;
            /// <summary>相手のクラブ情報のGetter</summary>
            public ClubRoyalMatchHistoryClubInfoView.ClubInfo OpponentClubInfo => opponentClubInfo;

            public MatchHistoryInfo(MatchResultType matchResult, string matchDate, ClubRoyalMatchHistoryClubInfoView.ClubInfo ownClubInfo, ClubRoyalMatchHistoryClubInfoView.ClubInfo opponentClubInfo)
            {
                this.matchResult = matchResult;
                this.matchDate = matchDate;
                this.ownClubInfo = ownClubInfo;
                this.opponentClubInfo = opponentClubInfo;
            }
        }
        
        /// <summary>勝利結果のバッジ</summary>
        [SerializeField]
        private GameObject matchResultWinBadge = null;
        
        /// <summary>敗北結果のバッジ</summary>
        [SerializeField]
        private GameObject matchResultLoseBadge = null;
        
        /// <summary>試合日時</summary>
        [SerializeField]
        private TextMeshProUGUI matchDateText = null;

        /// <summary>所属クラブ情報のView</summary>
        [SerializeField]
        private ClubRoyalMatchHistoryClubInfoView ownClubInfoView = null;
        
        /// <summary>対戦相手クラブ情報のView</summary>
        [SerializeField]
        private ClubRoyalMatchHistoryClubInfoView opponentClubInfoView = null; 

        public void SetView(MatchHistoryInfo matchHistoryInfo)
        {
            // 所属クラブ情報をセット
            ownClubInfoView.SetView(matchHistoryInfo.OwnClubInfo);
            
            // 対戦相手のクラブ情報をセット
            opponentClubInfoView.SetView(matchHistoryInfo.OpponentClubInfo);
            
            // 勝敗バッジをセット
            SetMatchResult(matchHistoryInfo.MatchResult);
            
            // 試合日時をセット
            SetMatchDate(matchHistoryInfo.MatchDate);
        }

        /// <summary>勝敗バッジのセット</summary>
        public void SetMatchResult(MatchResultType matchResult)
        {
            matchResultWinBadge.SetActive(matchResult == MatchResultType.Win);
            matchResultLoseBadge.SetActive(matchResult == MatchResultType.Lose);
        }

        /// <summary>試合日時のセット</summary>
        public void SetMatchDate(string matchDate)
        {
            // サーバーから取得した日時文字列をDateTimeに変換し再度文字列に変換して表示
            DateTime date = matchDate.TryConvertToDateTime();
            matchDateText.text = date.GetNewsDateTimeString();
        }
    }
}