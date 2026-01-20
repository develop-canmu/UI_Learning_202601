using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb.Common
{

    public class CharacterDetailEventItem : ScrollGridItem
    {
        
        [SerializeField]
        private TMPro.TMP_Text nameText = null;
        
        protected override void OnSetView(object value)
        {
            nameText.text = "Event Id " + value.ToString();
        }
    }
}