using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.Voice;

namespace Pjfb
{
    [System.Serializable]
    public class TrainingStatusUpVoiceThresholdData
    {
        [SerializeField]
        private int value = 0;
        /// <summary>値</summary>
        public int Value{get{return value;}}
        
        [SerializeField]
        private VoiceResourceSettings.LocationType voiceType = VoiceResourceSettings.LocationType.Unknown;
        /// <summary>ボイス</summary>
        public VoiceResourceSettings.LocationType VoiceType{get{return voiceType;}}
    }
    
    [System.Serializable]
    public class TrainingConditionMessageThresholdData
    {
        [SerializeField]
        private int value = 0;
        /// <summary>値</summary>
        public int Value{get{return value;}}
        
        [SerializeField]
        private string upMessageKey = string.Empty;
        /// <summary>上昇時のメッセージ</summary>
        public string UpMessageKey{get{return upMessageKey;}}
        
        [SerializeField]
        private string downMessageKey = string.Empty;
        /// <summary>現状時のメッセージ</summary>
        public string DownMessageKey{get{return downMessageKey;}}
    }
    
    /// <summary> コンディション帯ごとの情報データ </summary>
    [Serializable]
    public class TrainingConditionStateData
    {
        [SerializeField]
        private TrainingUtility.TrainingConditionType conditionType = TrainingUtility.TrainingConditionType.AWFUL;
        /// <summary>コンディションタイプ</summary>
        public TrainingUtility.TrainingConditionType ConditionType{get{return conditionType;}}
        
        [SerializeField]
        private bool canRest = true;
        /// <summary>休憩可能？</summary>
        public bool CanRest{get{return canRest;}}
        
        [SerializeField]
        private int effectIndex = 0;
        /// <summary>エフェクト</summary>
        public int EffectIndex{get{return effectIndex;}}
        
        [SerializeField]
        private VoiceResourceSettings.LocationType voiceType = VoiceResourceSettings.LocationType.Unknown;
        /// <summary>ボイス</summary>
        public VoiceResourceSettings.LocationType VoiceType{get{return voiceType;}}
    }
    
    [System.Serializable]
    public class TrainingRankVoiceData
    {
        [SerializeField]
        private int rankNumber = 0;
        /// <summary>ランク</summary>
        public int RankNumber{get{return rankNumber;}}
        
        [SerializeField]
        private VoiceResourceSettings.LocationType voiceType = VoiceResourceSettings.LocationType.Unknown;
        /// <summary>ボイス</summary>
        public VoiceResourceSettings.LocationType VoiceType{get{return voiceType;}}
    }
    
    public class TrainingConfig : ScriptableObject
    {
        
        [SerializeField]
        private int statusUpScenarioId = 300;
        /// <summary>ステータスアップ時のシナリオId</summary>
        public int StatusUpScenarioId{get{return statusUpScenarioId;}}
    
        [SerializeField]
        private float statusUpAnimationSkipDuration = 1.0f;
        /// <summary>ステータスアニメーションのスキップする時間</summary>
        public float StatusUpAnimationSkipDuration{get{return statusUpAnimationSkipDuration;}}
        
        [SerializeField]
        private int displayAuraFXThreshold = 200;
        /// <summary>オーラ表示</summary>
        public int DisplayAuraFXThreshold{get{return displayAuraFXThreshold;}}
    
        [SerializeField]
        private float derayTrainingSceneEffect = 1.0f;
        /// <summary>シーン演出の待機時間</summary>
        public float DerayTrainingSceneEffect{get{return derayTrainingSceneEffect;}}
    
    
        [SerializeField]
        private TrainingConditionMessageThresholdData[] conditionMessageThresholdData = null;
        /// <summary>コンディションの増減メッセージ定義</summary>
        public TrainingConditionMessageThresholdData[] ConditionMessageThresholdData{get{return conditionMessageThresholdData;}}
        
        [SerializeField]
        private TrainingConditionStateData[] conditionStateData = null;
        /// <summary>コンディション表示用</summary>
        public TrainingConditionStateData[] ConditionStateData{get{return conditionStateData;}}
        
        [SerializeField]
        private TrainingRankVoiceData[] rankVoiceDatas = null;
        /// <summary>ランク査定ボイス</summary>
        public TrainingRankVoiceData[] RankVoiceDatas{get{return rankVoiceDatas;}}
        
        [SerializeField]
        private TrainingStatusUpVoiceThresholdData[] statusUpVoiceThresholdDatas = null;
        /// <summary>ステータスアップ時のボイス</summary>
        public TrainingStatusUpVoiceThresholdData[] StatusUpVoiceThresholdDatas{get{return statusUpVoiceThresholdDatas;}} 

        [SerializeField]
        private List<string> helpCategories = new List<string>();
        /// <summary>ヘルプを開く際のカテゴリ</summary>
        public IReadOnlyList<string> HelpCategories{get{return helpCategories;}}
    }
}