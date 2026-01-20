using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/Tutorial")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Tutorial", "チュートリアル。")]
    public class AdvGraphNodeTutorial : AdvGraphNodeDefault<AdvCommandTutorial>
    {

    }
}
