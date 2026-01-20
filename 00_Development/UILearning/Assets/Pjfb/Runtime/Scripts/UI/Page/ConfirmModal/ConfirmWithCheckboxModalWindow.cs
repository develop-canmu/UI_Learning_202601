using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public class ConfirmWithCheckboxModalWindow : ConfirmModalWindow
    {
        #region Params  
        [SerializeField] private UIToggle checkBox;
        private ConfirmModalData _data;
        #endregion
        
        #region LifeCycle
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _data = (ConfirmModalData)args;
            checkBox.isOn = false;
            OnCheckboxValueChanged(checkBox.isOn);
            return base.OnPreOpen(args, token);
        }
        
        #endregion
        
        #region EventListeners
        public void OnCheckboxValueChanged(bool value)
        {
            if (_data.PositiveButtonParams.isRed)
                redPositiveButton.interactable = value;
            else
                positiveButton.interactable = value;
        }

        public void OnClickToggle()
        {
            checkBox.isOn = !checkBox.isOn;
        }

        #endregion
    }
}
