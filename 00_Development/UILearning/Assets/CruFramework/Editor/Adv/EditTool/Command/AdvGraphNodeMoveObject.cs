using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/MoveObject")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "MoveCharacter", "指定したオブジェクトの移動。")]
    public class AdvGraphNodeMoveObject : AdvGraphNodeDefault<AdvCommandMoveObject>
    {
    }
}