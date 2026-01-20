using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class SupportEquipmentRedrawingEffectModal : ModalWindow
    {
        public class Data
        {
            public UserDataSupportEquipment PrevUserDataSupportEquipment;
            public UserDataSupportEquipment CurrentUserDataSupportEquipment;
            public Action OnSetCloseParameter;

            private CharaTrainerLotteryReloadMasterObject reloadMaster;
            public CharaTrainerLotteryReloadMasterObject ReloadMaster{get => reloadMaster;}

            private long[] updateNumberList;
            public long[] UpdateNumberList{get => updateNumberList;}
            
            // 抽選完了時の処理
            private Action<UserDataSupportEquipment, long> onCompleteRedraw = null;
            public Action<UserDataSupportEquipment, long> OnCompleteRedraw {get => onCompleteRedraw;}
            
            public Data(UserDataSupportEquipment prevUserDataSupportEquipment,UserDataSupportEquipment currentUserDataSupportEquipment, Action onSetCloseParameter,  CharaTrainerLotteryReloadMasterObject reloadMaster, long[] updateNumberList, Action<UserDataSupportEquipment, long> onCompleteRedraw = null)
            {
                PrevUserDataSupportEquipment = prevUserDataSupportEquipment;
                CurrentUserDataSupportEquipment = currentUserDataSupportEquipment;
                OnSetCloseParameter = onSetCloseParameter;
                this.reloadMaster = reloadMaster;
                this.updateNumberList = updateNumberList;
                this.onCompleteRedraw = onCompleteRedraw;
            }
        }
        
        [SerializeField]
        private SupportEquipmentRedrawingEffect effect = null;
        
        [SerializeField] 
        private GameObject skipButton;
        
        private Data modalData;
        private bool isSkip;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentRedrawingEffect, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            isSkip = false;
            effect.Init(modalData, () =>
            {
                if(isSkip) return;
                //Closeアニメーションの最後にskipButtonを非アクティブにする
                skipButton.SetActive(false);
                //Closeアニメーションの最後に次のモーダルを開く
                SupportEquipmentRedrawingResultModal.Open(new SupportEquipmentRedrawingResultModal.Data(
                    modalData.PrevUserDataSupportEquipment, modalData.CurrentUserDataSupportEquipment,
                    () => modalData.OnSetCloseParameter?.Invoke(),
                    modalData.ReloadMaster, modalData.UpdateNumberList, modalData.OnCompleteRedraw));
            });
            skipButton.SetActive(true);
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            effect.gameObject.SetActive(true);
            effect.Open();
        }

        public async void OnClickSkipButton()
        {
            isSkip = true;
            skipButton.SetActive(false);
            await effect.Close();
            SupportEquipmentRedrawingResultModal.Open(new SupportEquipmentRedrawingResultModal.Data(
                modalData.PrevUserDataSupportEquipment, modalData.CurrentUserDataSupportEquipment,
                () => modalData.OnSetCloseParameter?.Invoke(),
                modalData.ReloadMaster, modalData.UpdateNumberList, modalData.OnCompleteRedraw));
        }
    }
}