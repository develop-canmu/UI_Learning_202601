using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public abstract class RankImageBase : CancellableRawImageWithId
    {
        public enum ImageSize
        {
            Large,
            Small
        }
        
        [SerializeField]
        private ImageSize size = ImageSize.Small;
        protected ImageSize Size 
        {
             get { return size; }
        }
    }
}