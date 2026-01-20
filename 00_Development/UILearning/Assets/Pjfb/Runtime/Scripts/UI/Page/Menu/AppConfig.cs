using System;
using CruFramework.Audio;
using Pjfb.Storage;
using UnityEngine;

namespace Pjfb.Menu
{
    [Serializable]
    public class AppConfig
    {
        [Serializable]
        public enum DisplayType
        {
            Standard,
            Light
        }
        
        [SerializeField]
        private int _bgmVolume = 5;
        /// <summary>BGM Volume設定</summary>
        public int bgmVolume
        {
            get { return _bgmVolume;}
            set
            {
                AudioManager.Instance.SetVolume(AudioGroup.BGM, value / 10.0f);
                _bgmVolume = value;
            }
        }
        
        [SerializeField]
        private int _seVolume = 5;
        /// <summary>SE Volume設定</summary>
        public int seVolume
        {
            get { return _seVolume;}
            set
            {
                AudioManager.Instance.SetVolume(AudioGroup.SE, value / 10.0f);
                _seVolume = value;
            }
        }
        
        [SerializeField]
        private int _voiceVolume = 5;
        /// <summary>Voice Volume設定</summary>
        public int voiceVolume
        {
            get { return _voiceVolume;}
            set
            {
                AudioManager.Instance.SetVolume(AudioGroup.Voice, value / 10.0f);
                _voiceVolume = value;
            }
        }
        
        [SerializeField]
        private int _fieldDisplayType;
        /// <summary>フィールド表示設定</summary>
        public int fieldDisplayType
        {
            get { return _fieldDisplayType; }
            set
            {
                _fieldDisplayType = value;
            }
        }
        
        [SerializeField]
        private int _trainingDisplayType;
        /// <summary>トレーニング演出</summary>
        public int trainingDisplayType
        {
            get { return _trainingDisplayType; }
            set
            {
                _trainingDisplayType = value;
            }
        }
        
        [SerializeField]
        private int characterCardEffectType;
        /// <summary>キャラカードエフェクト表示</summary>
        public int CharacterCardEffectType
        {
            get { return characterCardEffectType; }
            set { characterCardEffectType = value; }
        }
        
        [SerializeField]
        private bool isTrainingConfirmRest = true;
        /// <summary>休憩時の確認</summary>
        public bool IsTrainingConfirmRest{get{return isTrainingConfirmRest;}set{isTrainingConfirmRest = value;}}
        
        [SerializeField]
        private bool isTrainingConfirmPracticeGame = true;
        /// <summary>練習試合の確認</summary>
        public bool IsTrainingConfirmPracticeGame{get{return isTrainingConfirmPracticeGame;}set{isTrainingConfirmPracticeGame = value;}}

        [SerializeField] 
        private bool isTrainingConfirmPracticeCardRedraw = true;
        /// <summary>練習カードの再抽選確認</summary>
        public bool IsTrainingConfirmPracticeCardRedraw{get{return isTrainingConfirmPracticeCardRedraw;}set{isTrainingConfirmPracticeCardRedraw = value;}}
        
        [SerializeField] 
        private bool isTrainingSkipBoostChanceEffect = false;
        /// <summary>ブーストチャンス演出スキップ</summary>
        public bool IsTrainingSkipBoostChanceEffect{get{return isTrainingSkipBoostChanceEffect;}set{isTrainingSkipBoostChanceEffect = value;}}

        
        /// <summary>Last BGM Volume設定</summary>
        [SerializeField]
        private int lastBgmVolume = 5;
        public int LastBgmVolume{get{return lastBgmVolume;}set{lastBgmVolume = value;}}
        
        /// <summary>Last Se Volume設定</summary>
        [SerializeField]
        private int lastSeVolume = 5;
        public int LastSeVolume{get{return lastSeVolume;}set{lastSeVolume = value;}}
        
        /// <summary>Last Voice Volume設定</summary>
        [SerializeField]
        private int lastVoiceVolume = 5;
        public int LastVoiceVolume{get{return lastVoiceVolume;}set{lastVoiceVolume = value;}}
        
        /// <summary>同じクラスの判断</summary>
        public bool IsEqual(object obj)
        {
            if(obj is AppConfig setting)
            {
                return setting.bgmVolume == bgmVolume &&
                       setting.seVolume == seVolume &&
                       setting.voiceVolume == voiceVolume &&
                       setting.fieldDisplayType == fieldDisplayType &&
                       setting.trainingDisplayType == trainingDisplayType &&
                       setting.CharacterCardEffectType == CharacterCardEffectType &&
                       setting.isTrainingConfirmRest == isTrainingConfirmRest &&
                       setting.isTrainingConfirmPracticeGame == isTrainingConfirmPracticeGame &&
                       setting.isTrainingConfirmPracticeCardRedraw == isTrainingConfirmPracticeCardRedraw &&
                       setting.lastBgmVolume == lastBgmVolume &&
                       setting.lastSeVolume == lastSeVolume &&
                       setting.lastVoiceVolume == lastVoiceVolume;
            }
            return false;
        }

        #region Static Method
        public static void ApplyConfig()
        {
            var config = LocalSaveManager.saveData.appConfig;
            AudioManager.Instance.SetVolume(AudioGroup.BGM, config.bgmVolume / 10.0f);
            AudioManager.Instance.SetVolume(AudioGroup.SE, config.seVolume / 10.0f);
            AudioManager.Instance.SetVolume(AudioGroup.Voice, config.voiceVolume / 10.0f);
        }

        public static void ResetConfig()
        {
            LocalSaveManager.saveData.appConfig = new AppConfig();
            LocalSaveManager.Instance.SaveData();
            ApplyConfig();
        }

        #endregion
    }
    
}