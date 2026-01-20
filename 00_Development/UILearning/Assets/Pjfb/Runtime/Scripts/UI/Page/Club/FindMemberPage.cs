using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Storage;



namespace Pjfb.Club
{
    public class FindMemberPage : Page
    {
        public class Param {
            public ClubData clubData = null;
            public ClubAccessLevel myAccessLevel = ClubAccessLevel.None;
        }

        public UINotification notification => _notification;


        [SerializeField]
        ClubFindMemberSheetManager _sheetManager = null;
        [SerializeField]
        UINotification _notification = null;
        [SerializeField]
        UIBadgeNotification _requestJoinBadge = null;
        [SerializeField]
        UIBadgeNotification _solicitingBadge = null;


        Param _param = null;


        protected async override UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            _sheetManager.OnOpenSheet += OnOpenSheet;
            UpdateNotificationBadge( _param.clubData );
            await _sheetManager.OpenSheetAsync(ClubFindMemberSheetType.FindMember, null);
            await base.OnPreOpen(args, token);
        }


        void OnOpenSheet( ClubFindMemberSheetType type ){
            var view = (FindMemberSheetView)_sheetManager.CurrentSheet;
            view.Init(_param.clubData, _param.myAccessLevel ,this, OnFinishedInitSheet);
            
        }

        void OnFinishedInitSheet( FindMemberSheetView sheet ){
            if( sheet is FindMemberSheetRequestJoinView ) {
                LocalSaveManager.saveData.clubCheckNotificationData.requestJoin.Update( _param.clubData );
                LocalSaveManager.Instance.SaveData();
            }
            UpdateNotificationBadge( _param.clubData );
        }
        void UpdateNotificationBadge( ClubData data ){
            var notificationData = LocalSaveManager.saveData.clubCheckNotificationData;
            _requestJoinBadge.SetActive(notificationData.requestJoin.IsNotification());
            _solicitingBadge.SetActive(notificationData.sendSolicitation.IsNotification());
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();
        }

    }
}
