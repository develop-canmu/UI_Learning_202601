using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingSupportEquipmentDetailView : MonoBehaviour
    {
        [SerializeField]
        private SupportEquipmentIcon icon = null;
        [SerializeField]
        private TMP_Text nameText = null;
        [SerializeField]
        private ScrollGrid skillScrollGrid = null;
        
        
        private long uEquipmentId = 0;
        

        
        
        /// <summary>Idをセット</summary>
        public void SetEquipmentId(long uEquipmentId, List<SupportEquipmentDetailData> detailOrderList)
        {
            this.uEquipmentId = uEquipmentId;
            // UserData
            UserDataSupportEquipment uEquipment = UserDataManager.Instance.supportEquipment.Find(uEquipmentId);
            // MasterData
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData( uEquipment.charaId );
            // アイコン
            icon.SetIconByUEquipmentId(uEquipmentId);
            int index = detailOrderList.FindIndex(v=>v.USupportEquipmentId == uEquipmentId);            
            icon.SwipeableParams = new SwipeableParams<SupportEquipmentDetailData>(detailOrderList, index, null);
            
            // 名前
            nameText.text = mChar.name;
            
            // スキルを取得
            List<PracticeSkillInfo> mainSkillList = PracticeSkillUtility.GetCharacterPracticeSkill(uEquipment.charaId, uEquipment.level);
            List<PracticeSkillInfo> subSkillList = PracticeSkillUtility.GetCharaTrainerLotteryStatusPracticeSkill(uEquipment.lotteryProcessJson.statusList);
            
            int maxCount = Mathf.Max(mainSkillList.Count, subSkillList.Count);
            List<PracticeSkillViewMiniGridItem.Info> skillList = new List<PracticeSkillViewMiniGridItem.Info>();
            
            for(int i=0;i<maxCount;i++)
            {
                skillList.Add(mainSkillList.Count > i
                    ? new PracticeSkillViewMiniGridItem.Info(mainSkillList[i], false, false)
                    : null);
                skillList.Add(subSkillList.Count > i
                    ? new PracticeSkillViewMiniGridItem.Info(subSkillList[i], false, false)
                    : null);
            }
            
            skillScrollGrid.SetItems(skillList);
        }
    }
}