using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class ProfilePartIcon : ItemIconBase
    {
        public override ItemIconType IconType { get; } = ItemIconType.ProfilePart;
        
        [SerializeField]
        private MyBadgeImage badgeImage;
        [SerializeField]
        private bool isOpenDetailModal = true;
        
        /// <summary>アイテムの種類</summary>
        private ProfilePartMasterObject.ProfilePartType profilePartType = 0;
        /// <summary>獲得するアイテムのID</summary>
        private long itemId = 0;
        
        /// <summary>マイバッチか</summary>
        private bool IsEmblem => profilePartType == ProfilePartMasterObject.ProfilePartType.Emblem;

        protected override async UniTask SetIconTextureAsync(long id)
        {
            // マスタ取得
            ProfilePartMasterObject master = MasterManager.Instance.profilePartMaster.FindData(id);
            // 値のキャッシュ
            profilePartType = master.partType;
            // imageId = 獲得するアイテムのid
            itemId = master.imageId;
            
            // バッジ画像の表示切り替え
            badgeImage.gameObject.SetActive(IsEmblem);
            RawImage.gameObject.SetActive(!IsEmblem);
            
            // バッチだけ別で表示する
            if (IsEmblem)
            {
                await badgeImage.SetTextureAsync(itemId);
                return;
            }
            
            await base.SetIconTextureAsync(id);
        }
        
        /// <summary>UGUI</summary>
        public void OnLongTap()
        {
            if (!isOpenDetailModal) return;
            OnLongTapAsync().Forget();
        }

        private async UniTask OnLongTapAsync()
        {
            switch (profilePartType)
            {
                case ProfilePartMasterObject.ProfilePartType.DisplayCharacter:
                    await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCardDisplayCharacterDetail, new TrainerCardDisplayCharacterDetailModal.ModalData(itemId), destroyCancellationToken);
                    break;
                case ProfilePartMasterObject.ProfilePartType.ProfileFrame:
                    await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCardCustomizeFrameDetail, new TrainerCardCustomizeFrameDetailModal.ModalData(itemId), destroyCancellationToken);
                    break;
                case ProfilePartMasterObject.ProfilePartType.Emblem:
                    await badgeImage.OnLongTapAsync();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}