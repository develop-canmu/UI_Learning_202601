using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    
    /// <summary>
    /// サポート器具選択画面
    /// </summary>
    
    public class TrainingSupportEquipmentSelectPage : TrainingPreparationPageBase
    {
        // デッキ制限の説明のpath
        private const string DeckLimitDescriptionPath = "training.support_equipment.deck_limit";
        
        [SerializeField]
        private UserSupportEquipmentScroll scroll = null;
        [SerializeField]
        private TrainingSupportEquipmentDetailView detailView = null;
        [SerializeField]
        private SupportEquipmentIcon detailIcon = null;
        [SerializeField] 
        private TMPro.TMP_Text deckLimitText;
        [SerializeField]
        private GameObject deckLimitRoot;
        [SerializeField]
        private UIButton nextButton;
        [SerializeField] 
        private RectTransform layoutGroupTransform;
        
        protected long selectedCharacterId = TrainingSupportDeckSelectPage.None;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            scroll.OnSelectedItem -= OnSelectSupportEquipment;
            scroll.OnSelectedItem += OnSelectSupportEquipment;
            
            scroll.OnSortFilter -= OnSortFilter;
            scroll.OnSortFilter += OnSortFilter;
            
            selectedCharacterId = TrainingSupportDeckSelectPage.None;
            Initialize();
            
            return base.OnPreOpen(args, token);
        }
        
        private void OnSortFilter()
        {
            SetFirstSelect();
        }
        
        private void Initialize()
        {
            UserDataChara trainingUChar = UserDataManager.Instance.chara.Find(Arguments.TrainingUCharId);
            // デッキ
            DeckData deck = Arguments.EquipmentDeckList.DeckDataList[Arguments.SupportCharacterDeckSelectedData.DeckIndex];
            List<long> formattingIds = new List<long>();
            long[] memberIds = deck.GetEquipmentMemberIds();
            // 開放チェック
            for(int i=0;i<memberIds.Length;i++)
            {
                if(memberIds[i] == DeckUtility.EmptyDeckSlotId)continue;
                if(deck.GetUnlockLevel(i) <= trainingUChar.level)
                {
                    formattingIds.Add(memberIds[i]);
                }
            }
            // 編成中
            scroll.SetFormattingIds(formattingIds.ToArray());

            DeckFormatSlotMasterObject slot = DeckUtility.GetSlotMaster(DeckType.SupportEquipment, Arguments.SupportCharacterDeckSelectedData.Order);
            DeckFormatConditionMasterObject condition = null;
            // 枠制限ありであれば制限をつける
            if (slot.mDeckFormatConditionId > 0)
            {
                // 枠制限
                condition = MasterManager.Instance.deckFormatConditionMaster.FindData(slot.mDeckFormatConditionId);
                deckLimitText.text = string.Format(StringValueAssetLoader.Instance[DeckLimitDescriptionPath], condition.description);
            }
            scroll.SetSupportEquipmentFormatCondition(condition);
            deckLimitRoot.SetActive(slot.mDeckFormatConditionId > 0);
            
            // 自動レイアウト+ScrollGridの関係で強制的に計算させる
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupTransform);

            scroll.InitializeScroll();
            SetFirstSelect();
            detailIcon.OnDetailModalIndexChange = scroll.OnDetailModalIndexChange;
        }
        
        private void SetFirstSelect()
        {
            // 選択中のIndex
            int selectedIndex = GetIndex(selectedCharacterId >= 0 ? selectedCharacterId : Arguments.SupportCharacterId);
            
            // 初期選択を一番目にする
            if(selectedIndex < 0 && scroll.ItemList.Count > 0)
            {
                selectedIndex = 0;
            }
            
            if(selectedIndex >= 0)
            {
                SupportEquipmentScrollData data = scroll.ItemList[selectedIndex];
                // デッキ制限がかかっていなければ編成ボタンをON
                nextButton.interactable = data.IsLimit == false;
                scroll.SelectSupportEquipment(data);
                SetDetailId(data.Id);
                detailView.gameObject.SetActive(true);
            }
            else
            {
                detailView.gameObject.SetActive(false);
            }
        }
        
        private void SetDetailId(long id)
        {
            selectedCharacterId = id;
            detailView.SetEquipmentId(id, scroll.DetailOrderList);
        }
        
        
        private int GetIndex(long uEquipmentId)
        {
            for(int i=0;i<scroll.ItemList.Count;i++)
            {
                if(scroll.ItemList[i].Id == uEquipmentId)
                {
                    return i;
                }
            }
            return -1;
        }
        
        private void OnSelectSupportEquipment(object value)
        {
            SupportEquipmentScrollData data = (SupportEquipmentScrollData)value;
            // デッキ制限がかかっていなければ編成ボタンをON
            nextButton.interactable = data.IsLimit == false;
            SetDetailId(data.Id);
        }
        
        /// <summary>UGUI</summary>
        public void OnDecisionButton()
        {
            Arguments.SelectedSupportCharacterId = selectedCharacterId;
            TrainingPreparationManager.PrevPage();
        }
        
        [CruEventTarget]
        private void OnCloseDetailModal(SupportEquipmentDetailModal.CloseUpdateType updateType)
        {
            SupportEquipmentScrollData scrollData = scroll.SelectedSupportEquipmentIds.FirstOrDefault();
            if (scrollData != null)
            {
                SetDetailId(scrollData.Id);
            }

            scroll.UpdateScrollFavorite();
            if(updateType != SupportEquipmentDetailModal.CloseUpdateType.None)
            {
                scroll.OnRedrawSupportEquipment();
                SetFirstSelect();
            }
        }
    }
}