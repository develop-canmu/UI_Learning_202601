using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/DeleteCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "DeleteCharacter", "指定したキャラクタの削除。")]
    public class AdvGraphNodeDeleteCharacter : AdvGraphNodeDefault<AdvCommandDeleteCharacter>
    {

    }
}