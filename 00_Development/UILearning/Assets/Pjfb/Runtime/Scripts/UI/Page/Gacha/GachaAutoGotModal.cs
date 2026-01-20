using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Gacha
{
    public class GachaAutoGotModal : ModalWindow {
        public class Param{
            public PrizeJsonViewData[] prizeList = null;
        }

        [SerializeField]
        PrizeJsonView _prizeJsonViewPrefab = null;
        [SerializeField]
        Transform _iconRoot = null;

        Param _param = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            CreatePrizeIcons();
            return default;
        }   

        public void OnClickClose(){
            Close();
        }

        void CreatePrizeIcons(){
            foreach( var prize in _param.prizeList ){
                var view = Instantiate<PrizeJsonView>(_prizeJsonViewPrefab, _iconRoot);
                view.SetView(prize);
            }
        }
    }
}
