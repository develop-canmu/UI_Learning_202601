using System;
using System.Text;
using CruFramework.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UniRx;
using UnityEngine;

namespace Pjfb.Home
{
    public class HomeSlideFestivalBanner : ScrollGridItem
    {
        #region Parameters
        public class Parameters
        {
            public FestivalTimetableMasterObject mFestivalTimetable;
            public HuntTimetableMasterObject mHuntTimetable;
            public FestivalMasterObject mFestival;
            public FestivalEffectStatus uFestivalEffectStatus;
            public bool showBadge;
            public Action<Parameters> onClickBanner;

            public Parameters(FestivalTimetableMasterObject mFestivalTimetable, HuntTimetableMasterObject mHuntTimetable, FestivalMasterObject mFestival, FestivalEffectStatus uFestivalEffectStatus, bool showBadge, Action<Parameters> onClickBanner)
            {
                this.mFestivalTimetable = mFestivalTimetable;
                this.mHuntTimetable = mHuntTimetable;
                this.mFestival = mFestival;
                this.uFestivalEffectStatus = uFestivalEffectStatus;
                this.showBadge = showBadge;
                this.onClickBanner = onClickBanner;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private EventLogoImage eventLogoImage;
        [SerializeField] private UIBadgeNotification badgeNotification;
        [SerializeField] private TMP_Text eventBonusRemainTimeText;
        [SerializeField] private GameObject eventBonusRemainTimeParent;
        #endregion

        #region PrivateFields
        private Parameters parameters;
        private IDisposable updateExitTimeDisposable;
        #endregion

        #region OverrideMethods
        protected override void OnSetView(object value)
        {
            parameters = (Parameters) value;
            badgeNotification.SetActive(parameters.showBadge);
            eventLogoImage.SetTexture(parameters.mFestival.id);
            DisposeExitTimeEvent();
            updateExitTimeDisposable = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => SetEventBonusTime())
                .AddTo(gameObject);
            SetEventBonusTime();
        }
        #endregion

        #region PublicMethods
        public void OnClickBanner()
        {
            parameters.onClickBanner.Invoke(parameters);
        }
        #endregion

        #region PrivateMethods
        private void SetEventBonusTime()
        {
            DateTime now = AppTime.Now;

            // イベントの自体の期間
            FestivalTimetableMasterObject mTimetable = MasterManager.Instance.festivalTimetableMaster.FindByFestivalId(parameters.uFestivalEffectStatus.mFestivalId);
            DateTime deadline = mTimetable == null ? DateTime.MinValue : mTimetable.deadlineAt.TryConvertToDateTime();
            // イベントボーナスの期間
            DateTime endDate = parameters.uFestivalEffectStatus.expireAt.TryConvertToDateTime();
            // イベントとボーナスの期間をチェック
            bool hasNoEventBonus = endDate.IsPast(now) || deadline.IsPast(now);
            // ボーナス表示
            eventBonusRemainTimeParent.SetActive(!hasNoEventBonus);
            if (hasNoEventBonus) DisposeExitTimeEvent();
            else {
                var remainTimeString = endDate.GetPreciseRemainingString(AppTime.Now, textFormat: StringValueAssetLoader.Instance["event.bonus.activation.remain"]);;
                eventBonusRemainTimeText.text = remainTimeString;
            }
        }
        
        private void DisposeExitTimeEvent()
        {
            if (updateExitTimeDisposable == null) return;
            updateExitTimeDisposable.Dispose();
            updateExitTimeDisposable = null;
        }
        #endregion
    }
}