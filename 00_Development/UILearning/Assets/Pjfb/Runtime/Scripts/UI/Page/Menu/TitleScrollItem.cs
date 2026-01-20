using System;
using System.Threading;
using CruFramework;
using CruFramework.ResourceManagement;
using CruFramework.UI;
using Pjfb.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class TitleScrollItem : ScrollGridItem
    {
        #region Params
        public class Info
        {
            public long Id;
            public bool Setted = false;
            public bool Selected = false;
            public Action<long> OnSelect;
        }

        [SerializeField] private UserTitleImage image;
        [SerializeField] private GameObject selectedObj;
        [SerializeField] private GameObject settingObj;
        [SerializeField] private GameObject newBadgeObj;
        [NonSerialized] public Info info;
        
        #endregion
        
        #region Life Cycle
        protected override void OnSetView(object value)
        {
            info = value as Info;
            if (info == null) return;
            bool isTitleViewed = LocalSaveManager.saveData.viewedTitles.Contains(info.Id);
            if(selectedObj != null) selectedObj.SetActive(info.Selected);
            if(settingObj != null) settingObj.SetActive(info.Setted);
            if(newBadgeObj != null) newBadgeObj.SetActive(!isTitleViewed);
            image.SetTexture(info.Id);
        }
        #endregion

        #region EventListeners
        
        public void OnClickSelect()
        {
            info.OnSelect?.Invoke(info.Id);
        }

        public void OnLongTap()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TitleDetail, new TitleDetailModalWindow.WindowParams()
            {
                Id = info.Id
            });
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