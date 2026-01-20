using System;
using System.Threading;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.Common;

namespace Pjfb
{
    public class StaminaInsufficientConfirmModal : ModalWindow
    {
        private const string TrainingStaminaKey = "character.status.stamina";
        private const string RivalryOrPvpStaminaKey = "rivalry.stamina";
        private const int MAX_COUNT = 100;
        private const int MIN_COUNT = 1;
        public class Data
        {
            public StaminaUtility.StaminaType staminaType;
            public long mStaminaId;
            public StaminaCureMasterObject mStaminaCure;
            public ItemIconContainerGridItem.Data itemData;
        }

        #region SerializeFields
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private ItemIconContainer itemIcon;
        [SerializeField] private PossessionItemUi requiredItemUi;
        [SerializeField] private PossessionItemUi possessionItemUi;
        [SerializeField] private UiSlider costSlider;
        [SerializeField] private TMP_Text staminaNameText;
        [SerializeField] private TMP_Text beforeCountText;
        [SerializeField] private TMP_Text afterCountText;
        #endregion

        private Data data;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
        }

        private void Init()
        {
            var mPoint = MasterManager.Instance.pointMaster.FindData(data.itemData.id);
            bodyText.text = string.Format(StringValueAssetLoader.Instance[GetConfirmBodyTextKey()], 
                mPoint.name,
                data.mStaminaCure.value,
                data.mStaminaCure.cureValue);

            itemIcon.SetIcon(data.itemData.iconType, data.itemData.id);
            itemIcon.SetCount(data.itemData.possessionValue);
            itemNameText.text = mPoint.name;
            InitializeSliderUi();
        }
        
        private string GetConfirmBodyTextKey()
        {
            switch(data.staminaType)
            {
                case StaminaUtility.StaminaType.RivalryBattle: return "rivalry.stamina_confirm.body_text";
                case StaminaUtility.StaminaType.Training: return "training.stamina_confirm.body_text";
                case StaminaUtility.StaminaType.Colosseum: return "pvp.stamina_confirm.body_text";
            }
            
            return string.Empty;
        }
        
        private string GetBodyTextKey()
        {
            switch(data.staminaType)
            {
                case StaminaUtility.StaminaType.RivalryBattle: return "stamina.rivalry.body";
                case StaminaUtility.StaminaType.Training: return "stamina.training.body";
                case StaminaUtility.StaminaType.Colosseum: return "stamina.pvp.body";
            }            
            return string.Empty;
        }
        
        private void InitializeSliderUi()
        {
            var costValue = data.mStaminaCure.value;
            var possessionCount = UserDataManager.Instance.point.Find(data.itemData.id)?.value ?? 0;
            var curableStamina = StaminaUtility.GetStaminaMax(data.mStaminaId) - StaminaUtility.GetStamina(data.mStaminaId);
            var cureValue = data.mStaminaCure.cureValue;
            int exchangeableCount = Mathf.Clamp(Mathf.FloorToInt((float)possessionCount / costValue), MIN_COUNT, Mathf.CeilToInt((float)curableStamina / cureValue));
            costSlider.SetSliderRange(1, exchangeableCount);
            if ((int)costSlider.GetSliderValue() == MIN_COUNT)
            {
                OnValueChangedSlider();
            }
            else
            {
                costSlider.SetSliderValue(MIN_COUNT);   
            }
            costSlider.SetButtonInteractable();

            requiredItemUi.SetRequiredCount(data.itemData.id, costValue);
            possessionItemUi.SetAfterCountByAmount(data.itemData.id, -costValue);

            // スタミナ表示
            var currentStamina = StaminaUtility.GetStamina(data.mStaminaId);
            staminaNameText.text = string.Format(StringValueAssetLoader.Instance[data.staminaType == StaminaUtility.StaminaType.Training ? TrainingStaminaKey : RivalryOrPvpStaminaKey]);
            beforeCountText.text = currentStamina.ToString();
            afterCountText.text = (currentStamina + data.mStaminaCure.cureValue).ToString();
        }

        #region EventListeners
        
        public void OnValueChangedSlider()
        {
            var exchangeCount = (int)costSlider.GetSliderValue();
            var requireCount = exchangeCount * data.mStaminaCure.value;
            requiredItemUi.UpdateRequiredCount(requireCount);
            possessionItemUi.UpdateAfterAmount(-requireCount);
            itemIcon.SetCount(requireCount);
            
            var mPoint = MasterManager.Instance.pointMaster.FindData(data.itemData.id);
            bodyText.text = string.Format(StringValueAssetLoader.Instance[GetConfirmBodyTextKey()], 
                mPoint.name,
                requireCount,
                data.mStaminaCure.cureValue * exchangeCount);

            // スタミナ表示
            var currentStamina = StaminaUtility.GetStamina(data.mStaminaId);
            beforeCountText.text = currentStamina.ToString();
            afterCountText.text = (currentStamina + (data.mStaminaCure.cureValue * exchangeCount)).ToString();
        }

        public async void OnClickUseItem()
        {
            StaminaCureAPIRequest request = new StaminaCureAPIRequest();
            //Token取得
            var token = await Pjfb.Networking.App.APIUtility.ConnectOneTimeTokeRequest();
            request.oneTimeToken = token.oneTimeToken;

            StaminaCureAPIPost post = new StaminaCureAPIPost();
            post.mStaminaCureId = data.mStaminaCure.id;
            post.value = (int)costSlider.GetSliderValue();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window != this);
            ConfirmModalData confirmData = new ConfirmModalData(
                StringValueAssetLoader.Instance["stamina.close_title"],
                string.Format(StringValueAssetLoader.Instance[GetBodyTextKey()], data.mStaminaCure.cureValue * (int)costSlider.GetSliderValue()),
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (window) => {
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(w => w != window);
                    window.Close();
                })
            );
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, confirmData);
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
        #endregion
    }
}
