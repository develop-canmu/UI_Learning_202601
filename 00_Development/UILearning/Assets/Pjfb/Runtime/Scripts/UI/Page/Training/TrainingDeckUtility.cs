using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using Pjfb.UserData;
using System.Linq;
using Pjfb.SystemUnlock;

namespace Pjfb
{
    
    // トレーニングデッキの編成枠のタイプ
    public enum TrainingDeckSlotType
    {
        // 育成対象
        GrowthTarget,
        // サポート選手
        SupportCharacter,
        // サポートカード
        SupportCard,
        // EXサポートカード
        ExSupportCard,
        // サポート器具
        SupportEquipment,
    }
    
    public struct TrainingDeckSlotData
    {
        private TrainingDeckSlotType slotType;
        public TrainingDeckSlotType SlotType{get => slotType;}
        
        // 何枠目か(育成対象選手など枠が固定のものは0が入る)
        private long index;
        public long Index{get => index;}
        
        public TrainingDeckSlotData(TrainingDeckSlotType slotType, long index)
        {
            this.slotType = slotType;
            this.index = index;
        }
    }
    
    public static class TrainingDeckUtility
    {
        /// <summary>
        /// スペシャルサポートキャラの１枠の目のId
        /// ２枠目以降は連番で定義されている
        /// </summary>
        public const int SpecialSupportSystemLockId = 421001;
        
        /// <summary>
        /// サポート器具の開放
        /// </summary>
        public const int SupportEquipmentSystemLockId = 441001;
        
        /// <summary>
        /// Exサポートキャラの１枠の目のId
        /// ２枠目以降は連番で定義されている
        /// </summary>
        public const int ExSupportSystemLockId = 451001;
        
        private struct SortData
        {
            public CharaV2FriendLend charData;
            public BigValue value;
            
            public SortData(CharaV2FriendLend charData, BigValue value)
            {
                this.charData = charData;
                this.value = value;
            }
        }
        
        public enum RecommendType
        {
            Lv, Stamina, Speed, Physical, Technique, Intelligence, Kick
        }
        
        private static SortData[] GetCardStatusSortData(RecommendType type, List<CharaV2FriendLend> datas)
        {
            // 練習カードのステータス
            SortData[] sortDatas = new SortData[datas.Count];
            // キャラが保有する練習カード
            for(int i=0;i<datas.Count;i++)
            {
                // data
                CharaV2FriendLend data = datas[i];
                // 保有する練習カード
                long[] cardIds = MasterManager.Instance.trainingCardCharaMaster.FindPracticeCardIds( data.mCharaId );
                // 練習カードのステータスの合計を計算
                CharacterStatus status = new CharacterStatus();
                foreach(long cardId in cardIds)
                {
                    TrainingCardLevelMasterObject mTrainingCardLevel = MasterManager.Instance.trainingCardLevelMaster.FindData(cardId, 1);
                    if(mTrainingCardLevel == null)continue;
                    status += TrainingUtility.GetStatus(mTrainingCardLevel);
                }
                
                // リストに追加
                switch(type)
                {
                    case RecommendType.Stamina:
                        sortDatas[i] = new SortData(data, status.Stamina);
                        break;
                    case RecommendType.Speed:
                        sortDatas[i] = new SortData(data, status.Speed);
                        break;
                    case RecommendType.Physical:
                        sortDatas[i] = new SortData(data, status.Physical);
                        break;
                    case RecommendType.Technique:
                        sortDatas[i] = new SortData(data, status.Technique);
                        break;
                    case RecommendType.Kick:
                        sortDatas[i] = new SortData(data, status.Kick);
                        break;
                    case RecommendType.Intelligence:
                        sortDatas[i] = new SortData(data, status.Intelligence);
                        break;
                    default:
                        sortDatas[i] = new SortData(data, BigValue.Zero);
                        break;
                }
            }
            
            return sortDatas;
        }
        
        private static List<CharaV2FriendLend> GetStatusSortData(RecommendType type, CharacterType trainingCharacterType, List<CharaV2FriendLend> datas)
        {
            // 練習カードのステータス
            SortData[] sortDatas = GetCardStatusSortData(type, datas);
            // ステータス順
            // トレーニングキャラと同じタイプ順
            IEnumerable<SortData> r =
                // 効果値が高い
                sortDatas.OrderByDescending(v=>v.value)
                    // Lv順
                    .ThenByDescending(v=>v.charData.level)
                    // トレーニングキャラとタイプが同じ
                    .ThenByDescending(v=> MasterManager.Instance.charaMaster.FindData(v.charData.mCharaId).charaType == trainingCharacterType ? 1 : 0)
                    // Id順
                    .ThenByDescending(v=> v.charData.mCharaId)
                    // フレンドの選手か
                    .ThenByDescending(v => v.charData.uMasterId != UserDataManager.Instance.user.uMasterId ? 1 : 0);

            // リストに変換
            return r.Select(v=>v.charData).ToList();
        }
        
        private static RecommendType CharacterTypeToRecommendType(CharacterType type)
        {
            switch(type)
            {
                case CharacterType.Stamina:return RecommendType.Stamina;
                case CharacterType.Speed:return RecommendType.Speed;
                case CharacterType.Intelligence:return RecommendType.Intelligence;
                case CharacterType.Kick:return RecommendType.Kick;
                case CharacterType.Physical:return RecommendType.Physical;
                case CharacterType.Technique:return RecommendType.Technique;
            }
            
            return RecommendType.Lv;
        }
        
        private static List<CharaV2FriendLend> GetLvSortData(CharacterType trainingCharacterType, List<CharaV2FriendLend> datas)
        {
            RecommendType statusType = CharacterTypeToRecommendType(trainingCharacterType);
            // 練習カードのステータス
            SortData[] sortDatas = GetCardStatusSortData(statusType, datas);
            
            
            // ステータス順
            // トレーニングキャラと同じタイプ順
            IEnumerable<SortData> r =
                // Lvが高い
                sortDatas.OrderByDescending(v=>v.charData.level)
                    // レア度が高い
                    .ThenByDescending(v=>RarityUtility.GetRarity( v.charData.mCharaId, v.charData.newLiberationLevel))
                    // ステータスが高い順
                    .ThenByDescending(v=>v.value)
                    // Id順
                    .ThenByDescending(v=> v.charData.mCharaId)
                    // フレンドの選手か
                    .ThenByDescending(v => v.charData.uMasterId != UserDataManager.Instance.user.uMasterId ? 1 : 0);
            // リストに変換
            return r.Select(v=>v.charData).ToList();
        }
        
        private static List<UserDataChara> GetSortNoStatusMembers(CardType[] cardTypes)
        {
            // ステータス順
            // トレーニングキャラと同じタイプ順
            IEnumerable<UserDataChara> r =
                UserDataManager.Instance.chara.data.Values
                    .Where(v=>cardTypes.Contains(v.CardType))
                    // Lvが高い
                    .OrderByDescending(v=>v.level)
                    // レア度が高い
                    .ThenByDescending(v=>RarityUtility.GetRarity( v.MChara.id, v.newLiberationLevel))
                    // レアリティId順
                    .ThenByDescending(v=> v.MChara.mRarityId)
                    // Id順
                    .ThenByDescending(v=> v.MChara.id);
            // リストに変換
            return r.ToList();
        }

        /// <summary>レベル順に取得</summary>
        public static long[] GetRecommendMembers(RecommendType type, long trainingUCharId, CharaV2FriendLend[] friendList, long trainingScenarioId, out CharaV2FriendLend friendResult)
        {
            long[] result = new long[DeckUtility.TrainingDeckSlotCount];
            // 全てEmptyで初期化
            for(int i=0;i<result.Length;i++)
            {
                result[i] = DeckUtility.EmptyDeckSlotId;
            }
            
            // トレーニングキャラ
            UserDataChara uTrainingChar = UserDataManager.Instance.chara.Find(trainingUCharId);
            CharaMasterObject mTrainingChar = MasterManager.Instance.charaMaster.FindData(uTrainingChar.charaId);

            // 重複チェック用
            List<long> parentMCharIdList = new List<long>();
            parentMCharIdList.Add(mTrainingChar.parentMCharaId);
            
            // キャラリスト作成
            List<CharaV2FriendLend> charaDataList = new List<CharaV2FriendLend>();
            charaDataList = friendList.ToList();
            foreach (UserDataChara charaData in UserDataManager.Instance.chara.data.Values)
            {
                CharaV2FriendLend data = ConvertUCharaToFriendLend(charaData);
                charaDataList.Add(data);
            }
             
            switch(type)
            {
                case RecommendType.Lv:
                {
                    charaDataList = GetLvSortData(uTrainingChar.MChara.charaType, charaDataList);
                    break;
                }
                
                // ステータス
                default:
                {
                    charaDataList = GetStatusSortData(type, uTrainingChar.MChara.charaType, charaDataList);
                }
                break;
            }
            
            // ステータスを持たないキャラのリスト
            CardType[] noStatusCardTypes = new CardType[]
            {
                CardType.Adviser
            };
            List<UserDataChara> noStatusCharaList = GetSortNoStatusMembers(noStatusCardTypes);
            
            
            // 上位を詰めて返す
            int supportCount = 0;
            int specialCount = 0;
            int extraCount = 0;
            int adviserCount = 0;
            
            long[] supportIndex = DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Support);
            long[] specialSupportIndex = DeckUtility.GetSpecialSupportCharacterIndex(DeckType.Training, false);
            long[] extraSupportIndex = DeckUtility.GetExtraSupportCharacterIndex(DeckType.Training, false);
            long[] adviserIndex = DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Adviser, false);
            
            // 編成制限があれば取得
            long deckFormatId = MasterManager.Instance.trainingScenarioMaster.FindData(trainingScenarioId).mDeckFormatId;
            DeckFormatConditionMasterObject supportLimit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.RarityFour, deckFormatId);
            DeckFormatConditionMasterObject specialSupportLimit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.UR, deckFormatId);
            bool hasSupportLimit = supportLimit != null;
            bool hasSpecialSupportLimit = specialSupportLimit != null;
            int supportLimitCount = 0;
            int specialSupportLimitCount = 0;
            long supportLimitCompareValue = 0;
            long specialSupportLimitCompareValue = 0;
            // 選手編成制限がある場合、育成対象のレアリティをチェックする
            if (hasSupportLimit)
            {
                supportLimitCompareValue = long.Parse(supportLimit.compareValue);
                long rarityId = UserDataManager.Instance.chara.data[trainingUCharId].MChara.mRarityId;
                if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == supportLimitCompareValue)
                {
                    supportLimitCount++;
                }
            }
            
            // サポカ制限がある場合、制限対象のレアリティをキャッシュする
            if (hasSpecialSupportLimit)
            {
                specialSupportLimitCompareValue = MasterManager.Instance.rarityMaster.FindData(long.Parse(specialSupportLimit.compareValue)).value;
            }
            
            for(int i = 0; i < charaDataList.Count; i++)
            {
                // 所持キャラのみ
                if(charaDataList[i].uMasterId != UserDataManager.Instance.user.uMasterId)continue;
   
                CharaV2FriendLend uChar = charaDataList[i];
                // 同じキャラの場合は除外する
                // mChar
                CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(uChar.mCharaId);
                // Idをチェック
                if(parentMCharIdList.Contains(mChar.parentMCharaId))continue;

                switch(mChar.cardType)
                {
                    case CardType.Character:
                        // 枠が埋まっている
                        if(supportCount >= supportIndex.Length)break;
                        // 編成制限がある場合
                        if (hasSupportLimit)
                        {
                            long rarityId = mChar.mRarityId;
                            if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == supportLimitCompareValue)
                            {
                                // 編成制限に達していた場合
                                if(supportLimitCount >= supportLimit.charaCount)continue;
                                supportLimitCount++;
                            }
                        }
                        // 結果に追加
                        result[supportIndex[supportCount++]] = uChar.id;
                        // リストに追加
                        parentMCharIdList.Add(mChar.parentMCharaId);
                        
                        break;
                    
                    case CardType.SpecialSupportCharacter:
                        
                        // 編成制限がある場合
                        if (hasSpecialSupportLimit)
                        {
                            long rarityId = mChar.mRarityId;
                            if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == specialSupportLimitCompareValue)
                            {
                                // 編成制限に達していた場合
                                if(specialSupportLimitCount >= specialSupportLimit.charaCount)continue;
                                specialSupportLimitCount++;
                            }
                        }
                        
                        if(mChar.isExtraSupport)
                        {
                            // 枠が埋まっている
                            if(extraCount >= extraSupportIndex.Length)break;
                            // 結果に追加
                            result[extraSupportIndex[extraCount++]] = uChar.id;
                            // リストに追加
                            parentMCharIdList.Add(mChar.parentMCharaId);
                        }
                        else
                        {
                            // 枠が埋まっている
                            if(specialCount >= specialSupportIndex.Length)break;
                            // 結果に追加
                            result[specialSupportIndex[specialCount++]] = uChar.id;
                            // リストに追加
                            parentMCharIdList.Add(mChar.parentMCharaId);
                        }
                        break;
                    default:
                        break;
                }
                
                // すべての枠が埋まったら終了
                if(supportCount >= supportIndex.Length && specialCount >= specialSupportIndex.Length && extraCount >= extraSupportIndex.Length)break;
            }

            // ステータスを持たないキャラの枠を埋める
            foreach (UserDataChara chara in noStatusCharaList)
            {
                // Idをチェック
                if(parentMCharIdList.Contains(chara.ParentMCharaId)) continue;
                switch (chara.CardType)
                {
                    case CardType.Adviser:
                        // アドバイザー
                        if (adviserCount >= adviserIndex.Length) continue;
                        // 結果に追加
                        result[adviserIndex[adviserCount++]] = chara.id;
                        // リストに追加
                        parentMCharIdList.Add(chara.ParentMCharaId);
                        break;
                    default:
                        break;
                }
                if(adviserCount >= adviserIndex.Length)break;
            }
            
            // 
            friendResult = null;
            
            // フレンド
            foreach(CharaV2FriendLend friend in charaDataList)
            {
                // mChar
                CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(friend.mCharaId);
                
                // キャラじゃなければスキップ
                if(mChar.cardType != CardType.Character)continue;
                
                // 重複チェック
                if(mChar.parentMCharaId == mTrainingChar.parentMCharaId)
                {
                    continue;
                }
                // Idをチェック
                if(parentMCharIdList.Contains(mChar.parentMCharaId))
                {
                    continue;
                }
                // 編成制限がある場合
                if (hasSupportLimit)
                {
                    long rarityId = mChar.mRarityId;
                    if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == supportLimitCompareValue)
                    {
                        // 編成制限に達していた場合
                        if(supportLimitCount >= supportLimit.charaCount)continue;
                    }
                }

                friendResult = friend;
                break;
            }
            
            return result;
        }
        
        /// <summary>スペシャルサポートのロック状態？</summary>
        public static bool IsSPSupportSystemLock(int index)
        {
            int systemNumber = SpecialSupportSystemLockId + index;
            // ユーザーがアンロックしているかチェック
            return UserDataManager.Instance.IsUnlockSystem(systemNumber) == false;
        }
        
        /// <summary>Exサポートのロック状態？</summary>
        public static bool IsExSupportSystemLock(int index)
        {
            int systemNumber = ExSupportSystemLockId + index;
            // ユーザーがアンロックしているかチェック
            return UserDataManager.Instance.IsUnlockSystem(systemNumber) == false;
        }
        
        /// <summary>アドバイザーのロック状態</summary>
        public static bool IsAdviserSystemLock(int index)
        {
            long systemNumber = (long)SystemUnlockDataManager.SystemUnlockNumber.TrainingAdviser + index;
            // ユーザーがアンロックしているかチェック
            return UserDataManager.Instance.IsUnlockSystem(systemNumber) == false;
        }
        
         //// <summary> 対象になるデッキタイプを取得する </summary>
        public static TrainingDeckSlotData GetEnhanceTargetType(DeckType deckType, long slotIndex)
        {
            switch (deckType)
            {
                case DeckType.GrowthTarget:
                {
                    return new TrainingDeckSlotData(TrainingDeckSlotType.GrowthTarget, 0);
                }
                case DeckType.Training:
                {
                    long index;
                    // サポートキャラのIndex番号
                    long[] indexArray = DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Support);
                    // 枠番号がサポートキャラのIndexと一致するならTypeと該当の枠番号のデータを返す
                    if (indexArray.Contains(slotIndex))
                    {
                        index = Array.IndexOf(indexArray, slotIndex) + 1;
                        return new TrainingDeckSlotData(TrainingDeckSlotType.SupportCharacter, index);
                    }

                    // スペシャルサポートのIndex番号を取得
                    indexArray = DeckUtility.GetSpecialSupportCharacterIndex(DeckType.Training, true);
                    if (indexArray.Contains(slotIndex))
                    {
                        index = Array.IndexOf(indexArray, slotIndex) + 1;
                        return new TrainingDeckSlotData(TrainingDeckSlotType.SupportCard, index);
                    }

                    // EXスペシャルサポートのIndex番号を取得
                    indexArray = DeckUtility.GetExtraSupportCharacterIndex(DeckType.Training, true);
                    if (indexArray.Contains(slotIndex))
                    {
                        index = Array.IndexOf(indexArray, slotIndex) + 1;
                        return new TrainingDeckSlotData(TrainingDeckSlotType.ExSupportCard, index);
                    }
                    break;
                }
                case DeckType.SupportFriend:
                {
                    // フレンドの枠を取得
                    long[] indexArray = DeckUtility.GetCharacterIndex(DeckType.SupportFriend, DeckSlotCardType.Support);
                    if (indexArray.Contains(slotIndex))
                    {
                        // フレンドの枠番号はサポートキャラの枠数を加算して表示
                        long index = Array.IndexOf(indexArray, slotIndex) + 1 + DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Support).Length;
                        return new TrainingDeckSlotData(TrainingDeckSlotType.SupportCharacter, index);
                    }
                    break;
                }
                // サポート器具
                case DeckType.SupportEquipment:
                {
                    long[] indexArray = DeckUtility.GetCharacterIndex(DeckType.SupportEquipment, DeckSlotCardType.SupportEquipment);
                    long index = Array.IndexOf(indexArray, slotIndex) + 1;
                    return new TrainingDeckSlotData(TrainingDeckSlotType.SupportEquipment, index);
                } 
            }
            
            // 見つからない時はエラーを出す
            CruFramework.Logger.LogError($"Not Find {deckType} slotIndex : {slotIndex}");
            return new TrainingDeckSlotData(TrainingDeckSlotType.GrowthTarget, 0);
        }
        
        public class DeckMember
        {
            
            private long[] memberIds = null;
            /// <summary>Id</summary>
            public long[] MemberIds{get{return memberIds;}}
            private Dictionary<long, long> memberlevelDictionary = null;
            /// <summary>メンバーのレベル</summary>
            public Dictionary<long, long> MemberLevelDictionary { get { return memberlevelDictionary; } } 

            public DeckMember(long trainingCharacterId, DeckData deckData)
            {
                List<long> list = new List<long>();
                Dictionary<long, long> memberLevel = new Dictionary<long, long>();

                long[] ids = deckData.GetMemberIds();
                
                for(int i=0;i<ids.Length;i++)
                {
                    long id = ids[i];
                    if(id == DeckUtility.EmptyDeckSlotId)continue;
                    
                    switch(deckData.GetMemberType(i))
                    {
                        case DeckMemberType.UChar:
                        case DeckMemberType.UCharaVariable:
                            var mCharaId = CharacterUtility.UserCharIdToMCharId(id);
                            list.Add( mCharaId );
                            memberLevel.TryAdd( mCharaId, UserDataManager.Instance.chara.Find(id).level );
                            break;
                        case DeckMemberType.UEquipment:
                            list.Add( CharacterUtility.UserEquipmentIdToMCharId(id) );
                            break;
                    }
                }

                if(deckData.Friend != null)
                {
                    list.Add( deckData.Friend.mCharaId );
                    memberLevel.TryAdd( deckData.Friend.mCharaId, deckData.Friend.level );
                }
                list.Add(trainingCharacterId);
                memberLevel.TryAdd(trainingCharacterId, UserDataManager.Instance.chara.data.Values.First(u => u.MChara.id == trainingCharacterId).level);
                
                memberIds = list.ToArray();
                memberlevelDictionary = memberLevel;
            }
            
            /// <summary>
            /// 育成選手・サポート選手・選択したフレンド・サポカ・アドバイザーの情報でデッキ情報を初期化
            /// </summary>
            public DeckMember(TrainingCharacterData trainingCharacter, List<TrainingCharacterData> supportCharaMembers, List<TrainingCharacterData> specialSupportCharaMembers)
            {
                var memberLevel = new Dictionary<long, long>();
                
                // トレーニングに関わる選手のIdをリストに追加
                var memberIdList = new List<long>();
                memberIdList.Add(trainingCharacter.MCharId);
                // サポートキャラ(編成した選手とフレンド)
                memberIdList.AddRange(supportCharaMembers.Select(supportMember => supportMember.MCharId));
                // スペシャルキャラクターを追加(サポカとアドバイザー)
                memberIdList.AddRange(specialSupportCharaMembers.Select(supportMember => supportMember.MCharId));
                
                this.memberIds = memberIdList.ToArray();

                // サポートキャラ
                foreach (var supportMember in supportCharaMembers)
                {
                    memberLevel.Add(supportMember.MCharId, supportMember.Lv);
                }
                
                // スペシャルサポートキャラ(サポカ、アドバイザー)
                foreach (var supportMember in specialSupportCharaMembers)
                {
                    memberLevel.Add(supportMember.MCharId, supportMember.Lv);
                }
                
                // 育成選手
                memberLevel.Add(trainingCharacter.MCharId, trainingCharacter.Lv);
                
                memberlevelDictionary = memberLevel;
            }
        }
        
        private static DeckMember deck = null;
        public static DeckMember Deck{get{return deck;}}
        
        public static void SetCurrentTrainingDeck(DeckMember deckMember)
        {
            deck = deckMember;
        }
        
        public static CharaV2FriendLend ConvertUCharaToFriendLend(UserDataChara charaData)
        {
            CharaV2FriendLend data = new CharaV2FriendLend();
            data.id = charaData.id;
            data.uMasterId = UserDataManager.Instance.user.uMasterId;
            data.mCharaId = charaData.MChara.id;
            data.level = charaData.level;
            data.newLiberationLevel = charaData.newLiberationLevel;
            return data;
        }
    }
}