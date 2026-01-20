using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using TMPro;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;

namespace Pjfb.Club
{
    public class SendJoinRequestListItem : ClubListItem {

        public new class Param : ClubListItem.Param {
            public string message = ""; // 勧誘メッセージ
		    public string expireAt = ""; // 勧誘が無効になる日時

            public Action<ClubListItem.Param> onClickedCancel {get; set;} 
            public Param( GuildSearchGuildStatus status, Action<ClubListItem.Param> onClickedCancel ) : base(status) {
                this.onClickedCancel = onClickedCancel;
            }
        }


        protected override async UniTask UpdateView( ClubListItem.Param param){
            await base.UpdateView(param);
        }
        public void OnClickedCancel(){
            if( _param is Param ) {
                var SendJoinRequestParam = _param as Param;
                SendJoinRequestParam.onClickedCancel?.Invoke(_param);
            }
            
        }

    }
}