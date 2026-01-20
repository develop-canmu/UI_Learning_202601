
using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/TrainingVoice")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "TrainingVoice", "トレーニング用のボイス再生。")]
    public class AdvGraphNodeTrainingVoice : AdvGraphNodeDefault<AdvCommandTrainingVoice>
    {

    }
}

