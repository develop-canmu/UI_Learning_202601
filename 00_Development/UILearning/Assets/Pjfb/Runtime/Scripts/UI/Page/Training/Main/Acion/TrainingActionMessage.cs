using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Pjfb.Training
{
    
    
    public class TrainingActionMessage : MonoBehaviour
    {
        // メッセージの表示時間
        private const float Duration = 2.0f;
        // スキップ時のメッセージ速度
        private const float SkipDuration = 2.0f;
        
        [SerializeField]
        private TMPro.TMP_Text text = null;
        
        [SerializeField]
        private int maxQueueCount = 5;
        
        private bool isSkip = false;
        /// <summary>スキップ</summary>
        public bool IsSkip{get{return isSkip;}set{isSkip = value;}}
        
        // 表示時間
        private float activeTimer = 0;
        
        /// <summary>再生中？</summary>
        public bool IsPlaying{get{return activeTimer > 0;}}
        
        // メッセージが複数来た時にキューに貯める
        private Queue<string> messageQueue = new Queue<string>();
        
        // メッセージを予約しておく
        private Queue<string> reservationQueue = new Queue<string>();
        
        /// <summary>メッセージの表示</summary>
        public void SetMessage(string msg)
        {
            // 再生中なのでキューに保存
            if(IsPlaying)
            {
                messageQueue.Enqueue(msg);
            }
            else
            {
                StartMessage(msg);
            }
        }
        
        /// <summary>メッセージの予約</summary>
        public void ReservationMessage(string msg)
        {
            reservationQueue.Enqueue(msg);
        }
        
        /// <summary>予約していたメッセージの表示</summary>
        public void SetReservationMessage()
        {
            while(reservationQueue.Count > 0)
            {
                SetMessage(reservationQueue.Dequeue());
            }
        }
        
        private void StartMessage(string msg)
        {
            // 表示On
            gameObject.SetActive(true);
            // メッセージ
            text.text = msg;
            // タイマー初期化
            activeTimer = Duration;
        }

        private void Update()
        {
            activeTimer -= (Time.deltaTime * (isSkip ? SkipDuration : 1.0f) );
            // 時間経過 or 一定以上溜めないようにする
            if(activeTimer <= 0 || messageQueue.Count > maxQueueCount)
            {
                // キューに溜まってる場合はそれを再生
                if(messageQueue.Count > 0)
                {
                    StartMessage( messageQueue.Dequeue() );
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}