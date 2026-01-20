using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Audio;
using UnityEngine;
using TMPro;

namespace CruFramework.Adv
{
    public sealed class AdvMessageWindow : MonoBehaviour
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.MessageWindows))]
        private int windowId = 0;
        /// <summary>Id</summary>
        public int WindowId{get{return windowId;}}
        
        [SerializeField]
        private RubyTextMeshProUGUI text = null;
        
        [SerializeField]
        private GameObject speakerRoot = null;
        
        [SerializeField]
        private TextMeshProUGUI speakerRubyName = null;
        [SerializeField]
        private TextMeshProUGUI speakerName = null;
        
        [SerializeField]
        private float messageSpeed = 0.1f;
        
        // メッセージ位置
        private int messageIndex = 0;
        // 時間
        private float messageTimer = 0;
        
        private float startAt = 0;
        
        private float currentMessageSpeed = 0;
        
        private AudioPlayer voicePlayer = null;
        
        private AdvText advText = new AdvText();
        
        /// <summary>メッセージのアニメーションが終わっているか</summary>
        public bool IsEndMesssageAnimation
        {
            get
            {
                return advText.Length <= messageIndex;
            }
        }
        
        public bool IsNextAutoMessage
        {
            get
            {
                // テキストが終わってない
                if(IsEndMesssageAnimation == false)return false;
                // ボイス再生中
                if(voicePlayer != null && voicePlayer.AudioSource.isPlaying)return false;
                // テキスト数で適当に時間待機
                if(startAt < (float)advText.Length * 0.1f)return false;
                
                return true;
            }
        }
        
        /// <summary>アニメーションを終了させて全てのメッセージを表示</summary>
        public void ForceEndMessageAnimation()
        {
            messageIndex = advText.Length;
            text.UnditedText = advText.GetText(messageIndex);
        }
        
        /// <summary>話者の表示</summary>
        public void ShowSpeaker(bool show)
        {
            speakerRoot.gameObject.SetActive(show);
        }
        
        /// <summary>話者の表示</summary>
        public void SetSpeakerRuby(string name)
        {
            speakerRubyName.text = name;
        }
        
        /// <summary>話者の表示</summary>
        public void SetSpeaker(string name)
        {
            speakerName.text = name;
            speakerName.gameObject.SetActive( string.IsNullOrEmpty(name) == false );
        }
        
        /// <summary>メッセージ</summary>
        public void SetMessage(string msg)
        {
            advText.SetText(msg);
            currentMessageSpeed = messageSpeed;
            messageIndex = 0;
            messageTimer = 0;
            text.UnditedText = string.Empty;
            startAt = 0;
        }
        
        public void SetVoicePlayer(AudioPlayer player)
        {
            voicePlayer = player;
        }

        private void Update()
        {
            
            startAt += Time.deltaTime;
            
            // 既にアニメーションが終わっている
            if(IsEndMesssageAnimation)return;
            messageTimer += Time.deltaTime;
            // 文字追加
            if(messageTimer >= currentMessageSpeed)
            {
                messageTimer -= currentMessageSpeed;
                messageIndex++;
                text.UnditedText = advText.GetText(messageIndex);
                
                // メッセージの速度
                if(advText.GetTagValue(AdvTextTagName.spd, messageIndex, out float speed))
                {
                    currentMessageSpeed = speed;
                }
                else
                {
                    currentMessageSpeed = messageSpeed;
                }
            }
        }
    }
}