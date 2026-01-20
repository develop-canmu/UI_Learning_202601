using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.ClubDeck
{
    public class ClubTeamSummaryWindow : ModalWindow
    {
        [SerializeField] private ScrollGrid clubDeckScroll;
        #region Params

        public class WindowParams
        {
            public int SelectingIndex;
            public DeckData[] DeckList;
            public Action<long> OnClosed;
        }

        #endregion
        private WindowParams _windowParams;
        private long selectingIndex;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubTeamSummary, data);
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
            selectingIndex = _windowParams.SelectingIndex;
            UpdateScroller(false);
        }

        private void UpdateScroller(bool keepScrollPosition = true)
        {
            var scrollDataList = _windowParams.DeckList.Select((t, i) => new ClubDeckSummaryScrollData(t, i == selectingIndex, OnDeckClick)).ToList();
            float pos = clubDeckScroll.verticalNormalizedPosition;
            clubDeckScroll.SetItems(scrollDataList);
            if(keepScrollPosition)
            {
                clubDeckScroll.verticalNormalizedPosition = pos;
            }
        }

        private void OnDeckClick(long index)
        {
            selectingIndex = index;
            UpdateScroller();
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: () => _windowParams.OnClosed?.Invoke(selectingIndex));
        }
        #endregion
       
        
        
    }
}
