using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvMessageLogData
    {
        private AdvManager manager = null;
        /// <summary>Manager</summary>
        public  AdvManager Manager{get{return manager;}}
                
        [SerializeField]
        private string speaker = string.Empty;
        /// <summary>話者</summary>
        public string Speaker{get{return speaker;}}
                
        [SerializeField]
        private string message = string.Empty;
        /// <summary>メッセージ</summary>
        public string Message{get{return message;}}
                        
        [SerializeField]
        private int speakerId = 0;
        /// <summary>話者Id</summary>
        public int SpeakerId{get{return speakerId;}}
                
        public AdvMessageLogData(AdvManager manager, string speaker, string message, int speakerId)
        {
            this.manager = manager;
            this.speaker = speaker;
            this.message = message;
            this.speakerId = speakerId;
        }
    }
}