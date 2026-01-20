
using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/TrainingChoiceCondition")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "TrainingChoiceCondition", "トレーニング用シナリオの選択肢表示。")]
    public class AdvGraphNodeTrainingChoiceCondition : AdvGraphNodeDefault<AdvCommandTrainingChoiceConditions>
    {
    }
}

