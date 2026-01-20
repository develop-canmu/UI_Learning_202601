using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class SliceableIconImage : SliceableIconImageBase
    {
        [SerializeField]
        private ItemIconType iconType = ItemIconType.Item;

        public override ItemIconType IconType { get { return iconType; } }
    }
}