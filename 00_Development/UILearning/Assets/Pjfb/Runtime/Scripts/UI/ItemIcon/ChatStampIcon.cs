using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb
{
    public class ChatStampIcon : ItemIconBase
    {
        public override ItemIconType IconType { get { return ItemIconType.ChatStamp; } }
        
        [SerializeField]
        private bool openDetailModal = true;
        private long stampId = 0;

        protected override void OnSetId(long id)
        {
            base.OnSetId(id);
            stampId = id;
        }
        
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            StampDetailModal.Open(new StampDetailModal.Data { Id = stampId });
        }
    }
}