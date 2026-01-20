using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Gacha;
using Pjfb.Master;
using System.Threading;
using TMPro;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Pjfb.Common;
using System;
using System.Drawing;
using Pjfb.Menu;
using Pjfb.Shop;

namespace Pjfb
{
    public class UserNameChangeConfirmModalWindow : ModalWindow
    {
        public class Params
        {
            public string oldName;
            public string newName;
            public Action<string> applyName;
        }

        [SerializeField] private TextMeshProUGUI beforeText = null;
        [SerializeField] private TextMeshProUGUI afterText = null;
        [SerializeField] private UserNameChangeConfirmItem itemEnoughUI = null;
        [SerializeField] private UserNameChangeConfirmItem itemShortageUI = null;
        [SerializeField] private UIButton okButton;
        [SerializeField] private UIButton shopButton;
        private Params param = null;
        private PointMasterObject pointMaster = null;

        public static void Open(Params data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserNameChangeConfirm, data);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (Params)args;

             pointMaster = MasterManager.Instance.pointMaster.FindData(ConfigManager.Instance.changeUserNamePointId);

            //nullチェック
            if (pointMaster == null)
            {
                throw new NullReferenceException();
            }

            beforeText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.change_before_text"], param.oldName);
            afterText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.change_after_text"], param.newName);

            long itemCount = 0;
            if (UserDataManager.Instance.point.data.ContainsKey(pointMaster.id))
            {
                itemCount = UserDataManager.Instance.point.data[pointMaster.id].value;               
            }
             SwithcUI(ConfigManager.Instance.changeUserNamePointValue,itemCount);
            


            return base.OnPreOpen(args, token);
        }

        private void SwithcUI(long pointValue,long itemCount)
        {
            bool isEnough = pointValue <= itemCount;
            itemEnoughUI.gameObject.SetActive(isEnough);
            itemShortageUI.gameObject.SetActive(!isEnough);
            if (isEnough)
            {
                itemEnoughUI.Init(pointMaster,pointValue, itemCount);
                okButton.gameObject.SetActive(true);
                shopButton.gameObject.SetActive(false);
            }
            else
            {
                itemShortageUI.Init(pointMaster,pointValue, itemCount);
                okButton.gameObject.SetActive(false);
                shopButton.gameObject.SetActive(true);
            }
        }

        public async void OnClickPositiveButton()
        {
            await SetUserName(param.newName);
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window.GetType() != typeof(TrainerCardModalWindow));
            Close();
        }
       
        public void OnClickShopButton()
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close();
            if (AppManager.Instance.UIManager.PageManager.CurrentPageType != PageType.Shop)
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, null);
            }
        }

        public void OnClickClose()
        {
            Close();
        }


        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }

        private async UniTask<string> UserUpdateNameAPI(string name)
        {
            UserUpdateNameAPIRequest request = new UserUpdateNameAPIRequest();
            UserUpdateNameAPIPost post = new UserUpdateNameAPIPost { name = name};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData().name;
        }

        private async UniTask SetUserName(string text)
        {
            param.newName = await UserUpdateNameAPI(text);
            param.applyName?.Invoke(param.newName);  //プロフィール画面に名前反映
        }
    }
}