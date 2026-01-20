using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;
using UnityEditor;
using System.Diagnostics.Contracts;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using System.Linq;

namespace Pjfb
{
    public class UserNameChangeModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public string useName;
            public Action<string> applyName;
        }

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private int characterLimit;
        [SerializeField] private UIButton okButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI userNameTitleText;
        [SerializeField] private TextMeshProUGUI wordCountText;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI annotationText;
        [SerializeField] private TextMeshProUGUI showText;

        private string changedName;
        private WindowParams windowParams;
        private PointMasterObject pointMaster;
        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserNameChange, data);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            okButton.interactable = false;
            windowParams = (WindowParams)args;
            inputField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, int.MaxValue, inputField.fontAsset,removeOtherSymbol:true);
            Init();
            return base.OnPreOpen(args, token);
        }
        private void Init()           
        {
            OnInputFieldValueChanged(windowParams.useName);
            pointMaster = MasterManager.Instance.pointMaster.FindData(ConfigManager.Instance.changeUserNamePointId);
            titleText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.title"]);   
            userNameTitleText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.nametitle"]);       
            headerText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.header"]);
            annotationText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.annotation"],pointMaster.name,ConfigManager.Instance.changeUserNamePointValue);
        }

        #endregion


        #region EventListeners

        public async void OnClickPositiveButton()
        {
            await ValidateUserNameAPI(changedName);
        }

        public void OnClickClose()
        {
            Close();
        }

        public void OnInputFieldValueChanged(string input)
        {
            inputField.SetTextWithoutNotify(input);
            showText.text = input;
            wordCountText.text = $"{input?.Length ?? 0}/{characterLimit}";
        }

        // onEndEditからわたってくる引数はCharacterLimit(文字数制限)がかかった状態でわたってくる
        public void OnInputFieldEndEdit(string input)
        {
            input = StringUtility.GetLimitNumUserName(inputField.text, characterLimit);
            wordCountText.text = $"{input?.Length ?? 0}/{characterLimit}";
            inputField.SetTextWithoutNotify(input);
            showText.text = input;
            okButton.interactable = (inputField.text.Length > 0 && inputField.text != windowParams.useName);
            changedName = input;
        }

        public void OnClickInputField()
        {
            inputField.ActivateInputField();
        }

        #endregion
        
        // 名前が適切か確認するAPI
    
        private async UniTask ValidateUserNameAPI(string text)
        {
            try
            {
                UserValidateNameAPIRequest request = new UserValidateNameAPIRequest();
                UserValidateNameAPIPost post = new UserValidateNameAPIPost { name = text };
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);
                var param = new UserNameChangeConfirmModalWindow.Params();
                param.oldName = windowParams.useName;
                param.newName = text;
                param.applyName = windowParams.applyName;
                UserNameChangeConfirmModalWindow.Open(param);
            }
            catch (APIException e)
            {
                if (e.errorParam == (long)APIResponseCode.Failed)
                {
                    //サーバー側で処理するため何もしない
                }
                else
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}