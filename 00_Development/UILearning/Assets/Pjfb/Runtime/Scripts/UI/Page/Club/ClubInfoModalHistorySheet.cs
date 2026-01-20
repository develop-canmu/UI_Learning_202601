using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Master;

namespace Pjfb.Club {
    public class ClubInfoModalHistorySheet : ClubInfoModalSheet {
        [SerializeField]
        private ScrollGrid _scrollGrid = null;

        [SerializeField]
        private TextMeshProUGUI _emptyText = null;

        ColosseumGetGuildSeasonStatusPastListAPIResponse _response = null;


        protected override async UniTask InitView( ClubInfoModalSheetParam data, ClubAccessLevel myAccessLevel, long myUserId, ClubInfoModal modal, Func<ClubAccessLevel,UniTask> updateViewRequest ){
            if( _response != null ) {
                return;
            }
            
            _scrollGrid.gameObject.SetActive( false );
            _emptyText.gameObject.SetActive( false );

            var paramList = new List<ClubInfoHistoryListItem.Param>();
            var requestParam = new ColosseumGetGuildSeasonStatusPastListAPIPost();
            requestParam.gMasterId = data.data.clubId;
            var request = new ColosseumGetGuildSeasonStatusPastListAPIRequest();
            request.SetPostData(requestParam);
            await APIManager.Instance.Connect(request);
            _response = request.GetResponseData();

            if( _response.groupSeasonHistoryList != null ) {
                foreach( var group in _response.groupSeasonHistoryList ){
                    if( group.scoreRanking < 0 ) {
                        continue;
                    }

                    if( !MasterManager.Instance.colosseumEventMaster.Contains( group.mColosseumEventId ) ) {
                        continue;
                    }
                    
                    var gradeMaster = MasterManager.Instance.colosseumEventMaster.FindData( group.mColosseumEventId );
                    var param = new ClubInfoHistoryListItem.Param();
                    param.startAt = group.startAt;
                    param.endAt = group.endAt;
                    param.name = gradeMaster.name;
                    param.rank = group.scoreRanking;
                    param.eventId = group.mColosseumEventId;
                    param.gradeAfter = group.gradeAfter;
                    param.gradeBefore = group.gradeBefore;
                    param.shiftMatchProgress = (ClubInfoHistoryListItem.ShiftMatchProgressType)group.shiftMatchProgress;
                    paramList.Add(param);
                }
            }

            _scrollGrid.SetItems(paramList);

            _scrollGrid.gameObject.SetActive( paramList.Count > 0 );
            _emptyText.gameObject.SetActive( paramList.Count <= 0 );
        }
    }
}