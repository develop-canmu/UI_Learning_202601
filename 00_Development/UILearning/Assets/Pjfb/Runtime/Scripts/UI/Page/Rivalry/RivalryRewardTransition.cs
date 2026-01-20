using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Rivalry
{
    
    public class RivalryRewardTransition : MonoBehaviour
    {
        private const string OpenKey = "Open";

        #region SerializeFields
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private CharacterCardImage whiteCardImage;
        [SerializeField] private Transform prizeRoot;
        [SerializeField] private GameObject prizePrefab;
        [SerializeField] private GameObject skipTrigger = null;
        #endregion

        private Action OnFinishedTransition;
        private List<GameObject> prizeJsonViewList;

        private bool isSkip = false;
        private CancellationTokenSource animationCancellationSource = null;

        #region PublicMethods
        public async UniTask Open() 
        {
            if( isSkip ) {
                OnFinished();
                return;
            }

            animationCancellationSource = new CancellationTokenSource();
            
            try{
                
                animator.SetTrigger(OpenKey);
                await AnimatorUtility.WaitStateAsync(animator, OpenKey, animationCancellationSource.Token);
            } catch( System.OperationCanceledException) {
                OnFinished();
                animationCancellationSource = null;
            } finally{
                OnFinished();
            }
        }

        public void Close() 
        {
            animator.SetTrigger("Close");
            foreach (var view in prizeJsonViewList)
            {
                Destroy(view.gameObject);
            }
            prizeJsonViewList = new List<GameObject>();
        }
        
        public void OnFinished()
        {
            skipTrigger.SetActive(false);
            OnFinishedTransition?.Invoke();
            OnFinishedTransition = null;
        }

        public void Init(long mCharaId, Action onFinished, HuntPrizeSet[] prizeSetList)
        {
            characterCardImage.SetTexture(mCharaId);
            whiteCardImage.SetTexture(mCharaId);
            OnFinishedTransition = onFinished;

            prizeJsonViewList = new List<GameObject>();
            foreach (var huntPrize in prizeSetList)
            {
                foreach (var prize in huntPrize?.prizeJsonList)
                {
                    var view = Instantiate(prizePrefab, prizeRoot);
                    view.gameObject.SetActive(true);
                    prizeJsonViewList.Add(view);
                }
            }

            skipTrigger.SetActive(true);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        }

        public void OnClickSkipTrigger(){
            isSkip = true;
            if( animationCancellationSource != null ) {
                animationCancellationSource.Cancel();
            }
        }
        #endregion
    }
}
