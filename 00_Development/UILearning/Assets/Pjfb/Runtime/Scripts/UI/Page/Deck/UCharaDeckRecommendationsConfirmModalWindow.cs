using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Deck
{
    public class UCharaDeckRecommendationsConfirmModalWindow : ModalWindow
    {
        public class ModalParams
        {
            // 実行前の編成
            private CharacterDetailData[] beforeData;
            public CharacterDetailData[] BeforeData => beforeData;
            // 実行後の編成
            private CharacterDetailData[] afterData;
            public CharacterDetailData[] AfterData => afterData;
            public ModalParams(CharacterDetailData[] beforeData, CharacterDetailData[] afterData)
            {
                this.beforeData = beforeData;
                this.afterData = afterData;
            }
        }
        
        [SerializeField]
        private AdviserDeckView beforeView;
        [SerializeField]
        private AdviserDeckView afterView;
        
        private ModalParams modalParams;
        

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalParams = (ModalParams)args;
            SetViewData();
            SetCloseParameter(false);
            return base.OnPreOpen(args, token);
        }

        private void SetViewData()
        {
            beforeView.InitView(modalParams.BeforeData);
            afterView.InitView(modalParams.AfterData);
        }
        
        public void OnClickApplyButton()
        {
            // 確定ボタンが押されたときの処理を実行する
            SetCloseParameter(true);
            Close();
        }
    }
}