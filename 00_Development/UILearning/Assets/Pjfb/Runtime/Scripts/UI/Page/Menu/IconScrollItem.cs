using System;
using System.Threading;
using CruFramework;
using CruFramework.ResourceManagement;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class IconScrollItem : ScrollGridItem
    {
        #region Params
        public class Info
        {
            public long Id;
            public bool Setted = false;
            public bool Selected = false;
            public Action<long> OnSelect;
        }

        [SerializeField] private ItemIconBase image;
        [SerializeField] private GameObject selectedObj;
        [SerializeField] private GameObject settingObj;
        [NonSerialized] public Info info;

        #endregion
        
        #region Life Cycle
        protected override void OnSetView(object value)
        {
            info = value as Info;
            if (info == null) return;
            if(selectedObj != null) selectedObj.SetActive(info.Selected);
            if(settingObj != null) settingObj.SetActive(info.Setted);
            image.SetIconIdAsync(info.Id).Forget();
        }
        #endregion

        #region EventListeners
        
        public void OnClickSelect()
        {
            info.OnSelect?.Invoke(info.Id);
        }

        #endregion

        #region Other

        public void Deselect()
        {
            selectedObj.SetActive(false);
            info.Selected = false;
        }

        #endregion
        

    }
}