using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.Common
{
    public class TrainingCardRewardSheetReward : Sheet
    {
        [SerializeField] 
        private ScrollGrid scrollGrid;
        [SerializeField]
        private GameObject noRewardMessage = null;

        public void SetDisplay(long cardId)
        {
            // マスタ
            TrainingCardRewardAbilityMasterObject[] mRewards = MasterManager.Instance.trainingCardRewardAbilityMaster.FindDataByCardId(cardId);
            // スキルデータに変換
            SkillData[] skillDatas = new SkillData[mRewards.Length];
            for(int i=0;i<mRewards.Length;i++)
            {
                skillDatas[i] = new SkillData(mRewards[i].abilityId, mRewards[i].abilityLevel);
            }
            
            // スキル獲得なし表示
            noRewardMessage.gameObject.SetActive(mRewards.Length == 0);
            
            // スクロールにセット
            scrollGrid.SetItems(skillDatas);
        }
    }
}