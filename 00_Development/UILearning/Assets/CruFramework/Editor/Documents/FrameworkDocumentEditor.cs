using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using CruFramework.Editor.Document;
using UnityEngine;
using UnityEditor;
using System;


namespace CruFramework.Editor
{

    public class FrameworkDocumentEditor : DocumentEditor<FrameworkDocumentAttribute>
    {
        [MenuItem("CruFramework/Document")]
        public static void Open()
        {
            GetWindow<FrameworkDocumentEditor>("FrameworkDocumentEditor");
        }
    }
}