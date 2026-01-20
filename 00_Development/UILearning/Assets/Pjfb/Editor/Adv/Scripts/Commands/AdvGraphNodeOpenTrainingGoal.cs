
using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/OpenTrainingGoal")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "OpenTrainingGoal", "トレーニングの目標モーダルを開く。")]
    public class AdvGraphNodeOpenTrainingGoal : AdvGraphNodeDefault<AdvCommandOpenTrainingGoal>
    {

    }
}

