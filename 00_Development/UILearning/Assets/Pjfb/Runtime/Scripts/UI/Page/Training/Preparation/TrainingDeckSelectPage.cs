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
    
    public abstract class TrainingDeckSelectPage : TrainingPreparationPageBase
    {
        public const int None = -2;
        public const int EmptyId = -1;
     
     
        [SerializeField]
        protected ScrollBanner scrollBanner = null;
        [SerializeField]
        protected UIButton nextButton = null;
        
        // 選択中のデッキ
        protected int selectedDeckIndex = 0;
        
        /// <summary>使用するデッキ</summary>
        public abstract DeckListData DeckList{get;}
        /// <summary>デッキ番号</summary>
        public  abstract long PartyNumber{get;}
        
        
        // デッキのセット
        protected virtual void SetDeck(int index){}
        /// <summary>選択時</summary>
        protected virtual void OnSelectedItem(ScrollGridItem item, object value){}

        protected override void OnEnablePage(object args)
        {
            
            // 初期選択デッキ
            SetDeck(0);

            // トレーニングキャラ
            scrollBanner.ScrollGrid.CommonItemValue = Arguments;
            // スクロールにリスト登録
            scrollBanner.SetBannerDatas(DeckList.DeckDataList);
            
            scrollBanner.ScrollGrid.OnItemEvent -= OnSelectedItem;
            scrollBanner.ScrollGrid.OnItemEvent += OnSelectedItem;
            
            scrollBanner.ScrollGrid.OnChangedPage -= OnChangePage;
            scrollBanner.ScrollGrid.OnChangedPage += OnChangePage;
            
            base.OnEnablePage(args);
        }

        public override void OnBackPage()
        {
            scrollBanner.ScrollGrid.RefreshItemView();
        }
        
        /// <summary>UGUI</summary>
        public void OnBackButton()
        {
            BackButtonAsync().Forget();
        }
        
        public async UniTask<bool> CheckDeckChange()
        {
            // 選択していたデッキ
            DeckData deckData = DeckList.DeckDataList[selectedDeckIndex];
            TrainingPreparation m = (TrainingPreparation)Manager;
            return await m.CheckDeckChange(deckData);
        }
        
        private async UniTask BackButtonAsync()
        {
            // デッキ変更チェック
            bool result = await CheckDeckChange();
            if(result)return;
            // 選択をリセット
            Arguments.SelectedSupportCharacterId = TrainingSupportDeckSelectPage.None;
            // 戻る
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }
        
        private void OnChangePage(int page)
        {
            OnChangePageAsync(page).Forget();
        }
        
        private async UniTask OnChangePageAsync(int page)
        {
            if(selectedDeckIndex == page)return;
            // デッキ変更のチェック
            bool isCancel = await CheckDeckChange();
            // キャンセル
            if(isCancel)
            {
                scrollBanner.SetIndex(selectedDeckIndex, true);
                return;
            }
            // 表示更新
            scrollBanner.ScrollGrid.RefreshItemView();
            SetDeck(page);
        }
        
        /// <summary>UGUI</summary>
        public void OnSkillListButton()
        {
            // デッキ
            DeckData characterDeck = Arguments.DeckList.GetDeck(Arguments.PartyNumber);
            DeckData equipmentDeck = Arguments.EquipmentDeckList.GetDeck(Arguments.EquipmentPartyNumber);
            
            UserDataChara trainingUChar = UserDataManager.Instance.chara.Find(Arguments.TrainingUCharId);
            
            List<TrainingPracticeSkillModal.CharacterData> characterList = new List<TrainingPracticeSkillModal.CharacterData>();
            List<TrainingPracticeSkillModal.CharacterData> specialSupportList = new List<TrainingPracticeSkillModal.CharacterData>();
            List<TrainingPracticeSkillModal.CharacterData> equipmentList = new List<TrainingPracticeSkillModal.CharacterData>();
            List<TrainingPracticeSkillModal.CharacterData> adviserList = new List<TrainingPracticeSkillModal.CharacterData>();
            
            long[] members = characterDeck.GetCharacterMemberIds();
            foreach(long id in members)
            {
                // Empty
                if(id == DeckUtility.EmptyDeckSlotId)continue;
                // uChar
                UserDataChara uChar = UserDataManager.Instance.chara.Find(id);
                // mChar
                CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(uChar.charaId);
                // キャラデータ
                TrainingCharacterData characterData = new TrainingCharacterData(uChar.charaId, uChar.level, uChar.newLiberationLevel, uChar.id);

                switch(mChar.cardType)
                {
                    case CardType.Character:
                        characterList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, TrainingPracticeSkillModal.OptionType.None));
                        break;
                    case CardType.SpecialSupportCharacter:
                        specialSupportList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, TrainingPracticeSkillModal.OptionType.None));
                        break;
                    case CardType.Adviser:
                        adviserList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, TrainingPracticeSkillModal.OptionType.None));
                        break;
                }
            }

            if (CharacterUtility.HasJoinTrainingHimselfBonus(trainingUChar.level))
            {
                var trainingCharacterData = new TrainingCharacterData(trainingUChar.charaId, trainingUChar.level, trainingUChar.newLiberationLevel, trainingUChar.id);
                characterList.Add(new TrainingPracticeSkillModal.CharacterData(trainingCharacterData, TrainingPracticeSkillModal.OptionType.None));
            }
            
            long[] equipMembers = equipmentDeck.GetEquipmentMemberIds();
            for(int i=0;i<equipMembers.Length;i++)
            {
                long id = equipMembers[i];
                // Empty
                if(id == DeckUtility.EmptyDeckSlotId)continue;
                // 開放チェック
                if(equipmentDeck.GetUnlockLevel(i) > trainingUChar.level)continue;
                // uChar
                UserDataSupportEquipment uEuqipment = UserDataManager.Instance.supportEquipment.Find(id);
                // mChar
                CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(uEuqipment.charaId);
                // キャラデータ
                TrainingCharacterData characterData = new TrainingCharacterData(uEuqipment.charaId, uEuqipment.level, 0, uEuqipment.id);
                // リストに追加
                equipmentList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, uEuqipment.lotteryProcessJson.statusList, TrainingPracticeSkillModal.OptionType.None));
            }
            
            // フレンド
            if(characterDeck.Friend != null)
            {
                TrainingCharacterData characterData = new TrainingCharacterData(characterDeck.Friend.mCharaId, characterDeck.Friend.level, characterDeck.Friend.newLiberationLevel, characterDeck.Friend.id);
                characterList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, TrainingPracticeSkillModal.OptionType.None));
            }
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingPracticeSkill, new TrainingPracticeSkillModal.Arguments(null, null, Arguments.TrainingScenarioId, false, characterList, specialSupportList, equipmentList, adviserList, true));
        }
        
        //// <summary> トレーニング編成強化のレベルアップ </summary>
        public void OnTrainingDeckEnhanceLevelUp()
        {
            // トレーニング編成強化画面に遷移(編成効果タブ)
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Character, true, new CharacterPage.Data(CharacterPageType.DeckEnhance, null));
        }

        //// <summary> トレーニング編成強化の強化詳細 </summary>
        public void OnTrainingDeckEnhanceDetail()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingDeckEnhanceDetail, null);
        }
    }
}
