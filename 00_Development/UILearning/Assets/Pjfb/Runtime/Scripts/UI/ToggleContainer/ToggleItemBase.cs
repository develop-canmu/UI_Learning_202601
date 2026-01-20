using System;
using UnityEngine;

namespace Pjfb.UI
{
    public class ToggleItemBase : MonoBehaviour
    {
        #region Properties
        public int index { get; private set; }
        private Action<int> onClickToggle;
        #endregion
        
        #region PublicMethods
        public virtual void Init(int index, Action<int> onClickToggle)
        {
            this.index = index;
            this.onClickToggle = onClickToggle;
            SetDisplay(false);
        }

        public virtual void SetDisplay (bool isSelected) { }
        #endregion
        
        #region EventHandler
        public void OnClickToggle()
        {
            onClickToggle?.Invoke(index);
        }
        #endregion
    }
}