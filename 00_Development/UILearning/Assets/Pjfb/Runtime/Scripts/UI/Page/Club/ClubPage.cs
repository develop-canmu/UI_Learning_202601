using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.SystemUnlock;
using Pjfb.UserData;


namespace Pjfb.Club
{
    public enum ClubPageType{
        MemberTop,
        FindClub,
        FindMember,
    }
    
    public class ClubPage : PageManager<ClubPageType>
    {
        public class Param {
            public ClubPageType firstPageType{get;set;}
        }

        protected override string GetAddress(ClubPageType page)
        {
            return $"Prefabs/UI/Page/Club/{page}Page.prefab";
        }
        
        /// <summary>ページ表示準備</summary>
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
         
            var playTutorial = AppManager.Instance.TutorialManager.OpenScenarioTutorialWithUnlockSystem(PageType.Club, true, args, SystemUnlockDataManager.SystemUnlockNumber.Club);
            if (playTutorial) return;

            if( Pjfb.UserData.UserDataManager.Instance.user.gMasterId == 0 ){
                // 未所属
                var param = new FindClubPage.Param();
                param.isFirstOpenSolicitationList = false;
                await OpenPageAsync(ClubPageType.FindClub, true, param);
            } else {
                // 所属済み
                var pageType = ClubPageType.MemberTop;
                // 引数の指定がある
                if(args != null){
                    var param = (Param)args;
                    pageType = param.firstPageType;
                }

                var request = new GuildGetGuildAPIRequest();
                await APIManager.Instance.Connect(request);
                var response = request.GetResponseData();
                
                if(pageType == ClubPageType.FindMember){
                    // メンバー検索へ
                    var param = new FindMemberPage.Param();
                    param.clubData = new ClubData(request.GetResponseData().guild);
                    param.myAccessLevel = ClubUtility.CreateAccessLevel( UserDataManager.Instance.user.uMasterId, param.clubData );
                    await OpenPageAsync(pageType, true, param);
                }
                else{
                    // メンバートップへ
                    var param = new MemberTopPage.Param();
                    param.isShowCreateNotification = false;
                    param.clubData = new ClubData(request.GetResponseData().guild);
                    param.myAccessLevel = ClubUtility.CreateAccessLevel( UserDataManager.Instance.user.uMasterId, param.clubData );
                    param.guildBattleMatchingList = response.guildBattleMatchingList;
                    await OpenPageAsync(pageType, true, param);
                }
            }
        }
        
        
        /// <summary>閉じる前の処理</summary>
        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            return base.OnPreClose(token);
        }


        async UniTask<GuildGetGuildAPIResponse> ConnectGetGuildAPI(){
            var request = new GuildGetGuildAPIRequest();
            var post = new GuildGetGuildAPIPost();
            post.gMasterId = 0;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
    }
}
