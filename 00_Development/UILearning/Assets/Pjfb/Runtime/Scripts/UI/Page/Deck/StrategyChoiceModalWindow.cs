using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Deck
{
    public class StrategyChoiceModalWindow : ModalWindow
    {
        private BattleConst.DeckStrategy selectingStrategy;

        [Serializable]
        private class StrategyCell
        {
            public UIButton Button;
            public BattleConst.DeckStrategy strategyType;
            public GameObject SelectedEffect;
            public GameObject Badge;
        }
        #region Params

        public class WindowParams
        {
            public BattleConst.DeckStrategy strategy;
            public Action<BattleConst.DeckStrategy> onStrategyChanged;
            public Action onClosed;
        }

        #endregion

        [SerializeField] private List<StrategyCell> strategyCells;
        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.StrategyChoice, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            selectingStrategy = _windowParams.strategy;
            foreach (var cell in strategyCells)
            {
                cell.SelectedEffect.SetActive(cell.strategyType == selectingStrategy);
                cell.Badge.SetActive(cell.strategyType == selectingStrategy);
            }
        }

        public void OnSelectStrategy(int strategy)
        {
            selectingStrategy = (BattleConst.DeckStrategy)strategy;
            foreach (var cell in strategyCells)
            {
                cell.SelectedEffect.SetActive(cell.strategyType == selectingStrategy);
            }
        }

        public void OnClickChangeButton()
        {
            _windowParams.onStrategyChanged?.Invoke(selectingStrategy);
            Close();
        }
        
        public void OnClickCancelButton()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        
    }
}
