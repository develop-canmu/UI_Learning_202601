using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{

    [AdvCommandNode("Framework/PlayBgm")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "PlayBgm", "BGMの再生。")]
    public class AdvGraphNodePlayBgm : AdvGraphNodeDefault<AdvCommandPlayBgm>
    {

    }
}
