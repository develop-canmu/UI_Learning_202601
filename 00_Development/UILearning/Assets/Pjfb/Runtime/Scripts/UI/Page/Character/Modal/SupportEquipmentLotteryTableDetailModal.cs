using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    /// <summary> サポート器具抽選テーブル詳細モーダル </summary>
    public class SupportEquipmentLotteryTableDetailModal : ModalWindow
    {
        public class Param
        {
            // 対象のサポ器具
            private UserDataSupportEquipment supportEquipment;
            public UserDataSupportEquipment SupportEquipment => supportEquipment;

            public Param(UserDataSupportEquipment supportEquipment)
            {
                this.supportEquipment = supportEquipment;
            }
        }

        // サポート器具名
        [SerializeField]
        private TMP_Text supportEquipmentName = null;

        [SerializeField]
        private ScrollDynamic scroll;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Param param = (Param)args;

            // サポート器具名
            supportEquipmentName.text = MasterManager.Instance.charaMaster.FindData(param.SupportEquipment.charaId).name;
            List<PracticeSkillLotteryTableSlotInfo> tableDataList = PracticeSkillLotteryUtility.GetTrainerLotteryTableList(param.SupportEquipment.charaId);
            List<SupportEquipmentLotteryTableDataScrollDynamicSelector.IScrollItem> itemDataList = new List<SupportEquipmentLotteryTableDataScrollDynamicSelector.IScrollItem>();

            foreach (var slotInfo in tableDataList)
            {
                // ラベルデータ追加
                itemDataList.Add(new SupportEquipmentLotteryTableSlotLabelScrollDynamicItem.Data(slotInfo.SlotNumbers));
                foreach (var tableInfo in slotInfo.TableInfos)
                {
                    foreach (var info in tableInfo.SkillInfos)
                    {
                        itemDataList.Add(new SupportEquipmentLotteryTableDataScrollDynamicItem.Data(info, tableInfo.LotteryFrameTableMaster.mRarityId));
                    }
                }
            }

            scroll.SetItems(itemDataList);

            return base.OnPreOpen(args, token);
        }
    }
}
