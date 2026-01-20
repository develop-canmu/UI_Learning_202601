using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> Flowポイント獲得通知 </summary>
    public class TrainingPointNotification : MonoBehaviour
    {
        // 表示メッセージデータ
        private struct MessageData
        {
            // 表示メッセージ
            public string Message;
            // 獲得ポイント
            public long GetPoint;
        }

        // ルートオブジェクト
        [SerializeField]
        private GameObject root;
        
        // 表示メッセージオブジェクト
        [SerializeField]
        private TMP_Text message = null;
        
        // 獲得ポイント
        [SerializeField]
        private TMP_Text getPoint = null;
        
        // メッセージの表示時間
        [SerializeField]
        private float messageTime = 0.5f;

        // 次のメッセージ表示時のインターバル
        [SerializeField]
        private float waitInterval = 0.2f;

        // 表示時間計測タイマー
        private float timer = 0f;
        
        // 表示中か？
        private bool isShowing = false;
        // 待機中か？
        private bool isWaiting = false;
        
        // 表示メッセージのキュー
        private Queue<MessageData> messageQueue = new Queue<MessageData>();
     
        /// <summary> メッセージの追加 </summary>
        public void AddMessage(string message, long getPoint)
        {
            messageQueue.Enqueue(new MessageData() { Message = message, GetPoint = getPoint });
            if (timer <= 0f)
            {
                timer = messageTime;
            }
            gameObject.SetActive(true);
        }
        
        /// <summary> 登録メッセージの破棄 </summary>
        public void ClearMessages()
        {
            gameObject.SetActive(false);
            root.SetActive(false);
            messageQueue.Clear();
            timer = 0f;
            isShowing = false;
            isWaiting = false;
        }

        /// <summary> メッセージの表示 </summary>
        private void ShowMessage(MessageData data)
        {
            message.text = data.Message;
            getPoint.text = string.Format(StringValueAssetLoader.Instance["common.plus_value"], data.GetPoint);
            // メッセージ表示中に
            isShowing = true;
            root.SetActive(true);
        }

        /// <summary> メッセージの非表示 </summary>
        private void HideMessage()
        {
            isShowing = false;
            root.SetActive(false);
        }
        
        private void Update()
        {
            // タイマー未セット
            if (timer <= 0f)
            {
                return;
            }
            
            timer -= Time.deltaTime;
                
            // 表示メッセージがあり現在何も表示してないならメッセージを表示する
            if (messageQueue.Count > 0 && !isShowing && !isWaiting)
            {
                MessageData data = messageQueue.Dequeue();
                ShowMessage(data);
            }
                
            // 表示時間が経過
            if (timer <= 0f)
            {
                // 待機中時間が経過
                if (isWaiting)
                {
                    timer = messageTime;
                    isWaiting = false;
                }
                
                // 表示中
                if (isShowing)
                {
                    // メッセージ表示をオフに
                    HideMessage();
                    // 次のメッセージがあるなら待機時間セット
                    if (messageQueue.Count > 0)
                    {
                        timer = waitInterval;
                        isWaiting = true;
                    }
                    else
                    {
                        // メッセージが無ければ全てクリア
                        ClearMessages();
                    }
                }
            }
        }
    }
}