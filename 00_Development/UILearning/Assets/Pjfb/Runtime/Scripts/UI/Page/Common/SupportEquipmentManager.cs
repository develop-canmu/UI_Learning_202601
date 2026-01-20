using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.UserData;

namespace Pjfb
{
    public static class SupportEquipmentManager
    {
        public const int SupportEquipmentLockId = 441001;
        
        public static bool HasSupportEquipmentBadge => IsUnLockSupportEquipment() &&　HasNewSupportEquipment();
        
        public static bool IsUnLockSupportEquipment()
        {
            return UserDataManager.Instance.IsUnlockSystem(SupportEquipmentLockId) || SystemUnlockDataManager.Instance.IsUnlockingSystem(SupportEquipmentLockId);
        }
        
        public static bool HasNewSupportEquipment()
        {
            var viewedUserSupportEquipmentIdList = LocalSaveManager.saveData.viewedUserSupportEquipmentIdList;
            return UserDataManager.Instance.supportEquipment.data.Values.Any(userDataSupportEquipment =>
                HasNewSupportEquipment(userDataSupportEquipment.id));
        }
        
        public static bool HasNewSupportEquipment(long uEquipmentId)
        {
            var viewedUserSupportEquipmentIdList = LocalSaveManager.saveData.viewedUserSupportEquipmentIdList;
            return !viewedUserSupportEquipmentIdList.Contains(uEquipmentId);
        }

        private static int GetCurrentSupportEquipmentNum()
        {
            var supportEquipmentList = UserDataManager.Instance.supportEquipment.data.Values;
            return supportEquipmentList?.Count ?? 0;
        }

        private static long GetMaxSupportEquipmentNum()
        {
            return ConfigManager.Instance.uCharaVariableTrainerCountMax;
        }

        public static bool ShowOverLimitModal()
        {
            var currentNum = GetCurrentSupportEquipmentNum();
            var maxNum = GetMaxSupportEquipmentNum();
            var isOverLimit = currentNum > maxNum;

            if (isOverLimit)
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                    StringValueAssetLoader.Instance["support_equip.over_limit.title"], 
                    StringValueAssetLoader.Instance["support_equip.over_limit.body"], 
                    string.Empty, 
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => 
                    {
                        window.Close();
                        // ホームに遷移
                        if(AppManager.Instance.UIManager.PageManager.CurrentPageType != PageType.Home)
                        {
                            // モーダルクリア
                            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(_ => true);
                            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
                        }
                    })));
            }
            return isOverLimit;
        }

        #region SaveData

        public static void SaveViewedUserSupportEquipmentId(long uEquipmentId)
        {
            if (LocalSaveManager.saveData.viewedUserSupportEquipmentIdList.Contains(uEquipmentId)) return;
            
            LocalSaveManager.saveData.viewedUserSupportEquipmentIdList.Add(uEquipmentId);
            LocalSaveManager.Instance.SaveData();
        }
        
        public static void RemoveViewedUserSupportEquipmentId(long[] uEquipmentIds)
        {
            foreach (var uEquipmentId in uEquipmentIds)
            {
                if (LocalSaveManager.saveData.viewedUserSupportEquipmentIdList.Contains(uEquipmentId)) continue;
                LocalSaveManager.saveData.viewedUserSupportEquipmentIdList.Remove(uEquipmentId);
            }
            
            LocalSaveManager.Instance.SaveData();
        }
        
        #endregion
    }
}