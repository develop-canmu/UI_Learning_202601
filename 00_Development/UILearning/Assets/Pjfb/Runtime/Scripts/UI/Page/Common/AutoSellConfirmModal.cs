using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class AutoSellConfirmModal : ModalWindow
    {
        public class Data
        {
            public readonly List<NativeApiAutoSell> AutoSellList;
            public Action onClosed;
            
            public Data(NativeApiAutoSell autoSell, Action onClosed = null)
            {
                AutoSellList = new List<NativeApiAutoSell>{autoSell};
            }
            
            public Data(List<NativeApiAutoSell> autoSellList, Action onClosed = null)
            {
                AutoSellList = autoSellList;
            }
        }
        
        [SerializeField] private ScrollGrid beforeConversionScrollGrid;
        [SerializeField] private ScrollGrid afterConversionScrollGrid;

        private Data modalData;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            InitializeUi();
            return base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            modalData.onClosed?.Invoke();
        }

        private void InitializeUi()
        {
            var prizeListSold = modalData.AutoSellList.SelectMany(x => x.prizeListSold.Select(prizeJsonWrap => new PrizeJsonGridItem.Data(prizeJsonWrap))).ToList();
            var prizeListGot = modalData.AutoSellList.SelectMany(x => x.prizeListGot.Select(prizeJsonWrap => new PrizeJsonGridItem.Data(prizeJsonWrap))).ToList();
            beforeConversionScrollGrid.SetItems(prizeListSold);
            afterConversionScrollGrid.SetItems(prizeListGot);
        }
    }
}