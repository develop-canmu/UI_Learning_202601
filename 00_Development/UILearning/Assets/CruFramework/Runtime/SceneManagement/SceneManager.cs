#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using CruFramework.Addressables;
using Cysharp.Threading.Tasks;

namespace CruFramework.SceneManagement
{
    public class SceneManager<T> where T : Enum
    {
        public static void LoadScene(T scene)
        {
            SceneManager.LoadScene(scene.ToString());
        }
    }
}

#endif