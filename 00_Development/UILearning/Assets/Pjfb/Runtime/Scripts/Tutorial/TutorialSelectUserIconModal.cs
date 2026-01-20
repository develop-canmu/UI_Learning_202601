using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;


namespace Pjfb
{
    public class TutorialSelectUserIconModal : ModalWindow
    {
        [SerializeField] private ScrollGrid scrollGrid;
        private long selectIconId = 0;
        
        private List<IconScrollItem.Info> iconList = new List<IconScrollItem.Info>();
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            iconList.Clear();
            var mIconIdList = MasterManager.Instance.iconMaster.values.Where(icon => icon.isPrimary);
            
            foreach (var icon in mIconIdList)
            {
                var info = new IconScrollItem.Info {Id = icon.id, Selected = iconList.Count == 0, OnSelect = OnClickIcon};
                iconList.Add(info);
            }
            selectIconId = iconList.FirstOrDefault().Id;
            scrollGrid.SetItems(iconList);
            return base.OnPreOpen(args, token);
        }

        public void OnClickBack()
        {
            Close();
        }

        public void OnClickAgree()
        {
            AppManager.Instance.TutorialManager.RegisterUserIcon(selectIconId);
            Close();
        }

        private void OnClickIcon(long selectedItemId)
        {
            selectIconId = selectedItemId;
            foreach (var item in iconList)
            {
                item.Selected = selectIconId == item.Id;
            }
            scrollGrid.RefreshItemView();
        }
    }
}