using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework;

namespace Pjfb
{
    public class ColorValueAttribute : ValueAssetRefAttribute
    {
        public override Type GetLoaderType()
        {
            return typeof(ColorValueAssetLoader);
        }
    }
}