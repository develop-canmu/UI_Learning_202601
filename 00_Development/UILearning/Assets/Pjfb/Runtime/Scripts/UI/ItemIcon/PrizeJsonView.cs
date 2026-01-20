using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb
{
    public class PrizeJsonViewData
    {
        private ItemIconType itemIconType = ItemIconType.Character;
        /// <summary>アイコンタイプ</summary>
        public ItemIconType ItemIconType { get { return itemIconType; } }
        
        private long id = 0;
        /// <summary>Id</summary>
        public long Id { get { return id; } }
        
        private string description = string.Empty;
        /// <summary>説明文</summary>
        public string Description { get { return description; } }
        
        private long value = 0;
        /// <summary>個数</summary>
        public long Value 
        { 
            get { return value; }
            set { this.value = value; } 
        }
        
        private bool isNew = false;
        /// <summary>初回入手</summary>
        public bool IsNew { get { return isNew; } }

        private long lockId;
        public long LockId { get { return lockId; } }
        
        private string name = string.Empty;
        /// <summary>名称</summary>
        public string Name { get { return name; } }
        
        private long correctRate = 0;
        /// <summary>加算補正倍率</summary>
        public long CorrectRate 
        { 
            get { return correctRate; }
            set { this.correctRate = value; } 
        }
        
        private long valueOriginal = 0;
        /// <summary>補正が掛かる前の値</summary>
        public long ValueOriginal 
        { 
            get { return valueOriginal; }
            set { this.valueOriginal = value; } 
        }
        
        private Networking.App.Request.CharaVariableTrainerLotteryProcess trainerLotteryProcess = null;
        /// <summary>トレーニング器具のサブ能力表示用</summary>
        public Networking.App.Request.CharaVariableTrainerLotteryProcess TrainerLotteryProcess 
        { 
            get { return trainerLotteryProcess; }
        }

        public PrizeJsonViewData(PrizeJsonViewData other)
        {
            itemIconType = other.itemIconType;
            id = other.id;
            description = other.description;
            value = other.value;
            isNew = other.isNew;
            lockId = other.lockId;
            name = other.name;
            CorrectRate = other.CorrectRate;
            ValueOriginal = other.ValueOriginal;
            trainerLotteryProcess = other.trainerLotteryProcess;
        }
        
        public PrizeJsonViewData(PrizeJsonViewData other, int value)
        {
            itemIconType = other.itemIconType;
            id = other.id;
            description = other.description;
            this.value = value;
            isNew = other.isNew;
            lockId = other.lockId;
            name = other.name;
            CorrectRate = other.CorrectRate;
            ValueOriginal = other.ValueOriginal;
            trainerLotteryProcess = other.trainerLotteryProcess;
        }

        public PrizeJsonViewData(Networking.App.Request.PrizeJsonWrap prizeJson)
        {
            itemIconType = GetItemIconType(prizeJson.type);
            id = GetId(prizeJson.args, itemIconType);
            description = prizeJson.description;
            value = prizeJson.args.value;
            isNew = prizeJson.args.get > 0;
            lockId = prizeJson.args.lockId;
            name = GetName(id, lockId, itemIconType);
            CorrectRate = prizeJson.args.correctRate;
            ValueOriginal = prizeJson.args.valueOriginal;
            trainerLotteryProcess = prizeJson.args.lotteryProcessJson;
        }
        
        public PrizeJsonViewData(Master.PrizeJsonWrap prizeJson)
        {
            itemIconType = GetItemIconType(prizeJson.type);
            id = GetId(prizeJson.args, itemIconType);
            description = prizeJson.description;
            value = prizeJson.args.value;
            isNew = prizeJson.args.get > 0;
            lockId = prizeJson.args.lockId;
            name = GetName(id, lockId, itemIconType);
            CorrectRate = prizeJson.args.correctRate;
            ValueOriginal = prizeJson.args.valueOriginal;
            // マスタの方は一旦考慮しない
        }

        public PrizeJsonViewData(long mPointId)
        {
            itemIconType = ItemIconType.Item;
            id = mPointId;
            name = GetName(id, itemIconType);
        }

        private string GetName(long id, long lockId, ItemIconType itemIconType)
        {
            return $"{(lockId > 0 ? PrizeJsonUtility.LockedText : string.Empty)}{GetName(id, itemIconType)}";
        }
        
        private string GetName(long id, ItemIconType itemIconType)
        {
            switch (itemIconType)
            {
                case ItemIconType.Item: return MasterManager.Instance.pointMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.Character: return MasterManager.Instance.charaMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.VariableCharacter: return MasterManager.Instance.charaMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.CharacterPiece: return MasterManager.Instance.charaMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.UserIcon: return MasterManager.Instance.iconMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.UserTitle: return MasterManager.Instance.titleMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.ChatStamp: return MasterManager.Instance.chatStampMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.SupportEquipment: return MasterManager.Instance.charaMaster.FindData(id)?.name ?? string.Empty;
                case ItemIconType.ProfilePart:
                    ProfilePartMasterObject profilePartMasterObject = MasterManager.Instance.profilePartMaster.FindData(id);
                    switch (profilePartMasterObject.partType)
                    {
                        case ProfilePartMasterObject.ProfilePartType.DisplayCharacter:
                            return MasterManager.Instance.profileCharaMaster.FindData(profilePartMasterObject.imageId)?.name ?? string.Empty;
                        case ProfilePartMasterObject.ProfilePartType.ProfileFrame:
                            return MasterManager.Instance.profileFrameMaster.FindData(profilePartMasterObject.imageId)?.name ?? string.Empty;
                        case ProfilePartMasterObject.ProfilePartType.Emblem:
                            return MasterManager.Instance.emblemMaster.FindData(profilePartMasterObject.imageId)?.name ?? string.Empty;
                        default:
                            throw new Exception($"{profilePartMasterObject.type}が定義されてません。");
                    }
                default: throw new Exception($"{itemIconType}が定義されてません。");
            }
        }
        
        /// <summary>アイコンタイプを取得</summary>
        private ItemIconType GetItemIconType(string type)
        {
            switch (type)
            {
                case "point": return ItemIconType.Item;
                case "chara": return ItemIconType.Character;
                case "charaPiece": return ItemIconType.CharacterPiece;
                case "charaVariable": return ItemIconType.VariableCharacter;
                case "icon": return ItemIconType.UserIcon;
                case "title": return ItemIconType.UserTitle;
                case "chatStamp": return ItemIconType.ChatStamp;
                case "charaVariableTrainer": return ItemIconType.SupportEquipment;
                case "profilePart" : return ItemIconType.ProfilePart;
                default: throw new Exception($"{type}が定義されてません。");
            }
        }
        
        /// <summary>アイコンIdを取得</summary>
        private long GetId(Networking.App.Request.PrizeJsonArgs args, ItemIconType itemIconType)
        {
            switch (itemIconType)
            {
                case ItemIconType.Item: return args.mPointId;
                case ItemIconType.Character: return args.mCharaId;
                case ItemIconType.VariableCharacter: return args.variableMCharaId;
                case ItemIconType.CharacterPiece: return args.pieceMCharaId;
                case ItemIconType.UserIcon: return args.mIconId;
                case ItemIconType.UserTitle: return args.mTitleId;
                case ItemIconType.ChatStamp: return args.mChatStampId;
                case ItemIconType.SupportEquipment: return args.variableTrainerMCharaId;
                case ItemIconType.ProfilePart: return args.mProfilePartId;
                default: throw new Exception($"{itemIconType}が定義されてません。");
            }
        }
        
        /// <summary>アイコンIdを取得</summary>
        private long GetId(Master.PrizeJsonArgs args, ItemIconType itemIconType)
        {
            switch (itemIconType)
            {
                case ItemIconType.Item: return args.mPointId;
                case ItemIconType.Character: return args.mCharaId;
                case ItemIconType.VariableCharacter: return args.variableMCharaId;
                case ItemIconType.CharacterPiece: return args.pieceMCharaId;
                case ItemIconType.UserIcon: return args.mIconId;
                case ItemIconType.UserTitle: return args.mTitleId;
                case ItemIconType.ChatStamp: return args.mChatStampId;
                case ItemIconType.SupportEquipment: return args.variableTrainerMCharaId;
                case ItemIconType.ProfilePart: return args.mProfilePartId;
                default: throw new Exception($"{itemIconType}が定義されてません。");
            }
        }
    }
    
    public class PrizeJsonView : MonoBehaviour
    {
        [SerializeField]
        private bool showNewIcon = true;
    
        [SerializeField]
        private ItemIconContainer itemIconContainer = null;
        
        [SerializeField]
        private GameObject newIcon = null;
        [SerializeField]
        private GameObject lockedItemIcon = null;
        
        /// <summary>アイコンタイプ</summary>
        public ItemIconType IconType { get { return itemIconContainer.IconType; } }
        
        /// <summary>アイコン取得</summary>
        public ItemIconBase GetIcon()
        {
            return itemIconContainer.GetIcon();
        }
        
        /// <summary>アイコン取得</summary>
        public T GetIcon<T>() where T : ItemIconBase
        {
            return (T)GetIcon();
        }
        
        /// <summary>データセット</summary>
        public void SetView(PrizeJsonViewData data)
        {
            itemIconContainer.SetIcon(data.ItemIconType, data.Id);
            
            // New表示
            if(showNewIcon)
            {
                newIcon.SetActive(data.IsNew);
            }
            lockedItemIcon.SetActive(data.LockId > 0);
            // キャラアイコン
            if(data.ItemIconType == ItemIconType.Character)
            {
                CharacterIcon characterIcon = GetIcon<CharacterIcon>();
                characterIcon.SetIcon(data.Id, 1, 0, true);
            }
            // サポート器具
            if(data.ItemIconType == ItemIconType.SupportEquipment)
            {
                CharacterIcon characterIcon = GetIcon<CharacterIcon>();
                characterIcon.SetIcon(data.Id, 1, 0, true);
                if(data.TrainerLotteryProcess != null)
                {
                    characterIcon.SetStatusIdList(data.TrainerLotteryProcess.statusList);
                }
            }
            
            // 個数セット
            SetCount(data.Value);
        }
        
        /// <summary>データセット</summary>
        public void SetView(Networking.App.Request.PrizeJsonWrap prizeJson)
        {
            SetView(new PrizeJsonViewData(prizeJson));
        }
        
        /// <summary>データセット</summary>
        public void SetView(Master.PrizeJsonWrap prizeJson)
        {
            SetView(new PrizeJsonViewData(prizeJson));
        }

        public void SetCount(long value)
        {
            itemIconContainer.SetCount(value);
        }
        
        public void SetCountTextColor(Color color)
        {
            itemIconContainer.SetCountTextColor(color);
        }

        /// <summary>newアイコン有効を外部から設定</summary>
        public void SetNewIconActive(bool isActive)
        {
            newIcon.SetActive(isActive);
        }
    }
}
