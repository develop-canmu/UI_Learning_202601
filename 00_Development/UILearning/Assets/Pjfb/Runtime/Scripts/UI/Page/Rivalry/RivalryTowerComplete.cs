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
    
    public class RivalryTowerComplete : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private Animator animator;
        #endregion

        private Action OnFinishedTransition;
        private List<GameObject> prizeJsonViewList;

        #region PublicMethods
        public void Open(Action onFinishedTransition = null)
        {
            OnFinishedTransition = onFinishedTransition;
            animator.SetTrigger("Open");
        }
        
        public void OnFinished()
        {
            gameObject.SetActive(false);
            OnFinishedTransition?.Invoke();
        }
        #endregion
    }
}
