using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class UserTitleImage : CancellableRawImageWithId
    {
        [SerializeField] private ImageEffect ImageEffect;
        [SerializeField] private RarityImage rarityImage;

        private async UniTask SetEffect(long id)
        {
            var iconMasterObject = MasterManager.Instance.titleMaster.FindData(id);
            var effectId = iconMasterObject?.imageEffectId ?? 0;
            ImageEffect.SetMask(RawImage.texture);
            await ImageEffect.LoadEffect(effectId);
        }
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetUserTitleImagePath(id);
        }

        /// <summary>画像セット</summary>
        public override async UniTask SetTextureAsync(long id, ResourcesLoader resourcesLoader)
        {
            if (rarityImage.gameObject.activeSelf)
            {
                TitleMasterObject master = MasterManager.Instance.titleMaster.FindData(id);
                await rarityImage.SetTextureAsync(master?.mRarityId ?? 0);
            }
            await base.SetTextureAsync(id, resourcesLoader);
            await SetEffect(id);
        }

        protected override void OnPreLoadTexture()
        {
            base.OnPreLoadTexture();
            if(ImageEffect.EffectRoot != null)
            {
                ImageEffect.EffectRoot.gameObject.SetActive(false);
            }
        }

        protected override void OnPostLoadTexture()
        {
            base.OnPostLoadTexture();
            if(ImageEffect.EffectRoot != null)
            {
                ImageEffect.EffectRoot.gameObject.SetActive(true);
            }
        }
    }
}