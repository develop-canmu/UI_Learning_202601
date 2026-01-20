using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using  Pjfb.Home;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.LoginBonus
{
    public class LoginBonusPageParam {
        public long masterId{get;private set;} = 0;
        public long prizeNumber{get;private set;} = 0;
        public long priority { get; private set; } = 0;

        public LoginBonusPageParam(Pjfb.Networking.App.Request.LoginStampReceiveResult receiveData,long priority = int.MaxValue){
            masterId = receiveData.mLoginStampId;
            prizeNumber = receiveData.prizeNumber;
            this.priority = priority;
        }
    }

    public class LoginBonusPage : Page
    {
        const string viewAddressKey = "LoginBonusViewAddress";

        LoginBonusView _currentView = null;

        List<LoginBonusPageParam> _loginBonusPageParams = null;
        int _loginViewIndex = 0;

        HomeGetDataAPIResponse _response = null;

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
        private const string DebugMenuKey = "ログインボーナス";
#endif


        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);

            _response = (HomeGetDataAPIResponse)args;
            if( _response == null ) {
                CruFramework.Logger.LogError("login bonus args is empty");
                var homepage = (HomePage)Manager;
                await homepage.OpenPageAsync(HomePageType.HomeTop, stack: false, new HomeTopPage.PageParam{homeApiResponse = _response, isOnMessageShowPopups = false}, token);
                return;
            }

            var loginStampReceiveResult = _response.loginStampReceiveResultList;
            if( loginStampReceiveResult == null || loginStampReceiveResult.Length <= 0 ) {
                CruFramework.Logger.LogError("login bonus list is empty");
                var homepage = (HomePage)Manager;
                await homepage.OpenPageAsync(HomePageType.HomeTop, stack: false, new HomeTopPage.PageParam{homeApiResponse = _response, isOnMessageShowPopups = false}, token);
                return;
            }
            _loginBonusPageParams = new List<LoginBonusPageParam>();
            foreach( var receiveData in loginStampReceiveResult ){
                var priority = MasterManager.Instance.loginStampMaster.FindData(receiveData.mLoginStampId)?.displayPriority ?? int.MaxValue;
                _loginBonusPageParams.Add(new LoginBonusPageParam(receiveData, priority));
            }
            //priorityで順番変更
            _loginBonusPageParams = _loginBonusPageParams.OrderByDescending(data => data.priority).ToList();
            _loginViewIndex = 0;

            await CreateView(_loginBonusPageParams[_loginViewIndex]);
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.AddOption(DebugMenuKey, "スキップ", () => ExitLoginBonusPage().Forget());
#endif
        }


        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.BeginFade:
                        AppManager.Instance.UIManager.Header.Hide(); 
                        AppManager.Instance.UIManager.Footer.Hide();
                        break;
                }
            }
            return base.OnMessage(value);
        }


        async UniTask CreateView(LoginBonusPageParam data) {
            var path = CruFramework.ResourcePathManager.GetPath(viewAddressKey, 0);
            LoginBonusView prefab = null;
            if (_currentView == null)
            {
                await LoadAssetAsync<LoginBonusView>(path, obj => prefab = obj);
                _currentView = Instantiate<LoginBonusView>(prefab, transform);
            }
            await _currentView.Init(data.masterId, data.prizeNumber);
            _currentView.onFinished = NextView;
        }

        void NextView() {
            CreateNextView().Forget();
        }

        async UniTask CreateNextView() {
            ++_loginViewIndex;
            if( _loginViewIndex >= _loginBonusPageParams.Count ) {
                await ExitLoginBonusPage();
            } else {
                
                await AppManager.Instance.UIManager.FadeManager.FadeOutAsync(FadeType.Color);
                
                await CreateView(_loginBonusPageParams[_loginViewIndex]);
                
                await AppManager.Instance.UIManager.FadeManager.FadeInAsync();
            }
        }
        
        private async UniTask ExitLoginBonusPage() 
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            // メニューの削除
            CruFramework.DebugMenu.RemoveOption(DebugMenuKey);
#endif
            await AppManager.Instance.UIManager.FadeManager.FadeOutAsync(FadeType.Color);
            var homepage = (HomePage)Manager;
            await homepage.OpenPageAsync(HomePageType.HomeTop, stack: false, new HomeTopPage.PageParam{homeApiResponse = _response, isOnMessageShowPopups = false, isFromTitle = true});
            await AppManager.Instance.UIManager.FadeManager.FadeInAsync();
        }
    }
}