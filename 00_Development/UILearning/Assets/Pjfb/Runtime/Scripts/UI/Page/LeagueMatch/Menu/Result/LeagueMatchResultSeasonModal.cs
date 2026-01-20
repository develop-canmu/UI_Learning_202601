using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchResultSeasonModal : ModalWindow
    {
    
        [SerializeField]
        private  ScrollGrid scrollGrid = null;
        
        [SerializeField]
        private TMPro.TextMeshProUGUI clubNameText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI detailPointText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI rankingText = null;
        [SerializeField] 
        private TMPro.TextMeshProUGUI notificationText = null;
        
        [SerializeField]
        private TMPro.TextMeshProUGUI seasonEndAtText = null;
        
        [SerializeField]
        private ClubEmblemImage emblemIcon = null;
        
        [SerializeField]
        private ClubRankImage rankIcon = null;
        [SerializeField]
        private GameObject rankIconDummy = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            LeagueMatchMenuModal.Arguments arguments = (LeagueMatchMenuModal.Arguments)args;
            // Post
            ColosseumGetGroupLeagueMatchHistoryAPIPost post = new ColosseumGetGroupLeagueMatchHistoryAPIPost();
            post.sColosseumEventId = arguments.SeasonId;
            // Request
            ColosseumGetGroupLeagueMatchHistoryAPIRequest request = new ColosseumGetGroupLeagueMatchHistoryAPIRequest();
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            // レスポンス取得
            ColosseumGetGroupLeagueMatchHistoryAPIResponse response = request.GetResponseData();

            // 対戦履歴がなければテキストを表示する
            notificationText.gameObject.SetActive(response.matchHistoryInfo.matchHistoryList.Length == 0);
            // スクロールデータ
            List<LeagueMatchResultScrollGridItem.Arguments> scrollGridDatas = new List<LeagueMatchResultScrollGridItem.Arguments>();
            // 逆順で表示
            for (int i = response.matchHistoryInfo.matchHistoryList.Length-1; i >= 0; i--)
            {
                scrollGridDatas.Add(new LeagueMatchResultScrollGridItem.Arguments(response.matchHistoryInfo.matchHistoryList[i], arguments.MColosseumEvent));
            }
            // スクロールにセット
            scrollGrid.SetItems(scrollGridDatas);
            
            GroupLeagueMatchGroupStatusDetail data = response.matchHistoryInfo.groupStatusDetailSelf;
            // クラブ名
            clubNameText.text = data.name;
            // クラブアイコン
            emblemIcon.SetTexture(data.mGuildEmblemId);
            
            // 初日
            if(data.winCount == 0 && data.loseCount == 0 && data.drawCount == 0)
            {
                // ランキング
                rankingText.text = StringValueAssetLoader.Instance["league.match.first_day_ranking"];
            }
            // 2日目以降
            else
            {
                // ランキング
                rankingText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.ranking"], data.ranking);
            }
            // ランキング
            detailPointText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.point_result_detail"], data.winCount, data.loseCount, data.drawCount );
            // 簡易大会の場合はダミーの画像を表示
            rankIconDummy.SetActive(arguments.MColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament);
            // 通常のリーグマッチはランク画像を表示
            rankIcon.gameObject.SetActive(arguments.MColosseumEvent.clientHandlingType != ColosseumClientHandlingType.InstantTournament);
            if(arguments.MColosseumEvent.clientHandlingType != ColosseumClientHandlingType.InstantTournament)
            {
                // ランクアイコン
                rankIcon.SetTexture(arguments.SeasonData.UserSeasonStatus.groupSeasonStatus.gradeNumber);
            }

            // 終了までの時間
            string remainString = AppTime.Parse(arguments.SeasonData.SeasonHome.endAt).GetRemainingString(AppTime.Now);
            // シーンズン終了までの時間
            seasonEndAtText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.season_end_at"], remainString);

            await base.OnPreOpen(args, token);
        }
    }
}