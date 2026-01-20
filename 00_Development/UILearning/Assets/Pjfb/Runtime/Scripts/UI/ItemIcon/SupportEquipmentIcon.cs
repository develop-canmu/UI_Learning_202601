using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class SupportEquipmentIcon : ItemIconBase
    {
        public override ItemIconType IconType => ItemIconType.SupportEquipment;
        
        
        
        // public UserDataSupportEquipment USupportEquipment => uSupportEquipment;
        
        [SerializeField]
        private bool openDetailModal = true;
        /// <summary>詳細を開ける</summary>
        public bool OpenDetailModal
        {
            get => openDetailModal;
            set => openDetailModal = value;
        }
        
        [SerializeField]
        private bool canRedrawing = true;
        [SerializeField]
        private bool canSell = true;

        /// <summary>
        /// If true, skip favorite settings.
        /// </summary>
        [SerializeField] private bool overrideFavoriteSetting;
        [SerializeField] private RarityImage rarityImage = null;
        [SerializeField] private CharacterCharacterTypeImage characterTypeIcon = null;
        [SerializeField] private ImageEffect iconImageEffect = null;
        [SerializeField] private GameObject rarityRoot = null;
        [SerializeField] private GameObject countRoot;
        [SerializeField] private TextMeshProUGUI countText;

        [SerializeField] private GameObject favoriteRoot = null;
        [SerializeField] private bool canChangeFavorite = true;
        [SerializeField] private CruEvent<SupportEquipmentDetailModal.CloseUpdateType> onCloseDetailModal = new CruEvent<SupportEquipmentDetailModal.CloseUpdateType>();

        public Action OnUpdateBadge;
        public Action<int,bool> OnChangeScrollFavorite;
        public Action OnSellSupportEquipment;
        public Action OnRedrawSupportEquipment;
        public Action<int> OnDetailModalIndexChange;
        
        private SupportEquipmentDetailData iconData = null;
        private UserDataSupportEquipment uSupportEquipment;
        private CharaMasterObject mSupportEquipment;
        
        public async UniTask SetIconAsync(SupportEquipmentDetailData detailData)
        {
            iconData = detailData;
            uSupportEquipment = UserDataManager.Instance.supportEquipment.Find(detailData.USupportEquipmentId);
            if(!overrideFavoriteSetting && favoriteRoot != null && uSupportEquipment != null) favoriteRoot.SetActive(uSupportEquipment.isLocked);
            
            mSupportEquipment = MasterManager.Instance.charaMaster.FindData(detailData.MCharaId);
            
            List<UniTask> taskList = new List<UniTask>();
            
            long rarityId = MasterManager.Instance.rarityMaster.FindData(mSupportEquipment.mRarityId).value;
            taskList.Add(SetRarityAsync(rarityId));

            // キャラタイプ
            taskList.Add(SetCharacterTypeIconAsync(mSupportEquipment.charaType));
            // アイコン
            taskList.Add(SetIconIdAsync(detailData.MCharaId));
            // エフェクト
            taskList.Add(SetEffectAsync(detailData.MCharaId));
            // 待機
            await UniTask.WhenAll(taskList);
        }
        
        public async UniTask SetIconAsync(long mCharId, long lv, long[] statusIdList)
        {
            iconData = new SupportEquipmentDetailData(-1, mCharId, lv, statusIdList);
            await SetIconAsync(iconData);
        }
        
      
        
        public async UniTask SetIconAsync(UserDataSupportEquipment uSupportEquipment)
        {
            iconData = new SupportEquipmentDetailData(uSupportEquipment);
            await SetIconAsync(iconData);
        }
        
        
        public async UniTask SetIconAsync(SupportEquipmentScrollData scrollData)
        {
            uSupportEquipment = UserDataManager.Instance.supportEquipment.Find(scrollData.Id);
            iconData = new SupportEquipmentDetailData(uSupportEquipment);
            await SetIconAsync(iconData);
        }
        
        
        /// <summary>アイコンセット</summary>
        public void SetIcon(SupportEquipmentScrollData data)
        {
            SetIconAsync(data).Forget();
        }
        
        public void SetIconByUEquipmentId(long uEquipmentId)
        {
            uSupportEquipment = UserDataManager.Instance.supportEquipment.Find(uEquipmentId);
            iconData = new SupportEquipmentDetailData(uSupportEquipment);
            SetIconAsync(iconData).Forget();
        }
        
        
        /// <summary>レアリティのアクティブ</summary>
        public void SetActiveRarity(bool value)
        {
            rarityRoot.SetActive(value);
        }
        
        /// <summary>レアリティの表示</summary>
        private async UniTask SetRarityAsync(long value)
        {
            SetActiveRarity(true);
            
            
            if(rarityImage != null)
            {
                rarityImage.gameObject.SetActive(true);
                await rarityImage.SetTextureAsync(value);
            }
        }
        
        private async UniTask SetEffectAsync(long mCharaId)
        {
            // エフェクト
            if (iconImageEffect != null)
            {
                long effectId = MasterManager.Instance.charaMaster.FindData(mCharaId)?.imageEffectId ?? 0;
                
                // エフェクトを持っているか
                bool hasEffect = effectId != 0;
                iconImageEffect.gameObject.SetActive(hasEffect);
                
                if (hasEffect)
                {
                    await iconImageEffect.LoadEffect(effectId);
                }
            }
        }
        
        /// <summary>タイプアイコンの表示</summary>
        private UniTask SetCharacterTypeIconAsync(CharacterType characterType)
        {
            SetActiveCharacterTypeIcon(true);
            return characterTypeIcon.SetTextureAsync(characterType);
        }
        
        /// <summary>タイプアイコンのアクティブ</summary>
        public void SetActiveCharacterTypeIcon(bool value)
        {
            characterTypeIcon.gameObject.SetActive(value);
        }
        
        public SwipeableParams<SupportEquipmentDetailData> SwipeableParams = new();
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            //TODO:動作確認と修正
            bool clearSwipeableParams = false;
            SwipeableParams ??= new SwipeableParams<SupportEquipmentDetailData>();
            
            //Scroll以外からのコールバック追加
            if (OnDetailModalIndexChange != null && SwipeableParams.OnIndexChanged == null)
            {
                SwipeableParams.OnIndexChanged = OnDetailModalIndexChange;
            }
            
            if (SwipeableParams?.DetailOrderList == null || SwipeableParams.DetailOrderList.Count == 0 || SwipeableParams.StartIndex == -1)
            {
                if(iconData == null)    return;
                SwipeableParams.StartIndex = 0;
                SwipeableParams.DetailOrderList = new List<SupportEquipmentDetailData>() { iconData };
                clearSwipeableParams = true;
            }
            
            OpenDetailModalAsync().Forget();

            if (clearSwipeableParams) SwipeableParams = null;
        }
        
        private async UniTask OpenDetailModalAsync()
        {
            SupportEquipmentDetailModalParams modalParam = new SupportEquipmentDetailModalParams(SwipeableParams, canSell, canRedrawing, false, OnUpdateBadge, canChangeFavorite ? OnChangeScrollFavorite : null, canChangeFavorite);
            CruFramework.Page.ModalWindow modal = await SupportEquipmentDetailModal.OpenModalAsync(ModalType.SupportEquipmentDetail, modalParam);
            SupportEquipmentDetailModal.CloseUpdateType closeUpdateType = (SupportEquipmentDetailModal.CloseUpdateType)await modal.WaitCloseAsync();
            switch (closeUpdateType)
            {
                case SupportEquipmentDetailModal.CloseUpdateType.Sell:
                    OnSellSupportEquipment?.Invoke();
                    break;
                case SupportEquipmentDetailModal.CloseUpdateType.Redrawing:
                    OnRedrawSupportEquipment?.Invoke();
                    break;
            }
            
            onCloseDetailModal.Invoke(closeUpdateType);
        }
        
        
        public void SetCount(int value)
        {
            SetActiveCount(true);
            countText.text = string.Format(StringValueAssetLoader.Instance["item.count_1"], value); 
        }
        
        /// <summary>Countのアクティブ</summary>
        public void SetActiveCount(bool value)
        {
            if (countRoot != null) countRoot.SetActive(value);
        }
        
        public void SetCountTextColor(Color color)
        {
            countText.color = color;
        }

        private void OnEnable()
        {
            //登録
            favorite.OnChangeFavorite += SwitchFavoriteIcon;
        }

        private void OnDisable()
        {
            //削除
            favorite.OnChangeFavorite -= SwitchFavoriteIcon;
        }

        private void SwitchFavoriteIcon()
        {
            if (iconData != null)
            {
                uSupportEquipment = UserDataManager.Instance.supportEquipment.Find(iconData.USupportEquipmentId);
                
                favoriteRoot.SetActive(uSupportEquipment.isLocked);
            }
        }
    }
}