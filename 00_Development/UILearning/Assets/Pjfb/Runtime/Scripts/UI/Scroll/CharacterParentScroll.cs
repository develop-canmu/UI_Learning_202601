using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Encyclopedia
{
    public class CharacterParentScroll : ItemIconScroll<CharaParentData>
    {

        private List<CharaParentData> scrollDataList = new ();
        public void SetItems(List<CharaParentData> data)
        {
            scrollDataList = data;
            Refresh();
        }
        protected override List<CharaParentData> GetItemList()
        {
            return scrollDataList;
        }
    }
    
}