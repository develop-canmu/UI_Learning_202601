using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine;
using Pjfb.UI;
using UnityEngine.UI;

namespace Pjfb.Common
{
    public class MissionModalEventPage : MonoBehaviour
    {
        #region Parameters
        public class Parameters
        {
            public DailyMissionCategoryMasterObject missionEventData;
            public List<MissionProgressPairData> missionProgressPairDataList;
            public float initalScrollValue = 1f;
            public Action<MissionModalListItem.ItemParams, DateTime, float> onListItemClickReceiveButton;
            public Action<MissionModalListItem.ItemParams> onListItemClickChallengeButton;
            public Action<List<DailyMissionCategoryMasterObject>, List<List<MissionProgressPairData>>, DateTime, float> onClickReceiveAllButton;
            public Action<MissionModalEventPage> onClickBackButton;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private Image eventLogo;
        [SerializeField] private UIButton allReceiveButton;
        [SerializeField] private GameObject closedGameObject;
        
        [SerializeField] private RectTransform body;
        [SerializeField] private RectTransform footerParent;
        [SerializeField] private TMPro.TMP_Text footerText;
        #endregion

        #region PrivateFields
        private Parameters _parameters;
        private CancellationTokenSource _source;
        private const string TimeTextFormat = "受け取り期間の更新まであと {0}";
        private const float FadeInDuration = 0.15f;
        #endregion

        #region PublicMethods
        public void Init()
        {
            gameObject.SetActive(false);
            canvasGroup.alpha = 0;
            poolListContainer.Clear();
            titleText.text = string.Empty;
            SetFooterText(string.Empty);
        }

        public void SetDisplay(Parameters parameters, bool useFade)
        {
            _parameters = parameters;

            UpdateDisplay();
            if (useFade) ShowFade(active: true).Forget();
            else
            {
                canvasGroup.alpha = 1;
                gameObject.SetActive(true);
            }
        }

        public async UniTask ShowFade(bool active)
        {
            if (active) gameObject.SetActive(true);
            var startAlpha = active ? 0f : 1f;
            var endAlpha = 1f - startAlpha;
            await DOTween.To(val => canvasGroup.alpha = val, startAlpha, endAlpha, FadeInDuration);
            if (!active) gameObject.SetActive(false);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay()
        {
            var missionEventData = _parameters.missionEventData;
            titleText.text = missionEventData.name;

            var footerText = string.Empty;
            if (!string.IsNullOrEmpty(missionEventData.endDescription)) footerText = missionEventData.endDescription;
            else
            {
                var dateTime = missionEventData.endAt.TryConvertToDateTime();
                footerText = dateTime.GetRemainingString(AppTime.Now);
                footerText = string.IsNullOrEmpty(footerText) ? string.Empty : string.Format(TimeTextFormat, footerText);
            }
            SetFooterText(footerText);

            SetEventLogo();
            closedGameObject.SetActive(_parameters.missionEventData.endAt.TryConvertToDateTime().IsPast(AppTime.Now));
            var missionDataDictionary = _parameters.missionProgressPairDataList
                .ToDictionary(aData => aData.missionData.id);
            var itemParamList = missionDataDictionary
                .Where(aData => aData.Value.ShowMission(missionDataDictionary))
                .Select(aData => new MissionModalListItem.ItemParams
                {
                    missionProgressPairData = aData.Value,
                    onClickReceiveButton = OnClickReceiveButton,
                    onClickChallengeButton = _parameters.onListItemClickChallengeButton
                })
                .OrderBy(aData => aData.missionProgressPairData.sortOrder).ToList();

            allReceiveButton.interactable = itemParamList.Select(aData => aData.missionProgressPairData).Any(aProgressPairData => aProgressPairData.hasReceivingReward);
            poolListContainer.SetDataList(itemParamList, scrollValue: _parameters.initalScrollValue).Forget();

        }

        private void SetEventLogo()
        {
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
                _source = null;
            }
            _source = new CancellationTokenSource();
            eventLogo.gameObject.SetActive(false);
            //symbolName定義あり場合は画像はsymbolNameを使う
            string imagePath = (_parameters.missionEventData.symbolName.StartsWith("event_logo_"))
                ? $"Images/MissionEventLogo/{_parameters.missionEventData.symbolName}.png" 
                : $"Images/MissionEventLogo/event_logo_{_parameters.missionEventData.id}.png";
            PageResourceLoadUtility.LoadAssetAsync<Sprite>(imagePath,
                callback: sprite =>
                {
                    eventLogo.sprite = sprite;
                    eventLogo.SetNativeSize();
                    eventLogo.gameObject.SetActive(true);
                }, token: _source.Token).Forget();
        }

        private void SetFooterText(string text)
        {
            if (string.IsNullOrEmpty(text)) SetActiveFooter(isActive: false);
            else
            {
                footerText.text = text;
                SetActiveFooter(isActive: true);
            }
        }

        private void SetActiveFooter(bool isActive)
        {
            footerParent.gameObject.SetActive(isActive);
            body.offsetMin = new Vector2(body.offsetMin.x, 100 + (isActive ? footerParent.sizeDelta.y : 0));
        }
        #endregion

        #region EventHandler
        public void OnClickBackButton()
        {
            _parameters?.onClickBackButton?.Invoke(this);
        }

        public void OnClickReceiveAllButton()
        {
            var missionCategory = _parameters.missionEventData;
            var receiveEndAt = missionCategory.receiveEndAt.TryConvertToDateTime(minValueDefault: false);
            var lastScrollValue = poolListContainer.scrollValue;
            _parameters?.onClickReceiveAllButton?.Invoke(new List<DailyMissionCategoryMasterObject> { _parameters.missionEventData }, new List<List<MissionProgressPairData>> { _parameters.missionProgressPairDataList }, receiveEndAt, lastScrollValue);
        }

        private void OnClickReceiveButton(MissionModalListItem.ItemParams itemParams)
        {
            var missionCategory = itemParams.missionProgressPairData.missionCategory;
            var receiveEndAt = missionCategory.receiveEndAt.TryConvertToDateTime(minValueDefault: false);
            var lastScrollValue = poolListContainer.scrollValue;
            _parameters.onListItemClickReceiveButton(itemParams, receiveEndAt, lastScrollValue);
        }
        #endregion
    }
}