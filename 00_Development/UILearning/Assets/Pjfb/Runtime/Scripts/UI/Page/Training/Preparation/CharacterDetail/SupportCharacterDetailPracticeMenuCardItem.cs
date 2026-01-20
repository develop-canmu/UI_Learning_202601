using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb
{
    public class SupportCharacterDetailPracticeMenuCardItem : ScrollGridItem
    {
    
        [SerializeField]
        private PracticeCardView cardView = null;
    
        protected override void OnSetView(object value)
        {
            int id = (int)value;
            cardView.SetCard(id);
        }
    }
}