using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/MoveCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "MoveCharacter", "指定したキャラクタの移動。")]
    public class AdvGraphNodeMoveCharacter : AdvGraphNodeDefault<AdvCommandMoveCharacter>
    {
    }
}