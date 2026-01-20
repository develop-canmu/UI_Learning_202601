using System;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Training
{ 
    public class DeckSupportEquipmentScrollData
    {
        public DeckSupportEquipmentScrollData(int order, bool haveLimit, Action<int> onSelected, long id = DeckUtility.EmptyDeckSlotId, SupportEquipmentDetailData detailData = null)
        {
            this.order = order;
            this.haveLimit = haveLimit;
            this.onSelected = onSelected;
            this.id = id;
            DetailData = detailData;
        }
        
        /// <summary>デッキ内の位置</summary>
        private int order;
        public int Order => order;
        
        /// <summary>編成制限</summary>
        private bool haveLimit;
        public bool HaveLimit => haveLimit;
        
        /// <summary>選択時コールバック</summary>
        private Action<int> onSelected;
        public Action<int> OnSelected => onSelected;

        /// <summary>サポート器具Id</summary>
        private long id;
        public long Id
        {
            get => id;
            set
            {
                id = value;
                if(id == DeckUtility.EmptyDeckSlotId)
                {
                    mCharaId = DeckUtility.EmptyDeckSlotId;
                }
                else
                {
                    mCharaId = CharacterUtility.UserEquipmentIdToMCharId(id);
                }
            }
        }

        /// <summary>枠のロック</summary>
        private long lockLevel;

        public long LockLevel
        {
            get => lockLevel;
            set => lockLevel = value;
        }

        private long mCharaId = DeckUtility.EmptyDeckSlotId; 
        
        public CharacterType CharType
        {
            get
            {
                if(mCharaId == DeckUtility.EmptyDeckSlotId)
                {
                    return CharacterType.None;
                }
                return MasterManager.Instance.charaMaster.FindData(mCharaId).charaType;
            }
        }

        private long supportEquipmentLockId = 0;
        public long SupportEquipmentLockId
        {
            get => supportEquipmentLockId;
            set => supportEquipmentLockId = value;
        }
        
        private SwipeableParams<SupportEquipmentDetailData> detailOrderList;

        public SwipeableParams<SupportEquipmentDetailData> DetailOrderList
        {
            get => detailOrderList;
            set => detailOrderList = value;
        }
        
        public SupportEquipmentDetailData DetailData { get; }
    }
    
    public class DeckSupportEquipmentScrollItem : ScrollGridItem
    {
        [SerializeField]
        private TrainingSupportEquipmentDeckEquipmentView equipmentView = null;
        
        private DeckSupportEquipmentScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            SetData((DeckSupportEquipmentScrollData)value);
        }

        public void SetData(DeckSupportEquipmentScrollData data)
        {
            scrollData = data;
            // デッキ内の位置
            equipmentView.SetOrder(scrollData.Order);
            // 選択時コールバック
            equipmentView.SetOnSelected(scrollData.OnSelected);
            // 編成制限
            equipmentView.SetLimit(scrollData.HaveLimit);
            
            // サポート器具ID
            // ユーザーデータにある場合はSupportEquipmentDetailDataを設定しない
            if (scrollData.DetailData == null)
            {
                equipmentView.SetUserEquipmentId(scrollData.Id).Forget();
            }
            // ユーザーデータにない場合はSupportEquipmentDetailDataを使ってアイコンをつける
            else
            { 
                equipmentView.SetIconAsyncByDetailData(scrollData.DetailData).Forget();
            }
            
            // 枠のロック
            equipmentView.SetLockLevel(scrollData.LockLevel);
            
            if (scrollData.SupportEquipmentLockId > 0)
            {
                equipmentView.SetLock(scrollData.SupportEquipmentLockId);
            }

            if (scrollData.DetailOrderList != null)
            {
                equipmentView.SetDetailOrderList(scrollData.DetailOrderList);
            }
        }
        
    }
}