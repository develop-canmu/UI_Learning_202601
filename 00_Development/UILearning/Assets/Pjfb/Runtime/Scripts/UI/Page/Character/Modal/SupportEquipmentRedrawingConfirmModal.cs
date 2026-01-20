using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Character
{
    public class SupportEquipmentRedrawingConfirmModal : ModalWindow
    {
        public class Data
        {
            public long USupportEquipmentId;
            public long MCharaTrainerLotteryReloadId;
            public Action OnSetCloseParameter;
            private List<PracticeSkillInfo> notSubjectSkillList;
            public List<PracticeSkillInfo> NotSubjectSkillList{get => notSubjectSkillList;}
            private  CharaTrainerLotteryReloadMasterObject reloadMaster;
            public  CharaTrainerLotteryReloadMasterObject ReloadMaster {get => reloadMaster;}
            //　テーブル抽選される枠数
            private long lotteryTableSlotNum = 0;
            public long LotteryTableSlotNum {get => lotteryTableSlotNum;}
            //　効果量抽選される枠数
            private long lotteryValueSlotNum = 0;
            public long LotteryValueSlotNum {get => lotteryValueSlotNum;}
            // 抽選完了時の処理
            private Action<UserDataSupportEquipment, long> onCompleteRedraw = null;
            public Action<UserDataSupportEquipment, long> OnCompleteRedraw {get => onCompleteRedraw;}
            
            public Data(long uSupportEquipmentId, long mCharaTrainerLotteryReloadId, Action onSetCloseParameter, List<PracticeSkillInfo> notSubjectSkillList,  CharaTrainerLotteryReloadMasterObject reloadMaster, long lotteryTableSlotNum, long lotteryValueSlotNum, Action<UserDataSupportEquipment, long> onCompleteRedraw = null)
            {
                USupportEquipmentId = uSupportEquipmentId;
                MCharaTrainerLotteryReloadId = mCharaTrainerLotteryReloadId;
                OnSetCloseParameter = onSetCloseParameter;
                this.notSubjectSkillList = notSubjectSkillList;
                this.reloadMaster = reloadMaster;
                this.lotteryTableSlotNum = lotteryTableSlotNum;
                this.lotteryValueSlotNum = lotteryValueSlotNum;
                this.onCompleteRedraw = onCompleteRedraw;
            }
        }
        
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private ItemIcon itemIcon;
        [SerializeField] private IconImage necessaryIconImage;
        [SerializeField] private TMP_Text necessaryCountText;
        [SerializeField] private PossessionItemUi possessionItemUi;
        [SerializeField] private TMP_Text commonText;
        [SerializeField] private GameObject subSkillView;
        [SerializeField] private TMP_Text subText;
        [SerializeField] private SupportEquipmentPracticeSkillListView practiceSkillView;
        
        private Data modalData;
        private CharaTrainerLotteryReloadMasterObject mCharaTrainerLotteryReload;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentRedrawingConfirm, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            mCharaTrainerLotteryReload = MasterManager.Instance.charaTrainerLotteryReloadMaster.FindData(modalData.MCharaTrainerLotteryReloadId);
            practiceSkillView.InitializeUI(modalData.NotSubjectSkillList);
            
            if (mCharaTrainerLotteryReload == null)
            {
                CruFramework.Logger.LogError(
                    $"mCharaTrainerLotteryReloadが取得できませんでした　MCharaTrainerLotteryReloadId : {modalData.MCharaTrainerLotteryReloadId}");
                return;
            }
            var mPoint = MasterManager.Instance.pointMaster.FindData(mCharaTrainerLotteryReload.mPointId);
            if (mPoint == null)
            {
                CruFramework.Logger.LogError(
                    $"mPointが取得できませんでした　mPointId : {mCharaTrainerLotteryReload.mPointId}");
                return;
            }
            itemNameText.text = mPoint.name;
            itemIcon.SetIconId(mPoint.id);
            itemIcon.SetCount(mCharaTrainerLotteryReload.value);
            necessaryIconImage.SetTexture(mPoint.id);
            necessaryCountText.text = mCharaTrainerLotteryReload.value.ToString();
            possessionItemUi.SetAfterCountByAmount(mPoint.id, -mCharaTrainerLotteryReload.value);
            var sb = new StringBuilder();
            
            // 抽選方式に応じて注意文言を変更
            switch (modalData.ReloadMaster.reloadType)
            {
                case PracticeSkillLotteryReloadType.All:
                        sb.AppendFormat(
                            StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_03"], 
                            mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName);
                    break;
                case PracticeSkillLotteryReloadType.SelectFrame:
                        sb.AppendFormat(
                            StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_selectFrame"],
                            mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName, modalData.LotteryTableSlotNum);
                    break;
                case PracticeSkillLotteryReloadType.SelectValue:
                        sb.AppendFormat(
                            StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_selectValue"],
                            mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName);
                    break;
                case PracticeSkillLotteryReloadType.SelectTable:
                        sb.AppendFormat(
                            StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_Common"],
                            mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName);
                        
                        //枠数があるなら文言を追加
                        if (modalData.LotteryTableSlotNum != 0)
                        {
                            sb.AppendFormat(
                                StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_frameSelectTable"],
                                modalData.LotteryTableSlotNum);
                        }
                        if (modalData.LotteryValueSlotNum != 0)
                        {
                            sb.AppendFormat(
                                StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_frameSelectValue"],
                                modalData.LotteryValueSlotNum);
                        }
                        sb.AppendFormat(
                            StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_Confirm"],
                            modalData.LotteryTableSlotNum);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modalData.ReloadMaster.reloadType), modalData.ReloadMaster.reloadType, null);
            }
            
            // 抽選されないスキルがあるなら文言を追加
            if (modalData.NotSubjectSkillList.Count > 0)
            {
                //　改行追加
                sb.AppendLine();
                sb.AppendLine(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_overwrite"]);
                subSkillView.gameObject.SetActive(true);
                subText.gameObject.SetActive(true);
                commonText.gameObject.SetActive(false);
                subText.text = sb.ToString();
            }
            // 抽選されないスキルがないなら表示を切り替え
            else
            {
                subSkillView.gameObject.SetActive(false);
                subText.gameObject.SetActive(false);
                commonText.gameObject.SetActive(true);
                commonText.text = sb.ToString();
            }
        }

        public void OnClickRedrawing()
        {
            ReLotterySupportEquipment().Forget();
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
        
        private async UniTask ReLotterySupportEquipment()
        {
            var userDataSupportEquipment =
                UserDataManager.Instance.supportEquipment.Find(modalData.USupportEquipmentId);
            // 更新前のデータを保持しないといけないためnewしておく
            var prevUserDataSupportEquipment = new UserDataSupportEquipment(new CharaVariableTrainerBase
            {
                id = userDataSupportEquipment.id,
                uMasterId = userDataSupportEquipment.masterId,
                mCharaId = userDataSupportEquipment.charaId,
                level = userDataSupportEquipment.level,
                firstParamAddMap = userDataSupportEquipment.firstParamAddMap,
                battleParamEnhanceRate = userDataSupportEquipment.battleParamEnhanceRate,
                rarePracticeEnhanceRate = userDataSupportEquipment.rarePracticeEnhanceRate,
                battleParamEnhanceMap = userDataSupportEquipment.battleParamEnhanceMap,
                conditionEffectGradeUpMapOnType = userDataSupportEquipment.conditionEffectGradeUpMapOnType,
                practiceParamAddBonusMap = userDataSupportEquipment.practiceParamAddBonusMap,
                practiceParamEnhanceMapOnType = userDataSupportEquipment.practiceParamEnhanceMapOnType,
                rarePracticeEnhanceRateMapOnType = userDataSupportEquipment.rarePracticeEnhanceRateMapOnType,
                popRateEnhanceMapOnType = userDataSupportEquipment.popRateEnhanceMapOnType,
                firstMTrainingEventRewardIdList = userDataSupportEquipment.firstRewardIdList,
                lotteryProcessJson = userDataSupportEquipment.lotteryProcessJson,
                isLocked = userDataSupportEquipment.isLocked,
                isReLotteryLocked = userDataSupportEquipment.isReLotteryLocked,
                rankNumber = userDataSupportEquipment.rankNumber,
                lotteryRarityValue = userDataSupportEquipment.lotteryRarityValue,
            });
            
            CharaVariableTrainerReLotteryAPIRequest request = new CharaVariableTrainerReLotteryAPIRequest();
            CharaVariableTrainerReLotteryAPIPost post = new CharaVariableTrainerReLotteryAPIPost();
            post.id = modalData.USupportEquipmentId;
            post.subNumber = mCharaTrainerLotteryReload.subNumber;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CharaVariableTrainerReLotteryAPIResponse response = request.GetResponseData();
            
            var currentUserDataSupportEquipment =
                UserDataManager.Instance.supportEquipment.Find(modalData.USupportEquipmentId);
            SupportEquipmentRedrawingEffectModal.Open(new SupportEquipmentRedrawingEffectModal.Data(
                prevUserDataSupportEquipment, currentUserDataSupportEquipment,
                () => modalData.OnSetCloseParameter?.Invoke(),
                modalData.ReloadMaster, response.updatedNumberList, modalData.OnCompleteRedraw));
        }
    }
}