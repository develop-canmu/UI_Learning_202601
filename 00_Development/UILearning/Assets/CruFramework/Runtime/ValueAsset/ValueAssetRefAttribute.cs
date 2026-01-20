using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CruFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ValueAssetRefAttribute : PropertyAttribute
    {
        public abstract Type GetLoaderType();
    }
}