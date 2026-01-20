using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class SupportEquipmentRedrawingResultModal : ModalWindow
    {
        public class Data
        {
            public UserDataSupportEquipment PrevUserDataSupportEquipment;
            public UserDataSupportEquipment CurrentUserDataSupportEquipment;
            public Action OnSetCloseParameter;

            private CharaTrainerLotteryReloadMasterObject reloadMaster;
            public  CharaTrainerLotteryReloadMasterObject ReloadMaster {get => reloadMaster;}
            
            private long[] updateNumberList;
            public long[] UpdateNumberList{get => updateNumberList;}
            // 抽選完了時の処理
            private Action<UserDataSupportEquipment, long> onCompleteRedraw = null;
            public Action<UserDataSupportEquipment, long> OnCompleteRedraw {get => onCompleteRedraw;}
            
            public Data(UserDataSupportEquipment prevUserDataSupportEquipment,UserDataSupportEquipment currentUserDataSupportEquipment, Action onSetCloseParameter,  CharaTrainerLotteryReloadMasterObject reloadMaster,  long[] updateNumberList, Action<UserDataSupportEquipment, long> onCompleteRedraw = null)
            {
                PrevUserDataSupportEquipment = prevUserDataSupportEquipment;
                CurrentUserDataSupportEquipment = currentUserDataSupportEquipment;
                OnSetCloseParameter = onSetCloseParameter;
                this.reloadMaster = reloadMaster;
                this.updateNumberList = updateNumberList;
                this.onCompleteRedraw = onCompleteRedraw;
            }
        }
        
        
        [SerializeField] private SupportEquipmentNameView supportEquipmentNameView;
        [SerializeField] private ScrollDynamic prevPracticeSkillScrollDynamic;
        [SerializeField] private ScrollDynamic currentPracticeSkillScrollDynamic;
        
        private Data modalData;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentRedrawingResult, data);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            supportEquipmentNameView.InitializeUI(modalData.CurrentUserDataSupportEquipment);
            // 抽選前
            var previewSkillList = PracticeSkillLotteryUtility.GetCharaTrainerLotteryInfo(modalData.PrevUserDataSupportEquipment.lotteryProcessJson.statusList);
            prevPracticeSkillScrollDynamic.SetItems(previewSkillList);
            // 抽選後
            var currentSkillList = PracticeSkillLotteryUtility.GetCharaTrainerLotteryInfo(modalData.CurrentUserDataSupportEquipment.lotteryProcessJson.statusList);
            currentPracticeSkillScrollDynamic.SetItems(PracticeSkillLotteryUtility.GetCharaTrainerLotteryResultPracticeInfo(currentSkillList, previewSkillList, modalData.ReloadMaster, modalData.UpdateNumberList));
        }

        public void OnClickClose()
        {
            modalData.OnSetCloseParameter?.Invoke();
            // 抽選完了時のコールバック処理
            modalData.OnCompleteRedraw?.Invoke(modalData.CurrentUserDataSupportEquipment, modalData.ReloadMaster.mPointId);
            // 再抽選モーダルまでは閉じる
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window.GetType() != typeof(SupportEquipmentRedrawingModal));
            Close();
        }
    }
}