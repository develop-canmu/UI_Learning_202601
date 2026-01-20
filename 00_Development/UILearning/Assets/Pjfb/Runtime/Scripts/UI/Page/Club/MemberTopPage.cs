using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using UnityEngine;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Pjfb.Storage;



namespace Pjfb.Club
{
    public class MemberTopPage : Page
    {

        public class Param {
            public bool isShowCreateNotification{get;set;} = false;
            public bool isShowJoinNotification{get;set;} = false;
            public ClubData clubData{get;set;} = null;
            public ClubAccessLevel myAccessLevel{get;set;} = ClubAccessLevel.None;
            public GuildBattleMatchingMatchingStatus[] guildBattleMatchingList{get;set;} = null;
        }

        [SerializeField]
        MemberTopClubInformation _information = null;

        [SerializeField]
        MemberTopMembers _members = null;

        [SerializeField]
        MemberTopSchedules _schedules = null;
        [SerializeField]
        UINotification _notification = null;

        [SerializeField]
        MemberTopPageBadgeController _badgeController = null;

        [SerializeField]
        ClubMatchBanner clubMatchBanner = null;
        

        ClubData _clubData = null;
        ClubAccessLevel _myAccessLevel = ClubAccessLevel.None;
        Param _param = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);

            _param = (Param)args;
            _clubData = _param.clubData;
            _myAccessLevel = _param.myAccessLevel;
            // クラブマッチのデータをセット
            ClubMatchBanner.Data bannerData = ClubMatchUtility.GetClubMatchBannerData();
            clubMatchBanner.SetView(bannerData);
            await UpdateView(_clubData);
            _schedules.UpdateView( _param.guildBattleMatchingList );
        }

        protected override void OnOpened(object args){
            if( _param.isShowCreateNotification ) {
                _notification.ShowNotification(StringValueAssetLoader.Instance["club.createClubNotification"]);
                _param.isShowCreateNotification = false;
            } else if( _param.isShowJoinNotification ){
                var text = string.Format(StringValueAssetLoader.Instance["club.joinClubNotification"], _clubData.name);
                _notification.ShowNotification(text);
                _param.isShowJoinNotification = false;
            }
            AppManager.Instance.TutorialManager.OpenClubTutorialAsync().Forget();

        }

        public void OnClickInfoButton(){
            OpenClubInfoModal().Forget();
        }
        
        public void OnClickRankingButton(){
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRanking, null);
        }

        public void OnClickFindMemberButton(){
            var param = new FindMemberPage.Param();
            param.clubData = _clubData; 
            param.myAccessLevel = _myAccessLevel; 
            var pageManager = (ClubPage)Manager;
            pageManager.OpenPage(ClubPageType.FindMember, true, param);
        }

        public void OnClickFindClubButton(){
            var pageManager = (ClubPage)Manager;
            var param = new FindClubPage.Param();
            param.isFirstOpenSolicitationList = false;
            pageManager.OpenPage(ClubPageType.FindClub, true, param);
        }

        public void OnClickChatButton(){
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, null);
        }

        public void OnClickInformationBoardButton(){
            OpenInformationBoardModal().Forget();
        }

        public async UniTask OpenClubInfoModal(){
            var param = new ClubInfoModal.Param();
            param.clubData = _clubData; 
            param.onUpdateClub = UpdateView; 
            param.myUserID = UserData.UserDataManager.Instance.user.uMasterId;
            param.onFinishedDissolution = ClubUtility.ChangeFindClubPage; 
            param.onFinishedSecession = ClubUtility.ChangeFindClubPage; 
            param.updateViewRequest = UpdateView;
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.ClubInfo, param);
            UpdateNotificationBadge( _clubData );
        }
        

        public async UniTask OpenInformationBoardModal(){
            var param = new ClubInformationBoardModal.Param();
            param.clubData = _clubData;
            param.myAccessLevel = _myAccessLevel; 
            
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.ClubInformationBoard, param);
            UpdateNotificationBadge( _clubData );
        }


        public async UniTask UpdateView(){
            await UpdateView(_clubData);
        }


        async UniTask UpdateView( ClubData data ){
            UpdateNotificationBadge( data );
            await _information.UpdateGuildView(_clubData);
            _members.UpdateView( _clubData.memberList );
            
        }

        void UpdateNotificationBadge( ClubData data ){
            _badgeController.UpdateNotificationBadge(data);
        }
    }
}
