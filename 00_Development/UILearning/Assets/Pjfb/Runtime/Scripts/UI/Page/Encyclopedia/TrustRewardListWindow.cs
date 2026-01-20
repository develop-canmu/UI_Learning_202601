using System;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb.Encyclopedia
{
    public class TrustRewardListWindow : ModalWindow
    {
        [SerializeField] private ScrollGrid rankRewardScroll;
        #region Params

        public class WindowParams
        {
            public long levelRewardId;
            public long receivedLevel;
            public Action onClosed;
        }

        #endregion
        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrustRewardList, data);
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
            var rewardTable =
                MasterManager.Instance.levelRewardPrizeMaster.values.Where(x => x.mLevelRewardId == _windowParams.levelRewardId);

            var rewardList = rewardTable.Select(x =>
                    new TrustRewardScrollData(x.level, x.prizeJson[0], x.level <= _windowParams.receivedLevel))
                .OrderBy(x => x.HasReceived).ThenBy(x => x.Level).ToList();
            
            rankRewardScroll.SetItems(rewardList);
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
       
        
        
    }
}
