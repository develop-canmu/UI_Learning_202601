using System.Collections;
using System.Collections.Generic;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingInspirationIcon : CancellableImageWithId
    {
        [SerializeField]
        private TrainingInspirationIconFrame frame = null;
        
        public void SetInspirationId(long id)
        {
            // マスタ
            TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(id);
            // フレーム
            frame.SetTexture(mCard.grade);
            // アイコン
            SetTexture(mCard.imageId);
        }
        
        protected override string GetKey(long id)
        {
            return ResourcePathManager.GetPath("InspirationIcon", id);
        }
    }
}