using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using UnityEngine.UI;

namespace Pjfb
{
    
    [RequireComponent(typeof(SheetTab))]
    public abstract class SheetSwitchButton<TManager, TSheet> : CruFramework.Page.SheetSwitchButton<TManager, TSheet> where TManager : SheetManager<TSheet> where TSheet : System.Enum
    {
        private SheetTab _tab = null;
        /// <summary>タブ情報</summary>
        public SheetTab Tab
        {
            get
            {
                if(_tab == null)
                {
                    _tab = gameObject.GetComponent<SheetTab>();
                }
                return _tab;
            }
        }

        protected override void OnClickOpenSheet()
        {
            if(Tab.Se != SE.None)
            {
                SEManager.PlaySE(Tab.Se);
            }
        }

        protected override void OnOpened()
        {
            // タブをアクティブ状態に
            Tab.SetActiveState(true);
        }
        
        protected override void OnClosed()
        {
            // タブを非アクティブ状態に
            Tab.SetActiveState(false);
        }
    }
}
