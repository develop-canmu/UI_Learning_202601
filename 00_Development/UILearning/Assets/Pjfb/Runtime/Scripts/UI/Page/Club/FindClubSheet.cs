using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using Pjfb.UserData;

namespace Pjfb.Club {
    public class FindClubSheet : FindClubPageSheet {

        [SerializeField]
        ScrollGrid _scrollGrid = null;
        [SerializeField]
        FindClubSettingView _findClubSetting = null;
        [SerializeField]
        TMPro.TextMeshProUGUI _emptyText = null;


        void Start(){
            _emptyText.gameObject.SetActive( _scrollGrid.GetItems().Count <= 0 );
            _findClubSetting.onClickFindButton = OnClickFindButton;
        }


        void OnClickFindButton(){ 
            UpdateList().Forget();
        }

        async UniTask UpdateList(){
            var param = _findClubSetting.CreateFindParam();

            var post = new GuildSearchGuildAPIPost();
            post.name = param.clubName;
            post.mGuildRankIdFrom = param.rankLower;
            post.mGuildRankIdTo = param.rankUpper;
            post.numberOfPeopleFrom = param.memberLower;
            post.numberOfPeopleTo = param.memberUpper;
            post.mGuildPlayStyleId = param.activityPolicy;
            post.autoEnrollmentFlg = param.isAutoEnrollment;
            post.membersWantedFlg = param.memberRecruitmentStatus;
            post.guildBattleStyleType = param.clubMatchPolicy;
            post.participationPriorityType = param.participationPriority;
            var request = new GuildSearchGuildAPIRequest();
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);

            var response = request.GetResponseData();

            var paramList = new List<ClubListItem.Param>();
            if( response.guildList?.Length > 0 ) {
                foreach( var guild in response.guildList ){
                    //自身のクラブは検索結果に出さない
                    if( guild.gMasterId == UserData.UserDataManager.Instance.user.gMasterId ) {
                        continue;
                    }
                    
                    //ローカルでも人数数等でチェック
                    if( param.memberRecruitmentStatus != 0 && (param.memberRecruitmentStatus == 1) != ClubUtility.IsMembersWantedFlg(guild.membersWantedFlg, guild.numberOfPeople) ) {
                        continue;
                    }

                    var itemParam = new ClubListItem.Param(guild);
                    paramList.Add(itemParam);
                }
            }
            _scrollGrid.SetItems(paramList);
            _emptyText.gameObject.SetActive( paramList.Count <= 0 );
        }
    }
}