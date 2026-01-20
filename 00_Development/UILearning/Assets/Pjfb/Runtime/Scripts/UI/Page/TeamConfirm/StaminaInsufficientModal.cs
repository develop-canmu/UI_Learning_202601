using System;
using System.Threading;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using System.Linq;

namespace Pjfb
{
    public class StaminaInsufficientModal : ModalWindow
    {
        public class Data
        {
            public StaminaUtility.StaminaType staminaType;
            public long mStaminaId;
        }

        #region SerializeFields
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private ScrollGrid itemScrollGrid;
        [SerializeField] private UIButton confirmButton;
        #endregion

        private Data data;
        private List<ItemIconContainerGridItem.Data> itemList;
        private List<StaminaCureMasterObject> mStaminaCureList;
        private int selectedItemIndex;

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
        
        private string GetTitleTextKey()
        {
            switch(data.staminaType)
            {
                case StaminaUtility.StaminaType.RivalryBattle:return "rivalry.stamina.title";
                case StaminaUtility.StaminaType.Training:return "training.stamina.title";
                case StaminaUtility.StaminaType.Colosseum:return "pvp.stamina.title";
            }
            
            return string.Empty;
        }
        
        private string GetDescriptionKey()
        {
            switch(data.staminaType)
            {
                case StaminaUtility.StaminaType.RivalryBattle:return "rivalry.stamina.body_text";
                case StaminaUtility.StaminaType.Training:return "training.stamina.body_text";
                case StaminaUtility.StaminaType.Colosseum:return "pvp.stamina.body_text";
            }
            
            return string.Empty;
        }
        
        private string GetBodyTextKey()
        {
            switch(data.staminaType)
            {
                case StaminaUtility.StaminaType.RivalryBattle:return "rivalry.stamina.use_item";
                case StaminaUtility.StaminaType.Training:return "training.stamina.use_item";
                case StaminaUtility.StaminaType.Colosseum:return "pvp.stamina.use_item";
            }
            
            return string.Empty;
        }

        private void Init()
        {
            mStaminaCureList = MasterManager.Instance.staminaCureMaster.values.ToList()
                .Where(mStaminaCure => mStaminaCure.mStaminaId == data.mStaminaId).ToList();
            itemList = new List<ItemIconContainerGridItem.Data>();
            selectedItemIndex = -1;
            var i = 0;
            foreach (var mStaminaCure in mStaminaCureList)
            {
                var point = UserDataManager.Instance.point.Find(mStaminaCure.mPointId);
                var isEnough = (point?.value ?? 0) >= mStaminaCure.value;
                if (isEnough && selectedItemIndex == -1) selectedItemIndex = i;
                i++;
                itemList.Add(new ItemIconContainerGridItem.Data(ItemIconType.Item, mStaminaCure.mPointId,
                    point?.value ?? 0, mStaminaCure.value, !isEnough));
            }
            if (selectedItemIndex >= itemList.Count || selectedItemIndex == -1) selectedItemIndex = 0;

            titleText.text = StringValueAssetLoader.Instance[GetTitleTextKey()];
            descriptionText.text = StringValueAssetLoader.Instance[GetDescriptionKey()];

            SetSelectedItem();
            SetBodyText();
        }

        private void SetSelectedItem()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].isSelected = i == selectedItemIndex;
            }
            itemScrollGrid.SetItems(itemList);

            confirmButton.interactable = !itemList[selectedItemIndex].isCoverActive;
        }

        private void SetBodyText()
        {
            if (selectedItemIndex >= itemList.Count) return;
            var selectedItem = itemList[selectedItemIndex];
            var mPoint = MasterManager.Instance.pointMaster.FindData(selectedItem.id);
            var mStaminaCure = mStaminaCureList[selectedItemIndex];
            bodyText.text = string.Format(StringValueAssetLoader.Instance[GetBodyTextKey()], 
                mPoint.name,
                mStaminaCure.value,
                mStaminaCure.cureValue);
        }
        
        #region EventListeners
        public void OnClickUseItem()
        {
            if (selectedItemIndex >= itemList.Count) return;
            var selectedItem = itemList[selectedItemIndex];
            if (selectedItem.isCoverActive) return;
            var postData = new StaminaInsufficientConfirmModal.Data();
            postData.itemData = selectedItem;
            postData.staminaType = data.staminaType;
            postData.mStaminaId = data.mStaminaId;
            postData.mStaminaCure = mStaminaCureList[selectedItemIndex];
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.StaminaInsufficientConfirm, postData);
        }

        public void OnClickSelectItem(ItemIconContainerGridItem itemIcon)
        {
            if (itemIcon.data.isCoverActive) return;
            selectedItemIndex = itemList.FindIndex(item => item.id == itemIcon.data.id);
            SetSelectedItem();
            SetBodyText();
        }
        #endregion
    }
}
