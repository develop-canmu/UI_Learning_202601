using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PolyQA;
using UnityEngine;

namespace Pjfb
{
    public static class AppErrorHandler
    {
        /// <summary>無視したい文字列</summary>
        private static string[] ignoreLogStrings = new string[]
        {
            // API関連は無視する
            "[API]"
        };

        /// <summary>例外が最後までキャッチされなかった場合タイトルに戻す</summary>
        public static void LogMessageReceived(string logString, string stackTrace, LogType type)
        {
            // Exceptionが最後までキャッチされなかった
            if (type == LogType.Exception)
            {
                // 文字列チェック
                for(int i = 0; i < ignoreLogStrings.Length; i++)
                {
                    // 無視したい文字列が含まれている
                    if(logString.Contains(ignoreLogStrings[i])) return; 
                }
                
                // モーダルデータ
                ConfirmModalData modalData = new ConfirmModalData();
                // タイトル
                modalData.Title = StringValueAssetLoader.Instance["common.error"];
                // メッセージ
                modalData.Message = StringValueAssetLoader.Instance["common.unkown_error_message"];
                // OKボタン
                modalData.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.ok"],
                    window =>
                    {
                        // タイトルに戻る
                        AppManager.Instance.BackToTitle();
                    }
                );
                // モーダルを開く
                AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, modalData);
                DataSender.Send("UnknownError", "Unknown");
            }
        }
    }
}
