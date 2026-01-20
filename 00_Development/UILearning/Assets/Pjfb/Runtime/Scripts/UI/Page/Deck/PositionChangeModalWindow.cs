using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb.Deck
{
    public class PositionChangeModalWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public RoleNumber CurrentRole;
            public Action<RoleNumber> onChanged;
            public Action onClosed;
        }

        #endregion

        [Serializable]
        private class PositionButton
        {
            public RoleNumber RoleNumber;
            public UIButton Button;
            public GameObject selectedHighlight;
            public Sprite selectedSprite;
            public Sprite deselectedSprite;

        }

        [SerializeField] private List<PositionButton> positionButtonList;


        private RoleNumber selectingRole;
        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PositionChange, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void Init()
        {
            selectingRole = _windowParams.CurrentRole;
            SetUI();
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }

        public void OnClickChange()
        {
            Close(onCompleted: () => _windowParams.onChanged?.Invoke(selectingRole));
        }

  
        public void OnClickPosition(int roleNumber)
        {
            selectingRole = (RoleNumber)roleNumber;
            SetUI();
        }
        
        #endregion

        private void SetUI()
        {
            foreach (var positionButton in positionButtonList)
            {
                bool isCurrentRole = selectingRole == positionButton.RoleNumber;
                positionButton.Button.image.sprite = (isCurrentRole) ? positionButton.selectedSprite : positionButton.deselectedSprite;
                positionButton.selectedHighlight.SetActive(isCurrentRole);
            }
        }
        
    }
}
