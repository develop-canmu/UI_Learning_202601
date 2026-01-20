using System;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Rivalry
{
    public enum RivalryPageType
    {
        RivalryTop,
        RivalryRegular,
        RivalryTeamSelect,
        RivalryEvent,
        RivalryRewardStealChara,
        RivalryRewardLotteryConfirm,
    }
    
    public class RivalryPage : PageManager<RivalryPageType>
    {
        public class Data
        {
            public RivalryPageType pageType;
            public bool showHeaderAndFooter;
            public object args;

            public Data()
            {
                pageType = RivalryPageType.RivalryTop;
                args = null;
                showHeaderAndFooter = true;
            }

            public Data(RivalryPageType _pageType, object _args, bool _showHeaderAndFooter = true)
            {
                pageType = _pageType;
                args = _args;
                showHeaderAndFooter = _showHeaderAndFooter;
            }
        }
        
        #region SerializeFields
        [SerializeField] private RivalryRewardTransition rewardTransition;
        #endregion
        
        protected Data data;

        protected override string GetAddress(RivalryPageType page)
        {
            return $"Prefabs/UI/Page/Rivalry/{page}Page.prefab";
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (TransitionType != PageTransitionType.Back)
            {
                ClearPageStack();
                data = (Data)args;

                if (data == null) data = new Data();
                return OpenPageAsync(data.pageType, true, data.args, token);
            }

            return default;
        }
        
        protected override void OnEnablePage(object args)
        {
            if (!data.showHeaderAndFooter)
            {
                AppManager.Instance.UIManager.Header.Hide();
                AppManager.Instance.UIManager.Footer.Hide();
            }
            base.OnEnablePage(args);
        }

        public void InitRewardTransition(long mCharaId, Action onFinished, HuntPrizeSet[] prizeSetList)
        {
            rewardTransition.Init(mCharaId, onFinished, prizeSetList);
        }

        public async void OpenRewardTransition()
        {
            await rewardTransition.Open();
        }

        public void CloseRewardTransition()
        {
            rewardTransition.Close();
        }
    }
}


