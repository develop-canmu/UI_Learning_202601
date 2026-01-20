using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.App;
using CruFramework;
using CruFramework.UI;
using Pjfb.Storage;


namespace Pjfb.Club
{
    public class FindClubPage : Page
    {

        public class Param{
            public bool isFirstOpenSolicitationList = false;
        }

        public UINotification notification => _notification;
        
        [SerializeField]
        FindClubPageTabSheetManager _sheetManager = null;
        [SerializeField]
        FindClubPageTabButton _solicitationListTabButton = null;
        [SerializeField]
        FindClubPageTabButton _createTabButton = null;
        [SerializeField]
        UIButton _backButton = null;
        [SerializeField]
        UINotification _notification = null;
        [SerializeField]
        UIBadgeNotification _solicitationBadge = null;
        [SerializeField]
        UIBadgeNotification _joinSolicitationBadge = null;

        

        bool _isJoinClub = false;
        UIBadgeNotification _targetBadge = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token){
            var param = (Param)args;

            var request = new GuildGetInvitationGuildListAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            _isJoinClub = Pjfb.UserData.UserDataManager.Instance.user.gMasterId != 0;
            _solicitationBadge.SetActive(false);
            _joinSolicitationBadge.SetActive(false);
            _targetBadge = _isJoinClub ? _joinSolicitationBadge : _solicitationBadge;
            _targetBadge.SetActive(LocalSaveManager.saveData.clubCheckNotificationData.solicitation.IsNotification());

            
            var clubPage = (ClubPage)Manager;
            
            _sheetManager.OnOpenSheet += OnOpenSheet;
            if( param.isFirstOpenSolicitationList ) {
                _sheetManager.OpenSheet(FindClubPageTabSheetType.SolicitationList, null);
            } else {
                _sheetManager.OpenSheet(FindClubPageTabSheetType.FindClub, null);
            }
            
            if( _isJoinClub ) {
                _solicitationListTabButton.SetRightImage();
            } else {
                _solicitationListTabButton.SetCenterImage();
            }

            _createTabButton.gameObject.SetActive(!_isJoinClub);
            _backButton.gameObject.SetActive(_isJoinClub);
            
            await base.OnPreOpen(args, token);

            
        }
        

        void OnOpenSheet( FindClubPageTabSheetType type ){
            var sheet = _sheetManager.CurrentSheet as FindClubPageSheet;
            sheet.SetPage(this);
            if( type == FindClubPageTabSheetType.SolicitationList ) {
                _targetBadge.SetActive(false);
                AppManager.Instance.UIManager.Footer.UpdateClubBadge();
            }

        }

        protected override UniTask OnMessage(object value)
        {
            if (value is PageManager.MessageType type)
            {
                switch (type)
                {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.OpenClubTutorialAsync().Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
    }
}
