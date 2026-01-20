using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;
using DG.Tweening;
using Pjfb.Character;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pjfb
{
    public enum SupportEquipmentScrollDataOptions
    {
        None = 0,
        Formatting = 1<<0,
    }
    
    public class SupportEquipmentScrollData
    {
        public SupportEquipmentScrollData(UserDataSupportEquipment supportEquipment,
            SwipeableParams<SupportEquipmentDetailData> swipeableParams = null,
            Func<float> getSelectEffectNormalizedTime = null,
            Func<float> getFavoriteEffectNormalizedTime = null,
            SupportEquipmentScrollDataOptions options = SupportEquipmentScrollDataOptions.None,
            Action onUpdateBadge = null,Action<int,bool> onChangeFavorite = null, Action onSellSupportEquipment = null, Action onRedrawSupportEquipment = null, bool isLimit = false)
        {
            Id = supportEquipment.id;
            MasterId = supportEquipment.masterId;
            MCharaId = supportEquipment.charaId;
            IsFavorite = supportEquipment.isLocked;
            GetSelectEffectNormalizedTime = getSelectEffectNormalizedTime;
            GetFavoriteEffectNormalizedTime = getFavoriteEffectNormalizedTime;
            this.options = options;
            OnUpdateBadge = onUpdateBadge;
            OnChangeFavorite = onChangeFavorite;
            OnSellSupportEquipment = onSellSupportEquipment;
            OnRedrawSupportEquipment = onRedrawSupportEquipment;
            SwipeableParams = swipeableParams;
            IsLimit = isLimit;
        }
        public readonly long Id;
        public readonly long MasterId;
        public readonly long MCharaId;

        public UserDataSupportEquipment USupportEquipment => UserDataManager.Instance.supportEquipment.Find(Id);
        public CharaMasterObject MChara => MasterManager.Instance.charaMaster.FindData(MCharaId);
        public bool IsSelecting;
        public bool IsFavorite;
        public readonly Func<float> GetSelectEffectNormalizedTime;
        public readonly Func<float> GetFavoriteEffectNormalizedTime;
        public SwipeableParams<SupportEquipmentDetailData> SwipeableParams;


        private SupportEquipmentScrollDataOptions options = SupportEquipmentScrollDataOptions.None;
        public SupportEquipmentScrollDataOptions Options{get{return options;}}
        
        public Action OnUpdateBadge;
        public Action<int,bool> OnChangeFavorite;
        public Action OnSellSupportEquipment;
        public Action OnRedrawSupportEquipment;
        
        // 編成不可
        public bool IsLimit { get; }


        public bool HasOption(SupportEquipmentScrollDataOptions option)
        {
            return (options & option) != SupportEquipmentScrollDataOptions.None;
        }
    }
    
    public class SupportEquipmentScrollItem : ScrollGridItem
    {
        [SerializeField] private SupportEquipmentIcon supportEquipmentIcon = null;
        [SerializeField] private Animator selectingAnimator;
        
        [SerializeField] private GameObject newBadge = null;
        [SerializeField] private GameObject formattingBadge = null;
        [SerializeField] private GameObject favoriteRoot;
        [SerializeField] private Animator favoriteAnimator;
        [SerializeField] private GameObject NonPossessionObject;
        [SerializeField] private ImageCrossfade NonPossessionImage;
        [SerializeField] private Sprite cantSelectSprite = null;
        
        public SupportEquipmentScrollData supportEquipmentScrollData = null;
        


        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            TriggerEvent(supportEquipmentScrollData);
        }
        protected override void OnSetView(object value)
        {
            supportEquipmentScrollData = (SupportEquipmentScrollData)value; 
            supportEquipmentIcon.SetIcon(supportEquipmentScrollData);
            supportEquipmentIcon.SwipeableParams = supportEquipmentScrollData.SwipeableParams;
            
            newBadge.SetActive(SupportEquipmentManager.HasNewSupportEquipment(supportEquipmentScrollData.Id));
            formattingBadge.SetActive(supportEquipmentScrollData.HasOption(SupportEquipmentScrollDataOptions.Formatting));
            favoriteRoot.SetActive(supportEquipmentScrollData.IsFavorite);
            supportEquipmentIcon.OnUpdateBadge = () => supportEquipmentScrollData.OnUpdateBadge?.Invoke();
            supportEquipmentIcon.OnChangeScrollFavorite =  supportEquipmentScrollData.OnChangeFavorite;
            supportEquipmentIcon.OnSellSupportEquipment = () => supportEquipmentScrollData.OnSellSupportEquipment?.Invoke();
            supportEquipmentIcon.OnRedrawSupportEquipment = () => supportEquipmentScrollData.OnRedrawSupportEquipment?.Invoke();
            
            if (selectingAnimator != null)
            {
                selectingAnimator.gameObject.SetActive(supportEquipmentScrollData.IsSelecting);
                if(supportEquipmentScrollData.IsSelecting)  selectingAnimator.Play("Idle", -1, supportEquipmentScrollData.GetSelectEffectNormalizedTime?.Invoke() ?? 0);
            }
            
            if (favoriteAnimator != null)
            {
                if(supportEquipmentScrollData.IsFavorite)  favoriteAnimator.Play("Idle", -1, supportEquipmentScrollData.GetFavoriteEffectNormalizedTime?.Invoke() ?? 0);
            }

            // 編成不可ラベル
            NonPossessionImage.gameObject.SetActive(supportEquipmentScrollData.IsLimit);
            NonPossessionImage.SetSpriteList(new List<Sprite>{cantSelectSprite});
            // 編成不可UI
            NonPossessionObject.SetActive(supportEquipmentScrollData.IsLimit);
        }
    }
}