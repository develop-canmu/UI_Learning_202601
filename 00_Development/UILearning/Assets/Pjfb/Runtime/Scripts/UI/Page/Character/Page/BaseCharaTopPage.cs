using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class BaseCharaTopPage : Page
    {
        [SerializeField] private GameObject growthLiberationBadge;
        [SerializeField] private GameObject pieceToCharaBadge;

        [SerializeField]
        private GameObject adviserGrowthLiberationBadge = null;

        [SerializeField]
        private GameObject adviserPieceToCharaBadge = null;
        
        [SerializeField]
        private GameObject adviserComposeButtonLockObject = null;
        
        [SerializeField]
        private GameObject adviserListButtonLockObject = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 強化/能力解放できる場合はバッジをつける
            growthLiberationBadge.SetActive(BadgeUtility.IsBaseCharacterGrowthOrLiberationBadge);
            // 解放できる場合はバッジをつける
            pieceToCharaBadge.SetActive(BadgeUtility.IsBaseCharacterPieceToCharaBadge);
            
            // 強化・能力解放可能ならバッジをオンに
            adviserGrowthLiberationBadge.SetActive(BadgeUtility.IsAdviserGrowthOrLiberationBadge);
            // キャラ解放可能ならバッジをオンに
            adviserPieceToCharaBadge.SetActive(BadgeUtility.IsAdviserPieceToCharaBadge);
            
            // アドバイザークラロワ編成のロック状態を確認
            SystemLockMasterObject systemLock = MasterManager.Instance.systemLockMaster.GetFastReleaseSystemLock((long)SystemUnlockDataManager.SystemUnlockNumber.ClubRoyalAdviser, (long)SystemUnlockDataManager.SystemUnlockNumber.TrainingAdviser);
            bool isAdviserLock = UserDataManager.Instance.IsUnlockSystem(systemLock.systemNumber) == false || SystemUnlockDataManager.Instance.IsUnlockingSystem(systemLock.systemNumber);
            // どちらかが解放されていればアドバイザーを利用可能にする
            adviserComposeButtonLockObject.SetActive(isAdviserLock);
            adviserListButtonLockObject.SetActive(isAdviserLock);
            
            return base.OnPreOpen(args, token);
        }

        public void OnClickComposeButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.BaseCharaGrowthLiberationList, true, null);
        }
        
        public void OnClickListButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.BaseCharaList, true, null);
        }

        /// <summary> アドバイザー強化、能力解放画面 </summary>
        public void OnClickAdviserGrowthLiberationListButton()
        {
            // アドバイザーのロック状態を確認
            CheckOpenAdviserPage(CharacterPageType.AdviserGrowthLiberationList);
        }

        /// <summary> アドバイザー一覧 </summary>
        public void OnClickAdviserListButton()
        {
            // アドバイザーのロック状態を確認
            CheckOpenAdviserPage(CharacterPageType.AdviserList);
        }

        
        // アドバイザーのロック状態を確認して、開くページ、モーダルを決定
        private void CheckOpenAdviserPage(CharacterPageType pageType)
        {
            SystemLockMasterObject systemLock = MasterManager.Instance.systemLockMaster.GetFastReleaseSystemLock((long)SystemUnlockDataManager.SystemUnlockNumber.ClubRoyalAdviser, (long)SystemUnlockDataManager.SystemUnlockNumber.TrainingAdviser);
            if(UserDataManager.Instance.IsUnlockSystem(systemLock.systemNumber) == false || SystemUnlockDataManager.Instance.IsUnlockingSystem(systemLock.systemNumber))
            {
                string description = systemLock.description;
                ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(pageType, true, null);
        }
    }
}


