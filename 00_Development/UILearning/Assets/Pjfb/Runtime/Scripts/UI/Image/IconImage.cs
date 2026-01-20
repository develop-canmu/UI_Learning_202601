using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class IconImage : IconImageBase
    {
        [SerializeField]
        private ItemIconType iconType = ItemIconType.Item;
        
        public override ItemIconType IconType { get { return iconType; } }
    }
}
