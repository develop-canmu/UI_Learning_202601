using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Shop;
using Pjfb.UserData;
using Pjfb.Common;

namespace Pjfb
{
    public class MaintenanceModal : ModalWindow {

        public class Param {
            public string errorMessage { get; set;} = "";
            public string maintenanceStartAt { get; set;} = "";
            public string maintenanceEndAt { get; set;} = "";
            public string twitterUrl { get; set;} = "";
        }

        [SerializeField] 
        TextMeshProUGUI _text = null;

        [SerializeField] 
        TextMeshProUGUI _dateText = null;


        Param _param = null;
        protected override UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            _text.text = _param.errorMessage;
            UpdateDateText();
            return base.OnPreOpen(args, token);
        }

        public void OnClickTwitter(){
            var uri = new System.Uri(_param.twitterUrl);
            Application.OpenURL(uri.AbsoluteUri);
        }


        void UpdateDateText(){
            var startAt = DateTime.Parse(_param.maintenanceStartAt);
            var endAt = DateTime.Parse(_param.maintenanceEndAt);
           
            var startAtText = string.Format(StringValueAssetLoader.Instance["maintenance.date_format"], startAt.Month, startAt.Day, startAt.Hour, startAt.Minute );
            var endAtText = string.Format(StringValueAssetLoader.Instance["maintenance.date_format"], endAt.Month, endAt.Day, endAt.Hour, endAt.Minute );
            _dateText.text = string.Format(StringValueAssetLoader.Instance["maintenance.date_format_full"], startAtText, endAtText );
        }

    }
}
