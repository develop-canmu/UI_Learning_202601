using Pjfb.Master;
using Pjfb.Training;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    public class DeckLockCharacterView : TrainingDeckCharacterView
    {
        [SerializeField]
        private GameObject lockRoot = null;

        [SerializeField]
        private TMPro.TMP_Text unlockText = null;
        
        private int lockNumber = 0;
        
        public void SetLockNumber(int number)
        {
            lockNumber = number;
            // mSystemLock
            SystemLockMasterObject mSystemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(lockNumber);
            if(mSystemLock != null)
            {
                unlockText.text = mSystemLock.description;
            }
        }
        
        public void SetLockState(bool isLock)
        {
            lockRoot.SetActive(isLock);
            addButton.interactable = !isLock;
        }
        
        /// <summary>ロック状態</summary>
        public void OnLockButton()
        {
            // mSystemLock
            SystemLockMasterObject mSystemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(lockNumber);
            // マスタなし
            if(mSystemLock == null)return;
            // 説明
            string description = mSystemLock.description;
            // 説明無し
            if(string.IsNullOrEmpty(description))return;
            
            // モーダル
            ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
            ConfirmModalData data = new ConfirmModalData( StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            
        }
    }
}