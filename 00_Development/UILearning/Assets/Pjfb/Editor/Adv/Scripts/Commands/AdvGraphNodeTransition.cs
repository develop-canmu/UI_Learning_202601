using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/Transition")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "Transition ", "場面転換。")]
    public class AdvGraphNodeTransition : AdvGraphNodeDefault<AdvCommandTransition>
    {

    }
}