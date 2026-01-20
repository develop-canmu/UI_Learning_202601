using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using CruFramework.Editor.Document;
using UnityEngine;
using UnityEditor;
using System;


namespace CruFramework.Editor.Adv
{

    public class AdvDocumentEditor : DocumentEditor<AdvDocumentAttribute>
    {
        
        public const string CommandCategory = "Command";
        public const string EditorCategory = "Editor";
        
        [MenuItem("CruFramework/Adv/Document")]
        public static void Open()
        {
            GetWindow<AdvDocumentEditor>("AdvEditorDocument");
        }
        
        public static void Open(Type type)
        {
            AdvDocumentEditor e = GetWindow<AdvDocumentEditor>("AdvEditorDocument");
            e.SetCategory(CommandCategory);
            e.SetDocumentType(type);
        }
    }

}