using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.UI
{
    public class ToggleContainer : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private List<ToggleItemBase> toggles;
        #endregion

        #region PrivateFields
        private Func<int, UniTask> onSelectIndex;
        private int currentIndex;
        private bool disableClick;
        #endregion

        #region PublicMethods
        public void Init(int initialIndexDisplay, Func<int, UniTask> onSelectIndex)
        {
            this.onSelectIndex = onSelectIndex;
            currentIndex = initialIndexDisplay;
            disableClick = false;
            
            for (var i = 0; i < toggles.Count; i++) toggles[i].Init(i, OnClickToggle);
            toggles[initialIndexDisplay].SetDisplay(true);
        }
        #endregion

        #region EventHandler
        private async void OnClickToggle(int index)
        {
            if (currentIndex == index || disableClick) return;
            currentIndex = index;
            
            foreach (var aToggle in toggles) aToggle.SetDisplay(aToggle.index == index);
            disableClick = true;
            if (onSelectIndex != null) await onSelectIndex(index);
            disableClick = false;
        }
        #endregion
    }
}