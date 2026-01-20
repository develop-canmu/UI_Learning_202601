using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/ShakeCharacter")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "ShakeCharacter", "キャラクタを揺らす。")]
    public class AdvGraphNodeShakeCharacter : AdvGraphNodeDefault<AdvCommandShakeCharacter>
    {

    }
}