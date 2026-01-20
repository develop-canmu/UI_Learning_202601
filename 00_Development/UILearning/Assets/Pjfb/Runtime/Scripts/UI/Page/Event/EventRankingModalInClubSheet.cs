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
    public class EventRankingModalInClubSheet : EventRankingModalSheet {
        enum ViewListType {
            UserRank,
            Reword,
        }

        [SerializeField]
        GameObject _rankScrollRoot = null;

        [SerializeField]
        ScrollGrid _rankScrollGrid = null;

        [SerializeField]
        GameObject _rewordScrollRoot = null;
        
        [SerializeField]
        EventRankingUserListItem _myUser = null;

        [SerializeField]
        TextMeshProUGUI _switchButtonText = null;

        ViewListType _currentViewList = ViewListType.UserRank;

        protected override async UniTask OnPreOpen(object args) {
            var request = new RankingGetGuildInternalAPIRequest();
            var post = new RankingGetGuildInternalAPIPost();
            post.mPointId = pointId;
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            await UniTask.Yield();

            var userList = new List<EventRankingUserListItem.Param>();
            if( response.ranking.userRankList != null ){
                foreach( var user in response.ranking.userRankList ){
                    var param = new EventRankingUserListItem.Param();  
                    param.name = user.name;
                    param.userId = user.id;
                    param.point = new BigValue(user.value);
                    param.rank = user.rank;
                    param.iconId = user.mIconId;
                    param.isCurrent = user.id == UserDataManager.Instance.user.uMasterId;
                    userList.Add(param);
                }
            }
            _rankScrollGrid.SetItems(userList);

            
            UpdateViewType( ViewListType.UserRank );

            var myUserParam = new EventRankingUserListItem.Param();
            myUserParam.name = UserDataManager.Instance.user.name;
            myUserParam.userId = UserDataManager.Instance.user.uMasterId;
            myUserParam.point = new BigValue(response.ranking.myValue);
            myUserParam.rank = response.ranking.myRank;
            myUserParam.iconId = UserDataManager.Instance.user.mIconId;
            _myUser.UpdateView(myUserParam);
        }


        public void OnClickSwitchButton(){
            if( _currentViewList == ViewListType.UserRank ) {
                UpdateViewType( ViewListType.Reword );
            } else {
                UpdateViewType( ViewListType.UserRank );
            }
        }

        void UpdateViewType(ViewListType type) {
            _currentViewList = type;
            _rankScrollRoot.gameObject.SetActive( type == ViewListType.UserRank );
            _rewordScrollRoot.gameObject.SetActive( type == ViewListType.Reword );

            if( type == ViewListType.UserRank ) {
                _switchButtonText.text = StringValueAssetLoader.Instance["common.reword"];
            } else {
                _switchButtonText.text = StringValueAssetLoader.Instance["common.ranking"];
            }
        }


    }
}