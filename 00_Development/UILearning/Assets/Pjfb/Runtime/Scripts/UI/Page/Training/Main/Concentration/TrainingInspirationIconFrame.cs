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
    public class TrainingInspirationIconFrame : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return ResourcePathManager.GetPath("InspirationIconFrame", id);
        }
    }
}