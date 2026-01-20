using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Event;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Story;
using Pjfb.UserData;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

namespace Pjfb.Home
{
    public class HomeSlideFestivalView : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private SE slideSe = SE.se_common_slide;
        [SerializeField] private ScrollBanner scrollBanner;
        [SerializeField] private long autoScrollSecond = 5;
        [SerializeField] private GameObject pagerParent;
        #endregion

        #region PrivateProperties
        private int _currentIndex;
        private int _currentBannerCount;
        private long _nextScrollTicks;
        #endregion

        #region PublicMethods
        public async UniTask Init(FestivalTimetableMasterContainer festivalTimetableMaster, Action<HomeSlideFestivalBanner.Parameters> OnClickSlideFestivalBanner)
        {
            var mFestivalDictionary = MasterManager.Instance.festivalMaster.values.ToDictionary(aData => aData.id);
            var uPointDictionary = UserDataManager.Instance.point.data.ToDictionary(aData => aData.Value.pointId);
            var mHuntTimetableDictionary = MasterManager.Instance.huntTimetableMaster.values.ToDictionary(aData => aData.id);
            var mHuntEnemyDictionary = MasterManager.Instance.huntEnemyMaster.values.ToDictionaryOfList(aData => aData.mHuntId);
            var shownHuntEnemyIds = StoryManager.Instance.shownStoryHuntEnemyContainer.mHuntEnemyIds;
            var festivalStatusEffectList = EventManager.Instance.FestivalEffectStatusList.ToDictionary(aData => aData.mFestivalTimetableId);
            var mDailyMissionDictionary = MasterManager.Instance.dailyMissionMaster.values.ToDictionaryOfList(aData => aData.mDailyMissionCategoryId);
            var showingFestivalTimetableList = EventManager.GetShowingFestivalTimetableList(festivalTimetableMaster, mHuntTimetableDictionary).ToList();
            
            ResetDisplay();
            if (showingFestivalTimetableList.Count > 0)
            {
                var missionCacheData = await MissionManager.Instance.GetLatestMissionCacheData();
                var slideFestivalViewParams = showingFestivalTimetableList
                    .Select(aData =>
                    {
                        var uFestivalEffectStatus = festivalStatusEffectList.TryGetValue(aData.id, out var festivalEffectStatus) ? festivalEffectStatus : new FestivalEffectStatus();
                        long uPointValue = 0;
                        if (mFestivalDictionary.TryGetValue(aData.mFestivalId, out var mFestival) &&
                            uPointDictionary.TryGetValue(mFestival.mPointId, out var uPoint))
                        {
                            uPointValue = uPoint.Value.value;
                        }

                        var showBadge = false;
                        if (mHuntTimetableDictionary.TryGetValue(aData.mHuntTimetableId, out var mHuntTimetable) &&
                            mHuntEnemyDictionary.TryGetValue(mHuntTimetable.mHuntId, out var mHuntEnemyList))
                        {
                            // イベントストーリーバッジ判定
                            showBadge = mHuntEnemyList.Exists(aHuntEnemy =>
                                uPointValue >= aHuntEnemy.keyMPointValue && // 解放済み判定
                                !shownHuntEnemyIds.Contains(aHuntEnemy.id)); // 未視聴判定
                        }

                        if (!showBadge)
                        {
                            // イベントポイントミッションバッジ判定
                            var eventCategoryMissionId = aData.mDailyMissionCategoryIdList[0];
                            showBadge = missionCacheData.missionDataDictionary
                                .Where(aMissionKeyValuePair => aMissionKeyValuePair.Key.id == eventCategoryMissionId)
                                .SelectMany(aMissionKeyValuePair => aMissionKeyValuePair.Value)
                                .Any(aMissionProgressData => aMissionProgressData.hasReceivingReward);
                        }

                        EventManager.Instance.showHomeBadge = showBadge;
                        AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
                        
                        return new HomeSlideFestivalBanner.Parameters(
                            mFestivalTimetable: aData,
                            mHuntTimetable: mHuntTimetable,
                            mFestival: mFestival,
                            uFestivalEffectStatus: uFestivalEffectStatus,
                            showBadge: showBadge,
                            onClickBanner: OnClickSlideFestivalBanner);
                    })
                    .ToList();
                scrollBanner.SetBannerDatas(slideFestivalViewParams);
                
                _currentBannerCount = slideFestivalViewParams.Count;
                if (_currentBannerCount > 1) {
                    pagerParent.SetActive(true);
                    UpdateNextScrollSecond();    
                } else {
                    pagerParent.SetActive(false);
                    _nextScrollTicks = long.MaxValue;
                }
            }
        }
        #endregion

        #region PrivateMethods
        private void ResetDisplay()
        {
            _nextScrollTicks = long.MaxValue;
            pagerParent.SetActive(false);
            scrollBanner.SetBannerDatas(new List<HomeSlideFestivalBanner.Parameters>());
        }
        #endregion
        
        #region EventListener
        private void Awake()
        {
            ResetNextScrollSecond();
            scrollBanner.onChangedPage += OnChangePage;
            scrollBanner.ScrollGrid.OnBeginDragAsObservable().Subscribe(OnBeginDrag).AddTo(gameObject);
            scrollBanner.ScrollGrid.OnEndDragAsObservable().Subscribe(OnEndDrag).AddTo(gameObject);
            
            scrollBanner.ScrollGrid.OnBeginPageSnap += OnBeginPageSnap;
        }

        private void OnDestroy()
        {
            scrollBanner.onChangedPage -= OnChangePage;
            scrollBanner.ScrollGrid.OnBeginPageSnap -= OnBeginPageSnap;
        }

        private void OnBeginPageSnap()
        {
            if(slideSe != SE.None)
            {
                SEManager.PlaySE(slideSe);
            }
        }
        
        private void OnChangePage(int index)
        {
            _currentIndex = index;
            UpdateNextScrollSecond();
        }
        
        private void OnBeginDrag(PointerEventData index) => ResetNextScrollSecond();
        private void OnEndDrag(PointerEventData index) => UpdateNextScrollSecond();
        
        private void Update()
        {
            if (_currentBannerCount <= 0 || _nextScrollTicks > AppTime.Now.Ticks) return;
            ResetNextScrollSecond();
            _currentIndex = (_currentIndex + 1) % _currentBannerCount;
            scrollBanner.SetIndex(_currentIndex, isAnimation: true);
        }
        #endregion

        #region PrivateMethods(_nextScrollSTicks)
        private void UpdateNextScrollSecond()
        {
            var secondTicks = autoScrollSecond * 10000000;
            _nextScrollTicks = AppTime.Now.Ticks + secondTicks;
        }

        private void ResetNextScrollSecond()
        {
            _nextScrollTicks = long.MaxValue;
        }
        #endregion
    }
}