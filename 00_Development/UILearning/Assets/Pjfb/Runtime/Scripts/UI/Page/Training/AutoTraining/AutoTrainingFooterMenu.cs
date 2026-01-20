using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.SystemUnlock;
using UnityEngine.Events;

namespace Pjfb.Training
{
    public class AutoTrainingFooterMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject defaultRoot = null;
        [SerializeField]
        private GameObject autoTrainingRoot = null;
        
        [SerializeField]
        private RectTransform menuButtonRoot = null;
        [SerializeField]
        private AutoTrainingFooterMenuButton menuButtonPrefab = null;
        
        [SerializeField]
        private AutoTrainingFooterInfoButton infoButton = null;
        
        [SerializeField]
        private GameObject switchButton = null;
        
        [SerializeField]
        private UnityEvent onNext = new UnityEvent();
        [SerializeField]
        private UnityEvent onCompleted = new UnityEvent();
        [SerializeField]
        private UnityEvent onTrainingStatus = new UnityEvent();

        private List<AutoTrainingFooterMenuButton> createdAutoTrainingButtons = new List<AutoTrainingFooterMenuButton>();
        
        /// <summary>通常メニュー表示</summary>
        public void ShowDefaultMenu()
        {
            defaultRoot.SetActive(true);
            autoTrainingRoot.SetActive(false);
        }
        
        /// <summary>自動トレーニングメニュー表示</summary>
        public void ShowActiveAutoTrainingMenu()
        {
            defaultRoot.SetActive(false);
            autoTrainingRoot.SetActive(true);
        }
        
        /// <summary>自動トレーニングの有効か</summary>
        public void SetEnable(bool enable)
        {
            foreach(AutoTrainingFooterMenuButton button in createdAutoTrainingButtons)
            {
                button.SetEnable(enable);
            }
        }
        
        /// <summary>未解時のモーダル設定</summary>
        public void SetLockModalEnable(bool enable)
        {
            foreach(AutoTrainingFooterMenuButton button in createdAutoTrainingButtons)
            {
                button.SetLockModalEnable(enable);
            }
        }
        
        /// <summary>シナリオのロック状態</summary>
        public void SetScenarioLock(bool isLock)
        {
            foreach(AutoTrainingFooterMenuButton button in createdAutoTrainingButtons)
            {
                if(isLock)
                {
                    button.ShowScenarioLock();
                }
                else
                {
                    button.Reload();
                }
            }
        }
        
        /// <summary>シナリオのロック状態</summary>
        public void SetScenarioLockText(string text)
        {
            foreach(AutoTrainingFooterMenuButton button in createdAutoTrainingButtons)
            {
                button.SetLockText(text);
            }
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnLockButton()
        {
            // 機能が未開放の場合は解放条件を出す
            if(AutoTrainingUtility.IsUnLockAutoTraining() == false)
            {
                // 解放条件を出す
                string title = StringValueAssetLoader.Instance["common.release_condition"];
                string buttonText = StringValueAssetLoader.Instance["common.ok"];
                // マスタ
                SystemLockMasterObject mLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber((long)SystemUnlockDataManager.SystemUnlockNumber.AutoTraining);
                // 確認モーダル
                AutoTrainingUtility.OpenConfirmModal(title, mLock.description, buttonText);
            }
            // その他は実行不可のメッセージ
            else
            {
                // 確認
                string title = StringValueAssetLoader.Instance["training.menu.auto_training_disable_title"];
                string buttonText = StringValueAssetLoader.Instance["common.close"];
                string msg = StringValueAssetLoader.Instance["training.menu.auto_training_disable_modal_msg"];
                // 確認モーダル
                AutoTrainingUtility.OpenNegativeConfirmModal(title, msg, buttonText);
            }
        }
        
        public void Initialize(TrainingPreparationArgs args, TrainingAutoUserStatus userStatus, TrainingAutoPendingStatus[] pendingStatus)
        {
            // チュートリアル中は自動トレーニングの情報がない
            if(userStatus == null || pendingStatus == null)
            {
                // ロック状態にしておく
                infoButton.SetLockObject(true);
                return;
            }
            
            // 切り替えボタン
            switchButton.SetActive(args.IsAutoTrainingOnly == false);
            
            // 生成済みのボタンを破棄
            for(int i=0;i<createdAutoTrainingButtons.Count;i++)
            {
                GameObject.Destroy(createdAutoTrainingButtons[i].gameObject);
            }
            // リストを破棄
            createdAutoTrainingButtons.Clear();
            // 切り替えボタン初期化
            infoButton.Init(pendingStatus);
            
            // スロット分生成
            for(int i=0;i<userStatus.maxSlotCount;i++)
            {
                // ボタンを生成
                AutoTrainingFooterMenuButton button = GameObject.Instantiate<AutoTrainingFooterMenuButton>(menuButtonPrefab, menuButtonRoot);
                // 表示
                button.gameObject.SetActive(true);
                // リストに追加
                createdAutoTrainingButtons.Add(button);
                
                // ロック状態
                bool isLock = i >= userStatus.slotCount;
                
                if(isLock)
                {
                    button.ShowLock();
                }
                else
                {
                    // ステータス設定
                    button.SetStatus(pendingStatus[i]);
                    // 次へボタン押した時のコールバック登録
                    button.SetOnSelected(()=>
                    {
                        args.AutoTrainingSlot = button.SlotNumber;
                        onNext.Invoke();
                    });
                    
                    // 完了時
                    button.SetOnCompleted(()=>
                    {
                        args.AutoTrainingSlot = button.SlotNumber;
                        onCompleted.Invoke();
                    });
                    
                    // 実行中
                    button.SetOnTrainingStatus(()=>
                    {
                        args.AutoTrainingSlot = button.SlotNumber;
                        onTrainingStatus.Invoke();
                    });
                }
            }
        }
    }
}