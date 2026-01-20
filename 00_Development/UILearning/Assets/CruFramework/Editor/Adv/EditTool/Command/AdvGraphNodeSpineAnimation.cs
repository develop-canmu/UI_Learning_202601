
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    [AdvCommandNode("Framework/SpineAnimation")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "SpineAnimation", "Spineオブジェクトのアニメーション再生。")]
    public class AdvGraphNodeSpineAnimation : AdvGraphNodeDefault<AdvCommandSpineAnimation>
    {

    }
}

#endif // CRUFRAMEWORK_SPINE_SUPPORT