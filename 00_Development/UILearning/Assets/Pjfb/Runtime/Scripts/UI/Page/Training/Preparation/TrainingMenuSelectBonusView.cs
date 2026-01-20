using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingMenuSelectBonusView : MonoBehaviour
    {
        /// <summary>これ以降の年は無期限として表示しない</summary>
        public const int DisableDateTime = 2099;
        
        [SerializeField]
        private TMPro.TMP_Text bonusText = null;

        [SerializeField]
        private GameObject dateRoot = null;
        
        [SerializeField]
        private TMPro.TMP_Text dateText = null;
        
        [SerializeField]
        private float bonusDisplayInterval = 2.0f;
        
        // ボーナス表示Index
        private int bonusDisplayIndex = 0;
        // ボーナス表示切り替え用の時間
        private float bonusDisplayTimer = 0;
        // ボーナスリスト
        private List<long> mStatusBonusIds = new List<long>();
        
        /// <summary>表示</summary>
        public void SetView(long mTrainingScenarioId)
        {
            mStatusBonusIds.Clear();
            // ボーナスを取得
            foreach(TrainingScenarioStatusBonusMasterObject masterData in MasterManager.Instance.trainingScenarioStatusBonusMaster.values)
            {
                // シナリオIdをチェック
                if(masterData.mTrainingScenarioId != mTrainingScenarioId)
                {
                    continue;
                }
                
                // 期限ないかチェック
                if(AppTime.IsInPeriod(masterData.startAt, masterData.endAt) == false)
                {
                    continue;
                }
                
                // ボーナス確定
                mStatusBonusIds.Add(masterData.id);
            }
            
            // ボーナスがない場合は非表示
            if(mStatusBonusIds.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            // 期間の表示
            dateRoot.SetActive(true);
            // 表示
            gameObject.SetActive(true);
            
            // 時間を初期化
            bonusDisplayIndex = 0;
            bonusDisplayTimer = 0;
            
            // 最初のIdを表示
            DisplayBonus(mStatusBonusIds[0]);
        }
        
        private void DisplayBonus(long mTrainingScenarioId)
        {
            // マスタ取得
            TrainingScenarioStatusBonusMasterObject mBonus = MasterManager.Instance.trainingScenarioStatusBonusMaster.FindData(mTrainingScenarioId);
            // 効果値
            long value = mBonus.enhanceRate / 100;
            // 表示
            bonusText.text = string.Format(StringValueAssetLoader.Instance["training.menu.bonus_text." + mBonus.type], value);
            
            // 終了時間
            DateTime endAt = AppTime.Parse(mBonus.endAt);
            // 特定の時間以降は無期限扱いで表示しない
            if(endAt.Year >= DisableDateTime)
            {
                dateRoot.SetActive(false);
            }
            else
            {
                // 期間
                dateText.text = endAt.ToString(StringValueAssetLoader.Instance["training.menu.bonus_date_end"]);
                // 表示
                dateRoot.SetActive(true);
            }
        }
        
        private void Update()
        {
            // 時間で切り替え
            bonusDisplayTimer += Time.deltaTime;
            // 一定時間で切り替え
            if(bonusDisplayTimer >= bonusDisplayInterval)
            {
                // 次の表示へ
                bonusDisplayIndex = (bonusDisplayIndex + 1) % mStatusBonusIds.Count;
                // 表示
                DisplayBonus( mStatusBonusIds[bonusDisplayIndex] );
                // 時間リセット
                bonusDisplayTimer = 0;
            }
        }
    }
}