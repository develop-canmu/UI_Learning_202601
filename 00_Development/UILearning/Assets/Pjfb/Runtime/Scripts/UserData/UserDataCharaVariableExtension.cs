using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.UserData {

    public partial class UserDataCharaVariable
    {
        
        public long ParentMCharaId => MChara.parentMCharaId;
        public CharaMasterObject MChara => MasterManager.Instance.charaMaster.FindData(charaId);

        /// <summary>練習能力</summary>
        public CharaTrainingStatusMasterObject[] MCharaTrainingStatus => MasterManager.Instance
            .charaTrainingStatusMaster.values.Where(x => x.mCharaId == charaId).ToArray();
        
        /// <summary>練習メニューカード</summary>
        public TrainingCardCharaMasterObject[] MPracticeCardChara => MasterManager.Instance.trainingCardCharaMaster.values
            .Where(x => x.mCharaId == charaId).ToArray();
        
        /// <summary>育成イベント</summary>
        public TrainingEventMasterObject[] MTrainingEvents => MasterManager.Instance.trainingEventMaster.values
            .Where(x => x.trainingMCharaId == charaId).ToArray();
        
        /// <summary>移籍ポイント</summary>
        public CharaRankPointMasterObject MCharaRankPointMaster => MasterManager.Instance.charaRankPointMaster.values.FirstOrDefault(x => x.rankNumber == rank);
        
        private SkillData[] skillDataList = null;

        public SkillData[] SkillDataList
        {
            get
            {
                if(skillDataList == null)
                {
                    skillDataList = new SkillData[abilityList.Length];
                    for(int i=0;i<skillDataList.Length;i++)
                    {
                        skillDataList[i] = new SkillData(abilityList[i].l[0], abilityList[i].l[1] );
                    }
                }
				
                return skillDataList;
            }
        }

    }
}