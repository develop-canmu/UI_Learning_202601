using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.ClubRoyal
{
    /// <summary>クラブ・ロワイヤルの試合履歴のモーダル</summary>
    public class ClubRoyalMatchHistoryModalWindow : ModalWindow
    {
        /// <summary>試合履歴を表示するために必要なパラメータ</summary>
        public class MatchHistoryArguments
        {
            /// <summary>シーズンID</summary>
            private long seasonId;
            /// <summary>シーズンIDのgetter</summary>
            public long SeasonId => seasonId;

            /// <summary>所属クラブ名</summary>
            private string myClubName;
            /// <summary>所属クラブ名のgetter</summary>
            public string MyClubName => myClubName;

            /// <summary>所属クラブのエンブレムID</summary>
            private long myClubEmblemId;
            /// <summary>所属クラブのエンブレムIDのgetter</summary>
            public long MyClubEmblemId => myClubEmblemId;
            
            public MatchHistoryArguments(long seasonId, string myClubName, long myClubEmblemId)
            {
                this.seasonId = seasonId;
                this.myClubName = myClubName;
                this.myClubEmblemId = myClubEmblemId;
            }
        }
        
        /// <summary>試合履歴のスクロール</summary>
        [SerializeField]
        private ScrollGrid scrollGrid = null;

        /// <summary>試合履歴がない場合に表示するオブジェクト</summary>
        [SerializeField]
        private GameObject emptyTextObj = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // シーズンIDを取得する
            MatchHistoryArguments arguments = (MatchHistoryArguments)args;
            long seasonId = arguments.SeasonId;
            
            // リクエスト
            ColosseumGetGroupLeagueMatchHistoryAPIRequest request = new ColosseumGetGroupLeagueMatchHistoryAPIRequest();
            // ポスト
            ColosseumGetGroupLeagueMatchHistoryAPIPost post = new ColosseumGetGroupLeagueMatchHistoryAPIPost();
            post.sColosseumEventId = seasonId;
            // リクエストで送るパラメータをセット
            request.SetPostData(post);
            // API接続
            await APIManager.Instance.Connect(request);
            
            // レスポンス取得
            ColosseumGetGroupLeagueMatchHistoryAPIResponse response = request.GetResponseData();
            
            // 試合履歴のデータを取得
            GroupLeagueMatchMatchHistory[] matchHistories = response.matchHistoryInfo.matchHistoryList;
            
            // 試合履歴がない場合はemptyテキストを表示
            if (matchHistories.Length == 0)
            {
                SetEmpty(true);
                return;
            }
            SetEmpty(false);
            
            // 試合履歴のViewのデータ作成
            List<ClubRoyalMatchHistoryItemView.MatchHistoryInfo> matchHistoryInfoList = new List<ClubRoyalMatchHistoryItemView.MatchHistoryInfo>();
            foreach (GroupLeagueMatchMatchHistory data in matchHistories)
            {
                // 自分のクラブ情報
                ClubRoyalMatchHistoryClubInfoView.ClubInfo ownClubInfo = new ClubRoyalMatchHistoryClubInfoView.ClubInfo(
                    response.matchHistoryInfo.groupStatusDetailSelf.name,
                    response.matchHistoryInfo.groupStatusDetailSelf.mGuildEmblemId,
                    data.resultDetailArgs.occupiedSpotIdList,
                    data.winningPoint);
                
                // 相手のクラブ情報
                ClubRoyalMatchHistoryClubInfoView.ClubInfo opponentClubInfo = new ClubRoyalMatchHistoryClubInfoView.ClubInfo(
                    data.opponentName,
                    data.opponentMGuildEmblemId,
                    data.resultDetailArgsOpponent.occupiedSpotIdList,
                    data.winningPointOpponent);
                
                ClubRoyalMatchHistoryItemView.MatchHistoryInfo matchHistoryInfo = new ClubRoyalMatchHistoryItemView.MatchHistoryInfo(
                    (ClubRoyalMatchHistoryItemView.MatchResultType)data.result,
                    data.battleStartAtSub,
                    ownClubInfo,
                    opponentClubInfo);
                
                matchHistoryInfoList.Add(matchHistoryInfo);
            }
            
            // スクロールにデータをセット
            scrollGrid.SetItems(matchHistoryInfoList);
            
            await base.OnPreOpen(args, token);
        }
        
        /// <summary>閉じるボタン</summary>
        public void OnClickCloseButton()
        {
            Close();
        }
        
        /// <summary>試合履歴がない場合の表示</summary>
        public void SetEmpty(bool isEmpty)
        {
            emptyTextObj.SetActive(isEmpty);
            
            // スクロールバーの非表示
            scrollGrid.vertical = !isEmpty;
            scrollGrid.viewport.gameObject.SetActive(!isEmpty);
        }
    }
}