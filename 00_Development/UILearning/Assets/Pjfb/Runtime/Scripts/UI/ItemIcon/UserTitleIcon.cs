using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb
{
    public class UserTitleIcon : ItemIconBase
    {
        public override ItemIconType IconType { get { return ItemIconType.UserTitle; } }
        
        [SerializeField]
        private bool openDetailModal = true;
        private long titleId = 0;
        protected override UniTask SetIconTextureAsync(long id)
        {
            // 汎用画像のみの表示
            return UniTask.Delay(0);
        }
        
        protected override void OnSetId(long id)
        {
            base.OnSetId(id);
            titleId = id;
        }
        
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            TitleDetailModalWindow.Open(new TitleDetailModalWindow.WindowParams{Id = titleId});
        }
    }
}