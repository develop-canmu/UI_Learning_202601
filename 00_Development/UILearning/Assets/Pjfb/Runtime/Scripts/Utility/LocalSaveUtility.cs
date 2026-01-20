using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Menu;
using Pjfb.Storage;
using UnityEngine;

namespace Pjfb
{
    public static class LocalSaveUtility
    {
        /// <summary>不変データの生成</summary>
        public static void CreateImmutableSaveData(string appToken)
        {
            ImmutableSaveData saveData = new ImmutableSaveData(appToken);
            LocalSaveManager.Instance.SaveImmutableData(saveData);
        }
    }
}
