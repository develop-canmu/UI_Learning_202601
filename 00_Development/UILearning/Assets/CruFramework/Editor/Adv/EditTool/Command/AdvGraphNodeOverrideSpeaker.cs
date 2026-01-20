using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/OverrideSpeaker")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "OverrideSpeaker", "メッセージウィンドウの話者の文字列を上書き。")]
    public class AdvGraphNodeOverrideSpeaker : AdvGraphNodeDefault<AdvCommandOverrideSpeaker>
    {
    }
}