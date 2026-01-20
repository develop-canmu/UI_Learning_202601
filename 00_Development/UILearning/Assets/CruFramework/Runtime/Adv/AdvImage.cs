using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Adv
{
    [RequireComponent(typeof(RawImage))]
    public class AdvImage : AdvTextureObject
    {
        public override void SetTexture(Texture texture)
        {
            RawImage image = GetComponent<RawImage>();
            image.texture = texture;
        }
    }
}
