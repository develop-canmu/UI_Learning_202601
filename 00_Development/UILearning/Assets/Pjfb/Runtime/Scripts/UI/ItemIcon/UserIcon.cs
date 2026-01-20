using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb
{
    public class UserIcon : ItemIconBase
    {
        public override ItemIconType IconType { get { return ItemIconType.UserIcon; } }
        
        [SerializeField]
        private bool openDetailModal = true;

        [SerializeField] private ImageEffect imageEffect;
        
        [SerializeField] private RarityImage rarityImage;
        private long iconId = 0;
        
        protected override void OnSetId(long id)
        {
            base.OnSetId(id);
            iconId = id;
        }
        
        /// <summary>テクスチャ更新</summary>
        protected override async UniTask SetIconTextureAsync(long iconId)
        {
            await base.SetIconTextureAsync(iconId);

            var iconMasterObject = MasterManager.Instance.iconMaster.FindData(iconId);
            var effectId = iconMasterObject?.imageEffectId ?? 0;
            imageEffect.SetMask(RawImage.texture);
            if (rarityImage != null && rarityImage.gameObject.activeSelf)
            {
                await rarityImage.SetTextureAsync(iconMasterObject?.mRarityId ?? 0);
            }
            await imageEffect.LoadEffect(effectId);
        }
        
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            UserIconDetailModalWindow.WindowParams data = new UserIconDetailModalWindow.WindowParams()
            {
                Id = iconId,
                onClosed = null,
            };
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserIconDetail, data);
        }

        protected override void OnPreLoadTexture()
        {
            base.OnPreLoadTexture();
            if(imageEffect.EffectRoot != null)
            {
                imageEffect.EffectRoot.gameObject.SetActive(false);
            }
        }

        protected override void OnPostLoadTexture()
        {
            base.OnPostLoadTexture();
            if(imageEffect.EffectRoot != null)
            {
                imageEffect.EffectRoot?.gameObject.SetActive(true);
            }
        }
    }
}