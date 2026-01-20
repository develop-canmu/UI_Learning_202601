using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Combination;
using Pjfb.Encyclopedia;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class CharacterTopPage : Page
    {
        [SerializeField] private GameObject baseCharaBadge;
        [SerializeField] private GameObject specialSupportCardBadge;
        [SerializeField] private GameObject charaEncyclopediaBadge;
        [SerializeField] private GameObject combinationLockObject;
        [SerializeField] private GameObject combinationBadge;
        [SerializeField] private GameObject supportEquipmentObject;
        [SerializeField] private GameObject supportEquipmentBadge;
        [SerializeField] private GameObject trainingDeckEnhanceLockObject;
        [SerializeField] private GameObject trainingDeckEnhanceBadge;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
            var unlockCombination = CombinationManager.IsUnLockCombination();
            combinationLockObject.SetActive(!unlockCombination);
            var unLockSupportEquipment = SupportEquipmentManager.IsUnLockSupportEquipment();
            supportEquipmentObject.SetActive(!unLockSupportEquipment);
            if (unlockCombination)
            {
                await CombinationManager.GetCombinationCollectionTrainingBuffAPI();
            }
            
            trainingDeckEnhanceLockObject.SetActive(!TrainingDeckEnhanceUtility.IsUnLockTrainingEnhance());
            UpdateBadges();
        }

        public void UpdateBadges()
        {
            // 強化/能力解放できる場合はバッジをつける
            baseCharaBadge.SetActive(BadgeUtility.IsBaseCharacterBadge);
            // 強化/能力解放できる場合はバッジをつける
            specialSupportCardBadge.SetActive(BadgeUtility.IsSpecialSupportCardBadge);
            // 信頼度報酬を受け取れる場合はバッジをつける
            charaEncyclopediaBadge.SetActive(UserDataManager.Instance.hasTrustPrize);
            // スキルコネクトシステムが解放済みで該当のスキルのどれかにバッジがつく場合はバッジをつける
            var unlockCombination = CombinationManager.IsUnLockCombination();
            var hasCombinationBadge = unlockCombination && 
                                      (CombinationManager.HasNewCombinationMatchBadge ||
                                      CombinationManager.HasNewCombinationTrainingBadge ||
                                      CombinationManager.CanActiveCombinationCollectionBadge);
            combinationBadge.SetActive(hasCombinationBadge);
            supportEquipmentBadge.SetActive(SupportEquipmentManager.HasNewSupportEquipment());

            // 現在のレベルから次のレベルに強化できるかでバッジを表示する
            trainingDeckEnhanceBadge.SetActive(TrainingDeckEnhanceUtility.IsTrainingDeckEnhanceBadge);
        }

        public void OnClickSuccessCharacterButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SuccessCharaTop, true, null);
        }
        
        public void OnClickTrainingCharacterButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.BaseCharaTop, true, null);
        }
        
        public void OnClickSpecialSupportCardButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SpecialSupportCardList, true, null);
        }
        
        public void OnClickFriendBorrowingButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.FriendBorrowing, true, null);
        }
        
        public void OnClickCharaEncyclopediaButton()
        {
            EncyclopediaPage.OpenPage(true,null);
        }
        
        
        public void OnClickCombinationButton()
        {
            if (!CombinationManager.IsUnLockCombination())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(CombinationManager.CombinationLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.CombinationTop, true, null);
        }
        
       
        public void OnClickSupportEquipmentListButton()
        {
            if (!SupportEquipmentManager.IsUnLockSupportEquipment())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(SupportEquipmentManager.SupportEquipmentLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SupportEquipmentList, true, null);
        }

        public void OnClickTrainingDeckEnhanceButton()
        {
            // 機能解放されてない状態で開こうとしたときは解放条件を表示する
            if (!TrainingDeckEnhanceUtility.IsUnLockTrainingEnhance())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(TrainingDeckEnhanceUtility.TrainingDeckEnhanceLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["common.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.DeckEnhance, true, null);
        }
    }
}

