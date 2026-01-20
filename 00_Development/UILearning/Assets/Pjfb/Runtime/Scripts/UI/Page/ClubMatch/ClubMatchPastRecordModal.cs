using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.UI;
using UnityEngine;
using Pjfb.Club;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Master;
using TMPro;
using System.Collections.Generic;

namespace Pjfb.ClubMatch
{
    public class ClubMatchPastRecordModal : ModalWindow
    {
        /// <summary>過去戦績で表示するカテゴリを判別するパラメータ</summary>
        public class PastRecordArguments
        {
            /// <summary>過去戦績で表示するカテゴリのタイプリスト</summary>
            private ColosseumClientHandlingType[] clientHandlingTypes;
            /// <summary>Getter</summary>
            public ColosseumClientHandlingType[] ClientHandlingTypes => clientHandlingTypes;
            
            public PastRecordArguments(ColosseumClientHandlingType[] types)
            {
                clientHandlingTypes = types;
            }
        }
        
        [SerializeField]
        private ScrollGrid _scrollGrid = null;

        [SerializeField]
        private TextMeshProUGUI _emptyText = null;

      
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // パラメータの型にキャスト
            PastRecordArguments pastRecordArguments = (PastRecordArguments)args;
            
            // リクエストに渡す型に変換する
            long[] clientHandlingTypes = new long[pastRecordArguments.ClientHandlingTypes.Length];
            for (int i = 0; i < pastRecordArguments.ClientHandlingTypes.Length; i++)
            {
                clientHandlingTypes[i] = (long)pastRecordArguments.ClientHandlingTypes[i];
            }
            
            // 表示するアイテムのパラメータリストを作成
            List<ClubInfoHistoryListItem.Param> paramList = new List<ClubInfoHistoryListItem.Param>();
            
            // 過去戦績のリクエストを送信
            ColosseumGetGuildSeasonStatusPastListAPIRequest request = new ColosseumGetGuildSeasonStatusPastListAPIRequest();
            ColosseumGetGuildSeasonStatusPastListAPIPost post = new ColosseumGetGuildSeasonStatusPastListAPIPost();
            // 取得したい過去戦績のclientHandlingTypeを設定
            post.clientHandlingTypeList = clientHandlingTypes;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // レスポンスを取得
            ColosseumGetGuildSeasonStatusPastListAPIResponse response = request.GetResponseData();
            
            if( response.groupSeasonHistoryList != null ) {
                foreach( var group in response.groupSeasonHistoryList ){
                    // シーズン中は除外
                    if( group.scoreRanking < 0 ) {
                        continue;
                    }

                    ColosseumEventMasterObject mEvent = MasterManager.Instance.colosseumEventMaster.FindData( group.mColosseumEventId );
                    if( mEvent == null ) {
                        continue;
                    }
                    
                    ColosseumEventMasterObject gradeMaster = MasterManager.Instance.colosseumEventMaster.FindData( group.mColosseumEventId );
                    ClubInfoHistoryListItem.Param param = new ClubInfoHistoryListItem.Param();
                    param.startAt = group.startAt;
                    param.endAt = group.endAt;
                    param.name = gradeMaster.name;
                    param.rank = group.scoreRanking;
                    param.gradeBefore = group.gradeBefore;
                    param.gradeAfter = group.gradeAfter;
                    param.shiftMatchProgress = (ClubInfoHistoryListItem.ShiftMatchProgressType)group.shiftMatchProgress;
                    param.eventId = group.mColosseumEventId;
                    param.isRankChangeConsidered = IsRankChangeConsidered(mEvent.clientHandlingType);
                    paramList.Add(param);
                }
            }
            
            _scrollGrid.SetItems(paramList);

            _scrollGrid.gameObject.SetActive( paramList.Count > 0 );
            _emptyText.gameObject.SetActive( paramList.Count <= 0 );

            await base.OnPreOpen(args, token);
        }

        public void OnClickedClose()
        {
            Close();
        }
        
        /// <summary>ランク変動を考慮したテキスト表示形式にするか</summary>
        private bool IsRankChangeConsidered(ColosseumClientHandlingType clientHandlingType)
        {
            switch (clientHandlingType)
            {
                case ColosseumClientHandlingType.ClubMatch:
                case ColosseumClientHandlingType.InstantTournament:
                    return false;
                case ColosseumClientHandlingType.LeagueMatch:
                case ColosseumClientHandlingType.ClubRoyal:
                    return true;
                default:
                    return false;
            }
        }
    }
}