using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb.Training
{

    /// <summary>
    /// トレーニングメニューで表示するScrollGrid内アイテム
    /// </summary>
    public class TrainingMenuItem: ScrollGridItem
    {
        public class Arguments
        {
            private long scenarioId = 0;
            /// <summary>シナリオId</summary>
            public long ScenarioId{get{return scenarioId;}}
            
            private  bool isLocked = false;
            /// <summary>未開放</summary>
            public bool IsLocked{get{return isLocked;}}

            public Arguments(long scenarioId, bool isLocked)
            {
                this.scenarioId = scenarioId;
                this.isLocked = isLocked;
            }
        }
    
        [SerializeField]
        private CancellableRawImage bannerImage = null;
        
        private Arguments args = null;

        protected override void OnSetView(object value)
        {
            args = (Arguments)value;
            // バナーの読み込み
            bannerImage.SetTexture(ResourcePathManager.GetPath("TrainingMenuSmallBanner", args.ScenarioId));
        }
        

    }
}