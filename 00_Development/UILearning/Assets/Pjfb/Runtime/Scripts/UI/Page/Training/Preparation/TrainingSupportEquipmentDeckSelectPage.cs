using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UserData;

namespace Pjfb.Training
{
    
    /// <summary>
    /// トレーニングサポート器具選択画面
    /// </summary>
    
    public class TrainingSupportEquipmentDeckSelectPage : TrainingDeckSelectPage
    {
        public override DeckListData DeckList{get{return Arguments.EquipmentDeckList;}}
        public override long PartyNumber{get{return Arguments.EquipmentPartyNumber;}}
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {

            if(TransitionType == PageTransitionType.Back)
            {
                scrollBanner.ScrollGrid.RefreshItemView();
                return;
            }
            
            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
        
            // サポート器具のチュートリアル
            if( UserDataManager.Instance.IsUnlockSystem(TrainingDeckUtility.SupportEquipmentSystemLockId) )
            {
                AppManager.Instance.TutorialManager.OpenTrainingSupportEquipmentTutorialAsync().Forget();
            }
        
            if(TransitionType == PageTransitionType.Back)
            {
                // キャラ変更
                if(Arguments.SelectedSupportCharacterId != TrainingSupportDeckSelectPage.None)
                {
                    // デッキ選択情報
                    TrainingSupportCharacterDeckView.SelectData select = Arguments.SupportCharacterDeckSelectedData;
                    // デッキ情報
                    int deckIndex = Arguments.SupportCharacterDeckSelectedData.DeckIndex;
                    DeckData deckData = Arguments.EquipmentDeckList.DeckDataList[deckIndex];
                    // デッキ変更
                    deckData.SetMemberId(select.Order, Arguments.SelectedSupportCharacterId, true);
                    // ビューの更新
                    scrollBanner.ScrollGrid.RefreshItemView();
                }
                
                return;
            }
            
            base.OnEnablePage(args);
        }
        
        protected override void SetDeck(int index)
        {
            selectedDeckIndex = index;
            
            TrainingPreparation m = (TrainingPreparation)Manager;
            DeckData deckData = Arguments.EquipmentDeckList.DeckDataList[index];
            m.SetDeckIndex(deckData);
            // 選択デッキ番号を登録
            Arguments.EquipmentPartyNumber = deckData.Deck.partyNumber;
        }
        
        protected override void OnSelectedItem(ScrollGridItem item, object value)
        {
            TrainingSupportEquipmentDeckItem.EventData e = (TrainingSupportEquipmentDeckItem.EventData)value;
            switch(e.Type)
            {
                case TrainingSupportEquipmentDeckItem.EventType.Reset:
                    OnResetDeck( (TrainingSupportEquipmentDeckView)e.Value );
                    break;
                case TrainingSupportEquipmentDeckItem.EventType.Equipment:
                    OnSelectedCharacter((TrainingSupportEquipmentDeckView.SelectData)e.Value, TrainingPreparationPageType.SupportEquipmentSelect);
                    break;
            }
        }

        
        private void OnResetDeck(TrainingSupportEquipmentDeckView view)
        {
            // デッキ情報
            DeckData deckData = view.DeckData;
            
            // サポート器具を解除
            for(int i=0;i<deckData.MemberCount;i++)
            {
                if(deckData.GetMemberType(i) == DeckMemberType.UEquipment)
                {
                    deckData.SetMemberId(i, DeckUtility.EmptyDeckSlotId);
                }
            }
            // Viewの更新
            view.SetView(Arguments, deckData);
        }
        
        public void OpenDetail(CharacterIcon icon)
        {

        }
        
        private void OnSelectedCharacter(TrainingSupportEquipmentDeckView.SelectData selectData, TrainingPreparationPageType pageType)
        {
            // デッキ情報
            DeckData deckData = Arguments.EquipmentDeckList.DeckDataList[selectData.DeckIndex];
            // 選択結果を初期化
            Arguments.SelectedSupportCharacterId = TrainingSupportDeckSelectPage.None;
            // フレンド枠
            Arguments.CharacterMemberType = selectData.Type;
            // 選択情報
            Arguments.SupportCharacterDeckSelectedData = selectData;
            
            switch(selectData.Type)
            {
                case TrainingDeckMemberType.Equipment:
                    // 現在選択中
                    Arguments.SupportCharacterId = deckData.GetMemberId(selectData.Order);
                    break;
            }

            // サポートキャラ選択画面へ移動
            TrainingPreparationManager.OpenPage(pageType, true, Arguments);
        }
        
        /// <summary>UGUI</summary>
        public void OnNextButton()
        {
            if(scrollBanner.ScrollGrid.IsPagingAnimation)
            {
                return;
            }
            NextAsync().Forget();
        }
        
        
        private async UniTask NextAsync()
        {
            // デッキに変更があれば保存
            await Arguments.EquipmentDeckList.SaveAsync(Arguments.PartyNumber);
            // キャラの所持数をチェック
            if(UserDataManager.Instance.charaVariable.data.Values.Count >= ConfigManager.Instance.uCharaVariableCountMax)
            {
                ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (modal)=>modal.Close());
                ConfirmModalData modalData = new ConfirmModalData(StringValueAssetLoader.Instance["common.confirm"], StringValueAssetLoader.Instance["common.character_max_modal.msg"], string.Empty, button);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, modalData);
                return;
            }
            
            switch(Arguments.Mode)
            {
                case TrainingMode.Auto:
                {
                    // 自動トレーニングの場合は育成方針選択モーダル
                    CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoTrainingStatusPolicySetting, Arguments);
                    // closedParameterがlongじゃなければキャンセル
                    if (await modal.WaitCloseAsync() is not long autoTrainingPolity)
                    {
                        return;
                    }
                    Arguments.AutoTrainingPolity = autoTrainingPolity;
                    break;
                }
            }
            
            // 開くモーダル
            ModalType modalType = ModalType.TrainingStartConfirm;

            object arguments = null;
            
            switch(Arguments.Mode)
            {
                case TrainingMode.Default:
                    arguments = Arguments;
                    modalType = ModalType.TrainingStartConfirm;
                    break;
                case TrainingMode.Auto:
                    modalType = ModalType.AutoTrainingConfirm;
                    arguments = new AutoTrainingConfirmModal.Arguments(Arguments, CurrentSlotAutoTrainingPendingStatus, AutoTrainingUserStatus, AutoTrainingModalType.Start);
                    break;
            }
            
            

            
            // 確認モーダル
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, arguments );
            if(await modalWindow.WaitCloseAsync() is bool result && result == true)
            {
                AppManager.Instance.UIManager.PageManager.PrevPage();
            }
        }
        
        
        [CruEventTarget]
        private void OnCloseDetailModal(SupportEquipmentDetailModal.CloseUpdateType updateType)
        {
            if(updateType != SupportEquipmentDetailModal.CloseUpdateType.None)
            {
                // 表示の更新
                scrollBanner.ScrollGrid.RefreshItemView();
            }
        }
        
        public void OnClickRarityLimitation()
        {
            // レアリティ制限モーダルで使用する引数を設定して開く
            TrainingRarityLimitationModal.ModalArgs args = new (Arguments.TrainingScenarioId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.FormationLimitation, args);
        }
    }
}