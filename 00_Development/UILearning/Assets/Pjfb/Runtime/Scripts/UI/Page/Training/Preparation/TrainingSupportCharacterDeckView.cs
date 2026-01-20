using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

using System;
using Pjfb.Character;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingSupportCharacterDeckView : TrainingSupportDeckView
    {
        [SerializeField]
        private TrainingSupportCharacterDeckCharacterView[] characters = null;
        [SerializeField]
        private DeckLockCharacterView specialSupportCharacter = null;
        [SerializeField]
        private Transform specialSupportCharacterRoot = null;
        [SerializeField]
        private DeckLockCharacterView extraSupportCharacter = null;
        [SerializeField]
        private Transform extraSupportCharacterRoot = null;
        [SerializeField]
        private DeckLockCharacterView adviserCharacter = null;
        [SerializeField]
        private Transform adviserCharacterRoot = null;
        [SerializeField]
        private TrainingSupportCharacterDeckCharacterView friendCharacter = null;
        [SerializeField]
        private ScrollRect deckScrollRect = null;
        
        private List<DeckLockCharacterView> specialSupportCharacters = new ();
        private List<DeckLockCharacterView> extraSupportCharacters = new ();
        private List<DeckLockCharacterView> adviserCharacters = new ();
        
        private bool isInitialized = false;
        
        /// <summary>ビューの初期化</summary>
        private void Initialize()
        {
            long[] index = DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Support);
            // 各ビューの初期化
            for(int i=0;i<characters.Length;i++)
            {
                // デッキ内の位置
                characters[i].SetOrder((int)index[i]);
                // 選択時のコールバック
                characters[i].SetOnSelected(OnSelectedCharacter);
            }
            
            index = DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Adviser, true);
            // 各ビューの初期化
            for(int i = 0; i < index.Length; i++)
            {
                // ビューの生成
                adviserCharacters.Add(Instantiate(adviserCharacter, adviserCharacterRoot));
                adviserCharacters[i].gameObject.SetActive(true);
                // デッキ内の位置
                adviserCharacters[i].SetOrder((int)index[i]);
                // 選択時のコールバック
                adviserCharacters[i].SetOnSelected(OnSelectedAdviser);
                // ロック番号
                adviserCharacters[i].SetLockNumber((int)SystemUnlockDataManager.SystemUnlockNumber.TrainingAdviser + i);
            }
            
            index = DeckUtility.GetSpecialSupportCharacterIndex(DeckType.Training, true);
            
            // 各ビューの初期化
            for(int i=0;i<index.Length;i++)
            {
                // ビューの生成
                specialSupportCharacters.Add(Instantiate(specialSupportCharacter, specialSupportCharacterRoot));
                specialSupportCharacters[i].gameObject.SetActive(true);
                // デッキ内の位置
                specialSupportCharacters[i].SetOrder((int)index[i]);
                // 選択時のコールバック
                specialSupportCharacters[i].SetOnSelected(OnSelectedSpecialSupportCharacter);
                // ロック番号
                specialSupportCharacters[i].SetLockNumber( TrainingDeckUtility.SpecialSupportSystemLockId + i);
            }
            
            index = DeckUtility.GetExtraSupportCharacterIndex(DeckType.Training, true);
            // 各ビューの初期化
            for(int i=0;i<index.Length;i++)
            {
                // ビューの生成
                extraSupportCharacters.Add(Instantiate(extraSupportCharacter, extraSupportCharacterRoot));
                extraSupportCharacters[i].gameObject.SetActive(true);
                // デッキ内の位置
                extraSupportCharacters[i].SetOrder((int)index[i]);
                // 選択時のコールバック
                extraSupportCharacters[i].SetOnSelected(OnSelectedExtraSupportCharacter);
                // ロック番号
                extraSupportCharacters[i].SetLockNumber( TrainingDeckUtility.ExSupportSystemLockId + i);
            }
            
            // フレンドの設定
            friendCharacter.SetOrder(-1);
            friendCharacter.SetOnSelected(OnSelectedFriend);
        }
        
        private void OnSelectedCharacter(int order)
        {
            CallOnSelect(order, TrainingDeckMemberType.Support, false);
        }
        
        private void OnSelectedSpecialSupportCharacter(int order)
        {
            CallOnSelect(order, TrainingDeckMemberType.SpecialSupport, false);
        }
        
        private void OnSelectedExtraSupportCharacter(int order)
        {
            CallOnSelect(order, TrainingDeckMemberType.SpecialSupport, true);
        }
        
        private void OnSelectedAdviser(int order)
        {
            CallOnSelect(order, TrainingDeckMemberType.Adviser, false);
        }
        
        private void OnSelectedFriend(int order)
        {
            CallOnSelect(order, TrainingDeckMemberType.Friend, false);
        }


        /// <summary>タイプ数表示更新</summary>
        protected override void OnUpdateTypeCount(Dictionary<CharacterType, int> typeCount)
        {
            // サポート枠
            for(int i=0;i<characters.Length;i++)
            {
                typeCount[characters[i].CharType]++;
            }
            // スペシャルサポート
            for(int i=0;i<specialSupportCharacters.Count;i++)
            {
                typeCount[specialSupportCharacters[i].CharType]++;
            }
            
            // スペシャルサポート
            for(int i=0;i<extraSupportCharacters.Count;i++)
            {
                typeCount[extraSupportCharacters[i].CharType]++;
            }
            
            // アドバイザー
            for(int i = 0;i < adviserCharacters.Count; i++)
            {
                typeCount[adviserCharacters[i].CharType]++;
            }
            
            // フレンド
            typeCount[friendCharacter.CharType]++;
        }

        protected override void OnUpdateView()
        {
            if (isInitialized == false)
            {
                isInitialized = true;
                Initialize();
            }
            
            Dictionary<long, long> characterTupleDic = new Dictionary<long, long>();
            List<TrainingCharacterData> characterDataList = new List<TrainingCharacterData>();
            // 設定中のキャラId
            long trainingParentCharId = CharacterUtility.UserCharIdToParentId(arguments.TrainingUCharId);
            
            UserDataChara trainingChar = UserDataManager.Instance.chara.Find(arguments.TrainingUCharId);
            
            
            characterDataList.Add( new TrainingCharacterData( trainingChar.MChara.id, trainingChar.level, trainingChar.newLiberationLevel, trainingChar.id) );
            
            characterTupleDic[trainingChar.MChara.id] = trainingChar.level;
            
            // サポート枠
            long[] supportIds = DeckData.GetMemberIds(DeckSlotCardType.Support);
            
            List<CharacterDetailData> characterDetailOrderList = new();
            List<CharacterDetailData> specialSupportCardDetailOrderList = new();
            List<CharacterDetailData> adviserCardDetailOrderList = new();
            
            // 編成制限取得
            long deckFormatId = MasterManager.Instance.trainingScenarioMaster.FindData(arguments.TrainingScenarioId).mDeckFormatId;
            DeckFormatConditionMasterObject limit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.RarityFour, deckFormatId);
            
            int limitCharaCount = 0;
            long compareValue = 0;
            bool hasLimit = limit != null;
            if (hasLimit)
            {
                compareValue = MasterManager.Instance.rarityMaster.FindData(long.Parse(limit.compareValue)).value;
                
                // 現在の対象レアリティが何体編成されているか取得する
                limitCharaCount = DeckUtility.GetRareLimitCount(supportIds, limit.operatorType, compareValue);
                // 育成対象が対象レアリティだった場合はカウントアップ
                if (UserDataManager.Instance.chara.data[arguments.TrainingUCharId].MChara.mRarityId == compareValue)
                {
                    limitCharaCount++;
                }
                // フレンドが対象レアリティだった場合はカウントアップ
                if (DeckData.Friend != null && DeckData.Friend.mCharaId != DeckUtility.EmptyDeckSlotId)
                {
                    long rarityId = MasterManager.Instance.charaMaster.FindData(DeckData.Friend.mCharaId).mRarityId;
                    if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue)
                    {
                        limitCharaCount++;
                    }
                }
            }
            
            int charaScrollIndex = 0;
            int specialSupportCardScrollIndex = 0;
            int adviserCardScrollIndex = 0;
            
            for(int i=0;i<characters.Length;i++)
            {
                long id = supportIds[i];
                UserDataChara chara = UserDataManager.Instance.chara.Find(id);
                
                // 編成不可
                characters[i].SetImpossible(id != DeckUtility.EmptyDeckSlotId && trainingParentCharId == CharacterUtility.UserCharIdToParentId(id) );
                // 特攻
                characters[i].SetSpecialAttack(id != DeckUtility.EmptyDeckSlotId && CharacterUtility.IsTrainingScenarioSpAttackCharacter(chara.MChara.id, chara.level, arguments.TrainingScenarioId));
                // UCharId
                characters[i].SetUserCharacterId(id);
                
                if (chara is not null)
                {
                    characters[i].SetDetailOrderList(new SwipeableParams<CharacterDetailData>(characterDetailOrderList, charaScrollIndex++, null));
                    characterDetailOrderList.Add(new CharacterDetailData(chara));   

                    // 重複しないようにリストを作成
                    characterTupleDic[chara.MChara.id] = chara.level;
                    
                    // キャラデータ
                    characterDataList.Add( new TrainingCharacterData( chara.MChara.id, chara.level, chara.newLiberationLevel, chara.id) );
                    
                }
                
                // 編成制限
                if(hasLimit && limitCharaCount > limit.charaCount)
                {
                    long rarityId = chara.MChara.mRarityId;

                    if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue)
                    {
                        characters[i].SetImpossible(true);
                    }
                }
            }
            
            // スペシャルサポート
            long[] specialSupportIds = DeckData.GetMemberIds(DeckSlotCardType.SpecialSupport);
            for(int i=0;i<specialSupportCharacters.Count;i++)
            {
                long id = specialSupportIds[i];
                UserDataChara chara = UserDataManager.Instance.chara.Find(id);
                // 編成不可
                specialSupportCharacters[i].SetImpossible(id != DeckUtility.EmptyDeckSlotId && trainingParentCharId == CharacterUtility.UserCharIdToParentId(id) );
                // 特攻
                specialSupportCharacters[i].SetSpecialAttack(id != DeckUtility.EmptyDeckSlotId && CharacterUtility.IsTrainingScenarioSpAttackCharacter(chara.MChara.id, chara.level, arguments.TrainingScenarioId));
                // UCharId
                specialSupportCharacters[i].SetUserCharacterId( id );
                // ロック状態
                specialSupportCharacters[i].SetLockState( TrainingDeckUtility.IsSPSupportSystemLock(i) );
                                
                if (chara is not null)
                {
                    specialSupportCharacters[i]
                        .SetDetailOrderList(new SwipeableParams<CharacterDetailData>(specialSupportCardDetailOrderList,
                            specialSupportCardScrollIndex++, null));
                    specialSupportCardDetailOrderList.Add(new CharacterDetailData(chara));   
                    
                    // 重複しないようにリストを作成
                    characterTupleDic[chara.MChara.id] = chara.level;
                    
                    // キャラデータ
                    characterDataList.Add( new TrainingCharacterData( chara.MChara.id, chara.level, chara.newLiberationLevel, chara.id) );
                }
            }
            
            
            // スペシャルサポート
            long[] extraSupportIds = DeckData.GetExMemberIds(DeckSlotCardType.SpecialSupport);
            for(int i=0;i<extraSupportCharacters.Count;i++)
            {
                long id = extraSupportIds[i];
                UserDataChara chara = UserDataManager.Instance.chara.Find(id);
                // 編成不可
                extraSupportCharacters[i].SetImpossible(id != DeckUtility.EmptyDeckSlotId && trainingParentCharId == CharacterUtility.UserCharIdToParentId(id) );
                // 特攻
                extraSupportCharacters[i].SetSpecialAttack(id != DeckUtility.EmptyDeckSlotId && CharacterUtility.IsTrainingScenarioSpAttackCharacter(chara.MChara.id, chara.level, arguments.TrainingScenarioId));
                // UCharId
                extraSupportCharacters[i].SetUserCharacterId( id );
                // ロック状態
                extraSupportCharacters[i].SetLockState( TrainingDeckUtility.IsExSupportSystemLock(i) );
                                
                if (chara is not null)
                {
                    extraSupportCharacters[i]
                        .SetDetailOrderList(new SwipeableParams<CharacterDetailData>(specialSupportCardDetailOrderList,
                            specialSupportCardScrollIndex++, null));
                    specialSupportCardDetailOrderList.Add(new CharacterDetailData(chara));   
                    
                    // 重複しないようにリストを作成
                    characterTupleDic[chara.MChara.id] = chara.level;
                    
                    // キャラデータ
                    characterDataList.Add( new TrainingCharacterData( chara.MChara.id, chara.level, chara.newLiberationLevel, chara.id) );
                }
            }
            
            // アドバイザー
            long[] adviserIds = DeckData.GetMemberIds(DeckSlotCardType.Adviser);
            for(int i = 0; i < adviserCharacters.Count; i++)
            {
                long id = adviserIds[i];
                UserDataChara chara = UserDataManager.Instance.chara.Find(id);
                // 編成不可
                adviserCharacters[i].SetImpossible(id != DeckUtility.EmptyDeckSlotId && trainingParentCharId == CharacterUtility.UserCharIdToParentId(id));
                // 特攻
                adviserCharacters[i].SetSpecialAttack(id != DeckUtility.EmptyDeckSlotId && CharacterUtility.IsTrainingScenarioSpAttackCharacter(chara.MChara.id, chara.level, arguments.TrainingScenarioId));
                // UCharId
                adviserCharacters[i].SetUserCharacterId(id);
                // ロック状態
                adviserCharacters[i].SetLockState(TrainingDeckUtility.IsAdviserSystemLock(i));
                                
                if (chara != null)
                {
                    adviserCharacters[i].SetDetailOrderList(new SwipeableParams<CharacterDetailData>(adviserCardDetailOrderList, adviserCardScrollIndex++, null));
                    adviserCardDetailOrderList.Add(new CharacterDetailData(chara));   
                    
                    // 重複しないようにリストを作成
                    characterTupleDic[chara.MChara.id] = chara.level;
                    
                    // キャラデータ
                    characterDataList.Add(new TrainingCharacterData(chara.MChara.id, chara.level, chara.newLiberationLevel, chara.id));
                }
            }
            
            // フレンド枠
            if(DeckData.Friend == null)
            {
                friendCharacter.SetImpossible(false);
                friendCharacter.SetSpecialAttack(false);
                friendCharacter.SetFriend(false);
                friendCharacter.SetFollowIcon(TrainingUtility.FriendFollowType.None);
                friendCharacter.SetCharacterId(DeckUtility.EmptyDeckSlotId, 0, 0);
            }
            else
            {
                // メンバーId
                long[] memberIds = DeckData.GetMemberIds();
                // フレンドのキャラId
                long friendParentId = DeckData.Friend.mCharaId == DeckUtility.EmptyDeckSlotId ? DeckUtility.EmptyDeckSlotId : CharacterUtility.CharIdToParentId(DeckData.Friend.mCharaId);
                // 選択可能チェック
                bool impossible = trainingParentCharId == friendParentId;
                
                // メンバーと同じキャラは編成不可
                if(impossible == false)
                {
                    for(int i=0;i<memberIds.Length;i++)
                    {
                        long id = memberIds[i];
                        if(id == DeckUtility.EmptyDeckSlotId)continue;
                        if(DeckData.GetMemberType(i) == DeckMemberType.UEquipment)continue;
                        
                        if(friendParentId == CharacterUtility.UserCharIdToParentId(id))
                        {
                            impossible = true;
                            break;
                        }
                    }
                }

                // 自分のキャラだった場合最新の状態を取得
                if (DeckData.Friend.uMasterId == UserDataManager.Instance.user.uMasterId)
                {
                    DeckData.Friend = TrainingDeckUtility.ConvertUCharaToFriendLend(UserDataManager.Instance.chara.Find(DeckData.Friend.id));
                    friendCharacter.SetFriend(false);
                }
                else
                {
                    friendCharacter.SetFriend(true);
                }
                
                // 編成不可
                friendCharacter.SetImpossible( impossible );
                // 編成制限
                if(hasLimit && limitCharaCount > limit.charaCount)
                {
                    long rarityId = MasterManager.Instance.charaMaster.FindData(DeckData.Friend.mCharaId).mRarityId;
                    if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue)
                    {
                        friendCharacter.SetImpossible(true);
                    }
                }
                // 特攻
                friendCharacter.SetSpecialAttack(DeckData.Friend.mCharaId != DeckUtility.EmptyDeckSlotId && CharacterUtility.IsTrainingScenarioSpAttackCharacter(DeckData.Friend.mCharaId, DeckData.Friend.level, arguments.TrainingScenarioId));
                // フォロー状態を表示
                friendCharacter.SetFollowIcon((TrainingUtility.FriendFollowType)DeckData.Friend.relationType);
                
                friendCharacter.SetCharacterId(DeckData.Friend.mCharaId, DeckData.Friend.level, DeckData.Friend.newLiberationLevel);
                
                friendCharacter.SetDetailOrderList(new SwipeableParams<CharacterDetailData>(characterDetailOrderList, charaScrollIndex++, null));
                characterDetailOrderList.Add(new CharacterDetailData(DeckData.Friend.id, DeckData.Friend.mCharaId, DeckData.Friend.level, DeckData.Friend.newLiberationLevel));   

                // 重複しないようにリストを作成
                characterTupleDic[DeckData.Friend.mCharaId] = DeckData.Friend.level;
                
                // キャラデータ
                characterDataList.Add( new TrainingCharacterData( DeckData.Friend.mCharaId, DeckData.Friend.level, DeckData.Friend.newLiberationLevel, -1) );
            
            }

            characterTupleDictionary = characterTupleDic;
            
            // キャラ配列
            characterDatas = characterDataList.ToArray();

            // スクロール位置リセット
            ResetDeckScrollPosition();
        }

        /// <summary>
        /// サポート編成のページ切り替え時にRectTransformY座標位置リセット
        /// </summary>
        private void ResetDeckScrollPosition()
        {
            deckScrollRect.verticalNormalizedPosition = 1.0f;
        }
    }
}
