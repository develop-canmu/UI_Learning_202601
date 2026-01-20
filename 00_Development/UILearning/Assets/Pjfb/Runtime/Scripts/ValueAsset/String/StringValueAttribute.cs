using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework;

namespace Pjfb
{
    public class StringValueAttribute : ValueAssetRefAttribute
    {
        public override Type GetLoaderType()
        {
            return typeof(StringValueAssetLoader);
        }
    }
}