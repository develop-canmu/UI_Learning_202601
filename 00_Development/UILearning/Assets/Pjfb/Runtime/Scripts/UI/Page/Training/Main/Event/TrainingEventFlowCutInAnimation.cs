using System.Threading;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using CruFramework.Timeline;
using UnityEngine.Playables;
using CruFramework.H2MD;

namespace Pjfb.Training
{
    /// <summary> FLOWカットイン演出 </summary>
    public class TrainingEventFlowCutInAnimation : TrainingEventCutInAnimationBase
    {
        /// <summary> FLOW用のカットインのパスを取得 </summary>
        protected override string GetCutInPath(long mCharaId)
        {
            return $"H2MD/InGame/SkillCutIn/Flow_SkillCutin.h2md";
        }
    }
}