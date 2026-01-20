using System;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;
using Pjfb.Shop;
using Pjfb.Story;
using Pjfb.UserData;
using UnityEngine;
using Pjfb.Runtime.Scripts.Utility;

namespace Pjfb.Home
{
    public enum HomePageType
    {
        HomeTop,
        LoginBonus,
    }
    
    public class HomePage : PageManager<HomePageType>
    {
        public class PageParameters
        {
            public bool isFromTitle;
        }

        private static DateTime HomeGetDataAPIResponseTimeStamp = DateTime.MinValue;
        protected override string GetAddress(HomePageType page)
        {
            switch(page)
            {
                case HomePageType.LoginBonus : return "Prefabs/UI/Page/LoginBonus/LoginBonusPage.prefab";
                case HomePageType.HomeTop: return "Prefabs/UI/Page/Home/HomeTopPage.prefab";
                default: throw new Exception("PageTypeが定義されてません。");
                
            }
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            var pageParameters = args != null ? (PageParameters) args : new PageParameters();
            
            // ページスタック削除
            AppManager.Instance.UIManager.PageManager.ClearAllPageStack();
            AppManager.Instance.UIManager.PageManager.AddPageStack(PageType.Home, args);

            var ignoreUpdate = HomeGetDataAPIResponseTimeStamp.IsSameMinute(AppTime.Now);
            var homeApiResponse = await ConnectHomeGetDataApi(
                ignoreUpdate: ignoreUpdate ? 1 : 0
            );
            var unlockCombination = CombinationManager.IsUnLockCombination();
            if (unlockCombination)
            {
                await CombinationManager.GetCombinationCollectionListOpenedAPI();
            }
            
            // ハントデータ取得
            var huntResultList = RivalryManager.Instance.huntResultList;
            if (huntResultList == null || huntResultList.Count == 0)
            {
                await RivalryManager.Instance.GetHuntGetDataAPI();
            }
            
            HomeGetDataAPIResponseTimeStamp = AppTime.Now;
            UserDataManager.Instance.UpdateHasTrustPrize(homeApiResponse.hasTrustPrize);
            AppManager.Instance.UIManager.Footer.ShopButton.SetNotificationBadge(
                ShopManager.HasNewBillingReward(homeApiResponse.mBillingRewardBonusIdList.ToList()) ||
                homeApiResponse.freeBillingRewardBonusCount > 0);
            AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
            
            var gachaFooterButton = AppManager.Instance.UIManager.Footer.GachaButton.GetComponent<UIGachaFooterButton>();
            if( gachaFooterButton != null ) {
                gachaFooterButton.UpdateBallon( homeApiResponse.hasFreeGacha, homeApiResponse.hasPendingGacha );
            }
            
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();
            
            RivalryManager.ResetCache();
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
            
            CombinationManager.ClearCache();
            
            LocalPushNotificationUtility.SetLocalNotification();
            
            if (homeApiResponse.loginStampReceiveResultList?.Length > 0)
            {
                await OpenPageAsync(HomePageType.LoginBonus, stack: false, homeApiResponse, token);
            }
            else
            {
                await OpenPageAsync(HomePageType.HomeTop, stack: false, new HomeTopPage.PageParam {
                    homeApiResponse = homeApiResponse,
                    isOnMessageShowPopups = true,
                    isFromTitle = pageParameters.isFromTitle && TransitionType == PageTransitionType.Open
                }, token);
            }
        }

        private async UniTask<HomeGetDataAPIResponse> ConnectHomeGetDataApi(long ignoreUpdate)
        {
            var request = new HomeGetDataAPIRequest();
            request.SetPostData(new HomeGetDataAPIPost{ignoreUpdate = ignoreUpdate});
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();

            return response;
        }
    }
}
