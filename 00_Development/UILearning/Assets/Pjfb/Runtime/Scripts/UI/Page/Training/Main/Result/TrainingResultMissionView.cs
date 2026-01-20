using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Logger = CruFramework.Logger;

namespace Pjfb.Training
{
    public class TrainingResultMissionView : MonoBehaviour
    {
        private List<MissionScrollData> scrollData = new List<MissionScrollData>();
        private Dictionary<long, TrainingResultMissionCell> cellList = new Dictionary<long, TrainingResultMissionCell>();
        public Dictionary<long, bool> FinAnimation;

        [SerializeField]
        private RectTransform contents;

        [SerializeField]
        private TrainingResultMissionCell trainingResultMissionCell;

        public Animator animator = null;

        private Action onClosed = null;
        
        private bool finAnimation = false;
        public bool skipAnimation { get; private set; }= false;

        private static readonly string OpenAnimation = "Open";
        private static readonly string OpenTapAnimation = "OpenTap";
        private static readonly string CloseAnimation = "Close";

        private enum PreviousMissionStatus
        {
            NotExitInMissionList,
            ExistPreviousMissionInMissionList,
            ProgressingPreviousMission
        }
        
        public void Set(TrainingMainArguments args)
        {
            if (args.MissionList == null) return;

            scrollData.Clear();
            foreach (var missionList in args.MissionList)
            {
                var mission = MasterManager.Instance.dailyMissionMaster.FindData(missionList.mDailyMissionId);
                var missionCategory =
                    MasterManager.Instance.dailyMissionCategoryMaster.FindData(mission.mDailyMissionCategoryId);

                PreviousMissionStatus previousMissionStatus = GetPreviousMissionProgressing(args.MissionList, mission.previousMDailyMissionId);
                
                if(missionCategory.trainingSortNumber == 0) continue;
                
                // 同じミッションがあれば表示しない
                if (ExistIdInScrollData(mission.groupNumber)) continue;

                // ファースト・オーダーの場合、もらったデータのなかに前提ミッションが存在したら表示しない
                if(missionCategory.trainingSortNumber == 4 && previousMissionStatus != PreviousMissionStatus.NotExitInMissionList) continue;
                
                // 前提ミッションが進行中であれば表示しない
                if (previousMissionStatus == PreviousMissionStatus.ProgressingPreviousMission) continue;

                // 前提ミッションが存在し、scrollDataに存在しない場合
                if (previousMissionStatus != PreviousMissionStatus.NotExitInMissionList && !ExistPreviousMissionInScrollData(mission.previousMDailyMissionId))
                {
                    SetMissionId(mission.previousMDailyMissionId);
                }
                
                scrollData.Add(new MissionScrollData(mission.id, missionCategory.trainingSortNumber, mission.groupNumber, missionCategory.typeName, mission.description, new BigValue(missionList.passingCount), new BigValue(mission.requireCount), mission.previousMDailyMissionId, previousMissionStatus == PreviousMissionStatus.ExistPreviousMissionInMissionList));
            }

            scrollData = scrollData.OrderBy(data => data.categoryTrainingSortNumber).ThenBy(data => data.id).ToList();
        }
        
        ///<summary>表示する配列にdescriptionとソート番号が同じのが存在すれば消す</summary>
        private bool ExistIdInScrollData(long groupNumber)
        {
            foreach (var data in scrollData)
            {
                if (data.groupNumber == groupNumber)
                {
                    return true;
                }
            }
        
            return false;
        }

        ///<summary>scrollDataの中にidの物が存在するか</summary>
        private bool ExistPreviousMissionInScrollData(long id)
        {
            foreach (var data in scrollData)
            {
                if (data.id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> Idでの判断ができない場合、descriptionとtrainingSortNumberが同じ場合に前提ミッションのIdを修正する </summary>
        private void SetMissionId(long previousMissionId)
        {
            var mission = MasterManager.Instance.dailyMissionMaster.FindData(previousMissionId);
            var missionCategory = MasterManager.Instance.dailyMissionCategoryMaster.FindData(mission.mDailyMissionCategoryId);
            foreach (var data in scrollData)
            {
                if (data.groupNumber == mission.groupNumber)
                {
                    data.id = previousMissionId;
                }
            }
        }

        ///<summary>前提ミッションが受け取ったデータに存在するかと前提ミッションが進行中かの判定</summary>
        private PreviousMissionStatus GetPreviousMissionProgressing(MissionUserAndGuild[] missionList, long id)
        {
            PreviousMissionStatus data = PreviousMissionStatus.NotExitInMissionList;
            if (id == 0) return data;
            foreach (var mission in missionList)
            {
                if (mission.mDailyMissionId == id)
                {
                    data = PreviousMissionStatus.ExistPreviousMissionInMissionList;
                    if (mission.progressStatus == (long)MissionProgressStatus.Progressing)
                    {
                        data = PreviousMissionStatus.ProgressingPreviousMission;
                    }
                    return data;
                }
            }
            return data;
        }

        ///<summary>全てのCellのAnimationが終わったかの判定</summary>
        private bool GetFinAnimation()
        {
            if (cellList == null) return true;
            foreach (var cell in cellList.Values)
            {
                if (!cell.finAnimation)
                {
                    return false;
                }
            }

            return true;
        }
        
        public async void Open(Action onClosed)
        {
            gameObject.SetActive(true);
            this.onClosed = onClosed;
            // Cellの生成
            Generate();
            animator.SetTrigger(OpenAnimation);
            // 全てのセルのanimationが終わるまで待つ
            await UniTask.WaitUntil(GetFinAnimation);
            finAnimation = true;
            animator.SetTrigger(OpenTapAnimation);
        }

        ///<summary>Cellの生成</summary>
        private void Generate()
        {
            cellList.Clear();
            foreach (var data in scrollData)
            {
                GameObject missionCell = Instantiate(trainingResultMissionCell.gameObject, contents);
                var cellView = missionCell.GetComponent<TrainingResultMissionCell>();
                cellView.gameObject.SetActive(true);
                cellView.SetView(data);
                cellList.Add(data.id, cellView);
            }
        }

        ///<summary>前提ミッションのAnimationが終わるまで待つ</summary>
        public async UniTask WaitPreviousMission(TrainingResultMissionCell cell, long next)
        {
            cell.gameObject.SetActive(false);
            await UniTask.WaitWhile(() => !cellList[next].finAnimation);
            cell.gameObject.SetActive(true);
        }

        public void OnScreenButtonTap()
        {
            if (finAnimation)
            {
                CloseAsync().Forget();
            }
            else
            {
                skipAnimation = true;
            }
        }

        private async UniTask CloseAsync()
        {
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            onClosed();
        }
    }
}