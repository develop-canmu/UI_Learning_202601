using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Training
{
    
    /// <summary>
    /// トレーニングキャラクタ選択画面
    /// </summary>
    
    public class TrainingSupportCharacterSelectPage : TrainingPreparationPageBase
    {

        private const string DetailTitleKey = "character.detail_modal.support_character_info";

        [SerializeField]
        private CharacterStatusView statusView = null;

        [SerializeField]
        private UserCharacterScroll userCharacterScroll = null;
        [SerializeField]
        private UserCharacterScroll userSpecialCharacterScroll = null;
        [SerializeField]
        private UserCharacterScroll userExtraCharacterScroll = null;
        [SerializeField]
        private UserCharacterScroll userAdviserScroll;
        [SerializeField]
        private FriendCharacterScroll friendCharacterScroll = null;
        
        [SerializeField]
        private TMPro.TMP_Text pageNameText = null;
        
        [SerializeField]
        private UIButton nextButton = null;
        
        [SerializeField]
        private UIButton helpButton = null;
        
        [SerializeField]
        private UINotification description = null;
        
        [SerializeField]
        private GameObject limitationCauntion = null;
        [SerializeField]
        private TMPro.TMP_Text textCauntion = null;
        
        [SerializeField]
        private UIButton switchListButton = null;
        [SerializeField]
        private TMPro.TMP_Text switchListButtonText = null;
        
        protected long selectedCharacterId = TrainingSupportDeckSelectPage.None;
        
        private Action<string> OnClickHelp = (category) => HelpModalWindow.Open(new HelpModalWindow.WindowParams { categoryList = new List<string>{category}});
        
        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            Arguments.SelectedSupportCharacterId = selectedCharacterId;
            TrainingPreparationManager.PrevPage();
        }

        public override void OnBackPage()
        {
            Initialize();
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            helpButton.OnClickEx.RemoveAllListeners();
            switchListButton.gameObject.SetActive(false);
            switch(Arguments.CharacterMemberType)
            {
                // フレンド
                case TrainingDeckMemberType.Friend:
                {
                    pageNameText.text = StringValueAssetLoader.Instance["common.friend"];
                    helpButton.OnClickEx.AddListener(() => OnClickHelp(StringValueAssetLoader.Instance["help.category.friend_borrowing"]));
                    SetSwitchButtonText();
                    switchListButton.gameObject.SetActive(true);
                    break;
                }
                
                case TrainingDeckMemberType.SpecialSupport:
                {
                    string key = Arguments.IsExtraCharacter ? "common.extra_support_card" : "common.special_support_card";
                    pageNameText.text = StringValueAssetLoader.Instance[key];
                    helpButton.OnClickEx.AddListener(() => OnClickHelp(StringValueAssetLoader.Instance[key]));
                    break;
                }
                
                case TrainingDeckMemberType.Adviser:
                {
                    pageNameText.text = StringValueAssetLoader.Instance["common.adviser"];
                    helpButton.OnClickEx.AddListener(() => OnClickHelp(StringValueAssetLoader.Instance["common.adviser"]));
                    break;
                }
                
                case TrainingDeckMemberType.Support:
                {
                    pageNameText.text = StringValueAssetLoader.Instance["common.support_character"];
                    helpButton.OnClickEx.AddListener(() => OnClickHelp(StringValueAssetLoader.Instance["footer.character"]));
                    break;
                }
            }
            return base.OnPreOpen(args, token);
        }

        private void SelectFirstCharacter()
        {
            // デッキ
            DeckData deckData = Arguments.DeckList.DeckDataList[Arguments.SupportCharacterDeckSelectedData.DeckIndex];
            
            switch(Arguments.CharacterMemberType)
            {
                // フレンド
                case TrainingDeckMemberType.Friend:
                {
                    // 選択済みのキャラを表示
                    if(friendCharacterScroll.ItemList.Count > 0)
                    {
                        CharacterScrollData characterData = friendCharacterScroll.ItemList[0];
                        ScrollGridItem item = friendCharacterScroll.Scroll.GetItem(characterData);
                        if(item != null)
                        {
                            friendCharacterScroll.Scroll.SelectItem(item);
                            OnSelectedCharacter(characterData, false);
                            // 表示
                            statusView.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        // 次へボタン
                        nextButton.interactable = false;
                        friendCharacterScroll.Scroll.DeselectAllItems();
                        // 非表示に
                        statusView.gameObject.SetActive(false);
                    }
                    
                    break;
                }
                
                case TrainingDeckMemberType.Support:
                {
                    // 選択済みのキャラを表示
                    if(userCharacterScroll.ItemList.Count > 0)
                    {
                        CharacterScrollData characterData = userCharacterScroll.ItemList[0];
                        ScrollGridItem item = userCharacterScroll.Scroll.GetItem(characterData);
                        if(item != null)
                        {
                            userCharacterScroll.Scroll.SelectItem(item);
                            OnSelectedCharacter(characterData, false);
                            // 表示
                            statusView.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        // 次へボタン
                        nextButton.interactable = false;
                        userCharacterScroll.Scroll.DeselectAllItems();
                        // 非表示に
                        statusView.gameObject.SetActive(false);
                    }
                    break;
                }
                
                case TrainingDeckMemberType.SpecialSupport:
                {
                    if (userExtraCharacterScroll != null && Arguments.IsExtraCharacter)
                    {
                        // 選択済みのキャラを表示
                        if(userExtraCharacterScroll.ItemList.Count > 0)
                        {
                            CharacterScrollData characterData = userExtraCharacterScroll.ItemList[0];
                            ScrollGridItem item = userExtraCharacterScroll.Scroll.GetItem(characterData);
                            if(item != null)
                            {
                                userExtraCharacterScroll.Scroll.SelectItem(userExtraCharacterScroll.Scroll.GetItem(characterData));
                                OnSelectedCharacter(characterData, false);
                                // 表示
                                statusView.gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            // 次へボタン
                            nextButton.interactable = false;
                            userExtraCharacterScroll.Scroll.DeselectAllItems();
                            // 非表示に
                            statusView.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // 選択済みのキャラを表示
                        if(userSpecialCharacterScroll.ItemList.Count > 0)
                        {
                            CharacterScrollData characterData = userSpecialCharacterScroll.ItemList[0];
                            ScrollGridItem item = userSpecialCharacterScroll.Scroll.GetItem(characterData);
                            if(item != null)
                            {
                                userSpecialCharacterScroll.Scroll.SelectItem(userSpecialCharacterScroll.Scroll.GetItem(characterData));
                                OnSelectedCharacter(characterData, false);
                                // 表示
                                statusView.gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            // 次へボタン
                            nextButton.interactable = false;
                            userSpecialCharacterScroll.Scroll.DeselectAllItems();
                            // 非表示に
                            statusView.gameObject.SetActive(false);
                        }
                    }
                    break;
                }
                
                case TrainingDeckMemberType.Adviser:
                {
                    // 選択済みのキャラを表示
                    if(userAdviserScroll.ItemList.Count > 0)
                    {
                        CharacterScrollData characterData = userAdviserScroll.ItemList[0];
                        ScrollGridItem item = userAdviserScroll.Scroll.GetItem(characterData);
                        if(item != null)
                        {
                            userAdviserScroll.Scroll.SelectItem(item);
                            OnSelectedCharacter(characterData, false);
                            // 表示
                            statusView.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        // 次へボタン
                        nextButton.interactable = false;
                        userAdviserScroll.Scroll.DeselectAllItems();
                        // 非表示に
                        statusView.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }
        
        private void Initialize()
        {
            
            
            // デッキ
            DeckData deckData = Arguments.DeckList.DeckDataList[Arguments.SupportCharacterDeckSelectedData.DeckIndex];
            // 編成中のキャラは選択できないように
            List<long> disableIds = new List<long>();
            // トレーニングキャラ
            long trainingCharacterId = CharacterUtility.UserCharIdToParentId(Arguments.TrainingUCharId);
            disableIds.Add(trainingCharacterId);
            // 制限キャラ
            List<long> limitedIds = new List<long>();
            
            
            userCharacterScroll.OnSelectedItem -= OnSelectedCharacter;
            userCharacterScroll.OnSelectedItem += OnSelectedCharacter;
            userCharacterScroll.OnSortFilter -= OnSortFilter;
            userCharacterScroll.OnSortFilter += OnSortFilter;
            userCharacterScroll.OnReverseCharacterOrder -= OnSortFilter;
            userCharacterScroll.OnReverseCharacterOrder += OnSortFilter;
            userCharacterScroll.GetTrainingScenarioId = () => Arguments.TrainingScenarioId;
            
            userSpecialCharacterScroll.OnSelectedItem -= OnSelectedCharacter;
            userSpecialCharacterScroll.OnSelectedItem += OnSelectedCharacter;
            userSpecialCharacterScroll.OnSortFilter -= OnSortFilter;
            userSpecialCharacterScroll.OnSortFilter += OnSortFilter;
            userSpecialCharacterScroll.OnReverseCharacterOrder -= OnSortFilter;
            userSpecialCharacterScroll.OnReverseCharacterOrder += OnSortFilter;
            userSpecialCharacterScroll.GetTrainingScenarioId = () => Arguments.TrainingScenarioId;
            
            userExtraCharacterScroll.OnSelectedItem -= OnSelectedCharacter;
            userExtraCharacterScroll.OnSelectedItem += OnSelectedCharacter;
            userExtraCharacterScroll.OnSortFilter -= OnSortFilter;
            userExtraCharacterScroll.OnSortFilter += OnSortFilter;
            userExtraCharacterScroll.OnReverseCharacterOrder -= OnSortFilter;
            userExtraCharacterScroll.OnReverseCharacterOrder += OnSortFilter;
            userExtraCharacterScroll.GetTrainingScenarioId = () => Arguments.TrainingScenarioId;
            
            userAdviserScroll.OnSelectedItem -= OnSelectedCharacter;
            userAdviserScroll.OnSelectedItem += OnSelectedCharacter;
            userAdviserScroll.OnSortFilter -= OnSortFilter;
            userAdviserScroll.OnSortFilter += OnSortFilter;
            userAdviserScroll.OnReverseCharacterOrder -= OnSortFilter;
            userAdviserScroll.OnReverseCharacterOrder += OnSortFilter;
            userAdviserScroll.GetTrainingScenarioId = () => Arguments.TrainingScenarioId;
            
            friendCharacterScroll.OnSelectedItem -= OnSelectedCharacter;
            friendCharacterScroll.OnSelectedItem += OnSelectedCharacter;
            friendCharacterScroll.OnSortFilter -= OnSortFilter;
            friendCharacterScroll.OnSortFilter += OnSortFilter;
            friendCharacterScroll.OnReverseCharacterOrder -= OnSortFilter;
            friendCharacterScroll.OnReverseCharacterOrder += OnSortFilter;
            friendCharacterScroll.GetTrainingScenarioId = () => Arguments.TrainingScenarioId;

            CharacterScroll targetScroll = null;

            switch(Arguments.CharacterMemberType)
            {
                // フレンド
                case TrainingDeckMemberType.Friend:
                {
                    CharaV2FriendLend[] charaList;
                    
                    // 表示切り替え
                    userCharacterScroll.gameObject.SetActive(false);
                    userSpecialCharacterScroll.gameObject.SetActive(false);
                    userExtraCharacterScroll.gameObject.SetActive(false);
                    friendCharacterScroll.gameObject.SetActive(true);
                    userAdviserScroll.gameObject.SetActive(false);
                    
                    // フレンドキャラか自キャラかでリストの中身だけ切り変わる
                    if (Arguments.IsFriendSlotSelectMyChara == true)
                    {
                        // 所持キャラからリストを作成
                        List<CharaV2FriendLend> charaDataList = new List<CharaV2FriendLend>();
                        foreach (UserDataChara charaData in UserDataManager.Instance.chara.data.Values)
                        {
                            if(charaData.MChara.cardType != CardType.Character) continue;
                            charaDataList.Add(TrainingDeckUtility.ConvertUCharaToFriendLend(charaData));
                        }
                        charaList = charaDataList.ToArray();
                    }
                    else
                    {
                        charaList = Arguments.FriendList;
                    }

                    // 編成済みのキャラを取得する
                    List<long> selectCharaIdList = new List<long>();
                    // 選択中のフレンド
                    if(deckData.Friend != null)
                    {
                        selectCharaIdList.Add(deckData.Friend.id);
                    }
                    // 選択中所持キャラ
                    selectCharaIdList.AddRange(deckData.GetMemberIds(DeckSlotCardType.Support));
                    // 選択中の選手をセット
                    friendCharacterScroll.SetSelectedCharacterIds(selectCharaIdList);
                    friendCharacterScroll.SetFixedCharacterIds(selectCharaIdList);
                    
                    // メンバーは編成不可
                    foreach(long id in deckData.GetCharacterMemberIds())
                    {
                        if(id == DeckUtility.EmptyDeckSlotId)continue;
                        disableIds.Add(CharacterUtility.UserCharIdToParentId(id));
                    }
                    
                    // 編成制限取得
                    long deckFormatId = MasterManager.Instance.trainingScenarioMaster.FindData(Arguments.TrainingScenarioId).mDeckFormatId;
                    DeckFormatConditionMasterObject limit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.RarityFour, deckFormatId);
                    
                    bool hasLimit = limit != null;
                    limitationCauntion.SetActive(hasLimit);
                    if(hasLimit)
                    {
                        long compareValue = long.Parse(limit.compareValue);
                        
                        // 現在の対象レアリティが何体編成されているか取得する
                        int count = DeckUtility.GetRareLimitCount(deckData, DeckSlotCardType.Support, limit.operatorType, compareValue);
                        // 育成対象が対象レアリティだった場合はカウントアップ
                        if (UserDataManager.Instance.chara.data[Arguments.TrainingUCharId].MChara.mRarityId == compareValue)
                        {
                            count++;
                        }
                        // 編成制限に達していたら選択不可にする
                        if(count >= limit.charaCount)
                        {
                            if (Arguments.IsFriendSlotSelectMyChara == true)
                            {
                                // 所持キャラの該当レアリティを全て選択不可に
                                foreach (UserDataChara chara in UserDataManager.Instance.chara.data.Values)
                                {
                                    if (MasterManager.Instance.charaMaster.FindData(chara.MChara.id).Rarity == compareValue && chara.MChara.cardType == CardType.Character && !deckData.Contains(chara.id))
                                    {
                                        limitedIds.Add(chara.charaId);
                                    }
                                }
                            }
                            else
                            {
                                // フレンドの該当レアリティを全て選択不可に
                                foreach (CharaV2FriendLend friend in Arguments.FriendList)
                                {
                                    long charaId = friend.mCharaId;
                                    if (MasterManager.Instance.charaMaster.FindData(charaId).Rarity== compareValue)
                                    {
                                        limitedIds.Add(charaId);
                                    }
                                }
                            }
                        }
                        // 編成制限のメッセージを表示
                        textCauntion.text = limit.description;
                    }

                    // トレーニングキャラ
                    friendCharacterScroll.TrainingCharacterId = trainingCharacterId;
                    // トレーニングId
                    friendCharacterScroll.TrainingScenarioId = Arguments.TrainingScenarioId;
                    // 選択不可
                    friendCharacterScroll.SetDisableCharacterParentIds(disableIds);
                    // 制限キャラ
                    friendCharacterScroll.SetLimitedCharacterIds(limitedIds);
                    // リスト登録
                    friendCharacterScroll.SetFriendList(charaList);
                    // スクロール更新
                    friendCharacterScroll.Refresh();
                    
                    targetScroll = friendCharacterScroll;

                    break;
                }
                
                case TrainingDeckMemberType.Support:
                {
                    // 表示切り替え
                    userCharacterScroll.gameObject.SetActive(true);
                    userSpecialCharacterScroll.gameObject.SetActive(false);
                    userExtraCharacterScroll.gameObject.SetActive(false);
                    friendCharacterScroll.gameObject.SetActive(false);
                    userAdviserScroll.gameObject.SetActive(false);
                    
                    // 選択中
                    userCharacterScroll.SetSelectedCharacterIds(deckData.GetMemberIds(DeckSlotCardType.Support));
                    userCharacterScroll.SetFixedCharacterIds(deckData.GetMemberIds(DeckSlotCardType.Support));
                    // フレンドのキャラを選択不可に
                    if(deckData.Friend != null)
                    {
                        disableIds.Add( CharacterUtility.CharIdToParentId(deckData.Friend.mCharaId) );
                    }
                    
                    // 編成制限取得
                    long deckFormatId = MasterManager.Instance.trainingScenarioMaster.FindData(Arguments.TrainingScenarioId).mDeckFormatId;
                    DeckFormatConditionMasterObject limit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.RarityFour, deckFormatId);
                    
                    bool hasLimit = limit != null;
                    limitationCauntion.SetActive(hasLimit);
                    if(hasLimit)
                    {
                        long compareValue = long.Parse(limit.compareValue);
                        
                        // 現在の対象レアリティが何体編成されているか取得する
                        int count = DeckUtility.GetRareLimitCount(deckData, DeckSlotCardType.Support, limit.operatorType, compareValue, Arguments.SupportCharacterDeckSelectedData.Order);
                        // 育成対象が対象レアリティだった場合はカウントアップ
                        if (UserDataManager.Instance.chara.data[Arguments.TrainingUCharId].MChara.mRarityId == compareValue)
                        {
                            count++;
                        }
                        // フレンドが対象レアリティだった場合はカウントアップ
                        if (deckData.Friend != null)
                        {
                            long rarityId = MasterManager.Instance.charaMaster.FindData(deckData.Friend.mCharaId).mRarityId;
                            if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue)
                            {
                                count++;
                            }
                        }

                        // 編成制限に達していたら選択不可にする
                        if(count >= limit.charaCount)
                        {
                            // 所持キャラの該当レアリティを全て選択不可に
                            foreach (UserDataChara chara in UserDataManager.Instance.chara.data.Values)
                            {
                                long rarityId = chara.MChara.mRarityId;
                                if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue && 
                                    chara.MChara.cardType == CardType.Character && 
                                    !deckData.Contains(chara.id))
                                {
                                    limitedIds.Add(chara.charaId);
                                }
                            }
                        }
                        // 編成制限のメッセージを表示
                        textCauntion.text = limit.description;
                    }
                    
                    // トレーニングキャラ
                    userCharacterScroll.TrainingCharacterId = trainingCharacterId;
                    // トレーニングId
                    userCharacterScroll.TrainingScenarioId = Arguments.TrainingScenarioId;
                    // 選択不可
                    userCharacterScroll.SetDisableCharacterParentIds(disableIds);
                    // 制限キャラ
                    userCharacterScroll.SetLimitedCharacterIds(limitedIds);
                    // データ設定
                    userCharacterScroll.SetUserCharacterList();
                    userCharacterScroll.SetCharacterList();
                    // スクロール更新
                    userCharacterScroll.Refresh();
                    
                    targetScroll = userCharacterScroll;
                    
                    break;
                }
                
                case TrainingDeckMemberType.SpecialSupport:
                {
                    // 表示切り替え
                    userCharacterScroll.gameObject.SetActive(false);
                    friendCharacterScroll.gameObject.SetActive(false);
                    userAdviserScroll.gameObject.SetActive(false);

                    // フレンドのキャラを選択不可に
                    if(deckData.Friend != null)
                    {
                        disableIds.Add( CharacterUtility.CharIdToParentId(deckData.Friend.mCharaId) );
                    }
                    
                    // 編成制限取得
                    long deckFormatId = MasterManager.Instance.trainingScenarioMaster.FindData(Arguments.TrainingScenarioId).mDeckFormatId;
                    DeckFormatConditionMasterObject limit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.UR, deckFormatId);
                        
                    bool hasLimit = limit != null;
                    limitationCauntion.SetActive(hasLimit);
                    if(hasLimit)
                    {
                        long compareValue = MasterManager.Instance.rarityMaster.FindData(long.Parse(limit.compareValue)).value;
                        
                        // 現在の対象レアリティが何体編成されているか取得する
                        int count = DeckUtility.GetSpecialSupportLimitCount(deckData, limit.operatorType, compareValue, Arguments.SupportCharacterDeckSelectedData.Order);

                        // 編成制限に達していたら選択不可にする
                        if(count >= limit.charaCount)
                        {
                            // 所持キャラの該当レアリティを全て選択不可に
                            foreach (UserDataChara chara in UserDataManager.Instance.chara.data.Values)
                            {
                                long rarityId = chara.MChara.mRarityId;
                                if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue &&
                                    chara.MChara.cardType == CardType.SpecialSupportCharacter &&
                                    !deckData.Contains(chara.id))
                                { 
                                    limitedIds.Add(chara.charaId);
                                }
                            }
                        }
                        // 編成制限のメッセージを表示
                        textCauntion.text = limit.description;
                    }
                    
                    // 選択中
                    if(userExtraCharacterScroll != null && Arguments.IsExtraCharacter)
                    {
                        userExtraCharacterScroll.gameObject.SetActive(true);
                        userSpecialCharacterScroll.gameObject.SetActive(false);
                        userExtraCharacterScroll.SetSelectedCharacterIds(deckData.GetExMemberIds(DeckSlotCardType.SpecialSupport));
                        userExtraCharacterScroll.SetFixedCharacterIds(deckData.GetExMemberIds(DeckSlotCardType.SpecialSupport));
                        // データ設定
                        userExtraCharacterScroll.SetUserExtraCharacterList();
                        // トレーニングId
                        userExtraCharacterScroll.TrainingScenarioId = Arguments.TrainingScenarioId;
                        // 選択不可
                        userExtraCharacterScroll.SetDisableCharacterParentIds(disableIds);
                        // 制限キャラ
                        userExtraCharacterScroll.SetLimitedCharacterIds(limitedIds);

                        userExtraCharacterScroll.SetCharacterList();
                        // スクロール更新
                        userExtraCharacterScroll.Refresh();
                        
                        targetScroll = userExtraCharacterScroll;
                    }
                    else
                    {
                        userSpecialCharacterScroll.gameObject.SetActive(true);
                        userExtraCharacterScroll.gameObject.SetActive(false);
                        userSpecialCharacterScroll.SetSelectedCharacterIds(deckData.GetMemberIds(DeckSlotCardType.SpecialSupport));
                        userSpecialCharacterScroll.SetFixedCharacterIds(deckData.GetMemberIds(DeckSlotCardType.SpecialSupport));
                        // データ設定
                        userSpecialCharacterScroll.SetUserNonExtraCharacterList();
                    
                        // トレーニングId
                        userSpecialCharacterScroll.TrainingScenarioId = Arguments.TrainingScenarioId;
                        // 選択不可
                        userSpecialCharacterScroll.SetDisableCharacterParentIds(disableIds);
                        // 制限キャラ
                        userSpecialCharacterScroll.SetLimitedCharacterIds(limitedIds);

                        userSpecialCharacterScroll.SetCharacterList();
                        // スクロール更新
                        userSpecialCharacterScroll.Refresh();
                        
                        targetScroll = userSpecialCharacterScroll;
                        
                    }
                    break;
                }
                
                // アドバイザー
                case TrainingDeckMemberType.Adviser:
                {
                    // 表示切り替え
                    userCharacterScroll.gameObject.SetActive(false);
                    friendCharacterScroll.gameObject.SetActive(false);
                    userExtraCharacterScroll.gameObject.SetActive(false);
                    userSpecialCharacterScroll.gameObject.SetActive(false);
                    userAdviserScroll.gameObject.SetActive(true);
                    
                    limitationCauntion.SetActive(false);

                    // 選択中のキャラ
                    long[] selectedCharacterIds = deckData.GetMemberIds(DeckSlotCardType.Adviser);
                    userAdviserScroll.SetSelectedCharacterIds(selectedCharacterIds);
                    userAdviserScroll.SetFixedCharacterIds(selectedCharacterIds);
                    
                    // フレンドのキャラを選択不可に
                    if(deckData.Friend != null)
                    {
                        disableIds.Add( CharacterUtility.CharIdToParentId(deckData.Friend.mCharaId));
                    }
                    
                    // トレーニング対象のキャラ
                    userAdviserScroll.TrainingCharacterId = trainingCharacterId;
                    // トレーニングId
                    userAdviserScroll.TrainingScenarioId = Arguments.TrainingScenarioId;
                    // 選択不可
                    userAdviserScroll.SetDisableCharacterParentIds(disableIds);
                    // 制限キャラ
                    userAdviserScroll.SetLimitedCharacterIds(limitedIds);
                    // データ設定
                    userAdviserScroll.SetUserCharacterList();
                    userAdviserScroll.SetCharacterList();
                    // スクロール更新
                    userAdviserScroll.Refresh();
                    
                    targetScroll = userAdviserScroll;
                    
                    break;
                }
                
            }
            
            // 選択中のIndex
            int selectedIndex = -1;
            long checkId = selectedCharacterId >= 0 ? selectedCharacterId : Arguments.SupportCharacterId;
            for(int i=0;i<targetScroll.ItemList.Count;i++)
            {
                if(targetScroll.ItemList[i].UserCharacterId == checkId)
                {
                    selectedIndex = i;
                    break;
                }
            }
                    
            // 初期選択
            if(selectedIndex >= 0)
            {
                targetScroll.Scroll.SelectItem(selectedIndex);
                OnSelectedCharacter(targetScroll.ItemList[selectedIndex], false);
                targetScroll.Scroll.ScrollToItemIndex(selectedIndex);
                statusView.gameObject.SetActive(true);
            }
            else
            {
                SelectFirstCharacter();
            }
            
            userCharacterScroll.OnSwipeDetailModal = (data) =>
            {
                int index = userCharacterScroll.ItemListSrc.ToList().IndexOf(data);
                if (index >= 0) userCharacterScroll.Scroll.SelectItem(index);
                OnSelectedCharacter(data);
            };
            
            userSpecialCharacterScroll.OnSwipeDetailModal = (data) =>
            {
                int index = userSpecialCharacterScroll.ItemListSrc.ToList().IndexOf(data);
                if (index >= 0) userSpecialCharacterScroll.Scroll.SelectItem(index);
                OnSelectedCharacter(data);
            };
            
            userExtraCharacterScroll.OnSwipeDetailModal = (data) =>
            {
                int index = userExtraCharacterScroll.ItemListSrc.ToList().IndexOf(data);
                if (index >= 0) userExtraCharacterScroll.Scroll.SelectItem(index);
                OnSelectedCharacter(data);
            };
            
            userAdviserScroll.OnSwipeDetailModal = (data) =>
            {
                int index = userAdviserScroll.ItemListSrc.ToList().IndexOf(data);
                if (index >= 0) userAdviserScroll.Scroll.SelectItem(index);
                OnSelectedCharacter(data);
            };
            
            friendCharacterScroll.OnSwipeDetailModal = (data) =>
            {
                int index = friendCharacterScroll.ItemListSrc.ToList().IndexOf(data);
                if (index >= 0) friendCharacterScroll.Scroll.SelectItem(index);
                OnSelectedCharacter(data);
            };
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            selectedCharacterId = TrainingSupportDeckSelectPage.None;
            Initialize();
        }
        
        private void OnSortFilter()
        {
            // 最初のキャラを選択
            SelectFirstCharacter();
        }
        
        private void OnSelectedCharacter(CharacterScrollData data)
        {
            OnSelectedCharacter(data, true);
        }  
        
        private void OnSelectedCharacter(CharacterScrollData data, bool isShowNotification)
        {
            selectedCharacterId = data.UserCharacterId;
            currentScrollData = data;
            // 説明表示
            if(isShowNotification && string.IsNullOrEmpty(data.Description) == false)
            {
                description.ShowNotification(data.Description);
            }

            // ビューの更新
            switch(Arguments.CharacterMemberType)
            {
                case TrainingDeckMemberType.Friend:
                {
                    statusView.SetCharacter(data.CharacterId, data.CharacterLv, data.LiberationLv, Arguments.TrainingScenarioId);
                    // 次へボタン
                    nextButton.interactable = data.HasOption(CharacterScrollDataOptions.Disable) == false;
                    break;
                }
                
                case TrainingDeckMemberType.SpecialSupport:
                case TrainingDeckMemberType.Support:
                case TrainingDeckMemberType.Adviser:
                {
                    statusView.SetUserCharacter(data.UserCharacterId, Arguments.TrainingScenarioId);
                    // 次へボタン
                    nextButton.interactable = data.HasOption(CharacterScrollDataOptions.Disable) == false;
                    break;
                }
            }
        }
        
        private void OnEnable()
        {
            if(Arguments == null)return;
            DeckData deck = Arguments.DeckList.GetDeck(Arguments.PartyNumber);
            TrainingDeckUtility.SetCurrentTrainingDeck( new TrainingDeckUtility.DeckMember(CharacterUtility.UserCharIdToMCharId(Arguments.TrainingUCharId), deck) );
        }

        protected override void OnClosed()
        {
            // 通知表示を切る
            description.OnShowComplete();
            base.OnClosed();
        }

        private CharacterScrollData currentScrollData;
        public void OpenDetail()
        {
            switch (Arguments.CharacterMemberType)
            {
                case TrainingDeckMemberType.Support:
                    BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail,
                        new BaseCharaDetailModalParams(currentScrollData.SwipeableParams,  false, false, titleStringKey: DetailTitleKey, Arguments.TrainingScenarioId, true));
                    break;
                case TrainingDeckMemberType.SpecialSupport:
                    SpecialSupportCardDetailModalWindow.Open(ModalType.SpecialSupportCardDetail,
                        new BaseCharaDetailModalParams(currentScrollData.SwipeableParams, false,　false,
                            titleStringKey: DetailTitleKey, -1, true));
                    break;
                case TrainingDeckMemberType.Friend:
                    BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail,
                        new BaseCharaDetailModalParams(currentScrollData.SwipeableParams, false, false, DetailTitleKey, Arguments.TrainingScenarioId, Arguments.IsFriendSlotSelectMyChara));
                    break;
                case TrainingDeckMemberType.Adviser:
                    BaseCharacterDetailModal.Open(ModalType.AdviserDetail, 
                        new BaseCharaDetailModalParams(currentScrollData.SwipeableParams, false, false, DetailTitleKey, Arguments.TrainingScenarioId, true));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }

        public void OnClickFriendListSwitchButton()
        {
            Arguments.ChangeFriendSelectedUserChara(!Arguments.IsFriendSlotSelectMyChara);
            SetSwitchButtonText();
            Initialize();
        }

        private void SetSwitchButtonText()
        {
            if (Arguments.IsFriendSlotSelectMyChara)
            {
                switchListButtonText.text = StringValueAssetLoader.Instance["training.chara_select.friend_chara"];
            }
            else
            {
                switchListButtonText.text = StringValueAssetLoader.Instance["training.chara_select.user_chara"];
            }
        }
    }
}