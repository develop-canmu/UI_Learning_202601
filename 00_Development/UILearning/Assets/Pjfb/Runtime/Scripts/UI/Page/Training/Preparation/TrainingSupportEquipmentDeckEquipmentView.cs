using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;

using System;
using System.Linq;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingSupportEquipmentDeckEquipmentView : MonoBehaviour
    {
        [SerializeField]
        private SupportEquipmentIcon icon = null;
     
        [SerializeField]
        private TrainingSupportEquipmentAddIcon addIcon = null;
        
        [SerializeField]
        private GameObject lockRoot = null;
        [SerializeField]
        private TMPro.TMP_Text lockText = null;
        
        // デッキの位置
        private int order = 0;
        // 選択時
        private Action<int> onSelected = null;

        private long lockNumber = 0;
        
        private bool limited = false;
        
        
        /// <summary>デッキの位置設定</summary>
        public void SetOrder(int i)
        {
            order = i;
        }
        
        /// <summary>選択時コールバック</summary>
        public void SetOnSelected(Action<int> action)
        {
            onSelected = action;
        }

        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            onSelected(order);
        }
        
        public void SetLimit(bool isLimited)
        {
            limited = isLimited;
        }
        
        /// <summary>ユーザーデータの表示</summary>
        public async UniTask SetUserEquipmentId(long id)
        {
            // Empty
            if(id == DeckUtility.EmptyDeckSlotId)
            {
                addIcon.gameObject.SetActive(true);
                await addIcon.SetTextureAsync(TrainingSupportEquipmentAddIcon.GetPath(limited));
                icon.gameObject.SetActive(false);
                return;
            }
            
            addIcon.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
            // アイコン設定
            icon.SetIconByUEquipmentId(id);
        }
        
        /// <summary>自キャラではない場合のアイコン設定</summary>
        public async UniTask SetIconAsyncByDetailData(SupportEquipmentDetailData detailData)
        { 
            addIcon.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
            await icon.SetIconAsync(detailData);
        }
        
        public void SetLock(long number)
        {
            lockRoot.SetActive(true);
            // テキスト
            lockText.text = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(number).description;
            lockNumber = number;
        }
        
        public void SetLockLevel(long lv)
        {
            // ロックなし
            if(lv <= 0)
            {
                lockRoot.SetActive(false);
                return;
            }
            
            lockRoot.SetActive(true);
            // テキスト
            lockText.text = string.Format(StringValueAssetLoader.Instance["character.support_equipment_.lock"], lv);
        }
        
        public void SetDetailOrderList(SwipeableParams<SupportEquipmentDetailData> swipeableParams)
        {
            icon.SwipeableParams = swipeableParams;
        }
        
        /// <summary>ロック時、解放条件ダイアログ表示</summary>
        public void OnLockButton()
        {
            SystemLockMasterObject mSystemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(lockNumber);
            
            // マスタなし
            if(mSystemLock == null)return;
            
            // ダイアログテキスト
            string description = mSystemLock.description;
            
            // モーダル表示
            ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
            ConfirmModalData data = new ConfirmModalData( StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
    }
}