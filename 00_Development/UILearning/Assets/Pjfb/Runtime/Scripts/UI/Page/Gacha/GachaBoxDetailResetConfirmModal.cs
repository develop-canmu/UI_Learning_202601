using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxDetailResetConfirmModal : ModalWindow
    {
        public class Param {
            public long gachaCategoryId = 0; // ガチャカテゴリID
            public Action<long, bool> onReset = null;
        }
        
        Param _param = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _param = (Param)args;
            await base.OnPreOpen(args, token);
        }
        
        public void OnClickReset()
        {
            ConnectReset().Forget();
        }

        public void OnClickClose()
        {
            Close();
        }

        async UniTask ConnectReset(){
            var request = new GachaBoxResetAPIRequest();
            var post = new GachaBoxResetAPIPost();
            post.mGachaCategoryId = _param.gachaCategoryId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            _param.onReset?.Invoke( response.boxContentCount, response.canResetBox );
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop( window => true );
            Close();
        }
        
    }
}
