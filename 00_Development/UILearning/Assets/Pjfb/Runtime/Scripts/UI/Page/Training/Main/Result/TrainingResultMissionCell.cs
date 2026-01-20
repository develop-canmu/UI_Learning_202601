using System;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pjfb.Training
{
    [Serializable]
    public class MissionScrollData
    {
        public long id;
        public long categoryTrainingSortNumber;
        public long groupNumber;
        public string categoryName;
        public string missionTitle;
        public BigValue nowProgress;
        public BigValue maxProgress;
        public long previousMission;
        public bool existPreviousMission;

        private void Initialize(long id, long categoryTrainingSortNumber, long groupNumber,string categoryName, string missionTitle, BigValue nowProgress, BigValue maxProgress, long previousMission, bool existPreviousMission)
        {
            this.id = id;
            this.categoryTrainingSortNumber = categoryTrainingSortNumber;
            this.groupNumber = groupNumber;
            this.categoryName = categoryName;
            this.missionTitle = missionTitle;
            this.nowProgress = nowProgress;
            this.maxProgress = maxProgress;
            this.previousMission = previousMission;
            this.existPreviousMission = existPreviousMission;
        }
        
        public MissionScrollData(long id,long cateTrainingSortNumber, long groupNumber, string categoryName ,string missionTitle, BigValue nowProgress, BigValue maxProgress, long previousMission, bool existPreviousMission)
        {
            Initialize(id, cateTrainingSortNumber, groupNumber, categoryName, missionTitle, nowProgress, maxProgress, previousMission, existPreviousMission);
        }
    }

    public class TrainingResultMissionCell : MonoBehaviour
    {
        private MissionScrollData scrollData;
        
        [SerializeField]
        private TrainingResultMissionView trainingResultMissionView = null;
        
        public Animator animator = null;

        [SerializeField]
        private GameObject Band;
        [SerializeField]
        private GameObject BadgeCategory;
        [SerializeField]
        private GameObject Adjuster;
        [SerializeField]
        private TMPro.TMP_Text category = null;
        [SerializeField]
        private TMPro.TMP_Text missionTitle = null;
        [SerializeField]
        private Slider progressBar = null;
        [SerializeField]
        private TMPro.TMP_Text progress = null;
        [SerializeField]
        private OmissionTextSetter omissionTextSetter = null;

        private bool isAchieve = false;
        
        private float beforeValue = 0;
        private float afterValue = 0;

        private const string Appear = "Appear";
        private const string Achieve = "Achieve";      
        private const string Achieved = "Achieved";
        private const string Unachieved = "Unachieved";

        public bool finAnimation { get; private set;} = false;
        
        [SerializeField, Header("Sliderをアニメーションさせる時間")]
        private float animationTime = 1f;

        public async void SetView(MissionScrollData value)
        {
            scrollData = value;
            // データ挿入
            category.text = scrollData.categoryName;
            missionTitle.text = scrollData.missionTitle;
            isAchieve = scrollData.nowProgress >= scrollData.maxProgress;
            afterValue = isAchieve ? 1 : ((float) BigValue.RatioCalculation(scrollData.nowProgress, scrollData.maxProgress));
            progress.text = string.Format(StringValueAssetLoader.Instance["mission.progress"], scrollData.nowProgress.ToDisplayString(omissionTextSetter.GetOmissionData()),
                scrollData.maxProgress.ToDisplayString(omissionTextSetter.GetOmissionData()));

            // ミッション達成かつ、skip
            bool isAchieved = trainingResultMissionView.skipAnimation && isAchieve;

            // 前提ミッションで変更
            Band.SetActive(!scrollData.existPreviousMission);
            BadgeCategory.SetActive(!scrollData.existPreviousMission);
            Adjuster.SetActive(scrollData.existPreviousMission);

            await AnimatorUtility.WaitStateAsync(trainingResultMissionView.animator, "Open");

            if (isAchieved)
            {
                animator.SetTrigger(Achieved);
            }
            else
            {
                animator.SetTrigger(Unachieved);
            }

            // 前提ミッションのアニメーションが終わるまで待つ
            if (scrollData.existPreviousMission && scrollData.previousMission != 0)
            {
                await trainingResultMissionView.WaitPreviousMission(this, scrollData.previousMission);
            }
            
            // AppearのAnimationを再生したかの判定
            bool finAppearAnimation = false;
            
            if (!trainingResultMissionView.skipAnimation)
            {
                await AnimatorUtility.WaitStateAsync(animator, Appear);
                finAppearAnimation = true;
            }

            // sliderのアニメーションが終わるまで待つ
            await PlayAnimation();

            // 達成かつ、skipかで判定
            if (isAchieve && !trainingResultMissionView.skipAnimation)
            {
                await AnimatorUtility.WaitStateAsync(animator, Achieve);
                await AnimatorUtility.WaitStateAsync(animator, Achieved);
            }
            else if (isAchieve && trainingResultMissionView.skipAnimation)
            {
                await AnimatorUtility.WaitStateAsync(animator, Achieved);
            }

            // 途中でskip選択された場合にAppearが再生されていなければ再生する
            if (trainingResultMissionView.skipAnimation && !finAppearAnimation)
            {
                animator.SetTrigger(Appear);
            }

            finAnimation = true;
        }

        private async UniTask PlayAnimation()
        {
            // 時間
            float time = animationTime;
            while (true)
            {
                if (trainingResultMissionView.skipAnimation) break;
                // 進捗加算
                float progressBarValue = beforeValue + (afterValue - beforeValue) * (1.0f - time / animationTime);
                // 表示
                progressBar.value = progressBarValue;
                // 時間計測
                time -= Time.deltaTime;
                // 終了
                if (time <= 0) break;
                // 1フレ
                await UniTask.DelayFrame(1);
            }
            progressBar.value = afterValue;
        }
    }
}