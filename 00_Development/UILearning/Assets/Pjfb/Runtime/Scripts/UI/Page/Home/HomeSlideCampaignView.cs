using System;
using System.Linq;
using UnityEngine;
using Pjfb.Networking.App.Request;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

namespace Pjfb.Home
{
    public class HomeSlideCampaignView : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private SE slideSe = SE.se_common_slide;
        [SerializeField] private ScrollBanner scrollBanner;
        [SerializeField] private long autoScrollSecond = 5;
        #endregion

        #region PrivateProperties
        private int _currentIndex;
        private int _currentBannerCount;
        private long _nextScrollTicks;
        #endregion

        #region PublicMethods
        
        public void SetBannerDatas(NewsBanner[] newsBannerList)
        {
            scrollBanner.SetBannerDatas(newsBannerList.Select(aData => new HomeSlideCampaignBanner.Parameters{bannerData = aData}).ToList());
            _currentBannerCount = newsBannerList.Length;
            UpdateNextScrollSecond();
        }
        #endregion

        #region EventListener
        private void Awake()
        {
            ResetNextScrollSecond();
            scrollBanner.onChangedPage += OnChangePage;
            scrollBanner.ScrollGrid.OnBeginDragAsObservable().Subscribe(OnBeginDrag);
            scrollBanner.ScrollGrid.OnEndDragAsObservable().Subscribe(OnEndDrag);
            
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