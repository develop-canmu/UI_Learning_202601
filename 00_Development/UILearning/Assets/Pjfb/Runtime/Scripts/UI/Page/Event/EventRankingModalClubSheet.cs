using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;


namespace Pjfb.EventRanking {
    public class EventRankingModalClubSheet : EventRankingModalSheet {
        enum ViewListType {
            ClubRank,
            Reword,
        }

        [SerializeField]
        GameObject _rankScrollRoot = null;

        [SerializeField]
        ScrollGrid _rankScrollGrid = null;

        [SerializeField]
        GameObject _rewordScrollRoot = null;
        [SerializeField]
        ScrollGrid _rewordScrollGrid = null;
        [SerializeField]
        GameObject _rewordEmptyText = null;

        
        [SerializeField]
        EventRankingClubListItem _myClub = null;

        [SerializeField]
        TextMeshProUGUI _switchButtonText = null;


        ViewListType _currentViewList = ViewListType.ClubRank;

        protected override async UniTask OnPreOpen(object args) {
            var request = new RankingGetGuildAPIRequest();
            var post = new RankingGetGuildAPIPost();
            post.mPointId = pointId;
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            await UniTask.Yield();

            var clubList = new List<EventRankingClubListItem.Param>();
            if( response.ranking.guildRankList != null ){
                foreach( var guild in response.ranking.guildRankList ){
                    var param = new EventRankingClubListItem.Param();  
                    param.name = guild.name;
                    param.id = guild.id;
                    param.point = new BigValue(guild.value);
                    param.rank = guild.rank;
                    param.isCurrent = guild.id == UserDataManager.Instance.user.gMasterId;
                    param.emblemId = guild.mGuildEmblemId;  
                    clubList.Add(param);
                }
            }
            _rankScrollGrid.SetItems(clubList);

            var prizeList = new List<EventRankingRewardListItem.Param>();
            if( response.ranking.rankingPrizeList != null ){
                foreach( var prize in response.ranking.rankingPrizeList ){
                    var param = new EventRankingRewardListItem.Param();  
                    param.upperRanking = prize.upperRanking;
                    param.lowerRanking = prize.lowerRanking;
                    param.isCurrent = prize.upperRanking <= response.ranking.myRank && response.ranking.myRank <= prize.lowerRanking;
                    param.prizeList = prize.prizeList;
                    prizeList.Add(param);
                }
            }
            _rewordScrollGrid.SetItems(prizeList);
            _rewordScrollGrid.gameObject.SetActive(prizeList.Count > 0);
            _rewordEmptyText.gameObject.SetActive(prizeList.Count <= 0);
            
            UpdateViewType( ViewListType.ClubRank );

            var myClubParam = new EventRankingClubListItem.Param();
            myClubParam.name = clubData.name;
            myClubParam.id = 0;
            myClubParam.point = new BigValue(response.ranking.myValue);
            myClubParam.rank = response.ranking.myRank;
            myClubParam.emblemId = clubData.emblemId;
            _myClub.UpdateView(myClubParam);
        }


        public void OnClickSwitchButton(){
            if( _currentViewList == ViewListType.ClubRank ) {
                UpdateViewType( ViewListType.Reword );
            } else {
                UpdateViewType( ViewListType.ClubRank );
            }
        }

        void UpdateViewType(ViewListType type) {
            _currentViewList = type;
            _rankScrollRoot.gameObject.SetActive( type == ViewListType.ClubRank );
            _rewordScrollRoot.gameObject.SetActive( type == ViewListType.Reword );

            if( type == ViewListType.ClubRank ) {
                _switchButtonText.text = StringValueAssetLoader.Instance["common.reword"];
            } else {
                _switchButtonText.text = StringValueAssetLoader.Instance["common.ranking"];
            }
        }

        
    }
}