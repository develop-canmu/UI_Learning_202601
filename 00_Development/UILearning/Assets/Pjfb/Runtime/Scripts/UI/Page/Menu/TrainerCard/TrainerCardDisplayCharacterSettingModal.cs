using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using Pjfb.Utility;
using UnityEngine;

namespace Pjfb
{
    public class TrainerCardDisplayCharacterSettingModal : ModalWindow
    {
        public class ModalData
        {
            /// <summary>表示キャラのID</summary>
            public long DisplayCharacterId { get; }
            
            public long CharaBackGroundId { get; }
            
            public ModalData(long displayCharacterId, long charaBackGroundId)
            {
                DisplayCharacterId = displayCharacterId;
                CharaBackGroundId = charaBackGroundId;
            }
        }
        
        [SerializeField]
        private TrainerCardCharacterImageGroup displayCharacterGroup = null;
        
        [SerializeField]
        private UIBadgeNotification DisplayCharacterNewBadge = null;
        
        [SerializeField]
        private UIButton applyButton = null;
        
        private ModalData modalData = null;

        private long currentCharaId = 0;
        private long currentCharaBackGroundId = 0;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (ModalData)args;
            currentCharaId = modalData.DisplayCharacterId;
            currentCharaBackGroundId = modalData.CharaBackGroundId;
            
            // バッチの更新
            UpdateNewBadge();
            
            // 画像設定
            await displayCharacterGroup.SetTextureAsync(currentCharaId, currentCharaBackGroundId, token);
            
            // 画像更新
            await UpdateCharaTextureAsync();
            
            applyButton.interactable = false;
            
            await base.OnPreOpen(args, token);
        }

        /// <summary>バッチ更新</summary>
        private void UpdateNewBadge()
        {
            // キャラの新規表示バッジの表示
            DisplayCharacterNewBadge.SetActive(HaveNewDisplayCharacter());
        }
        
        /// <summary>未読の表示選手を持っているか</summary>
        private bool HaveNewDisplayCharacter()
        {
            foreach (ProfileCharaMasterObject masterObject in MasterManager.Instance.profileCharaMaster.values.Where(master => master.IsHave))
            {
                if (!LocalSaveManager.saveData.viewedDisplayCharaIds.Contains(masterObject.id))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>キャラ画像更新</summary>
        private async UniTask UpdateCharaTextureAsync()
        {
            await displayCharacterGroup.UpdateCharacterTextureAsync(currentCharaId);
        }
        
        /// <summary>closeParameterに情報設定</summary>
        private async UniTask OnApplyAsync()
        {
            SetCloseParameter(await TrainerCardUtility.UpdateProfileChara(currentCharaId));
            await CloseAsync();
        }
        
        /// <summary>選手選択への遷移</summary>
        private async UniTask OnClickChangeCharacterAsync()
        {
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCardDisplayCharacterList, new TrainerCardDisplayCharacterListModal.ModalData(currentCharaId), destroyCancellationToken);
            long result = (long)await modal.WaitCloseAsync(destroyCancellationToken);
            
            // バッチの更新
            UpdateNewBadge();
            
            if (result != 0)
            {
                currentCharaId = result;
                
                // ボタンの有効化
                applyButton.interactable = modalData.DisplayCharacterId != currentCharaId;
                
                // 画像更新
                await UpdateCharaTextureAsync();
            }
        }

        /// <summary>UGUI</summary>
        public void OnApply()
        {
            OnApplyAsync().Forget();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickChangeCharacter()
        {
            OnClickChangeCharacterAsync().Forget();
        }
    }
}