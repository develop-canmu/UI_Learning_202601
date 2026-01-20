using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Utility;
using TMPro;
using UnityEngine;

namespace Pjfb.Home
{
    public class HomeBeginnerMissionClearPopupWindow : ModalWindow
    {
        #region InnerClass
        public class Parameters
        {
            public MissionCategoryStatus beginnerMissionCategoryStatus;
            public DailyMissionMasterObject beginnerMissionObject;
            public Action<Parameters> onComplete;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private TextMeshProUGUI orderText = null;
        [SerializeField] private List<PrizeJsonView> itemIcons;
        #endregion

        #region PrivateFields
        private Parameters _parameters;
        #endregion

        #region StaticMethods
        public static void Open(Parameters parameters)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HomeBeginnerMissionPopup, args: parameters);
        }
        #endregion
        
        #region OverrideMethods
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _parameters = (Parameters) args;
            descriptionText.text = _parameters.beginnerMissionObject.description;
            orderText.text = string.Format(StringValueAssetLoader.Instance["beginner_mission.order_format"], _parameters.beginnerMissionObject.sortNumber);
            var prizeJson = _parameters.beginnerMissionObject.prizeJson;
            var showingPrizeCount = Mathf.Min(itemIcons.Count, prizeJson.Length);
            for (var i = 0; i < showingPrizeCount; i++) {
                itemIcons[i].gameObject.SetActive(true);
                itemIcons[i].SetView(prizeJson[i]);
            }
            for (var i = showingPrizeCount; i < itemIcons.Count; i++) itemIcons[i].gameObject.SetActive(false);
            return base.OnPreOpen(args, token);
        }
        #endregion

        #region PrivateMethods
        #endregion
        
        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: () => _parameters.onComplete?.Invoke(_parameters));
        }

        public void OnClickShowDetailButton()
        {
            ServerActionCommandUtility.ProceedAction(_parameters.beginnerMissionObject.linkEx);
        }
        #endregion
    }
}
