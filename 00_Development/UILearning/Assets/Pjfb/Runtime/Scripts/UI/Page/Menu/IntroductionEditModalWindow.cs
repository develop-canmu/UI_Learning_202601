using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pjfb.Community;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Utility;
using TMPro;
using UnityEngine;

namespace Pjfb.Menu
{
    public class IntroductionEditModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public string Introduction;
        }
        
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI limitCountText;
        [SerializeField] private UIButton okButton;
        [SerializeField] private TextMeshProUGUI showText;
        
        private WindowParams _windowParams;
        #endregion
        
        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.IntroductionEdit, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            inputField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, inputField.characterLimit,inputField.fontAsset);
            Init();
            return base.OnPreOpen(args, token);
        }
        private void Init()
        {
            OnInputFieldValueChanged(_windowParams.Introduction);
        }
        
        #endregion
        
        
        #region EventListeners

        public void OnClickApply()
        {
            OnClickApplyAsync().Forget();
        }
        
        private async UniTask OnClickApplyAsync()
        {
            SetCloseParameter(await TrainerCardUtility.UpdateTrainerCardIntroduction(_windowParams.Introduction));
            await CloseAsync();
        }

        public void OnInputFieldValueChanged(string input)
        {
            limitCountText.text = $"{input?.Length ?? 0}/{inputField.characterLimit}";
            inputField.SetTextWithoutNotify(input);
            showText.text = input;
            okButton.interactable = inputField.text.Length > 0;
        }

        public void OnInputFieldEndEdit(string input)
        {
            input = StringUtility.GetLimitNumCharacter(input, inputField.characterLimit);
            inputField.SetTextWithoutNotify(input);
            showText.text = input;
            _windowParams.Introduction = input;
            limitCountText.text = $"{input.Length}/{inputField.characterLimit}";
        }

        public void OnClickInputField()
        {
            inputField.ActivateInputField();
        }

        #endregion
    }
}