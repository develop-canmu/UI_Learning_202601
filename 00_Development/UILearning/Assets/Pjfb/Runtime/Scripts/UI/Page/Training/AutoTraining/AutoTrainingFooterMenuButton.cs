using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using System;
using CruFramework;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class AutoTrainingFooterMenuButton : MonoBehaviour
    {
        [SerializeField]
        private UIButton button = null;

        [SerializeField]
        private GameObject emptyRoot = null;
        [SerializeField]
        private GameObject selectRoot = null;
        
        
        [SerializeField]
        private IconImage characterIcon = null;
        [SerializeField]
        private CancellableRawImage scenarioBannerImage = null;
        [SerializeField]
        private TMP_Text timeText = null;
        
        [SerializeField]
        private TMP_Text lockText = null;
        
        [SerializeField]
        private GameObject badge = null;
        
        private Action onSelected = null;
        private Action onCompleted = null;
        private Action onTrainingStatus = null;

        // ステータス
        private TrainingAutoPendingStatus status = null;
        // 時間更新よう
        private float updateTimer = 0;
        
        /// <summary>スロット番号</summary>
        public long SlotNumber{get{return status.slotNumber;}}
        
        // 完了済み
        private bool isCompleted = false;
        
        private bool isEnable = true;
        private bool lockModalEnable = true;
        
        /// <summary>選択時の処理</summary>
        public void SetOnSelected(Action action)
        {
            onSelected = action;
        }
        
        /// <summary>完了時の処理</summary>
        public void SetOnCompleted(Action action)
        {
            onCompleted = action;
        }
        
        /// <summary>実行中の処理</summary>
        public void SetOnTrainingStatus(Action action)
        {
            onTrainingStatus = action;
        }
        
        /// <summary>ステータス設定</summary>
        public void SetStatus(TrainingAutoPendingStatus status)
        {
            this.status = status;
            
            // トレーニング進行中？
            if(status.mTrainingScenarioId > 0)
            {
                // キャラの表示
                characterIcon.SetTexture( status.mCharaId );
                // シナリオ
                scenarioBannerImage.SetTexture( ResourcePathManager.GetPath("TrainingMenuSmallBanner", status.mTrainingScenarioId) );
                // 時間更新
                UpdateTime();
                // 表示
                ShowSelect();
            }
            else
            {
                ShowEmpty();
            }
        }
        
        private void HideAllRoot()
        {
            emptyRoot.SetActive(false);
            selectRoot.SetActive(false);
        }
        
        /// <summary>あきを表示</summary>
        public void ShowEmpty()
        {
            HideAllRoot();
            emptyRoot.SetActive(true);
            button.interactable = true;
        }
        
        /// <summary>選択済みを表示</summary>
        public void ShowSelect()
        {
            HideAllRoot();
            selectRoot.SetActive(true);
            button.interactable = true;
        }
        
        /// <summary>ロックテキスト</summary>
        public void SetLockText(string text)
        {
            lockText.text = text;
        }
        
        public void ShowScenarioLock()
        {
            // シナリオがロック状態
            if(status != null && status.mTrainingScenarioId <= 0)
            {
                ShowLock();
            }
        }
        
        /// <summary>再読み込み</summary>
        public void Reload()
        {
            if(status != null)
            {
                SetStatus(status);
            }
            else
            {
                ShowLock();
            }
        }
        
        /// <summary>ロックを表示</summary>
        public void ShowLock()
        {
            SetLockText(StringValueAssetLoader.Instance["auto_training.slot_lock"]);
            HideAllRoot();
            button.interactable = false;
        }
        
        public void SetEnable(bool isEnable)
        {
            this.isEnable = isEnable;
        }
        
        /// <summary>ボタン押下時にモーダルを出すか</summary>
        public void SetLockModalEnable(bool isEnable)
        {
            this.lockModalEnable = isEnable;
        }
        
        /// <summary>UGUI</summary>
        public void OnLockCoverButton()
        {
            if(lockModalEnable == false)return;
            
            // 無効
            if(isEnable == false)
            {
                // 確認
                string title = StringValueAssetLoader.Instance["training.menu.auto_training_disable_title"];
                string buttonText = StringValueAssetLoader.Instance["common.close"];
                string msg = StringValueAssetLoader.Instance["training.menu.auto_training_disable_modal_msg"];
                // 確認モーダル
                AutoTrainingUtility.OpenNegativeConfirmModal(title, msg, buttonText);
            }
            // 解放条件
            else if(status != null)
            {
                ConfirmModalData data = new ConfirmModalData();
                data.Message = lockText.text;
                data.PositiveButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            }
            else
            {
                AutoTrainingUtility.OpenPassModal( TrainingAutoCostMasterObject.CostType.FrameRelease );
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            // からのボタンを押した
            if(status.mTrainingScenarioId <= 0)
            {
                if(onSelected != null)
                {
                    onSelected();
                }
            }
            // 実行中
            else
            {
                // 時間を更新
                UpdateTime();
                // 完了済み
                if(isCompleted)
                {
                    if(onCompleted != null)
                    {
                        onCompleted();
                    }
                }
                // 実行中
                else
                {
                    if(onTrainingStatus != null)
                    {
                        onTrainingStatus();
                    }
                }
            }
        }
        
        private void UpdateTime()
        {
            if(status == null || status.mTrainingScenarioId <= 0)return;
            // 終了時間
            DateTime finishAt = AppTime.Parse(status.finishAt);
            // 完了済み
            if(AppTime.Now >= finishAt)
            {
                // テキストを「完了」に
                timeText.text = StringValueAssetLoader.Instance["auto_training.complete"];
                // 完了バッジ表示
                badge.SetActive(true);
                // 完了フラグ
                isCompleted = true;
            }
            // 残り時間を表示
            else
            {
                // 時間表示
                timeText.text = AutoTrainingUtility.ToTimeString(finishAt - AppTime.Now);
                // 完了バッジは非表示
                badge.SetActive(false);
                // 未完了
                isCompleted = false;
            }
        }


        private void Update()
        {
            if(status != null && status.mTrainingScenarioId > 0)
            {
                updateTimer += Time.deltaTime;
                // 一定時間で更新
                if(updateTimer >= AutoTrainingUtility.UpdateTimeInterval)
                {
                    updateTimer = 0;
                    UpdateTime();
                }
            }
        }

        private void OnEnable()
        {
            UpdateTime();
            updateTimer = 0;
        }
    }
}