using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class HelpItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private RawImage image;
        [SerializeField] private CancellableWebTexture texture;
        [SerializeField] private LayoutElement imageLayout;
        [SerializeField] private UIButton button;
        [SerializeField] private UIButton negativeButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private TextMeshProUGUI negativeButtonText;

        private HelpModalWindow.MHelp info;
        private string currentImagePath = "";
        private Action onButtonClick = null;
        public async void Init(HelpModalWindow.MHelp data)
        {
            info = data;
            titleText.text = info.title;
            bodyText.text = info.body;
            SetClickTextAndAction();
            
            if (string.IsNullOrEmpty(info.image)) return;
            currentImagePath =  AppEnvironment.AssetBrowserURL + "/help_image/" + info.image;
            await texture.SetTextureAsync(currentImagePath);
            AdjustImageSize();
        }

        private void AdjustImageSize()
        {
            image.SetNativeSize();
            float maxWidth = bodyText.rectTransform.rect.width;
            var rect = image.rectTransform.rect.size;
            if (imageLayout)
            {
                //画像が幅より大きい場合はサイズを調整します
                if (rect.x > maxWidth) 
                {
                    rect.y = maxWidth * rect.y / rect.x;
                    rect.x = maxWidth;
                    image.rectTransform.sizeDelta = new Vector2(rect.x, rect.y);
                }
                imageLayout.minWidth = rect.x;
                imageLayout.minHeight = rect.y;
            }
        }

        #region EventListeners

        private void SetClickTextAndAction()
        {
            var splitCommand = info.onClick?.Split(':');
            if (splitCommand == null || splitCommand?.Length < 2)
            {
                button.gameObject.SetActive(false);
                negativeButton.gameObject.SetActive(false);
                return;
            }
            
            switch (splitCommand[0])
            {
                case "openModal":
                    switch (splitCommand[1])
                    {
                        case "withdraw":
                            negativeButtonText.text = StringValueAssetLoader.Instance["menu.help.withdraw_button"];
                            onButtonClick = OnClickUserWithdrawButton;
                            button.gameObject.SetActive(false);
                            negativeButton.gameObject.SetActive(true);
                            break;
                    }
                    break;
            }
        }

        public void OnClickAction()
        {
            onButtonClick?.Invoke();
        }
        
        private void OnClickUserWithdrawButton()
        {
            // 確認モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["user.withdraw"];
            // メッセージ
            data.Message = StringValueAssetLoader.Instance["menu.help.user_withdraw_confirm"];
            // 注意
            data.Caution = StringValueAssetLoader.Instance["menu.help.user_withdraw_caution"];
            // 削除ボタン
            data.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["menu.help.withdraw_button"],
                window1 =>
                {
                    // 再確認モーダル
                    ConfirmModalData data = new ConfirmModalData();
                    // タイトル
                    data.Title = StringValueAssetLoader.Instance["common.confirm"];
                    // メッセージ
                    data.Message = StringValueAssetLoader.Instance["menu.help.user_withdraw_reconfirm"];
                    // 注意
                    data.Caution = StringValueAssetLoader.Instance["menu.help.user_withdraw_caution"];
                    // 削除ボタン
                    data.PositiveButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["menu.help.withdraw_button"],
                        async window2 =>
                        {
                            // ユーザー削除API
                            UserWithdrawAPIRequest request = new UserWithdrawAPIRequest();
                            UserWithdrawAPIPost post = new UserWithdrawAPIPost();
                            post.appToken = LocalSaveManager.immutableData.appToken;
                            request.SetPostData(post);
                            await APIManager.Instance.Connect(request);
                            
                            // 結果モーダル
                            ConfirmModalData data = new ConfirmModalData();
                            data.Title = StringValueAssetLoader.Instance["title.withdraw_finished"];
                            data.Message = StringValueAssetLoader.Instance["menu.help.withdraw_success"];
                            data.PositiveButtonParams = new ConfirmModalButtonParams(
                                StringValueAssetLoader.Instance["common.ok"],
                                window3 =>
                                {
                                    // モーダルクリア
                                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(_ => true);
                                    // 結果モーダルを閉じる、再起動
                                    window3.Close(()=>AppManager.Instance.BackToTitle());
                                }
                            );
                            // 結果モーダルを開く
                            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                        }
                    ,isRed:true);
                    // キャンセルボタン
                    data.NegativeButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.cancel"],
                        window2 =>
                        {
                            // 確認モーダルまでスタックから取り除く
                            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window.GetType() == typeof(ConfirmWithCheckboxModalWindow));
                            // 再確認モーダルを閉じる
                            window2.Close();
                        }
                    );
                    // 再確認モーダル開く
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                }
                , isRed:true);
            // キャンセルボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.cancel"],
                window1 => window1.Close()
            );
            // 確認モーダル(チェックボックス)を開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ConfirmWithCheckbox, data);
        }

        #endregion
    }
}