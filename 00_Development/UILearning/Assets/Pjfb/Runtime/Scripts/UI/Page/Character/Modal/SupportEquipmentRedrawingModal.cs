using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Character
{
    public class SupportEquipmentRedrawingModal : ModalWindow
    {
        public class Data
        {
            public UserDataSupportEquipment UserDataSupportEquipment;
            public Action OnSetCloseParameter;

            public Data(UserDataSupportEquipment userDataSupportEquipment, Action onSetCloseParameter)
            {
                UserDataSupportEquipment = userDataSupportEquipment;
                OnSetCloseParameter = onSetCloseParameter;
            }
        }

        [SerializeField] private SupportEquipmentNameView supportEquipmentNameView;
        [SerializeField] private ScrollDynamic previewSkillScroller;
        [SerializeField] private ScrollDynamic afterSkillScroller;
        [SerializeField] private ScrollGrid itemScrollGrid;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private UIButton confirmButton;

        private Data modalData;
        private List<CharaTrainerLotteryReloadMasterObject> mCharaTrainerLotteryReloadList;
        private List<ItemIconContainerGridItem.Data> itemList;
        private List<PracticeSkillLotteryInfo> previewSkillList;
        private List<PracticeSkillLotteryInfo> afterSkillList;
        private CharaTrainerLotteryReloadMasterObject mCharaTrainerLotteryReload;
        private int selectedItemIndex;
        private  CharaTrainerLotteryReloadMasterObject reloadType;
        // 前の再抽選の際に利用したポイントId
        private long preRedrawPointId = 0;
        // 抽選がされたか
        private bool isRedraw = false;
        
        //　抽選方法が抽選テーブルの枠数
        private long lotteryTableSlotNum = 0;
        //　抽選方法が効果量抽選の枠数
        private long lotteryValueSlotNum = 0;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentRedrawing, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            // パラメータの初期化
            preRedrawPointId = 0;
            isRedraw = false;
            
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            var mChara = MasterManager.Instance.charaMaster.FindData(modalData.UserDataSupportEquipment.charaId);
            if (mChara == null)
            {
                CruFramework.Logger.LogError(
                    $"mCharaが取得できませんでした　mCharaId : {modalData.UserDataSupportEquipment.charaId}");
                return;
            }
            var mCharaTrainerLottery = MasterManager.Instance.charaTrainerLotteryMaster.FindData(mChara.mCharaTrainerLotteryId);
            if (mCharaTrainerLottery == null)
            {
                CruFramework.Logger.LogError(
                    $"mCharaTrainerLotteryが取得できませんでした　mCharaTrainerLotteryId : {mChara.mCharaTrainerLotteryId}");
                return;
            }
            
            mCharaTrainerLotteryReloadList = MasterManager.Instance.charaTrainerLotteryReloadMaster.FindDataListByMCharaTrainerLotteryReloadGroupId(mCharaTrainerLottery.mCharaTrainerLotteryReloadGroupId);
            supportEquipmentNameView.InitializeUI(modalData.UserDataSupportEquipment);
            
            //スクロールダイナミックに変更
            previewSkillList = PracticeSkillLotteryUtility.GetCharaTrainerLotteryInfo(modalData.UserDataSupportEquipment.lotteryProcessJson.statusList);
            
            itemList = new List<ItemIconContainerGridItem.Data>();
            selectedItemIndex = -1;
            
            for(int i = 0; i < mCharaTrainerLotteryReloadList.Count; i++)
            {
                var point = UserDataManager.Instance.point.Find(mCharaTrainerLotteryReloadList[i].mPointId);
                var isEnough = (point?.value ?? 0) >= mCharaTrainerLotteryReloadList[i].value;
                // 以前選択された抽選ポイントがありIdが一致するなら以前選択していたアイテムを選択しておく
                if (preRedrawPointId != 0 && preRedrawPointId == mCharaTrainerLotteryReloadList[i].mPointId && isEnough) selectedItemIndex = i;
                // 選択可能なアイテムにセットする
                if (isEnough && selectedItemIndex == -1) selectedItemIndex = i;
                
                itemList.Add(new ItemIconContainerGridItem.Data(ItemIconType.Item,
                    mCharaTrainerLotteryReloadList[i].mPointId, point?.value ?? 0, mCharaTrainerLotteryReloadList[i].value,
                    !isEnough));
            }
            if (selectedItemIndex >= itemList.Count || selectedItemIndex == -1) selectedItemIndex = 0;
            itemScrollGrid.SetItems(itemList);
            // アイテムの位置までスクロール
            itemScrollGrid.ScrollToItemIndex(selectedItemIndex);

            SetSelectedItem();
            SetSkillScroller();
        }
        
        private void SetSelectedItem()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].isSelected = i == selectedItemIndex;
            }
            itemScrollGrid.Refresh(false);
            var selectedItem = itemList[selectedItemIndex];
            mCharaTrainerLotteryReload = mCharaTrainerLotteryReloadList.FirstOrDefault(data =>
                data.mPointId == selectedItem.id && data.value == selectedItem.necessaryValue);
            if (mCharaTrainerLotteryReload == null)
            {
                CruFramework.Logger.LogError(
                    $"mCharaTrainerLotteryReloadが取得できませんでした　mPointId : {selectedItem.id} necessaryValue : {selectedItem.necessaryValue}");
                confirmButton.interactable = false;
                return;
            }
            var mPoint = MasterManager.Instance.pointMaster.FindData(selectedItem.id);
            if (mPoint == null)
            {
                CruFramework.Logger.LogError($"mPointが取得できませんでした　mPointId : {selectedItem.id}");
                confirmButton.interactable = false;
                return;
            }
            var sb = new StringBuilder();
            
            //上書き不可のスキルの個数
            int notOverwriteSkillCount = previewSkillList.Count(x => (!x.CanReloadOverwrite()));
            reloadType = mCharaTrainerLotteryReload;
            //全て上書き不可か
            bool isNotAllReloadOverwrite = notOverwriteSkillCount >= previewSkillList.Count;
            //抽選対象となるスキル枠があるか
            bool IsLotteryTarget = false;
            //初期化
            lotteryTableSlotNum = 0;
            lotteryValueSlotNum = 0;
            
            switch (mCharaTrainerLotteryReload.reloadType)
            {
                // 完全再抽選
                case PracticeSkillLotteryReloadType.All:
                {
                    IsLotteryTarget = !isNotAllReloadOverwrite;
                    sb.AppendLine(string.Format(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_02"], mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName));
                    break;
                }
                // 枠数指定再抽選
                case PracticeSkillLotteryReloadType.SelectFrame:
                {
                    //　上書き不可のスキルを引いた再抽選対象の数
                    long reloadSlotNum = Math.Min(previewSkillList.Count - notOverwriteSkillCount, mCharaTrainerLotteryReload.reloadValue);
                    IsLotteryTarget = reloadSlotNum > 0;
                    lotteryTableSlotNum = (int)reloadSlotNum;
                    sb.AppendLine(string.Format(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_selectFrame"], mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName, reloadSlotNum));
                    break;
                }
                //　効果値再抽選
                case PracticeSkillLotteryReloadType.SelectValue:
                {
                    IsLotteryTarget = !isNotAllReloadOverwrite;
                    sb.AppendLine(string.Format(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_selectValue"], mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName));
                    break;
                }
                //　枠指定再抽選
                case PracticeSkillLotteryReloadType.SelectTable:
                {
                    // LotteryReloadDetailマスタをDetailGroupIdで検索し枠番号順にソートする
                    var mCharaTrainerLotteryDetailList = MasterManager.Instance.charaTrainerLotteryReloadDetailMaster
                        .FindDetailGroupId(mCharaTrainerLotteryReload.mCharaTrainerLotteryDetailGroupId)
                        .OrderBy(x => x.number).ToList();
                    
                    for (var i = 0; i < mCharaTrainerLotteryDetailList.Count; i++)
                    {
                        // サブスキルの数を超えたらループから出る
                        if (previewSkillList.Count < i + 1)
                        {
                            break;
                        }

                        int slotNumber = (int) mCharaTrainerLotteryDetailList[i].number;
                        var labelInfo = previewSkillList[slotNumber - 1];
                        // スキルが上書き不可の場合とばす
                        if(!labelInfo.CanReloadOverwrite()){continue;}


                        switch (mCharaTrainerLotteryDetailList[i].lotteryType)
                        {
                            case PracticeSkillLotteryReloadDetailType.None:
                                break;
                            //  練習能力の再抽選
                            case PracticeSkillLotteryReloadDetailType.SelectSkill:
                                lotteryTableSlotNum++;
                                break;
                            //　効果量の再抽選
                            case PracticeSkillLotteryReloadDetailType.SelectValue:
                                lotteryValueSlotNum++;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                    }
                    
                    sb.AppendLine(string.Format(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_02"], mPoint.name, mCharaTrainerLotteryReload.value, mPoint.unitName));
                    
                    if (lotteryTableSlotNum != 0)
                    {
                        sb.AppendLine(string.Format(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_frameSelectTable"], lotteryTableSlotNum));
                    }

                    if (lotteryValueSlotNum != 0)
                    {
                        sb.AppendLine(string.Format(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_frameSelectValue"], lotteryValueSlotNum));
                    }
                    
                    IsLotteryTarget = lotteryTableSlotNum > 0 || lotteryValueSlotNum > 0;
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mCharaTrainerLotteryReload.reloadType), mCharaTrainerLotteryReload.reloadType, null);
            }
                
            //　全ての上書き不可なら注意文言を追加する
            if (isNotAllReloadOverwrite)
            {
                sb.AppendLine(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_body_text_overwrite"]);
            }
            // 全てが上書き不可でなく抽選対象がないなら
            else if (!IsLotteryTarget)
            {
                sb.AppendLine(StringValueAssetLoader.Instance["character.support_equipment_.description_.lottery_confirm_body_text_frameSelectOverwrite"]);
            }
            
            bodyText.text = sb.ToString();
            // 抽選対象があるならアクティブに
            confirmButton.interactable = !itemList[selectedItemIndex].isCoverActive && IsLotteryTarget;
        }

        private void SetSkillScroller()
        {
            if (mCharaTrainerLotteryReload == null)
            {
                afterSkillList = previewSkillList;
            }

            //スキル抽選後の表示
            else
            {
                afterSkillList = PracticeSkillLotteryUtility.GetCharaTrainerLotteryAfterStatusPracticeSkill(modalData.UserDataSupportEquipment.lotteryProcessJson.statusList, mCharaTrainerLotteryReload);
            }
            
            SetScrollerItem();
        }

        private void SetScrollerItem()
        {
            previewSkillScroller.SetItems(previewSkillList);
            afterSkillScroller.SetItems(afterSkillList);
        }

        //// <summary> 再抽選完了時の処理(抽選に使ったポイントとフラグをオンにして描画を更新する) </summary>
        private void UpdateRedraw(UserDataSupportEquipment uSupportEquipment, long pointId)
        {
            // 抽選後のデータに変える
            modalData.UserDataSupportEquipment = uSupportEquipment;
            preRedrawPointId = pointId;
            isRedraw = true;
            Init();
        }
        
        #region EventListeners
        public void OnClickConfirm()
        {
            if (selectedItemIndex >= itemList.Count) return;
            var selectedItem = itemList[selectedItemIndex];
            if (selectedItem.isCoverActive) return;
            var mCharaTrainerLotteryReload = mCharaTrainerLotteryReloadList.FirstOrDefault(data =>
                data.mPointId == selectedItem.id && data.value == selectedItem.necessaryValue);
            if (mCharaTrainerLotteryReload == null) return;
            SupportEquipmentRedrawingConfirmModal.Open(
                new SupportEquipmentRedrawingConfirmModal.Data(modalData.UserDataSupportEquipment.id,
                    mCharaTrainerLotteryReload.id, () => modalData.OnSetCloseParameter?.Invoke(),
                    afterSkillList
                        .Where(x => !x.IsLotterySubject())
                        .Select(x => x.SkillInfo).ToList(), reloadType, lotteryTableSlotNum, lotteryValueSlotNum, onCompleteRedraw: UpdateRedraw));
        }

        public void OnClickSelectItem(ItemIconContainerGridItem itemIcon)
        {
            if (itemIcon.data.isCoverActive) return;
            selectedItemIndex = itemList.FindIndex(item => item.id == itemIcon.data.id);
            SetSelectedItem();
            SetSkillScroller();
        }

        //// <summary> モーダルを閉じる際の処理 </summary>
        public void OnClickClose()
        {
            // 抽選済みならDetailモーダルを閉じる
            if (isRedraw) AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close();
        }
        #endregion
    }
}