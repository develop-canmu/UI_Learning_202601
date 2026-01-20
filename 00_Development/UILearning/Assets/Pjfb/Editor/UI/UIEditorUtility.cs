using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;
using UnityEngine.UI;

namespace Pjfb
{
    public class UIEditorUtility
    {
        // なんか適当な空間あったらご指摘を.
        [MenuItem("Pjfb/UITools/RemoveAllChildrensRaycastTargets", false)]
        public static void RemoveRaycastTargetIncludeChildren(MenuCommand menuCommand)
        {
            var gameObject = Selection.activeGameObject;
            var graphicComponents = gameObject.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphicComponents)
            {
                graphic.raycastTarget = false;
            }
        
            EditorUtility.SetDirty(gameObject);
        }
    }
}