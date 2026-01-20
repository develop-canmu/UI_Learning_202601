using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework;
using CruFramework.Adv;
using Pjfb.Training;

namespace Pjfb.Storage
{
    [System.Serializable]
    public class TrainingLocalSaveData
    {
        [SerializeField]
        private AdvMessageLogData[] advLogDatas = null;
        /// <summary>ログ</summary>
        public AdvMessageLogData[] AdvLogDatas{get{return advLogDatas;}set{advLogDatas = value;}}
        
        [SerializeField]
        private int autoMode = 0;
        /// <summary>オートモード</summary>
        public  int AutoMode{get{return autoMode;}set{autoMode = value;}}
        
        [SerializeField]
        private bool skipCautionModal = false;
        /// <summary>スキップ注意モーダル</summary>
        public  bool SkipCautionModal{get{return skipCautionModal;}set{skipCautionModal = value;}}
        
        [SerializeField]
        private long latestPlayTrainingId = -1;
        /// <summary>最後に遊んだトレーニング</summary>
        public  long LatestPlayTrainingId{get{return latestPlayTrainingId;}set{latestPlayTrainingId = value;}}
    }
}