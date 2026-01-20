using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingPerformanceView : MonoBehaviour
    {
    
        private const float AnimationDuration = 0.5f;
    
        [SerializeField]
        private TMPro.TMP_Text currentTipsText = null;
        [SerializeField]
        private TMPro.TMP_Text rewardTipsText = null;
        [SerializeField]
        private TMPro.TMP_Text performanceLvText = null;

        [SerializeField]
        private Slider parformanceProgressbar = null;
        [SerializeField]
        private Slider nextParformanceProgressbar = null;
        
        
        private long tipCount = 0;
        private long addTipValue = 0;
        private long currentExp = 0;
        private long currentLv = 0;
        private float animationTime = 0;
        private TrainingOverallProgress addProgress = null;
        private Action onLvupAction = null;
        
        
        /// <summary>ビューの更新</summary>
        public void Set(TrainingOverallProgress progress, long tipCount, bool isCache)
        {
            // キャッシュしておく
            if(isCache)
            {
                TrainingUtility.PerformanceProgressCache = progress;
            }
            
            SetRewardTipCount(tipCount);
            // プログレスバー
            if(progress.nextLevelValue <= 0)
            {
                SetPerformance(0);
                SetNextPerformance(0);
                // Lv
                SetPerformanceLvMax();
            }
            else
            {
                // Lv
                SetPerformanceLv(progress.currentLevel);
                SetPerformance( (float)(progress.currentValue - progress.previousLevelValue) / (float)(progress.nextLevelValue - progress.previousLevelValue) );
                SetNextPerformance( (float)(progress.currentValue + tipCount - progress.previousLevelValue) / (float)(progress.nextLevelValue - progress.previousLevelValue) );
            }
        }
        
        /// <summary>追加</summary>
        public void Add(TrainingOverallProgress p, long value, Action onLvup)
        {
            // 追加する進捗
            addProgress = p;
            // 追加するチップ
            addTipValue = value;
            // 追加したチップ表示
            SetRewardTipCount(value);
            // アニメーション時間
            animationTime = AnimationDuration;
            // 現在のExp
            currentExp = TrainingUtility.PerformanceProgressCache.currentValue - TrainingUtility.PerformanceProgressCache.previousLevelValue;
            // 現在のLv
            currentLv = TrainingUtility.PerformanceProgressCache.currentLevel;
            // レベルアップ時のコールバック
            onLvupAction = onLvup;
        }
       
        /// <summary>チップ数</summary>
        public void SetTipCount(long count)
        {
            tipCount = count;
            currentTipsText.text = count.ToString();
        }
        
        /// <summary>報酬のチップ枚数</summary>
        public void SetRewardTipCount(long count)
        {
            if(count <= 0)
            {
                rewardTipsText.gameObject.SetActive(false);
            }
            else
            {
                rewardTipsText.gameObject.SetActive(true);
                rewardTipsText.text = string.Format( StringValueAssetLoader.Instance["training.performance.reward_tip"], count);
            }
            
        }
        
        /// <summary>パフォーマンスレベル</summary>
        public void SetPerformanceLv(long lv)
        {
            performanceLvText.text = string.Format(StringValueAssetLoader.Instance["training.performance.lv"], lv);
        }
        
        /// <summary>パフォーマンスレベル</summary>
        public void SetPerformanceLvMax()
        {
            performanceLvText.text = StringValueAssetLoader.Instance["training.performance.lv_max"];
        }
        
        /// <summary>パフォーマンス</summary>
        public void SetPerformance(float value)
        {
            parformanceProgressbar.value = value;
        }
        
        /// <summary>パフォーマンス</summary>
        public void SetNextPerformance(float value)
        {
            nextParformanceProgressbar.value = value;
        }
        
        private void Update()
        {
            if(animationTime > 0)
            {
                // 時間経過
                animationTime = Mathf.Max(0, animationTime - Time.deltaTime);
                // 表示するチップ数
                long count = (long)(addTipValue * (1.0f - animationTime / AnimationDuration));
                // チップ枚数更新
                currentTipsText.text = (tipCount + count).ToString();
                // Exp
                long exp = currentExp + count;
                // 次のLvまでの経験値
                long nextExp = TrainingUtility.PerformanceProgressCache.nextLevelValue - TrainingUtility.PerformanceProgressCache.previousLevelValue;
                
                // Lvアップチェック
                if(currentLv != addProgress.currentLevel && exp >= nextExp)
                {
                    // 現在のLv更新
                    currentLv = addProgress.currentLevel;
                    // Exp調整
                    exp -= nextExp;
                    currentExp = -count;
                    // Lv表示
                    if(addProgress.nextLevelValue > 0)
                    {
                        SetPerformanceLv(currentLv);
                    }
                    else
                    {
                        SetPerformanceLvMax();
                    }
                    // 進捗更新
                    TrainingUtility.PerformanceProgressCache = addProgress;
                    SetPerformance(0);
                    SetNextPerformance(0);
                    // コールバック
                    onLvupAction();
                }

                // 次のLvがある場合
                if(TrainingUtility.PerformanceProgressCache.nextLevelValue  > 0)
                {
                    SetPerformance( (float)exp / (float)(TrainingUtility.PerformanceProgressCache.nextLevelValue - TrainingUtility.PerformanceProgressCache.previousLevelValue) );
                }
            }
        }
    }
}