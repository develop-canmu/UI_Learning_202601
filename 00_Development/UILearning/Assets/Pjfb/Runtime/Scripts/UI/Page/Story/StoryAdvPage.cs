using System;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.Adv;
using Pjfb.Master;
using Pjfb.Story;
using Pjfb.UI;

namespace Pjfb
{
    
    public class StoryAdvPage : StoryPageBase
    {
        public enum State
        {
            Before,
            After
        }
        
        public class PageParam
        {
            public HuntEnemyScenarioMasterObject mScenario;
            public bool showToBeContinued;
            public State state;
            public Action onFinish;
        }
        
        [SerializeField]
        private AppAdvManager advManager = null;
        [SerializeField]
        private AnimatorController toBeContinuedAnimatorController;
        
        private PageParam _pageParams;
        private bool waitForToBeContinuedClicked;
            
        private async void OnAdvEnd()
        {
            if (_pageParams.showToBeContinued)
            {
                toBeContinuedAnimatorController.gameObject.SetActive(true);
                await toBeContinuedAnimatorController.Play("FadeIn");
                waitForToBeContinuedClicked = true;
                while (waitForToBeContinuedClicked) await UniTask.NextFrame();
            }
            _pageParams.onFinish.Invoke();
        }

        private void Awake()
        {
            advManager.OnEnded += OnAdvEnd;
        }

#if UNITY_EDITOR || UNITY_ANDROID
        #region BackKey処理
        private bool isProcessingBackKeySkip;
        private async void Update()
        {
            if (!isProcessingBackKeySkip && Input.GetKeyDown(KeyCode.Escape) && AppManager.Instance.UIManager.ModalManager.GetTopModalWindow() == null)
            {
                isProcessingBackKeySkip = true;
                SEManager.PlaySEAsync(SE.se_common_tap).Forget();
                await advManager.OnSkipButtonAsync();
                isProcessingBackKeySkip = false;
            }
        }
        #endregion
#endif // UNITY_EDITOR || UNITY_ANDROID
        
        protected override void OnDestroy()
        {
            advManager.OnEnded -= OnAdvEnd;
            base.OnDestroy();
        }

        /// <summary>
        /// メモ：
        /// インゲームのOnPreCloseより先にストーリーのサブページのオープン処理が呼ばれてるため、
        /// メインページ側であるStoryPageのOnEnablePageで非表示を行う
        /// </summary>
        public static void HideHeaderFooter()
        {
            AppManager.Instance.UIManager.Header.Hide(); 
            AppManager.Instance.UIManager.Footer.Hide();
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _pageParams = (PageParam)args;
            toBeContinuedAnimatorController.gameObject.SetActive(false);
            
            var id = _pageParams.state == State.Before ? _pageParams.mScenario.beforeScenarioNumber : _pageParams.mScenario.afterScenarioNumber;
            await advManager.LoadAdvFile( ResourcePathManager.GetPath(AppAdvManager.ResourcePathKey, id) );
            await base.OnPreOpen(args, token);
        }

        public void OnClickToBeContinuedCollider()
        {
            waitForToBeContinuedClicked = false;
        }
    }
}
