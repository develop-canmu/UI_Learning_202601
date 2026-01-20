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
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.UserData;

namespace Pjfb.Training
{
    
    /// <summary>
    /// トレーニングキャラクタ選択画面
    /// </summary>
    
    public class TrainingSupportDeckSelectPage : TrainingDeckSelectPage
    {
        public override DeckListData DeckList{get{return Arguments.DeckList;}}
        public override long PartyNumber{get{return Arguments.PartyNumber;}}



        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if(TransitionType == PageTransitionType.Back)
            {
                UpdateFriendList().Forget();
                scrollBanner.ScrollGrid.RefreshItemView();
                return;
            }
         
            // フレンドのエラー
            PjfbAPIErrorHandler handler = (PjfbAPIErrorHandler)APIManager.Instance.errorHandler;
            handler.SetUniqueErrorHandleFunc(TrainingUtility.API_ERROR_CODE_FRIEND_LIMIT, (request, errorCode, errorMessage)=>
            {
                return default;
            });
            
            // 初期化
            await InitializePageAsync();
            await base.OnPreOpen(args, token);
        }
        
        private async UniTask InitializePageAsync()
        {
            // リスト保持
            Arguments.DeckList = await DeckUtility.GetDeckList(DeckType.Training);
            Arguments.EquipmentDeckList = await DeckUtility.GetDeckList(DeckType.SupportEquipment);
            // パーティ番号初期化
            if(Arguments.PartyNumber == 0)
            {
                Arguments.PartyNumber = Arguments.DeckList.DeckDataList[0].PartyNumber;
            }
            if(Arguments.EquipmentPartyNumber == 0)
            {
                Arguments.EquipmentPartyNumber = Arguments.EquipmentDeckList.DeckDataList[0].PartyNumber;
            }
            
            await UpdateFriendList();
        }
        
        private async UniTask UpdateFriendList()
        {
            if(Arguments.IsUpdateFriendList)
            {
                // フレンド取得API
                TrainingGetFriendListAPIRequest getFriendRequest = new TrainingGetFriendListAPIRequest();
                await APIManager.Instance.Connect(getFriendRequest);
                // 結果を取得
                TrainingGetFriendListAPIResponse getFriendResponse = getFriendRequest.GetResponseData();
                Arguments.FriendList = getFriendResponse.friendCharaList;
                
                // フレンド枠を初期化
                foreach(DeckData deck in Arguments.DeckList.DeckDataList)
                {
                    deck.Friend = null;
                }
                
                // ビューの更新
                scrollBanner.ScrollGrid.RefreshItemView();
                
                Arguments.IsUpdateFriendList = false;
                
                // ボタンを更新
                int deckIndex = Arguments.SupportCharacterDeckSelectedData.DeckIndex;
                DeckData deckData = Arguments.DeckList.DeckDataList[deckIndex];
                UpdateNextButton(deckData);
            }
        }

        protected override void OnEnablePage(object args)
        {
            
            if(TransitionType == PageTransitionType.Back)
            {
                // キャラ変更
                if(Arguments.SelectedSupportCharacterId != TrainingSupportDeckSelectPage.None)
                {
                    // デッキ選択情報
                    TrainingSupportCharacterDeckView.SelectData select = Arguments.SupportCharacterDeckSelectedData;
                    // デッキ情報
                    int deckIndex = Arguments.SupportCharacterDeckSelectedData.DeckIndex;
                    DeckData deckData = Arguments.DeckList.DeckDataList[deckIndex];
                    // デッキ変更
                    if(select.Type == TrainingDeckMemberType.Friend)
                    {
                        if (Arguments.IsFriendSlotSelectMyChara == true)
                        {
                            UserDataChara uCharaData = UserDataManager.Instance.chara.Find(Arguments.SelectedSupportCharacterId);
                            deckData.Friend = TrainingDeckUtility.ConvertUCharaToFriendLend(uCharaData);
                        }
                        else
                        {
                            foreach(CharaV2FriendLend friend in Arguments.FriendList)
                            {
                                if(friend.id == Arguments.SelectedSupportCharacterId)
                                {
                                    deckData.Friend = friend;
                                    break;
                                }
                            }   
                        }
                    }
                    else
                    {
                        deckData.SetMemberId(select.Order, Arguments.SelectedSupportCharacterId);
                    }
                    // ビューの更新
                    scrollBanner.ScrollGrid.RefreshItemView();
                    
                    // 使用可能なデッキ？
                    UpdateNextButton(deckData);
                    // 現在のデッキをセット
                    TrainingDeckUtility.SetCurrentTrainingDeck(new TrainingDeckUtility.DeckMember(CharacterUtility.UserCharIdToMCharId(Arguments.TrainingUCharId), deckData) );
                }
                
                return;
            }
            
            base.OnEnablePage(args);
            // チュートリアル
            OpenTutorial().Forget();
        }
        
        /// <summary>チュートリアル開く</summary>
        private async UniTask OpenTutorial()
        {
            await AppManager.Instance.TutorialManager.OpenSupportCardTutorialAsync();
            await AppManager.Instance.TutorialManager.OpenExtraSupportCardTutorialAsync();
            await CheckShowPageTransitionTutorialAsync();
        }
        
        /// <summary>ページ切り替えタイプのチュートリアル表示</summary>
        private async UniTask CheckShowPageTransitionTutorialAsync()
        {
            // アドバイザーが解放されているかを確認
            if(UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.TrainingAdviser) == false) return;
            // アドバイザー編成のチュートリアル
            long tutorialId = (long)HowToPlayUtility.TutorialType.AdviserTrainingDeck;
            if (LocalSaveManager.saveData.tutorialIdConfirmList.Contains(tutorialId) == false)
            {
                CruFramework.Page.ModalWindow howToModal = await HowToPlayUtility.OpenHowToPlayModal(tutorialId, StringValueAssetLoader.Instance["character.detail_modal.adviser.title"]);
                await howToModal.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
                LocalSaveManager.saveData.tutorialIdConfirmList.Add(tutorialId);
                LocalSaveManager.Instance.SaveData();
            }
        }
        
        private void UpdateNextButton(DeckData deckData)
        {
            long deckFormatId = MasterManager.Instance.trainingScenarioMaster.FindData(Arguments.TrainingScenarioId).mDeckFormatId;
            // 使用可能なデッキ？
            nextButton.interactable = deckData.IsEnableDeck( CharacterUtility.UserCharIdToParentId(Arguments.TrainingUCharId), deckFormatId, Arguments.TrainingUCharId);
        }

        protected override void SetDeck(int index)
        {
            selectedDeckIndex = index;
            
            TrainingPreparation m = (TrainingPreparation)Manager;
            DeckData deckData = Arguments.DeckList.DeckDataList[index];
            m.SetDeckIndex(deckData);
            // 選択デッキ番号を登録
            Arguments.PartyNumber = deckData.Deck.partyNumber;
            // 使用可能なデッキ？
            UpdateNextButton(deckData);
            // 現在のデッキをセット
            TrainingDeckUtility.SetCurrentTrainingDeck( new TrainingDeckUtility.DeckMember(CharacterUtility.UserCharIdToMCharId(Arguments.TrainingUCharId), deckData) );
        }
        
        private void OnEnable()
        {
            if(Arguments == null)return;
            DeckData deck = Arguments.DeckList.GetDeck(Arguments.PartyNumber);
            TrainingDeckUtility.SetCurrentTrainingDeck( new TrainingDeckUtility.DeckMember(CharacterUtility.UserCharIdToMCharId(Arguments.TrainingUCharId), deck) );
        }
        
        protected override void OnSelectedItem(ScrollGridItem item, object value)
        {
            TrainingSupportCharacterDeckItem.EventData e = (TrainingSupportCharacterDeckItem.EventData)value;
            switch(e.Type)
            {
                // サポート変更
                case TrainingSupportCharacterDeckItem.EventType.SelectCharacter:
                    OnSelectedCharacter((TrainingSupportCharacterDeckView.SelectData)e.Value, TrainingPreparationPageType.SupportCharacterSelect);
                    break;
                case TrainingSupportCharacterDeckItem.EventType.Reset:
                    OnResetDeck( (TrainingSupportCharacterDeckView)e.Value );
                    break;
                case TrainingSupportCharacterDeckItem.EventType.Recommend:
                    OnRecommendDeck((TrainingSupportCharacterDeckView)e.Value);
                    break;
            }
        }

        
        private void OnResetDeck(TrainingSupportCharacterDeckView view)
        {
            // デッキ情報
            DeckData deckData = view.DeckData;
            
            // サポートキャラを解除
            for(int i=0;i<deckData.MemberCount;i++)
            {
                if(deckData.GetMemberType(i) == DeckMemberType.UChar)
                {
                    deckData.SetMemberId(i, DeckUtility.EmptyDeckSlotId);
                }
            }
            // フレンドを解除
            deckData.Friend = null;
            // 選択不可
            nextButton.interactable = false;
            // Viewの更新
            view.SetView(Arguments, deckData);
        }
        
        private void OnRecommendDeck(TrainingSupportCharacterDeckView view)
        {
            OnRecommendDeckAsync(view).Forget();
        }

        public void OpenDetail(CharacterIcon icon)
        {
            switch (icon.BaseCharaType)
            {
                case BaseCharacterType.SupportCharacter:
                    BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail,
                        new BaseCharaDetailModalParams(icon.SwipeableParams,  false, false, titleStringKey: "character.detail_modal.support_character_info", Arguments.TrainingScenarioId, icon.CanGrowth));
                    break;
                case BaseCharacterType.SpecialSupportCard:
                    SpecialSupportCardDetailModalWindow.Open(ModalType.SpecialSupportCardDetail,
                        new BaseCharaDetailModalParams(icon.SwipeableParams, false,　false,
                            titleStringKey: "character.detail_modal.special_support_info", -1, icon.CanGrowth));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private async UniTask OnRecommendDeckAsync(TrainingSupportCharacterDeckView view)
        { 
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingDeckRecommend, Arguments);
            bool isChange = (bool)await modal.WaitCloseAsync();
            // デッキが変わったのでViewを更新
            view.UpdateView();
            
            // 選択可能？
            UpdateNextButton(view.DeckData);
            DeckData currentDeckData = Arguments.DeckList.DeckDataList[selectedDeckIndex];
            // 現在のデッキをセット
            TrainingDeckUtility.SetCurrentTrainingDeck( new TrainingDeckUtility.DeckMember(CharacterUtility.UserCharIdToMCharId(Arguments.TrainingUCharId), currentDeckData) );
        }
        
        private void OnSelectedCharacter(TrainingSupportCharacterDeckView.SelectData selectData, TrainingPreparationPageType pageType)
        {
            // デッキ情報
            DeckData deckData = Arguments.DeckList.DeckDataList[selectData.DeckIndex];
            // 選択結果を初期化
            Arguments.SelectedSupportCharacterId = TrainingSupportDeckSelectPage.None;
            // フレンド枠
            Arguments.CharacterMemberType = selectData.Type;
            // 選択情報
            Arguments.SupportCharacterDeckSelectedData = selectData;
            
            switch(selectData.Type)
            {
                case TrainingDeckMemberType.Friend:
                    // 現在選択中
                    Arguments.SupportCharacterId = deckData.Friend == null ? DeckUtility.EmptyDeckSlotId : deckData.Friend.id;
                    break;
                case TrainingDeckMemberType.Support:
                    // 現在選択中
                    Arguments.SupportCharacterId = deckData.GetMemberId(selectData.Order);
                    break;
                case TrainingDeckMemberType.SpecialSupport:
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
            await Arguments.DeckList.SaveAsync(Arguments.PartyNumber);
            TrainingPreparationManager.OpenPage(TrainingPreparationPageType.SupportEquipmentDeckSelect, true, Arguments);
        }

        public void OnClickRarityLimitation()
        {
            // レアリティ制限モーダルで使用する引数を設定して開く
            TrainingRarityLimitationModal.ModalArgs args = new (Arguments.TrainingScenarioId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.FormationLimitation, args);
        }
    }
}