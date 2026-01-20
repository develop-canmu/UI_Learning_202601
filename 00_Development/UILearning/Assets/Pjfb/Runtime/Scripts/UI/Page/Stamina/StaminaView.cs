using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using UnityEngine;
using UnityEngine.UI;
using static Pjfb.StaminaUtility;

namespace Pjfb
{
    public class StaminaView : MonoBehaviour
    {
        private static readonly string TrainingTitleKey = "training.stamina";
        private static readonly string AutoTrainingTitleKey = "auto_training.stamina";
        private static readonly string RivalryTitleKey = "rivalry.stamina";
        private static readonly string ColosseumTitleKey = "pvp.stamina";
        private static readonly string TrainingStaminaValueKey = "training.stamina_value"; // (Current/Max)
        
        [SerializeField]
        private TMPro.TMP_Text titleText = null;
        [SerializeField]
        private TMPro.TMP_Text valueText = null;
        [SerializeField]
        private TMPro.TMP_Text cureText = null;
        [SerializeField]
        private GameObject staminaTextRoot = null;
        [SerializeField]
        private GameObject staminaFreeTextRoot = null;
        [SerializeField]
        private UIButton addButton = null;
        [SerializeField]
        private Slider slider = null;
        [SerializeField]
        private bool useAddButton = true;
        
        private StaminaType staminaType;
        private float updateTimer = 0;
        private long currentStaminaId = 0;
        
        private TrainingAutoUserStatus autoUserStatus = null;
        public TrainingAutoUserStatus AutoUserStatus
        {
            set { autoUserStatus = value; }
        }

        public Action OnUpdateStamina;

        /// <summary>APIで取得して更新</summary>
        public async UniTask UpdateAsync(StaminaType _staminaType, long mStaminaId = -1)
        {
            staminaType = _staminaType;
            currentStaminaId = mStaminaId == -1 ? (long)staminaType : mStaminaId;
            var titleKey = GetTitleKey(staminaType);
            titleText.text = string.Format(StringValueAssetLoader.Instance[titleKey]);
            
            addButton.gameObject.SetActive(staminaType != StaminaType.Colosseum && useAddButton);

            await StaminaUtility.UpdateStaminaAsync();
            UpdateView();
        }
        /// <summary>APIで取得して更新</summary>
        public void InitWithoutUpdateAsync(StaminaType _staminaType, long mStaminaId = -1)
        {
            staminaType = _staminaType;
            currentStaminaId = mStaminaId == -1 ? (long)staminaType : mStaminaId;
            var titleKey = GetTitleKey(staminaType);
            titleText.text = string.Format(StringValueAssetLoader.Instance[titleKey]);

            UpdateView();
        }
        
        private void UpdateView()
        {
            // スタミナ最大数
            long max = StaminaUtility.GetStaminaMax(currentStaminaId);
            if(max <= 0)return;
            // 現在のスタミナ
            long current = StaminaUtility.GetStamina(currentStaminaId);
            
            // 上限上昇
            bool isLimitUp = currentStaminaId == (long)StaminaType.AutoTraining;
            
            if(isLimitUp)
            {
                max += StaminaUtility.GetMaxStaminaAddition(currentStaminaId);
                current += StaminaUtility.GetStaminaAddition(currentStaminaId);
            }
            
            // スタミナ表示
            valueText.text = string.Format(StringValueAssetLoader.Instance[TrainingStaminaValueKey], current, max);
            slider.value = (float)current / (float)max;
            
            if(current >= max)
            {
                cureText.gameObject.SetActive(false);
                addButton.interactable = false;
            }
            else
            {
                addButton.interactable = true;
                cureText.gameObject.SetActive(true);
                // 回復時間
                string cureStr = StaminaUtility.GetNextCureSecondString(currentStaminaId);
                cureText.text = string.Format(StringValueAssetLoader.Instance["training.stamina.cure"], cureStr);    
            }

            long currentAddition = StaminaUtility.GetStaminaAddition(currentStaminaId);
            bool hasStaminaAddition = currentAddition > 0 && isLimitUp == false;
            staminaFreeTextRoot.SetActive(hasStaminaAddition);
            staminaTextRoot.SetActive(!hasStaminaAddition);
            
            OnUpdateStamina?.Invoke();
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnAddButton()
        {
            OnAddButtonAsync().Forget();
        }
        
        private async UniTask OnAddButtonAsync()
        {
            CruFramework.Page.ModalWindow modalWindow;
            if (staminaType == StaminaType.AutoTraining)
            {
                AutoTrainingRemainingTimesAddModal.RemainingTimesAddData data = new AutoTrainingRemainingTimesAddModal.RemainingTimesAddData();
                data.UserStatus = autoUserStatus;
                data.StaminaValue = StaminaUtility.GetStamina(currentStaminaId) + StaminaUtility.GetStaminaAddition(currentStaminaId);
                data.OnSetUserStatus = OnUpdateAutoUserStatus;
                modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoTrainingRemainingTimesAdd, data);
            }
            else
            {
                // 引数
                StaminaInsufficientModal.Data data = new StaminaInsufficientModal.Data();
                data.staminaType = staminaType;
                data.mStaminaId = currentStaminaId;
                // モーダル開く
                modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.StaminaInsufficient, data);
            }

            await modalWindow.WaitCloseAsync();
            // ビューの更新
            UpdateView();
        }

        private void Update()
        {
            updateTimer += Time.deltaTime;
            if(updateTimer >= 1.0f)
            {
                UpdateView();
                updateTimer = 0;
            }
        }

        private string GetTitleKey(StaminaType staminaType)
        {
            switch (staminaType)
            {
                case StaminaType.RivalryBattle:
                    return RivalryTitleKey;
                case StaminaType.Training:
                    return TrainingTitleKey;
                case StaminaType.Colosseum:
                    return ColosseumTitleKey;
                case StaminaType.AutoTraining:
                    return AutoTrainingTitleKey;
                default:
                    return string.Empty;
            }
        }

        private void OnUpdateAutoUserStatus(TrainingAutoUserStatus status)
        {
            autoUserStatus = status;
        }
    }
}