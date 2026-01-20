
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using CruFramework.Editor.Adv;
using CruFramework.Adv;
using Pjfb.Adv;
using UnityEngine;

namespace Pjfb.Editor.Adv
{
    [AdvCommandNode("App/SpineSetFace")]
    [AdvDocument(AdvDocumentEditor.CommandCategory, "SpineSetFace", "Spineオブジェクトの表情指定。")]
    public class AdvGraphNodeSpineSetFace : AdvGraphNodeDefault<AdvCommandSpineSetFace>, IAdvGraphNodeWidth
    {
        float IAdvGraphNodeWidth.NodeWidth{get{return 400.0f;}}
    }
}

#endif // CRUFRAMEWORK_SPINE_SUPPORT