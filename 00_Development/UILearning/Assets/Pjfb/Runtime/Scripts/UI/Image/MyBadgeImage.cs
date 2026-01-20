using CruFramework.Addressables;
using CruFramework.H2MD;
using UnityEngine;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Storage;

namespace Pjfb
{
    public class MyBadgeImage : CancellableRawImageWithId
    {
        [SerializeField]
        private RarityImage rarityImage;
        
        [SerializeField]
        private H2MDUIPlayer effectPlayer = null;
        
        [SerializeField]
        private bool forceImageDisplay = false;

        [SerializeField]
        private bool openDetailModal = true;
        
        [SerializeField]
        private bool displayRarity = true;
        
        private long id = 0;
        
        /// <summary>画像のパス取得</summary>
        protected override string GetKey(long id)
        {
            EmblemMasterObject master = MasterManager.Instance.emblemMaster.FindData(id);
            return PageResourceLoadUtility.GetMyBadgeImagePath(master.imageId);
        }
        
        /// <summary>エフェクトのパス取得</summary>
        private string GetEffectPath(long id)
        {
            return PageResourceLoadUtility.GetMyBadgeEffectPath(id);
        }
        
        public override async UniTask SetTextureAsync(long id, ResourcesLoader resourcesLoader)
        {
            // 非表示にしておく
            effectPlayer.gameObject.SetActive(false);
            RawImage.gameObject.SetActive(false);
            
            this.id = id;
            
            EmblemMasterObject master = MasterManager.Instance.emblemMaster.FindData(id);
            if (displayRarity)
            {
                // レアリティ表示
                await rarityImage.SetTextureAsync(master?.mRarityId ?? 0);
            }

            // エフェクトのパス
            string path = GetEffectPath(master?.imageEffectId ?? 0);
            // エフェクトが存在れば再生
            if (HasEffect(path))
            {
                // キャンセル
                Cancel();
                await resourcesLoader.LoadAssetAsync<H2MDAsset>(path,
                    h2MD =>
                    {
                        // エフェクト再生
                        effectPlayer.Load(h2MD);
                        effectPlayer.Play();
                        // エフェクトを表示
                        effectPlayer.gameObject.SetActive(true);
                    },
                    // オブジェクトが破棄されたらキャンセル
                    destroyCancellationToken );
            }
            else
            {
                await base.SetTextureAsync(id, resourcesLoader);
                RawImage.gameObject.SetActive(true);
            }
        }
        
        /// <summary>エフェクトがあるか</summary>
        private bool HasEffect(string key)
        {
            return !forceImageDisplay && IsConfigStandard() && ExistEffect(key) && effectPlayer != null;
        }
        
        /// <summary>キーがカタログにあるかどうか</summary>
        private bool ExistEffect(string key)
        {
            return AddressablesManager.HasResources<H2MDAsset>(key);
        }
        
        /// <summary>設定画面で標準設定されているか</summary>
        private bool IsConfigStandard()
        {
            return LocalSaveManager.saveData.appConfig.CharacterCardEffectType == (int)AppConfig.DisplayType.Standard;
        }
        
        /// <summary>UGUI</summary>
        public void OnLongTap()
        {
            if (openDetailModal)
            {
                OnLongTapAsync().Forget();
            }
        }
        
        public async UniTask OnLongTapAsync()
        {
            MyBadgeDetailModal.MyBadgeDetailModalParam param = new MyBadgeDetailModal.MyBadgeDetailModalParam(id);
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.MyBadgeDetail, param, destroyCancellationToken);
        }
    }
}