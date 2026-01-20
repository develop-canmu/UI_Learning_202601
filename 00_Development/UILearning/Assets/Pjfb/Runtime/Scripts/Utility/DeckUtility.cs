using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb;
using Pjfb.ClubMatch;
using Pjfb.LeagueMatch;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using PseudoRandom;
using UnityEngine;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;

    public enum DeckType
    {
        Battle = 4,
        Training = 1000001,
        Friend = 4001,
        Club = 1001,
        SupportEquipment = 1100001,
        LeagueMatch = 1101,
        //トレーニング育成枠
        GrowthTarget = 1000000,
        // トレーニングサポートのフレンド枠
        SupportFriend = 5001,
        // クラブ・ロワイヤル
        ClubRoyal = 1201,
        // アドバイザー
        Adviser = 1301,
    }

    public enum DeckFormatIdType
    {
        Battle = 2,
        Training = 1,
        Friend = 3,
        Club = 4,
        SupportEquipment = 5,
        LeagueMatch = 6,
        ClubRoyal = 8,
        Adviser = 10,
    }

    public enum DeckMemberType
    {
        None = 0,
        UChar = 1,
        UCharaVariable = 2,
        UEquipment = 4,
    }

    public enum TrainingDeckMemberType
    {
        Support, SpecialSupport, Friend, Equipment, Adviser
    }

    public enum TrainingDeckLimitTarget
    {
        RarityFour = 4,
        UR = 1004,
    }

    public class DeckSlotCharacter
    {
        public DeckSlotCharacter(UserDataCharaVariable chara, RoleNumber position)
        {
            Chara = chara;
            Position = position;
        }
        public readonly UserDataCharaVariable Chara;
        public readonly RoleNumber Position;
    }
    public class DeckData
    {
        private const int TypeIndex = 0;
        private const int CharaIdIndex = 1;
        private const int PositionIndex = 2;
        
        public DeckData(DeckBase deck, DeckType type, int index)
        {
            deckType = type;
            // 初期化
            InitializeDeck(deck);
            Deck = deck;
            PartyNumber = deck.partyNumber;
            Index = index;

            deckFormatId = deckType switch
            {
                DeckType.Battle => DeckFormatIdType.Battle,
                DeckType.Training => DeckFormatIdType.Training,
                DeckType.Friend => DeckFormatIdType.Friend,
                DeckType.Club => DeckFormatIdType.Club,
                DeckType.SupportEquipment => DeckFormatIdType.SupportEquipment,
                DeckType.LeagueMatch => DeckFormatIdType.LeagueMatch,
                DeckType.ClubRoyal => DeckFormatIdType.ClubRoyal,
                DeckType.Adviser => DeckFormatIdType.Adviser,
                _ => throw new ArgumentOutOfRangeException()
            };

            rules = MasterManager.Instance.deckFormatConditionMaster.values.Where(x =>
                x.mDeckFormatId == (int)deckFormatId)?.ToArray();
            
            SetOriginalData();
        }
        
        /// <summary>デッキの配列番号</summary>
        public int Index { get; private set; }
        public long PartyNumber { get; private set; }
        public CharaV2FriendLend Friend { get; set; }
        public DeckBase Deck { get; private set; }
        public BigValue CombatPower => MemberIdList.Select(x => x.l[CharaIdIndex]).Sum(id =>
        {
            UserDataCharaVariable data = UserDataManager.Instance.charaVariable.Find(id);
            return data == null ? BigValue.Zero : new BigValue(data.combatPower);
        });
        
        /// <summary>編成がロックされているかどうか</summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>編成できない期間かどうか</summary>
        public bool IsLockedPeriod
        {
            get
            {
                // マスタがないのでロックされてない
                if(MDeckExtra == null) return false;
                // 編成不可の時間帯かチェック
                foreach (NativeApiPeriodTime periodTime in MDeckExtra.lockTimeJson)
                {
                    // 現在時間
                    DateTime now = AppTime.Now;
                    // 開始時間
                    DateTime startAt = DateTime.Parse(periodTime.startAt);
                    startAt = new DateTime(now.Year, now.Month, now.Day, startAt.Hour, startAt.Minute, startAt.Second);
                    // 終了時間
                    DateTime endAt = DateTime.Parse(periodTime.endAt);
                    endAt = new DateTime(now.Year, now.Month, now.Day, endAt.Hour, endAt.Minute, endAt.Second);
                    // 期間内かどうか
                    if(DateTimeExtensions.IsWithinPeriod(now, startAt, endAt))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        private DeckExtraMasterObject mDeckExtra = null;
        /// <summary>エクストラマスタ</summary>
        public DeckExtraMasterObject MDeckExtra
        {
            get
            {
                if(mDeckExtra == null)
                {
                    foreach (DeckExtraMasterObject master in MasterManager.Instance.deckExtraMaster.values)
                    {
                        if((DeckType)master.useType == deckType)
                        {
                            mDeckExtra = master;
                        } 
                    }
                }
                return mDeckExtra;
            }
        }
        
        /// <summary>
        /// Update FixedFatigueValue by calling UpdateFatigueValue().
        /// </summary>
        public long FixedFatigueValue{get; private set; }
        
        public ClubConditionData FixedClubConditionData{get; private set; }
        
        /// <summary> エントリー済みか？ </summary>
        public bool LeagueMatchAlreadyEntered { get; private set; }

        public void UpdateFatigueValue()
        {
            if (deckType is not DeckType.Club)
            {
                FixedFatigueValue = 0;
                FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Best);
                return;
            }

            DateTime expireAt = Deck.tiredness.expireAt.TryConvertToDateTime();
            DateTime now = AppTime.Now;
            
            
            if (now.IsFuture(expireAt))
            {
                FixedFatigueValue = 0;
                FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Good);
                return;
            }
            
            DateTime penaltyExpireAt = Deck.tiredness.penaltyExpireAt.TryConvertToDateTime();
            if (penaltyExpireAt.IsFuture(AppTime.Now))
            {
                FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Awful);
                return;
            }
                
            var mDeckExtraTiredness = MasterManager.Instance.deckExtraTirednessMaster.values.FirstOrDefault(x => x.useType == (int)deckType);
            if (mDeckExtraTiredness is null)
            {
                FixedFatigueValue = 0;
                FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Best);
                return;
            } 
                    

            DateTime changeAt = Deck.tiredness.changeAt.TryConvertToDateTime();

            DateTime midnightOfChargeAt = changeAt.Date;
            DateTime nextMidnightOfChargeAt = midnightOfChargeAt.AddDays(1);

            long cureUnitSeconds = mDeckExtraTiredness.cureUnitMinutes * 60;
            int totalTurn = Mathf.CeilToInt((float)(nextMidnightOfChargeAt - midnightOfChargeAt).TotalSeconds / cureUnitSeconds);

            int pastTurn = Mathf.FloorToInt((float)(changeAt - midnightOfChargeAt).TotalSeconds / cureUnitSeconds);
            int futureTurn = Mathf.CeilToInt((float)(nextMidnightOfChargeAt - now).TotalSeconds / cureUnitSeconds);

            long recoveryAmount = (totalTurn - pastTurn - futureTurn) * mDeckExtraTiredness.cureValue;

            FixedFatigueValue = Math.Clamp(Deck.tiredness.value - recoveryAmount, 0, mDeckExtraTiredness.valueMax);
            
            if (FixedFatigueValue >= mDeckExtraTiredness.valueMaxToAction)
            {
                FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Awful);
                return;
            }

            
            DrawCondition(mDeckExtraTiredness);
        }

        private void DrawCondition(DeckExtraTirednessMasterObject mDeckExtraTiredness)
        {
            // Get Seed
            ulong conditionBase = (ulong)Deck.tiredness.conditionBase * 1000;
            //conditionBase += (ulong)GetUnitCountFromAncientTime(mDeckExtraTiredness.cureUnitMinutes);

            MersenneTwister mt = new MersenneTwister(conditionBase);
            long randomRate = (long)(mt.GenrandReal1() * 10000);

            long tableType = Deck.tiredness.conditionTableType;
            long currentRate = 0;
            CruFramework.Logger.Log($"{Deck.name} : seed = {conditionBase}, rate = {randomRate:0}");
            
            foreach (var table in MasterManager.Instance.colosseumBattleCorrectionMaster.values)
            {
                if (table.optionType != tableType) continue;
                currentRate += table.threshold;
                if (randomRate <= currentRate)
                {
                    FixedClubConditionData = ClubMatchUtility.GetConditionData((ClubDeckCondition)table.labelNumber);
                    return;
                }
            }
            
            if(FixedClubConditionData == null)
            {
                FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Good);
            }
            // FixedClubConditionData = ClubMatchUtility.GetConditionData(ClubDeckCondition.Best);
        }
        
        private long GetUnitCountFromAncientTime(long cureUnitMinutes)
        {
            var ancientTime = new DateTime(1980, 1, 1);
            DateTime now = AppTime.Now;
            double diffMinutes = (float)(now - ancientTime).TotalMinutes;
            CruFramework.Logger.Log($"{Deck.name} : diffMinutes = {diffMinutes:0}");
            return (long)(diffMinutes / cureUnitMinutes) % 1000;
        }

        public bool CanEditDeck
        {
            get
            {
                if (deckType is not DeckType.Club) return true;
                return FixedClubConditionData.condition is not ClubDeckCondition.Awful;
            }
        }
        public Action<long> OnSelectPartyNumber;
        
       
        private readonly DeckFormatIdType deckFormatId;
        private readonly DeckType deckType;

        /// <summary>デッキ</summary>

        private long originalStrategy;
        private WrapperIntList[] originalCharaIdList;
        private DeckFormatConditionMasterObject[] rules;

        
        /// <summary>メンバー数</summary>
        public int MemberCount => Deck.memberIdList.Length;
        /// <summary>スロット数</summary>
        public int SlotCount{get{return DeckUtility.GetDeckSlotCount(deckType);}}


        public bool IsEnableDeck(bool isCheckFriend = true)
        {
            // トレーニングの場合はフレンド必須
            if(deckType == DeckType.Training && isCheckFriend)
            {
                if(Friend == null)return false;
            }
            
            // デッキのスロットマスタ
            DeckFormatSlotMasterObject[] mSlots = DeckUtility.GetSlotMaster(deckType);
            // Empty許容チェック
            foreach(DeckFormatSlotMasterObject mSlot in mSlots)
            {
                // 必須枠
                if(mSlot.conditionRequired)
                {
                    if(GetMemberId(mSlot.index) == DeckUtility.EmptyDeckSlotId)return false;
                }
            }
                
            return true;
        }
        
        /// <summary>レベル制限取得</summary>
        public long GetUnlockLevel(long index)
        {
            // デッキのスロットマスタ
            DeckFormatSlotMasterObject[] mSlots = DeckUtility.GetSlotMaster(deckType);
            foreach(DeckFormatSlotMasterObject slot in mSlots)
            {
                if(slot.index == index)
                {
                    return slot.traineeMinLevel;
                }
            }
            return 0;
        }

        public bool IsEnableDeck(long parentCharacterId, long deckFormatId, long trainingUCharaId)
        {
            // 枠が埋まっているか
            if(IsEnableDeck() == false)return false;
            
            // MemberのId
            long[] ids = GetMemberIds();
            // フレンドのId
            long friendParentId = CharacterUtility.CharIdToParentId(Friend.mCharaId);
            
            // フレンドに同じキャラがいる
            if(friendParentId == parentCharacterId)
            {
                return false;
            }
            
            for(int i=0;i<ids.Length;i++)
            {
                long id = ids[i];
                if(id == DeckUtility.EmptyDeckSlotId)continue;
                if(GetMemberType(i) == DeckMemberType.UEquipment)continue;
                // parentId
                long parentId = CharacterUtility.UserCharIdToParentId(id);
                // メンバーに
                if(parentCharacterId == parentId)return false;
                // フレンドに同じキャラがいる
                if(friendParentId == parentId)return false;
            }
            
            bool isSupportWithinLimit = true;
            bool isSpecialSupportWithinLimit = true;
            int count = 0;
            long compareValue = 0;
            // 編成制限取得
            DeckFormatConditionMasterObject supportLimit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.RarityFour, deckFormatId);
            DeckFormatConditionMasterObject specialSupportLimit = DeckUtility.GetDeckFormatConditionMaster(TrainingDeckLimitTarget.UR, deckFormatId);
            
            // 選手制限がある場合
            if (supportLimit != null)
            {
                count = 0;
                compareValue = long.Parse(supportLimit.compareValue);
                
                // 育成対象のレアリティチェック
                long rarityId = UserDataManager.Instance.chara.data[trainingUCharaId].MChara.mRarityId;
                if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue)
                {
                    count++;
                }
                
                // フレンドのレアリティチェック
                if (Friend != null)
                {
                    rarityId = MasterManager.Instance.charaMaster.FindData(Friend.mCharaId).mRarityId;
                    if (MasterManager.Instance.rarityMaster.FindData(rarityId).value == compareValue)
                    {
                        count++;
                    }
                }
                
                // サポート選手のレアリティチェック
                count += DeckUtility.GetRareLimitCount(this, DeckSlotCardType.Support, supportLimit.operatorType, compareValue);
                
                isSupportWithinLimit = count <= supportLimit.charaCount;
            }
            
            // サポカ制限がある場合
            if (specialSupportLimit != null)
            {
                count = 0;
                compareValue = long.Parse(specialSupportLimit.compareValue);
                
                // サポカのレアリティチェック
                count += DeckUtility.GetSpecialSupportLimitCount(this, specialSupportLimit.operatorType, compareValue);
                
                isSpecialSupportWithinLimit = count <= specialSupportLimit.charaCount;
            }
            return isSupportWithinLimit && isSpecialSupportWithinLimit;
        }

        public WrapperIntList[] MemberIdList => Deck.memberIdList;

        public bool IsAllEmpty => MemberIdList.All(x => x.l[CharaIdIndex] == DeckUtility.EmptyDeckSlotId) &&
                                  ((deckType is not DeckType.Battle or DeckType.Club or DeckType.LeagueMatch) ||
                                  MemberIdList.All(x => x.l[PositionIndex] == (int)RoleNumber.None));
        public bool HasEmptySlot => MemberIdList.Any(x => x.l[CharaIdIndex] == DeckUtility.EmptyDeckSlotId);

        public bool IsOriginDeckAllEmpty => originalCharaIdList.All(x => x.l[CharaIdIndex] == DeckUtility.EmptyDeckSlotId);
        public bool HasEmptyRoleNumber => (deckType is DeckType.Battle or DeckType.Club or DeckType.LeagueMatch) && MemberIdList.Any(x => x.l[PositionIndex] == (int)RoleNumber.None);
        public bool HasDuplicateMCharaParentId
        {
            get
            {
                HashSet<long> idSet = new();
                foreach (var wrapperIntList in MemberIdList)
                {
                    long id = UserDataManager.Instance.charaVariable.Find(wrapperIntList.l[CharaIdIndex])?.ParentMCharaId ?? -1;
                    if (id > 0 && idSet.Contains(id)) return true;
                    idSet.Add(id);
                }
                return false;
            }
        }
        
       

        /// <summary>変更があるか</summary>
        public bool IsDeckChanged
        {
            get
            {
                if (deckType is DeckType.Battle or DeckType.Club or DeckType.LeagueMatch or DeckType.ClubRoyal)
                {
                    for (int i = 0; i < MemberIdList.Length; i++)
                    {
                        if (MemberIdList[i].l[PositionIndex] != originalCharaIdList[i].l[PositionIndex]) return true;
                    }
                }
                
                for (int i = 0; i < MemberIdList.Length; i++)
                {
                    if (MemberIdList[i].l[CharaIdIndex] != originalCharaIdList[i].l[CharaIdIndex]) return true;
                }

                if (originalStrategy != Deck.optionValue) return true;
                
                return false;
            }
        }
        
        public bool HasNewCharacterFormed
        {
            get
            {
                var hashSet = originalCharaIdList.Select(x => x.l[CharaIdIndex]).ToHashSet(); 
                foreach (var id in MemberIdList)
                {
                    if (!hashSet.Contains(id.l[CharaIdIndex])) return true;
                }
                return false;
            }
        }
        private void InitializeDeck(DeckBase deck)
        {
            DeckFormatSlotMasterObject[] mSlots = DeckUtility.GetSlotMaster(deckType);
            WrapperIntList[] newDeckList = new WrapperIntList[SlotCount];
                
            // 既存のものをコピー
            for (int i = 0; i < deck.memberIdList.Length && i < newDeckList.Length; i++) 
            {
                newDeckList[i] = deck.memberIdList[i];
                // DeckMemberTypeは空編成の場合サーバー都合で0でくる場合があるが、クライアント側で編成時に確認する必要があるため、
                // あらかじめセットしておく
                switch (deckType)
                {
                    case DeckType.Battle:
                    case DeckType.Club:
                    case DeckType.LeagueMatch:
                    case DeckType.ClubRoyal:
                        newDeckList[i].l[TypeIndex] = (int)DeckMemberType.UCharaVariable;
                        break;
                    default:
                        newDeckList[i].l[TypeIndex] = mSlots[i].conditionTableType;
                        break;
                }
                
            }
                
            // 追加分
            for (int i = deck.memberIdList.Length; i < newDeckList.Length; i++)
            {
                newDeckList[i] = new WrapperIntList();
                switch (deckType)
                {
                    case DeckType.Battle:
                    case DeckType.Club:
                    case DeckType.LeagueMatch:
                    case DeckType.ClubRoyal:
                        newDeckList[i].l = new long[]{ (int)DeckMemberType.UCharaVariable, DeckUtility.EmptyDeckSlotId, (int)RoleNumber.None };
                        break;
                    case DeckType.Training:
                    case DeckType.SupportEquipment:
                        newDeckList[i].l = new long[] { mSlots[i].conditionTableType, DeckUtility.EmptyDeckSlotId};
                        break;
                    case DeckType.Adviser:
                        newDeckList[i].l = new long[] { mSlots[i].conditionTableType, DeckUtility.EmptyDeckSlotId, (int)RoleNumber.None };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            deck.memberIdList = newDeckList;
        }

        private void SetOriginalData()
        {
            originalCharaIdList = MemberIdList.Select(x => new WrapperIntList{l = x.l.ToArray()}).ToArray();
            originalStrategy = Deck.optionValue;
        }

        public long GetMemberId(long order)
        {
            return MemberIdList[order].l[CharaIdIndex];
        }

        public void SetMemberType(int order, DeckMemberType type)
        {
            MemberIdList[order].l[TypeIndex] = (int)type;
        }
        
        public DeckMemberType GetMemberType(int order)
        {
            return (DeckMemberType)MemberIdList[order].l[TypeIndex];
        }
        
        public long[] GetMemberIds()
        {
            long[] result = new long[MemberCount];
            for(int i=0;i<MemberIdList.Length;i++)
            {
                result[i] = MemberIdList[i].l[CharaIdIndex];
            }
            return result;
        }
        
        public long[] GetCharacterMemberIds()
        {
            List<long> result = new List<long>();
            for(int i=0;i<MemberIdList.Length;i++)
            {
                if(GetMemberType(i) == DeckMemberType.UEquipment)continue;
                result.Add(MemberIdList[i].l[CharaIdIndex]);
            }
            return result.ToArray();
        }

        public long[] GetEquipmentMemberIds()
        {
            List<long> result = new List<long>();
            for(int i=0;i<MemberIdList.Length;i++)
            {
                if(GetMemberType(i) != DeckMemberType.UEquipment)continue;
                result.Add(MemberIdList[i].l[CharaIdIndex]);
            }
            return result.ToArray();
        }   

        public bool Contains(long id)
        {
            return MemberIdList.Any(x => x.l[CharaIdIndex] == id);
        }

        public RoleNumber GetMemberPosition(int order)
        {
            return (RoleNumber)MemberIdList[order].l[PositionIndex];
        }

        public void SetRecommendedChara(DeckSlotCharacter[] charaList)
        {
            for (int i = 0; i < MemberIdList.Length; i++)
            {
                SetMemberId(i, charaList[i].Chara.id);
                SetPosition(i, charaList[i].Position);
            }
        }
        
        private long GetParentId(long uCharId, int order)
        {
            DeckMemberType type = GetMemberType(order);
            switch(type)
            {
                case DeckMemberType.UChar:
                    return CharacterUtility.UserCharIdToParentId(uCharId);
                case DeckMemberType.UCharaVariable:
                    return CharacterUtility.UserVariableCharIdToParentId(uCharId);
                case DeckMemberType.UEquipment:
                    return CharacterUtility.UserEquipmentIdToParentId(uCharId);
            }
            return uCharId;
        }
        
        public void SetMemberId(int order, long uCharId, bool canDuplicate = false)
        {
            int duplicateIndex = -1;
            if(uCharId != DeckUtility.EmptyDeckSlotId)
            {
                
                long parentId = GetParentId(uCharId, order);
                
                for(int i = 0; i < SlotCount; i++)
                {
                    // 同じ場所
                    if(i == order)  continue;
                    
                    long memberId = MemberIdList[i].l[CharaIdIndex];
                    // 同一キャラの場合は場所移動
                    if(uCharId == memberId)
                    {
                        duplicateIndex = i;
                        continue;
                    }
                    
                    // 同じキャラの場合は外す
                    if(canDuplicate == false)
                    {
                        if(memberId != DeckUtility.EmptyDeckSlotId && parentId == GetParentId(memberId, i))
                        {
                            MemberIdList[i].l[CharaIdIndex] = DeckUtility.EmptyDeckSlotId;
                        }
                    }
                }
            }
            
            if (duplicateIndex >= 0)
            {
                // サポート器具の場合は入れ替えに枠制限をかける
                if (deckType == DeckType.SupportEquipment && DeckUtility.IsInSupportEquipmentSlotLimit(deckType, duplicateIndex, MemberIdList[order].l[CharaIdIndex]) == false)
                {
                    MemberIdList[duplicateIndex].l[CharaIdIndex] = DeckUtility.EmptyDeckSlotId;
                    ConfirmModalData data = new ConfirmModalData()
                    {
                        Title = StringValueAssetLoader.Instance["training.support_equipment_confirm.title"],
                        Message = StringValueAssetLoader.Instance["training.support_equipment_confirm.description"],
                        NegativeButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (modal) => { modal.Close(); })
                    };
                    // 枠制限により外されたことのモーダルを表示
                    ConfirmModalWindow.Open(data);
                }
                else
                {
                    MemberIdList[duplicateIndex].l[CharaIdIndex] = MemberIdList[order].l[CharaIdIndex];
                }
            }
            
            if(deckType is DeckType.Battle or DeckType.Club or DeckType.LeagueMatch)
            {
                MemberIdList[order].l[CharaIdIndex] = uCharId;
                MemberIdList[order].l[PositionIndex] = MemberIdList[order].l[PositionIndex];
            }
            else 
            {
                MemberIdList[order].l[CharaIdIndex] = uCharId;
            }
        }
        
     
        
        public long[] GetMemberIds(DeckSlotCardType type)
        {
            long[] index = DeckUtility.GetCharacterIndex(deckType, type);
            long[] result = new long[index.Length];
            for(int i=0;i<index.Length;i++)
            {
                result[i] = MemberIdList[index[i]].l[CharaIdIndex];
            }
            return result;
        }
        
        public long[] GetExMemberIds(DeckSlotCardType type)
        {
            long[] index = DeckUtility.GetExtraSupportCharacterIndex(deckType, true);
            long[] result = new long[index.Length];
            for(int i=0;i<index.Length;i++)
            {
                result[i] = MemberIdList[index[i]].l[CharaIdIndex];
            }
            return result;
        }
        
        public int GetMemberIndex(long id)
        {
            long[] ids = GetMemberIds();
            for(int i=0;i<ids.Length;i++)
            {
                if(ids[i] == id)return i;
            }
            return -1;
        }
        
        public void SetPosition(int order, RoleNumber position)
        {
            MemberIdList[order].l[PositionIndex] = (int)position;
        }

        public void SetStrategy(long strategy)
        {
            Deck.optionValue = strategy;
        }
        
        public void SetEmptyUCharId(int order)
        {
            if (deckType is DeckType.Battle or DeckType.Club or DeckType.LeagueMatch or DeckType.ClubRoyal or DeckType.Adviser)
            {
                MemberIdList[order].l[CharaIdIndex] = DeckUtility.EmptyDeckSlotId;
                MemberIdList[order].l[PositionIndex] = (long)RoleNumber.None;
            }
            else
            {
                MemberIdList[order].l[CharaIdIndex] = DeckUtility.EmptyDeckSlotId;
            }
        }

        public void SetDeckEmpty()
        {
            for (int i = 0; i < MemberIdList.Length; i++)
            {
                SetEmptyUCharId(i);
            }
            Deck.optionValue = (long)BattleConst.DeckStrategy.Aggressive;
        }

        public void DiscardChanges()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                MemberIdList[i].l = originalCharaIdList[i].l.ToArray();
            }

            Deck.optionValue = originalStrategy;
        }
        
        /// <summary>デッキの保存</summary>
        public async UniTask<string> SaveDeckAsync(bool selectDeck = true, bool skipRule = false, bool emptySlot = false)
        {
            if (deckType == DeckType.Friend) selectDeck = false;
            if (!IsDeckChanged && !selectDeck) return string.Empty;
            
            if (!skipRule && !TryMatchDeckRule(out var errorMessage))
            {
                return errorMessage;
            }
            
            WrapperIntList[] postTypeIdList = null;
            
            if(deckType == DeckType.SupportEquipment || IsAllEmpty == false || emptySlot == true)
            {
                postTypeIdList = MemberIdList;
            }
            
            // Indexが0で送られてくるパターンがあってその場合、サポカの効果が発揮されないってバグがあるらしいので、クライアント側でマスタの値をみて上書き
            if(postTypeIdList != null)
            {
                // デッキスロットマスター取得
                DeckFormatSlotMasterObject[] mDeckFormatSlots = DeckUtility.GetSlotMaster(deckType);
                // Idリストチェック
                for(int i = 0; i < postTypeIdList.Length; i++)
                {
                    // 意図的に空編成で送られてきている部分に関しては0を送る
                    if (emptySlot == true && postTypeIdList[i].l[CharaIdIndex] == DeckUtility.EmptyDeckSlotId)
                    {
                        postTypeIdList[i].l[TypeIndex] = (int)DeckMemberType.None;
                    }
                    else
                    {
                        // index更新
                        postTypeIdList[i].l[TypeIndex] = mDeckFormatSlots[i].conditionTableType;
                    }
                }
            }
            
            DeckEditAPIRequest request = new DeckEditAPIRequest();
            DeckEditAPIPost post = new()
            {
                partyNumber = Deck.partyNumber,
                typeIdList = postTypeIdList,
                optionValue = deckType is DeckType.Battle or DeckType.Club or DeckType.LeagueMatch or DeckType.ClubRoyal or DeckType.Adviser ? Deck.optionValue : 0, 
                useType = (long)((selectDeck) ? deckType : 0),
            };
            request.SetPostData(post);
            
            await APIManager.Instance.Connect(request);
            if(selectDeck) OnSelectPartyNumber?.Invoke(PartyNumber);
            DeckEditAPIResponse res = request.GetResponseData();
            Deck = res.deckList.FirstOrDefault(x => x.partyNumber == Deck.partyNumber) ?? Deck;
            InitializeDeck(Deck);
            if (deckType is DeckType.Club)
            {
                UpdateFatigueValue();
            }
            SetOriginalData();
            return string.Empty;
        }

        public async UniTask SaveDeckNameAsync(string newDeckName)
        {
            if(Deck.name == newDeckName) return;
            DeckNameChangeAPIRequest request = new DeckNameChangeAPIRequest();
            DeckNameChangeAPIPost post = new DeckNameChangeAPIPost();
            post.partyNumber = Deck.partyNumber;
            post.name = newDeckName;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            DeckNameChangeAPIResponse res = request.GetResponseData();
            Deck.name = res.deckList.FirstOrDefault(x => x.partyNumber == Deck.partyNumber)?.name ?? newDeckName;

        }
        
        private bool TryMatchDeckRule(out string message)
        {
            message = string.Empty;
            if (deckType is not DeckType.Battle or DeckType.Club or DeckType.LeagueMatch) return true;
            long[] roleList = MemberIdList.Select(x => x.l[PositionIndex]).ToArray();
            
            foreach (var rule in rules)
            {
                long minimumCount;
                long maximumCount;
                // rule.charaCount以下
                if (rule.conditionType == DeckConditionType.LE)
                {
                    minimumCount = 0;
                    maximumCount = rule.charaCount;
                }
                // rule.charaCount以上
                else
                {
                    minimumCount = rule.charaCount;
                    maximumCount = long.MaxValue;
                }

                long[] compareValues = rule.compareValue.Trim('[', ']').Split(',').Select(x => Convert.ToInt64(x)).ToArray();
                

                int meetCount = 0;

                switch (rule.operatorType)
                {
                    case DeckOperatorType.EQ:
                        meetCount = roleList.Count(x => compareValues.Contains(x));
                        break;
                    case DeckOperatorType.NE:
                        break;
                    case DeckOperatorType.GE:
                        break;
                    case DeckOperatorType.GT:
                        break;
                    case DeckOperatorType.LE:
                        break;
                    case DeckOperatorType.LT:
                        break;
                    case DeckOperatorType.BETWEEN:
                        break;
                    case DeckOperatorType.IN:
                        break;
                    case DeckOperatorType.NOT_IN:
                        meetCount = roleList.Count(x => !compareValues.Contains(x));
                        break;
                    case DeckOperatorType.Undefined:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!(minimumCount <= meetCount && meetCount <= maximumCount))
                {
                    message = rule.errorMessage;
                    return false;
                } 
                    
            }

            return true;
        }
    }
    
    public class DeckListData
    {
        public DeckListData(DeckData[] deckDataList, DeckType type)
        {
            this.deckDataList = deckDataList;
            this.type = type;

            foreach (var deckData in deckDataList)
            {
                deckData.OnSelectPartyNumber = SetSelectingPartyNumber;
            }
        }

        public readonly DeckType type;
        public int SelectingIndex { get; private set; } = 0;
        public long SelectingPartyNumber { get; private set; } = 0;
        private readonly DeckData[] deckDataList = null;
        /// <summary>デッキ情報</summary>
        public DeckData[] DeckDataList => deckDataList;

        /// <summary>変更があるか</summary>
        public bool IsChanged => deckDataList.Any(deck => deck.IsDeckChanged);

        
        /// <summary>デッキ保存</summary>
        public async UniTask SaveAsync(long selectedPartyNumber)
        {
            foreach(DeckData deck in deckDataList)
            {
                await deck.SaveDeckAsync(deck.PartyNumber == selectedPartyNumber && selectedPartyNumber != SelectingPartyNumber);
            }
        }

        public void SetSelectingPartyNumber(long partyNumber)
        {
            SelectingPartyNumber = partyNumber;
            SelectingIndex = PartyNumberToIndex(type, partyNumber);
        }

        public static int PartyNumberToIndex(DeckType type, long partyNumber)
        {
            return type switch
            {
                DeckType.Battle => DeckUtility.BattleIndex.ToList().FindIndex(x => x == partyNumber),
                DeckType.Training => DeckUtility.TrainingIndex.ToList().FindIndex(x => x == partyNumber),
                DeckType.Club => DeckUtility.ClubIndex.ToList().FindIndex(x => x == partyNumber),
                DeckType.LeagueMatch => DeckUtility.LeagueMatchIndex.ToList().FindIndex(x => x == partyNumber),
                DeckType.ClubRoyal => DeckUtility.ClubRoyalIndex.ToList().FindIndex(x => x == partyNumber),
                DeckType.Adviser => DeckUtility.AdviserIndex.ToList().FindIndex(x => x == partyNumber),
                DeckType.Friend or _ => throw new NotImplementedException(),
            };
        }
        


        public bool Contains(long id) => deckDataList.Any(deck => deck.Contains(id));
        
        public async UniTask SelectDeckAsync(long partyNumber)
        {
            if(type == DeckType.Friend)  return;
            
            if(partyNumber == SelectingPartyNumber) return;
            DeckSelectAPIRequest request = new DeckSelectAPIRequest();
            DeckSelectAPIPost post = new DeckSelectAPIPost
            {
                useType = (int)type,
                partyNumber = partyNumber
            };
            request.SetPostData(post);
            SetSelectingPartyNumber(partyNumber);
            
            await APIManager.Instance.Connect(request);
        }

        public DeckData GetDeck(long partyNumber)
        {
            foreach(DeckData deck in deckDataList)
            {
                if(deck.PartyNumber == partyNumber)return deck;
            }
            return null;
        }
        
    }
    
    public static class DeckUtility
    {
        /// <summary>編成確認モダールのタイトル</summary>
        private static readonly string leaveDeckTitle = "character.deckedit.leave_deck_confirm_title"; 
        /// <summary>編成確認モダールの内容</summary>
        private static readonly string leaveDeckContent = "character.deckedit.leave_deck_confirm_content";
        
        public const int BattleDeckSlotCount = 5;
        public const int FriendDeckSlotCount = 1;
        public const int EmptyDeckSlotId = -1;
        
        public static readonly IReadOnlyList<long> BattleIndex = new List<long>{ 1, 2, 3, 4, 5 }.AsReadOnly();

        public static readonly IReadOnlyList<long> TrainingIndex = new List<long> { 1000001, 1000002, 1000003, 1000004, 1000005 }.AsReadOnly();
        public static readonly IReadOnlyList<long> SupportEquipmentIndex = new List<long> { 1100001, 1100002, 1100003, 1100004, 1100005 }.AsReadOnly();
        public static readonly IReadOnlyList<long> FriendIndex = new List<long>{ 4001 }.AsReadOnly();
        public static readonly IReadOnlyList<long> ClubIndex = new List<long>{ 1001,1002,1003,1004,1005 }.AsReadOnly();
        public static readonly IReadOnlyList<long> LeagueMatchIndex = new List<long>{ 1101,1102,1103,1104,1105 }.AsReadOnly();
        public static IReadOnlyList<long> ClubRoyalIndex => GetDeckIndexList(DeckType.ClubRoyal);
        public static IReadOnlyList<long> AdviserIndex => GetDeckIndexList(DeckType.Adviser);
        
        private static Dictionary<DeckType, DeckListData> deckDataDictionary = null;
        
        /// <summary>トレーニングデッキのスロット数</summary>
        public static int TrainingDeckSlotCount{get{return GetDeckSlotCount(DeckType.Training);}}
        
        /// <summary>デッキに編成されているか</summary>
        public static bool ExistsSupportEquipment(long uEquipmentId)
        {
            foreach(DeckListData deckDatas in deckDataDictionary.Values)
            {
                foreach(DeckData deck in deckDatas.DeckDataList)
                {
                    long[] memberIds = deck.GetMemberIds();
                    for(int i=0;i<memberIds.Length;i++)
                    {
                        if(deck.GetMemberType(i) == DeckMemberType.UEquipment && memberIds[i] == uEquipmentId)return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>デッキに編成されているか</summary>
        public static List<long> GetSupportEquipmentIds()
        {
            List<long> result = new List<long>();
            foreach(DeckListData deckDatas in deckDataDictionary.Values)
            {
                foreach(DeckData deck in deckDatas.DeckDataList)
                {
                    long[] memberIds = deck.GetMemberIds();
                    for(int i=0;i<memberIds.Length;i++)
                    {
                        if(deck.GetMemberType(i) != DeckMemberType.UEquipment) continue;
                        if(memberIds[i] == EmptyDeckSlotId) continue;
                        result.Add(memberIds[i]);
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>デッキのフォーマットId</summary>
        public static long GetDeckFormatId(DeckType deckType)
        {
            return MasterManager.Instance.deckFormatUseMaster.FindData(deckType).mDeckFormatId;
        }
        
        /// <summary>スロット数</summary>
        public static int GetDeckSlotCount(DeckType deckType)
        {
            long formatId = GetDeckFormatId(deckType);
            return MasterManager.Instance.deckFormatSlotMaster.FindDatas(formatId).Length;
        }
        
        /// <summary>デッキタイプでマスタ取得</summary>
        public static DeckFormatSlotMasterObject[] GetSlotMaster(DeckType deckType)
        {
            long formatId = GetDeckFormatId(deckType);
            return MasterManager.Instance.deckFormatSlotMaster.FindDatas(formatId);
        }
        
        /// <summary>デッキタイプでマスタ取得</summary>
        public static DeckFormatSlotMasterObject GetSlotMaster(DeckType deckType, long index)
        {
            DeckFormatSlotMasterObject[] deckFormatSlotMasterObjects = GetSlotMaster(deckType);
            foreach (DeckFormatSlotMasterObject deckFormatSlotMasterObject in deckFormatSlotMasterObjects)
            {
                if (deckFormatSlotMasterObject.index == index)
                {
                    return deckFormatSlotMasterObject;
                }
            }

            return null;
        }

        /// <summary>DeckFormatIdTypeからdeckFormatConditionの取得</summary>
        public static DeckFormatConditionMasterObject GetConditionMaster(DeckFormatIdType formatType)
        {
            return MasterManager.Instance.deckFormatConditionMaster.values.FirstOrDefault(x => x.mDeckFormatId == (long)formatType);
        }
        
        /// <summary>キャラのIndex</summary>
        public static long[] GetCharacterIndex(DeckType deckType, DeckSlotCardType slotCardType)
        {
            long formatId = GetDeckFormatId(deckType);
            DeckFormatSlotMasterObject[] slots = MasterManager.Instance.deckFormatSlotMaster.FindDatas(formatId, slotCardType);
            long[] result = new long[slots.Length];
            for(int i=0;i<result.Length;i++)
            {
                result[i] = slots[i].index;
            }
            return result;
        }

        public static long[] GetCharacterIndex(DeckType deckType, DeckSlotCardType slotCardType, bool includeLock)
        {
            long[] index = GetCharacterIndex(deckType, slotCardType);
            List<long> result = new List<long>();
            for(int i = 0; i < index.Length; i++)
            {
                // ロック状態を取得
                if(includeLock == false && TrainingDeckUtility.IsAdviserSystemLock(i))continue;
                result.Add(index[i]);
            }
            return result.ToArray();
        }
        
        /// <summary>キャラのIndex</summary>
        public static long[] GetSpecialSupportCharacterIndex(DeckType deckType, bool includeLock)
        {
            long formatId = GetDeckFormatId(deckType);
            DeckFormatSlotMasterObject[] slots = MasterManager.Instance.deckFormatSlotMaster.FindDatas(formatId, DeckSlotCardType.SpecialSupport);
            List<long> result = new List<long>();
            
            for(int i=0;i<slots.Length;i++)
            {
               // ロック状態を取得
               if(includeLock == false && TrainingDeckUtility.IsSPSupportSystemLock(i))continue;
               result.Add(slots[i].index);
            }
            return result.ToArray();
        }
        
        /// <summary>キャラのIndex</summary>
        public static long[] GetExtraSupportCharacterIndex(DeckType deckType, bool includeLock)
        {
            long formatId = GetDeckFormatId(deckType);
            DeckFormatSlotMasterObject[] slots = MasterManager.Instance.deckFormatSlotMaster.FindExtraSupportDatas(formatId);
            List<long> result = new List<long>();
            
            for(int i=0;i<slots.Length;i++)
            {
                // ロック状態を取得
                if(includeLock == false && TrainingDeckUtility.IsExSupportSystemLock(i))continue;
                result.Add(slots[i].index);
            }
            return result.ToArray();
        }

        public static async UniTask<DeckListData> GetBattleDeck()
        {
            return await GetDeckList(DeckType.Battle);
        }
        public static async UniTask<DeckListData> GetTrainingDeck()
        {
            return await GetDeckList(DeckType.Training);
        }
        public static async UniTask<DeckData> GetFriendDeck()
        {
            return (await GetDeckList(DeckType.Friend)).DeckDataList[0];
        }
        public static async UniTask<DeckListData> GetClubBattleDeck()
        {
            return await GetDeckList(DeckType.Club);
        }

        public static async UniTask<DeckListData> GetLeagueMatchDeck()
        {
            return await GetDeckList(DeckType.LeagueMatch);
        }

        public static async UniTask<DeckListData> GetClubRoyalDeck()
        {
            return await GetDeckList(DeckType.ClubRoyal);
        }
        
        public static async UniTask<DeckListData> GetDeckList(DeckType deckType)
        {
            if (deckDataDictionary != null)
            {
                return deckDataDictionary[deckType];
            }

            return (await GetDeckDataDictionary())[deckType];
        }

        public static void SetDeckDataDictionary(DeckBase[] deckListDataList, WrapperIntList[] useTypePartyNumberList)
        {
            var deckListData = GetDeckListData(deckListDataList, useTypePartyNumberList);
            deckDataDictionary = deckListData.ToDictionary(aData => aData.type);
        }
        
        private static async UniTask<Dictionary<DeckType, DeckListData>> GetDeckDataDictionary() 
        {
            if (deckDataDictionary == null)
            {
                var responseData = await GetDeckList();
                SetDeckDataDictionary(responseData.deckList, responseData.useTypePartyNumberList);
            }
            return deckDataDictionary;
        }

        /// <summary>デッキのIndexリスト</summary>
        private static List<long> GetDeckIndexList(DeckType deckType)
        {
            List<long> result = new List<long>();
            DeckExtraMasterObject master = MasterManager.Instance.deckExtraMaster.FindByUseType((long)deckType);
            for (long min = master.partyNumberMin; min <= master.partyNumberMax; min++)
            {
                result.Add(min);
            }
            
            return result;
        }
        
        private static async UniTask<DeckGetListAPIResponse> GetDeckList()
        {
            // デッキを取得
            DeckGetListAPIRequest deckGetRequest = new DeckGetListAPIRequest();
            await APIManager.Instance.Connect(deckGetRequest);
            // レスポンス
            return deckGetRequest.GetResponseData();
        }

        private static DeckListData[] GetDeckListData(DeckBase[] deckListDataList, WrapperIntList[] useTypePartyNumberList)
        {
            Dictionary<DeckType, List<DeckData>> deckDictionary = new();
            // Todo : default battle deck position is still empty
            foreach (var deckBase in deckListDataList)
            {
                DeckType deckType = deckBase.partyNumber switch
                {
                    var x when BattleIndex.Contains(x) => DeckType.Battle,
                    var x when TrainingIndex.Contains(x) => DeckType.Training,
                    var x when FriendIndex.Contains(x) => DeckType.Friend,
                    var x when ClubIndex.Contains(x) => DeckType.Club,
                    var x when SupportEquipmentIndex.Contains(x) => DeckType.SupportEquipment,
                    var x when LeagueMatchIndex.Contains(x) => DeckType.LeagueMatch,
                    var x when ClubRoyalIndex.Contains(x) => DeckType.ClubRoyal,
                    var x when AdviserIndex.Contains(x) => DeckType.Adviser,
                    _ => throw new NotImplementedException(),
                };

                
                if (!deckDictionary.ContainsKey(deckType))
                {
                    deckDictionary.Add(deckType, new List<DeckData>());
                }

                DeckData data = new DeckData(deckBase, deckType, deckDictionary[deckType].Count);
                
                deckDictionary[deckType].Add(data);
            }
            
            
            DeckListData[] deckListData = new DeckListData[8];
            deckListData[0] = new DeckListData(deckDictionary[DeckType.Battle].ToArray(), DeckType.Battle);
            deckListData[1] = new DeckListData(deckDictionary[DeckType.Training].ToArray(), DeckType.Training);
            deckListData[2] = new DeckListData(deckDictionary[DeckType.Friend].ToArray(), DeckType.Friend);
            deckListData[3] = new DeckListData(deckDictionary[DeckType.Club].ToArray(), DeckType.Club);
            deckListData[4] = new DeckListData(deckDictionary[DeckType.SupportEquipment].ToArray(), DeckType.SupportEquipment);
            deckListData[5] = new DeckListData(deckDictionary[DeckType.LeagueMatch].ToArray(), DeckType.LeagueMatch);
            deckListData[6] = new DeckListData(deckDictionary[DeckType.ClubRoyal].ToArray(), DeckType.ClubRoyal);
            deckListData[7] = new DeckListData(deckDictionary[DeckType.Adviser].ToArray(), DeckType.Adviser);
            
            foreach (var useType in useTypePartyNumberList)
            {
                if ((DeckType)useType.l[0] == DeckType.Battle)
                {
                    deckListData[0].SetSelectingPartyNumber(useType.l[1]);
                }
                else if ((DeckType)useType.l[0] == DeckType.Training)
                {
                    deckListData[1].SetSelectingPartyNumber(useType.l[1]);
                }
                else if((DeckType)useType.l[0] == DeckType.Club)
                {
                    deckListData[3].SetSelectingPartyNumber(useType.l[1]);
                }
            }
            
            
            return deckListData;
        }
        
        public static async UniTask UpdateClubDeckFatigue(DeckBase[] newDeckBaseList)
        {
            var clubBattleDeck = await GetClubBattleDeck();
            foreach (var deckBase in newDeckBaseList)
            {
                DeckData deck = clubBattleDeck.GetDeck(deckBase.partyNumber);
                if(deck is null) continue;
                deck.Deck.tiredness = deckBase.tiredness;
            }
        }

        public static void ClearDeckData()
        {
            deckDataDictionary = null;
        }

        /// <summary>サポ器具枠制限の範囲内かの判定</summary>
        public static bool IsInSupportEquipmentSlotLimit(DeckFormatConditionMasterObject deckFormatConditionMasterObject, long deckFormatGroup)
        {
            if (deckFormatConditionMasterObject.CompareValueArray == null) return true;
            switch (deckFormatConditionMasterObject.operatorType)
            {
                // INだけ実装
                case DeckOperatorType.IN:
                    return deckFormatConditionMasterObject.CompareValueArray.Contains(deckFormatGroup);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public static bool IsInSupportEquipmentSlotLimit(DeckType deckType, int index, long uEquipmentId)
        {
            if (uEquipmentId == EmptyDeckSlotId) return true;
            DeckFormatSlotMasterObject slotMaster = GetSlotMaster(deckType, index);
            if (slotMaster.mDeckFormatConditionId == 0) return true;
            DeckFormatConditionMasterObject conditionMaster = MasterManager.Instance.deckFormatConditionMaster.FindData(slotMaster.mDeckFormatConditionId);
            CharaMasterObject charaMaster = UserDataManager.Instance.supportEquipment.Find(uEquipmentId).MChara;
            return IsInSupportEquipmentSlotLimit(conditionMaster, charaMaster.deckConditionGroup);
        }
        
        /// <summary>デッキ編成を離れるかの確認</summary>
        public static async UniTask<bool> OnLeaveCurrentDeck(CancellationToken cancellationToken)
        {
            ConfirmModalButtonParams cancelButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (window =>
            {
                window.SetCloseParameter(false);
                window.Close();
            }));
            
            ConfirmModalButtonParams okButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (window) =>
            {
                window.SetCloseParameter(true);
                window.Close();
            });
            
            // タッチガードより手前に表示する
            CruFramework.Page.ModalWindow confirmWindow = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, new ConfirmModalData(
                StringValueAssetLoader.Instance[leaveDeckTitle],
                StringValueAssetLoader.Instance[leaveDeckContent], "", 
                okButtonParams,cancelButtonParams), cancellationToken);

            bool response = (bool)await confirmWindow.WaitCloseAsync(cancellationToken);

            return response;
        }
        
        /// <summary> 編成制限を取得する </summary>
        public static DeckFormatConditionMasterObject GetDeckFormatConditionMaster(TrainingDeckLimitTarget limitTarget, long formatId)
        {
            return MasterManager.Instance.deckFormatConditionMaster.values.SingleOrDefault(x =>
                x.mDeckFormatId == formatId && long.Parse(x.compareValue) == (long)limitTarget);
        }
        
        /// <summary> 編成の対象レアリティが何枚入っているか取得する </summary>
        public static int GetRareLimitCount(DeckData deckData, DeckSlotCardType cardType, DeckOperatorType operatorType, long rare, long index = -1)
        {
            long[] memberIds = deckData.GetMemberIds(cardType);
            
            return GetRareLimitCount(memberIds, operatorType, rare, index);
        }
        
        /// <summary> 編成の対象レアリティが何枚入っているか取得する </summary>
        public static int GetRareLimitCount(long[] memberIds, DeckOperatorType operatorType, long rare, long index = -1)
        {
            int count = 0;
            
            for(int i = 0; i < memberIds.Length; i++)
            {
                // 選択中の枠もしくは空枠はスキップ
                if (i == index || memberIds[i] == EmptyDeckSlotId)
                {
                    continue;
                }
                long rarityId = UserDataManager.Instance.chara.data[memberIds[i]].MChara.mRarityId;
                switch (operatorType)
                {
                    case DeckOperatorType.EQ:
                        if(MasterManager.Instance.rarityMaster.FindData(rarityId).value == rare)
                        {
                            count++;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            return count;
        }
        
        /// <summary> サポカの対象レアリティが何枚入っているか取得する </summary>
        public static int GetSpecialSupportLimitCount(DeckData deckData, DeckOperatorType operatorType, long rare, long index = -1)
        {
            // SPサポートの1枠目を取得する
            int specialSupportCount = 0;
            long firstSupportSlotIndex = 0;
            DeckFormatSlotMasterObject[] slots = GetSlotMaster(DeckType.Training);
            // SPサポートの1枠目のindexを取得する
            firstSupportSlotIndex = slots.First(x => x.conditionCardType == (long)DeckSlotCardType.SpecialSupport).index;
            // SPサポートの対象レアリティが何枚入っているか取得する
            specialSupportCount = GetRareLimitCount(deckData, DeckSlotCardType.SpecialSupport, operatorType, rare, index - firstSupportSlotIndex);
            
            // Exサポートの1枠目を取得する
            int exSpecialSupportCount = 0;
            long firstExSupportSlotIndex = 0;
            // Exサポートの1枠目のindexを取得する
            firstExSupportSlotIndex = slots.First(x => x.isExtraSupport).index;
            // Exサポートの編成を取得
            long[] exMemberIds = deckData.GetExMemberIds(DeckSlotCardType.SpecialSupport);
            // Exサポートの対象レアリティが何枚入っているか取得する
            for (int i = 0; i < exMemberIds.Length; i++)
            {
                // 選択中の枠もしくは空枠はスキップ
                if (i == index - firstExSupportSlotIndex || exMemberIds[i] == EmptyDeckSlotId)
                {
                    continue;
                }
                long rarityId = UserDataManager.Instance.chara.data[exMemberIds[i]].MChara.mRarityId;
                switch (operatorType)
                {
                    case DeckOperatorType.EQ:
                        if(MasterManager.Instance.rarityMaster.FindData(rarityId).value == rare)
                        {
                            exSpecialSupportCount++;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return specialSupportCount + exSpecialSupportCount;
        }
    }