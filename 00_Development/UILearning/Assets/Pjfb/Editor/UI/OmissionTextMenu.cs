using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using UnityEditor;

namespace Pjfb.Editor.UI
{
    public class OmissionTextMenu
    {
        [MenuItem("GameObject/UI/Pjfb/Text - with OmissionText Setter")]
        public static void CreateValueTextMenu(MenuCommand command)
        {
            // parent設定
            GameObject parent = Selection.gameObjects.Length > 0 ? Selection.gameObjects[0] : null;
            
            // テキストオブジェクト作成
            GameObject textObject = new GameObject();
            textObject.name = "Text (TMP)";

            // Undo登録
            Undo.RegisterCreatedObjectUndo(textObject, $"Create {textObject.name}");

            // 親を設定
            GameObjectUtility.SetParentAndAlign(textObject, parent);

            // レイヤー設定
            textObject.layer = parent.layer;

            // StringValueSetterを追加
            textObject.AddComponent<StringValueSetter>();
            
            // OmissionTextSetterを追加
            textObject.AddComponent<OmissionTextSetter>();
            
            // TextMeshProUGUIを追加
            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.SetText($"000<size={text.fontSize}>.</size>0<size={text.fontSize}>万</size>");

            // 名前を連番にする
            GameObjectUtility.EnsureUniqueNameForSibling(textObject);
            
            // 選択
            Selection.activeGameObject = textObject;
        }
    }
}