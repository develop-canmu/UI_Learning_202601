
using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/TrainingChoice")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "TrainingCoice", "トレーニング用シナリオの選択肢表示。")]
    public class AdvGraphNodeTrainingChoice : AdvGraphNodeSelectBase<AdvCommandTrainingChoice>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}

