using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CruFramework.UI;
using UnityEngine.Events;

namespace CruFramework.Adv
{
    [RequireComponent(typeof(UIButton))]
    public abstract class AdvSelectButton : MonoBehaviour
    {
        private int no = 0;
        /// <summary>番号</summary>
        public int No{get{return no;}set{no = value;}}
        
        [SerializeField]
        private UnityEvent<int> onSelected = new UnityEvent<int>();

        private UIButton button = null;
        /// <summary>ボタン</summary>
        public UIButton Button
        {
            get
            {
                if(button == null)button = gameObject.GetComponent<UIButton>();
                return button;
            }
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelected()
        {
            onSelected.Invoke(no);
        }
        
        public abstract void SetMessage(string message);
    }
}
