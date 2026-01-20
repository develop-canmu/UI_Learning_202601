using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    public class AutoTrainingStatusPolicyStatusButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectedRoot = null;
        
        [SerializeField]
        private long id = 0;
        public long Id{get{return id;}}
        
        /// <summary>選択</summary>
        public void SetSelect(bool select)
        {
            selectedRoot.SetActive(select);
        }
        
    }
}