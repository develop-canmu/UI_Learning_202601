using System;
using System.Linq;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Community;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameFooterUI : MonoBehaviour
    {
        [SerializeField] private UIButton sendButton;
        [SerializeField] private GameObject stampListRoot;
        [SerializeField] private Image stampButtonBG;
        [SerializeField] private Image stampButtonIcon;
        [SerializeField] private ScrollGrid stampScroll;
        [SerializeField] private TMP_InputField inputField;

        private string input;
        private bool isThrottling = false;
        private const int ChatThrottleSeconds = 3;

        public void Initialize()
        {
            inputField.onValidateInput = (currentStr, index, inputChar) =>
                StringUtility.OnValidateInput(currentStr, index, inputChar, inputField.characterLimit, inputField.fontAsset);
            OnInputStringChanged();
            SetStampScrollItems();
        }

        public void OnClickSendButton()
        {
            SendChat(input.Trim(), 0).Forget();
        }

        private async UniTask SendChat(string input, long stampId)
        {
            var isSendSucceed = await PjfbGameHubClient.Instance.SendChat(input, stampId);
            inputField.text = string.Empty;

            if (!isSendSucceed)
            {
                AppManager.Instance.UIManager.System.UINotification.ShowNotification(StringValueAssetLoader.Instance["clubroyalingame.failed_to_send_chat"]);
            }

            ThrottleSendChat().Forget();
        }

        private async UniTask ThrottleSendChat()
        {
            isThrottling = true;
            await UniTask.Delay(TimeSpan.FromSeconds(ChatThrottleSeconds), cancellationToken: destroyCancellationToken);
            isThrottling = false;
            OnInputStringChanged();
        }
        
        public void OnClickStampButton()
        {
            stampListRoot.SetActive(!stampListRoot.activeSelf);
            stampButtonBG.color = stampListRoot.activeSelf ?  ColorValueAssetLoader.Instance["default"] : Color.white;
            stampButtonIcon.color = stampListRoot.activeSelf ?  Color.white : ColorValueAssetLoader.Instance["default"];
        }
        
        public void OnClickLeaveButton()
        {
            var cancelButtonParam = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], modal => modal.Close());
            var confirmButtonParam = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.yes"], modal =>
            {
                LeaveAsync().Forget();
                modal.Close();
            }, true);
            
            var args = new ConfirmModalData(StringValueAssetLoader.Instance["common.confirm"], StringValueAssetLoader.Instance["clubroyalingame.leave_battle_modal_message"], string.Empty, confirmButtonParam, cancelButtonParam);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, args);
        }

        private async UniTask LeaveAsync()
        {
            await PjfbGameHubClient.Instance.LeaveAsync();
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null);
        }
        
        public void OnInputStringChanged()
        {
            input = StringUtility.GetLimitNumCharacter(inputField.text, inputField.characterLimit);
            inputField.SetTextWithoutNotify(input);
            sendButton.interactable = !string.IsNullOrEmpty(input) && !isThrottling;
        }
        
        public void OnEndEditInputString()
        {
            // ??w
            OnInputStringChanged();
        }

        private void SetStampScrollItems()
        {
            var stampInfoList = UserDataManager.Instance.userChatStamps.Select(id => new ChatStampInfo { stampId = id, OnClickEvent = OnClickStampItem }).ToList();
            stampScroll.SetItems(stampInfoList);
        }

        private void OnClickStampItem(long stampId)
        {
            if (isThrottling)
            {
                return;
            }
            
            SendChat(string.Empty, stampId).Forget();
            OnClickStampButton();
        }
    }
}