using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;

namespace Pjfb
{
    public class CombinationCollectionSkillUnlockedModal : ModalWindow
    {
        public class Data
        {
            public List<CombinationManager.CollectionProgressData> CollectionProgressDataList;
            public Action onClosed;

            public Data(List<CombinationManager.CollectionProgressData> collectionProgressDataList, Action onClosed)
            {
                CollectionProgressDataList = collectionProgressDataList;
                this.onClosed = onClosed;
            }
        }

        [SerializeField] private CombinationCollectionScrollDynamic activatedCollectionScrollDynamic;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject skipButton;
        [SerializeField] private CallAnimationEventAction callAnimationEventAction;

        private Data modalData;
        private bool isSkip;
        
        private Action onClosed;

        public static void Open(Data data)
        {
            // Openのアニメーション最中にタッチ判定を取る必要があり通常のモーダルを使用するとタッチガードがモーダルより前にあるためタッチ判定が取ることができない
            // エラーモーダルを使用することで同じくタッチガードは出るがタッチガードがモーダルより後ろにあるためタッチ判定を取ることができる
            AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.CombinationCollectionSkillUnlocked, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            onClosed = modalData.onClosed;
            
            isSkip = false;
            callAnimationEventAction.AnimationEventAction = () =>
            {
                if(isSkip) return;
                //アニメーションの最後にskipButtonを非アクティブにする
                skipButton.SetActive(false);
                // アニメーション関連でうまく表示されていなさそうなので表示更新を行う
                ScrollUpdate().Forget();
            };
            skipButton.SetActive(true);
            activatedCollectionScrollDynamic.InitializeActivating(modalData.CollectionProgressDataList);
        }

        public void OnClickSkipButton()
        {
            isSkip = true;
            skipButton.SetActive(false);
            // normalizeTimeに1を指定すると最初から再生されてしまうので0.9fで設定する
            animator.Play("Open", 0, 0.9f);
            // アニメーション関連でうまく表示されていなさそうなので表示更新を行う
            ScrollUpdate().Forget();
        }

        private async UniTask ScrollUpdate()
        {
            await UniTask.DelayFrame(1);
            activatedCollectionScrollDynamic.Refresh();
        }

        public void OnCloseButton()
        {
            Close(onClosed);
        }
    }
}