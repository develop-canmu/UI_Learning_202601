using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public class ChatStampInfo
    {
        public long stampId;
        public Action<long> OnClickEvent;
    }

    public class ChatStamp : ScrollGridItem
    {
        [SerializeField] private IconImage image;

        private ChatStampInfo info;

        protected override void OnSetView(object value)
        {
            info = value as ChatStampInfo;
            image.SetTexture(info.stampId);
        }
        public void OnClickButton()
        {
            info.OnClickEvent?.Invoke(info.stampId);
        }
    }
}

