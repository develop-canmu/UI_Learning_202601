using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.Training
{
    public interface IAutoTrainingReloadable
    {
        UniTask OnReloadAsync();
    }
}