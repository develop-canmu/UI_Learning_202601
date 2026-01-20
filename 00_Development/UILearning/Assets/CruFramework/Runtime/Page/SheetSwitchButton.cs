using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

using UnityEngine.UI;

namespace CruFramework.Page
{
    
    public abstract class SheetSwitchButton : MonoBehaviour
    {
        internal abstract void OnCloseInternal();
        internal abstract void OnOpenInternal();
    }
    
    [RequireComponent(typeof(Button))]
    public abstract class SheetSwitchButton<TManager, TSheet> : SheetSwitchButton where TManager : SheetManager<TSheet> where TSheet : System.Enum
    {
        [SerializeField]
        private TSheet sheetType = default;
        /// <summary>シートの種類</summary>
        public TSheet SheetType{get{return sheetType;}}
        
        [SerializeField]
        private bool isClickedOpen = true;
                
        private Button switchButton = null;
        /// <summary>ボタン</summary>
        public Button SwitchButton{get{return switchButton;}}
        
        private TManager _manager = null;
        /// <summary>Switcher</summary>
        public TManager Manager
        {
            get
            {
                if(_manager == null)
                {
                    _manager = gameObject.GetComponentInParent<TManager>();
                }
                return _manager;
            }
        }
        
        protected virtual void OnClickOpenSheet(){}
        
        private void Awake()
        {
            // ボタンを取得
            switchButton = gameObject.GetComponent<Button>();
            // クリックイベントを登録
            switchButton.onClick.AddListener(OnClick);
            Manager.RegisterButton(sheetType, this);
        }

        private void OnDestroy()
        {
            // クリックイベントを削除
            switchButton.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            // シートを開く
            if(isClickedOpen)
            {
                OnClickOpenSheet();
                Manager.OpenSheet(sheetType, null);
            }
        }
        
        protected virtual void OnOpened()
        {
        }
        
        protected virtual void OnClosed()
        {
        }
        
        internal sealed override void OnOpenInternal()
        {
            OnOpened();
        }

        internal sealed override void OnCloseInternal()
        {
            OnClosed();
        }
    }
}