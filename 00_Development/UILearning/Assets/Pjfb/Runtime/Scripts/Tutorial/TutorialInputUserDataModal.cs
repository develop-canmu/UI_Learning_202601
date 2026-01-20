
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using Pjfb.UI;
using Unity.VisualScripting;

namespace Pjfb
{
    public class TutorialInputUserDataModal : ModalWindow
    {
        private const int INPUT_NAME_LIMIT = 8;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private UIToggle firstPersonToggle;
        [SerializeField] private UIButton nextButton;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            nextButton.interactable = false;
            //char入力制限追加(入力中は制限かけない、確定後8文字以下に整形)
            inputField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, int.MaxValue,inputField.fontAsset,removeOtherSymbol:true);
            return base.OnPreOpen(args, token);
        }

        public void OnEndEdit()
        {
            inputField.text = StringUtility.GetLimitNumUserName(inputField.text,INPUT_NAME_LIMIT);
            nextButton.interactable = inputField.text.Length != 0;
        }

        public void OnClickNext()
        {
            var gender = firstPersonToggle.isOn ? 1 : 2;
            var userName = inputField.text;
            AppManager.Instance.TutorialManager.RegisterUserDetail(userName,gender);
            Close();
        }
    }
}