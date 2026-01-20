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
using Pjfb.Club;
using Pjfb.Event;
using Pjfb.Menu;

namespace Pjfb.EventRanking {
    public class EventRankingUserListItem : EventRankingListItem {
        public class Param{
            public string name = "";
            public long userId = 0;
            public long iconId = 0;
            public long rank = 0;
            public BigValue point = BigValue.Zero;
            public bool isCurrent = false;
        }

        [SerializeField]
        TextMeshProUGUI _name = null;
        [SerializeField]
        TextMeshProUGUI _point = null;
        [SerializeField]
        OmissionTextSetter _pointOmissionTextSetter = null;
        [SerializeField]
        UserIcon _icon = null;

        Param _param = null;
        public void UpdateView(Param param){
            _param = param;
            _name.text = param.name;
            _point.text = param.point.ToDisplayString(_pointOmissionTextSetter.GetOmissionData());
            _icon.SetIconId( param.iconId );
            UpdateRankView( param.rank, param.isCurrent );
        }

        public void OnClickItem(){
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(_param.userId, false, onDissolution:UpdateEventTopPage, onSecession:UpdateEventTopPage);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }

        //// <summary> EventTopPageを更新 </summary>
        private void UpdateEventTopPage()
        {
            if (AppManager.Instance.UIManager.PageManager.CurrentPageObject is EventPage eventPage)
            {
                AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                EventTopPage.Data data = new EventTopPage.Data(eventPage.MFestivalTimetableId);
                // トップページを開きなおして更新
                eventPage.OpenPage(EventPageType.EventTop, false, data);
            }
        }
        
        protected override void OnSetView(object value){
            var param = value as Param;
            UpdateView(param);
        }
        

        
    }
}