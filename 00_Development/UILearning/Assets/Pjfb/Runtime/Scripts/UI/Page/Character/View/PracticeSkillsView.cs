using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class PracticeSkillsView : MonoBehaviour
    {
        [SerializeField] private ScrollGrid scrollGrid = null;
        private List<PracticeSkillViewData> practiceSkillViewDataList = new();
        private long mCharaId = 0;
        private long currentLv = 0;

        public void InitializeUI(long id)
        {
            var uChar = UserDataManager.Instance.chara.Find(id);
            if (uChar is null) return;
            mCharaId = uChar.charaId;
            currentLv = uChar.level;
            scrollGrid.SetItems(CreatePracticeSkillDataList(currentLv));
        }

        public void SetAfterUi(long afterLv)
        {
            scrollGrid.SetItems(CreatePracticeSkillDataList(afterLv));
        }

        // Todo : use cache to increase performance
        private List<PracticeSkillViewMiniGridItem.Info> CreatePracticeSkillDataList(long afterLv)
        {
            var mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            var practiceSkillGridDataList = new List<PracticeSkillViewMiniGridItem.Info>();
            var acquireAndUnAcquireSkill = PracticeSkillUtility.GetCharacterPracticeSkillAcquireAndUnAcquire(mCharaId, afterLv);
            var currentSkillDataList = PracticeSkillUtility.GetCharacterPracticeSkill(mChara.id, currentLv);

            foreach (var skill in acquireAndUnAcquireSkill)
            {
                bool isLock = afterLv < skill.GetLevel();
                bool isLevelUp = false;
                PracticeSkillInfo skillData = currentSkillDataList.FirstOrDefault(data => data.TrainingStatusTypeDetailId == skill.TrainingStatusTypeDetailId);
                // 現在のレベルと比較して数値が上がっていた場合レベルアップとする
                isLevelUp = !isLock && skill.Value > skillData.Value;
                practiceSkillGridDataList.Add(new PracticeSkillViewMiniGridItem.Info(skill, isLevelUp, isLock));
            }

            return practiceSkillGridDataList;
        }
    }
}

