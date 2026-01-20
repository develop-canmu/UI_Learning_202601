using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UniRx;

namespace Pjfb.Character
{
    public class CombinationCollectionPage : Page
    {
        [SerializeField] private CombinationCollectionScrollDynamic allCombinationCollectionScrollDynamic;
        [SerializeField] private UIButton activeBulkButton;
        private List<CombinationManager.CombinationCollection> canActiveCombinationCollectionList;
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Init();
            allCombinationCollectionScrollDynamic.InitializeAll((id) =>
            {
                Init();
            });
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            canActiveCombinationCollectionList = CombinationManager.GetCanActiveCombinationCollectionList();
            activeBulkButton.interactable = canActiveCombinationCollectionList.Count > 0;
            AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
        }

        public void RefreshScroller()
        {
            allCombinationCollectionScrollDynamic.Refresh();
        }

        public void OnClickProgressBulkButton()
        {
            CombinationCollectionProgressBulk().Forget();
        }

        public void OnClickPracticeSkillListButton()
        {
            var collectionPracticeSkillInfoList = PracticeSkillUtility.GetCombinationStatusTrainingPracticeSkill(CombinationManager
                .CombinationCollectionTrainingBuffCache);
            var data = new CombinationCollectionPracticeSkillListModal.Data(collectionPracticeSkillInfoList);
            CombinationCollectionPracticeSkillListModal.Open(data);
        }

        private async UniTask CombinationCollectionProgressBulk()
        {
            // Apiを投げる前に古いデータを保持しておく
            var prevCombinationCollectionOpenedList = new List<CombinationOpenedMinimum>();
            foreach (var combinationOpenedMinimum in CombinationManager.CombinationCollectionOpenedListCache)
            {
                var prevCombinationCollectionOpened = new CombinationOpenedMinimum();
                prevCombinationCollectionOpened.mCombinationId = combinationOpenedMinimum.mCombinationId;
                prevCombinationCollectionOpened.progressLevel = combinationOpenedMinimum.progressLevel;
                prevCombinationCollectionOpenedList.Add(prevCombinationCollectionOpened);
            }
            var mCombinationIdList = canActiveCombinationCollectionList.Select(x => x.MCombinationId).ToList();
            await CombinationManager.CombinationCollectionProgressBulkAPI(mCombinationIdList);
            var collectionProgressDataList = new List<CombinationManager.CollectionProgressData>();
            // 更新後のデータと古いデータを比較して今回解放したもののデータを作成
            foreach (var combinationOpenedMinimum in CombinationManager.CombinationCollectionOpenedListCache)
            {
                var prevCombinationOpenedMinimum = prevCombinationCollectionOpenedList.FirstOrDefault(prevCollection =>
                    prevCollection.mCombinationId == combinationOpenedMinimum.mCombinationId);
                // 古いデータと現在のデータで同じprogressLevelだった場合continue
                if (prevCombinationOpenedMinimum != null && prevCombinationOpenedMinimum.progressLevel == combinationOpenedMinimum.progressLevel) continue;
                var collectionProgressData = new CombinationManager.CollectionProgressData(combinationOpenedMinimum.mCombinationId, new List<long>());
                var mCombination =  MasterManager.Instance.combinationMaster.FindData(combinationOpenedMinimum.mCombinationId);
                if (mCombination == null) continue;
                // 今回解放したprogressLevelのデータを追加する
                //progress側の定義としては「段階。0スタート。「そのレベルから次のレベルに強化するときの条件」の設定を入れる。」
                //上記よりprogressを取得する際はprogressのlevelに-1したものを渡す
                var mCombinationProgress =
                    MasterManager.Instance.combinationProgressMaster.FindDataByGroupIdIdAndProgressLevel(
                        mCombination.mCombinationProgressGroupId, combinationOpenedMinimum.progressLevel - 1);
                if(mCombinationProgress == null) continue;
                collectionProgressData.ProgressIdList.Add(mCombinationProgress.id);
                collectionProgressDataList.Add(collectionProgressData);
            }
            CombinationCollectionSkillUnlockedModal.Open(new CombinationCollectionSkillUnlockedModal.Data(collectionProgressDataList,
                () => { allCombinationCollectionScrollDynamic.InitializeAll((id) => Init()); }));
            allCombinationCollectionScrollDynamic.UpdateSkillView(mCombinationIdList.ToHashSet());
            Init();
        }

        
    }
}