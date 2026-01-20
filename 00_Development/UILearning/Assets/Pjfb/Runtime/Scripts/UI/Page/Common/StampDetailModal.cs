using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class StampDetailModal : ModalWindow
    {
        public class Data
        {
            public long Id;
        }
        
        [SerializeField] private ChatStampIcon stampIcon;
        private Data modalData;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.StampDetail, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            if(modalData!= null) stampIcon.SetTexture(modalData.Id);
            return base.OnPreOpen(args, token);
        }
    }
}