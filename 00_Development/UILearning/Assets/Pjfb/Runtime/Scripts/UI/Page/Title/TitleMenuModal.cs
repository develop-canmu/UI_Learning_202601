using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using CruFramework.Addressables;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;

namespace Pjfb.Title
{
    public class TitleMenuModal : ModalWindow
    {
        private static readonly string DecisionStringKey = "common.decision";
        private static readonly string CloseStringKey = "common.close";
        private static readonly string CancelStringKey = "common.cancel";
        
        [SerializeField]
        private UIButton userDeleteButton = null;

        protected override UniTask OnOpen(CancellationToken token)
        {
            // ユーザーが作られてる場合のみ押下可能にする
            userDeleteButton.interactable = !string.IsNullOrEmpty(LocalSaveManager.immutableData.appToken);
            return base.OnOpen(token);
        }

        /// <summary>
        /// uGUI
        /// キャッシュクリア
        /// </summary>
        public void OnClickCacheClearButton()
        {
            // 確認モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["title.cache_clear"];
            // メッセージ
            data.Message = StringValueAssetLoader.Instance["title.cache_clear_confirm"];
            // 注意
            data.Caution = StringValueAssetLoader.Instance["title.cache_clear_caution"];
            
            // OKボタン
            data.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance[DecisionStringKey],
                window1 =>
                {
                    // キャッシュクリア実行
                    AddressablesUtility.ClearCache();
                    Master.MasterManager.Instance.DeleteMaster();
                    WebTextureManager.ClearCache();
                    
                    // 結果モーダル
                    ConfirmModalData data = new ConfirmModalData();
                    // タイトル
                    data.Title = StringValueAssetLoader.Instance["title.cache_clear"];
                    // メッセージ
                    data.Message = StringValueAssetLoader.Instance["title.cache_clear_success"];
                    // 閉じるボタン
                    data.NegativeButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance[CloseStringKey],
                        window2 =>
                        {
                            // 確認モーダルをスタックから取り除く
                            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window != this);
                            // 結果モーダルを閉じる
                            window2.Close();
                        }
                    );
                    // 確認モーダルを開く
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                }
            );
            // キャンセルボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance[CancelStringKey],
                // 閉じる
                window => window.Close()
            );
            // 確認モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
        /// <summary>
        /// uGUI
        /// データ一括ダウンロード
        /// </summary>
        public async void OnClickAssetAllDownloadButton()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            AddressablesManager.DownloadInfo downloadInfo = null;
            await AppManager.Instance.LoadingActionAsync(async () =>
            {
                // カタログダウンロード
                var success= await AddressablesUtility.UpdateContentCatalogAsync(token);
                // 失敗した場合はタイトルに戻るので処理しない
                if(!success) return;   
                // ダウンロード情報取得
                downloadInfo = await AddressablesUtility.GetDownloadInfoAsync(AddressablesUtility.RemoteAllBundleKey, token);
            });
            
            if(downloadInfo.DownloadSize > 0)
            {
                await AddressablesUtility.ConfirmAndDownloadAsset(downloadInfo,
                    StringValueAssetLoader.Instance["title.asset_all_download"],
                    StringValueAssetLoader.Instance["title.asset_all_download_confirm"],
                    async () =>
                    {
                        // 結果モーダル
                        ConfirmModalData data = new ConfirmModalData();
                        // タイトル
                        data.Title = StringValueAssetLoader.Instance["title.asset_all_download"];
                        // メッセージ
                        data.Message = StringValueAssetLoader.Instance["title.asset_all_download_success"];
                        // 閉じるボタン
                        data.NegativeButtonParams = new ConfirmModalButtonParams(
                            StringValueAssetLoader.Instance[CloseStringKey],
                            window => 
                            {
                                // 確認モーダルをスタックから取り除く
                                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window != this);
                                // 結果モーダルを閉じる
                                window.Close();
                            }
                        );
                        // 結果モーダルを開く
                        var window = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data, token);
                        // 閉じるまで待機
                        await window.WaitCloseAsync();
                    },
                    null,
                    true,
                    token
                );
            }
            else
            {
                // 確認モーダル
                ConfirmModalData data = new ConfirmModalData();
                // タイトル
                data.Title = StringValueAssetLoader.Instance["title.asset_all_download"];
                // メッセージ
                data.Message = StringValueAssetLoader.Instance["title.asset_already_downloaded"];
                // キャンセルボタン
                data.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance[CloseStringKey],
                    window => window.Close()
                );
                // 確認モーダルを開く
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            }
        }
        
        /// <summary>
        /// uGUI
        /// ユーザーデータ削除
        /// </summary>
        public void OnClickUserDeleteButton()
        {
            // 確認モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["title.user_delete"];
            // メッセージ
            data.Message = StringValueAssetLoader.Instance["title.user_delete_confirm"];
            // 注意
            data.Caution = StringValueAssetLoader.Instance["title.user_delete_caution"];
            // 削除ボタン
            data.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["title.user_delete"],
                window1 =>
                {
                    // 再確認モーダル
                    ConfirmModalData data = new ConfirmModalData();
                    // タイトル
                    data.Title = StringValueAssetLoader.Instance["title.user_delete"];
                    // メッセージ
                    data.Message = StringValueAssetLoader.Instance["title.user_delete_reconfirm"];
                    // 注意
                    data.Caution = StringValueAssetLoader.Instance["title.user_delete_caution"];
                    // 削除ボタン
                    data.PositiveButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["title.user_delete"],
                        window2 =>
                        {
                            //アカウント削除
                            AppManager.Instance.DeleteUserAccount();
                            
                            // 結果モーダル
                            ConfirmModalData data = new ConfirmModalData();
                            data.Title = StringValueAssetLoader.Instance["title.user_delete_finished"];
                            data.Message = StringValueAssetLoader.Instance["title.user_delete_success"];
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
                        StringValueAssetLoader.Instance[CancelStringKey],
                        window2 =>
                        {
                            // 確認モーダルまでスタックから取り除く
                            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window != this);
                            // 再確認モーダルを閉じる
                            window2.Close();
                        }
                    );
                    // 再確認モーダル開く
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                }
            ,isRed:true);
            // キャンセルボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance[CancelStringKey],
                window1 => window1.Close()
            );
            // 確認モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        /// <summary>uGUI</summary>
        public void OnClickUserTransferButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserTransfer, null);
        }
        
        /// <summary>uGUI</summary>
        public void OnClickInquiryButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Inquiry, null);
        }
    }
}
