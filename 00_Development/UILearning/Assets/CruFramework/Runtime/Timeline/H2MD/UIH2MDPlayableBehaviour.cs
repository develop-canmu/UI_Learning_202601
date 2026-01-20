using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Timeline
{
    public class UIH2MDPlayableBehaviour : H2MDPlayableBehaviour
    {
        internal RawImage target = null;

        protected override void OnLoadAsset(Texture2D texture)
        {
            target.texture = texture;
        }
    }
}
