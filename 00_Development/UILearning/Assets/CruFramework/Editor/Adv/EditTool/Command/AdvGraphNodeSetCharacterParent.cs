using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/SetCharacterParent")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "SetCharacterParent", "指定したキャラクタの親を指定。")]
    public class AdvGraphNodeSetCharacterParent : AdvGraphNodeDefault<AdvCommandSetCharacterParent>
    {
    }
}