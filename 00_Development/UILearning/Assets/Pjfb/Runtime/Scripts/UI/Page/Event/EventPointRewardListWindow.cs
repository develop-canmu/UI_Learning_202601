using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Event
{
    public class EventPointRewardListWindow : ModalWindow
    {
        [SerializeField] private ScrollGrid eventRewardScroll;
        [SerializeField] private TextMeshProUGUI eventPointText;
        [SerializeField] private OmissionTextSetter eventPointOmissionTextSetter;
        #region Params

        public class WindowParams
        {
            public List<MissionProgressPairData> MissionProgressList;
            public long EventPoint;
            public Action OnClosed;
        }

        #endregion
        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.EventPointRewardList, data);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }



        #region PrivateMethods
        private void Init()
        {
            eventPointText.text = new BigValue(_windowParams.EventPoint).ToDisplayString(eventPointOmissionTextSetter.GetOmissionData());
            List<EventRewardScrollData> scrollDataList = new();
            foreach (var missionProgressPairData in _windowParams.MissionProgressList)
            {
                if(missionProgressPairData is null) continue;
                scrollDataList.Add(new EventRewardScrollData(missionProgressPairData.missionData, missionProgressPairData.MissionProgressStatus == MissionProgressStatus.End, missionProgressPairData.sortOrder));
            }

            scrollDataList = scrollDataList.OrderBy(x => x.SortOrder).ToList();
            eventRewardScroll.SetItems(scrollDataList); 
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.OnClosed);
        }
        #endregion
       
        
        
    }
}
