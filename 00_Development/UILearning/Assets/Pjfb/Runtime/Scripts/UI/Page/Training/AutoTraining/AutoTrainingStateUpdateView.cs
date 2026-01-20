using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using TMPro;

namespace Pjfb
{
    public abstract class AutoTrainingStateUpdateView : MonoBehaviour
    {
        public enum State
        {
            // 1枠でも自動トレーニング中
            AnyAutoTraining,
            // 1つでも自動トレーニングが完了済み
            AnyCompleteAutoTraining,
            // すべての自動トレーニングが完了済み
            AllCompleteAutoTraining,
            // すべての自動トレーニングが空いている状態
            AllEmptyAutoTraining,
        }
        
        [SerializeField]
        private TMP_Text completedText = null;
        protected TMP_Text CompleteText{get => completedText;}
        
        [SerializeField]
        private TMP_Text ableText = null;
        protected TMP_Text AbleText{get => ableText;}
        
        [SerializeField]
        private TMP_Text timeText = null;
        protected TMP_Text TimeText{get => timeText;}
        
        [SerializeField] private GameObject completedBadge = null;
        protected GameObject CompleteBadge{get => completedBadge;}
        
        [SerializeField] protected GameObject lockObject;
        protected GameObject LockObject{get => lockObject;}
        
        [SerializeField] protected UIButton button;
        protected UIButton Button{get => button;}
        
        private bool isAnyComplete = false;
        protected bool IsAnyComplete{get => isAnyComplete;}
        private bool isUpdateView = true;
        protected bool IsUpdateView{get => isUpdateView;}
        // 終了しているトレーニングの数
        private int finishTrainingCount = 0;
        protected int FinishTrainingCount {get => finishTrainingCount;}
        private TrainingAutoPendingStatus[] pendingStatusList;
        protected TrainingAutoPendingStatus[] PendingStatusList{get => pendingStatusList;}
        
        private State currentState = State.AllEmptyAutoTraining;
        // 実行している自動トレーニングの中で最も完了までの時間が短い終了時間
        private DateTime finishTime;
        protected DateTime FinishTime{get => finishTime;}

        // 更新用のタイマー
        private float updateTimer = 0;
        
        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Init(TrainingAutoPendingStatus[] pendingStatusList)
        {
            // 自動トレーニングの解放状況でロックかを切り替える
            bool isUnlock = AutoTrainingUtility.IsUnLockAutoTraining();
            SetLockObject(isUnlock == false);

            // 解放されてないなら非表示にして後続の処理をとばす
            if (isUnlock == false)
            {
                completedBadge.SetActive(false);
                HideStatusText();
                //　更新しない
                isUpdateView = false;
                return;
            }
            
            this.pendingStatusList = pendingStatusList;
            //更新
            UpdateView(true);
        }
        
        public void SetLockObject(bool enable)
        {
            lockObject.SetActive(enable);
            if(button != null)
            {
                button.interactable = !enable;
            }
        }

        private void Update()
        {
            if (pendingStatusList == null)
            {
                return;
            }

            if (!isUpdateView)
            {
                return;
            }
            
            updateTimer += Time.deltaTime;
            // 一定の時間で更新をするように
            if(updateTimer >= AutoTrainingUtility.UpdateTimeInterval)
            {
                //タイマーの初期化
                updateTimer = 0;
                //更新
                UpdateView();
            }
        }

        /// <summary>
        /// バッジとテキスト文言の更新
        /// </summary>
        private void UpdateView(bool isInitialize = false)
        {
            CheckAutoTraining(isInitialize);
            // バッジの更新
            UpdateBadge();
            // 自動トレーニングテキスト表示
            UpdateStatusText();
        }
        
        /// <summary>
        /// トレーニング状況のチェック
        /// </summary>
        /// <param name="isInitialize"></param>
        private void CheckAutoTraining(bool isInitialize = false)
        {
            int currentFinishCount = pendingStatusList.Count(TrainingManager.IsCompleteAutoTraining);
            
            // 初期化時と終了したトレーニング数が変化したとき
            if (isInitialize || currentFinishCount != finishTrainingCount)
            {
                //終了時間の再計算
                CalcRemainingTime();
                finishTrainingCount = currentFinishCount;
                // 一つでも完了しているなら完了表示
                isAnyComplete = finishTrainingCount > 0;
                UpdateState();
            }
        }

        /// <summary>
        /// 自動トレーニングの状態を更新
        /// </summary>
        private void UpdateState()
        {
            // すべての自動トレーニングが空いている状態
            if (pendingStatusList.All(status => status.mCharaId == 0))
            {
                currentState = State.AllEmptyAutoTraining;
                isUpdateView = false;
            }
            // 1つでも自動トレーニングが完了済み
            else if (isAnyComplete)
            {
                currentState = State.AnyCompleteAutoTraining;
                isUpdateView = false;
            }
            // すべての自動トレーニングが完了済み
            else if (finishTrainingCount == pendingStatusList.Count(status => status.mCharaId != 0))
            {
                currentState = State.AllCompleteAutoTraining;
                isUpdateView = false;
            }
            // 1枠でも自動トレーニング中
            else if (pendingStatusList.Any(status => status.mCharaId != 0 && AppTime.Parse(status.finishAt).IsFuture(AppTime.Now)))
            {
                currentState = State.AnyAutoTraining;
                isUpdateView = true;
            }
        }
        
        /// <summary>
        /// バッジの更新
        /// </summary>
        private void UpdateBadge()
        {
            completedBadge.SetActive(isAnyComplete);
        }
        
        /// <summary>
        /// トレーニング状況文言の設定
        /// </summary>
        private void UpdateStatusText()
        {
            switch (currentState)
            {
                // 1枠でも自動トレーニング中
                case State.AnyAutoTraining:
                {
                    timeText.text = AnyAutoTrainingStatusText();
                    ShowStatusText(timeText);
                    break;
                }
                // すべての自動トレーニングが完了済み
                case State.AllCompleteAutoTraining:
                case State.AnyCompleteAutoTraining:
                {
                    completedText.text = CompleteAutoTrainingStatusText();
                    ShowStatusText(completedText);
                    break;
                }
                // すべての自動トレーニングが空いている状態
                case State.AllEmptyAutoTraining:
                {
                    ableText.text = AllEmptyAutoTrainingStatusText();
                    ShowStatusText(ableText);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 一枠でもトレーニング中
        /// </summary>
        protected abstract string AnyAutoTrainingStatusText();

        /// <summary>
        ///　自動トレーニングが完了済み
        /// </summary>
        protected abstract string CompleteAutoTrainingStatusText();

        /// <summary>
        /// すべての自動トレーニング枠が空いているとき
        /// </summary>
        protected abstract string AllEmptyAutoTrainingStatusText();
        
        // トレーニング状況文言を非表示にする
        private void HideStatusText()
        {
            completedText.gameObject.SetActive(false);
            ableText.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// ステータステキストの表示
        /// </summary>
        /// <param name="statusText"></param>
        private void ShowStatusText(TMP_Text statusText)
        {
            // いったんすべて非表示
            HideStatusText();
            statusText.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// トレーニング完了までの時間を計算
        /// </summary>
        private void CalcRemainingTime()
        {
            // トレーニングが実行されている中で完了までの時間が一番短い時間を表示
            finishTime = 
                // トレーニングされているものを取得
                pendingStatusList.Where(status => status.mCharaId != 0)
                    //　終了時間を選択
                    .Select(status => AppTime.Parse(status.finishAt))
                    // 終了時間が未来の時間を優先
                    .OrderByDescending(x => x.IsFuture(AppTime.Now)).ThenBy(x => x).FirstOrDefault();
        }
    }
}