using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.ClubRoyal;
using Pjfb.Deck;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class SuccessCharaTopPage : Page
    {
        [SerializeField]
        private GameObject adviserDeckLockObject;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // アドバイザーデッキのロック状態を確認
            long systemNum = (long)SystemUnlockDataManager.SystemUnlockNumber.ClubRoyalAdviser;
            adviserDeckLockObject.SetActive(UserDataManager.Instance.IsUnlockSystem(systemNum) == false || SystemUnlockDataManager.Instance.IsUnlockingSystem(systemNum));
            
            return base.OnPreOpen(args, token);
        }
        
        public void OnClickDeckEditButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Deck, true, null);
        }
        
        public void OnClickListButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SuccessCharaList, true, null);
        }
        
        public void OnClickClubDeckEditButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubDeck, true, null);
        }

        public void OnClickLeagueMatchDeckEditButton()
        {
            var param = new DeckPageParameters{InitialPartyNumber = 1101, OpenFrom = PageType.Character};
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.LeagueMatchDeck, true, param);
        }
        
        public void OnClickClubRoyalDeckEditButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubRoyalDeck, true, null);
        }
        
        public void OnClickClubRoyalAdviserDeckEditButton()
        {
            SystemLockMasterObject systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber((long)SystemUnlockDataManager.SystemUnlockNumber.ClubRoyalAdviser);
            if(UserDataManager.Instance.IsUnlockSystem(systemLock.systemNumber) == false || SystemUnlockDataManager.Instance.IsUnlockingSystem(systemLock.systemNumber))
            {
                string description = systemLock.description;
                ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.AdviserDeck, true, null);
        }
    }
}

