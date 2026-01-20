using TMPro;
using UnityEditor;
using UnityEngine;

namespace Pjfb.Editor.UI
{
    public static class StringValueTextMenu
    {
        [MenuItem("GameObject/UI/Pjfb/Text - with String Value Setter")]
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
            SetLayer(textObject.transform, parent.layer);

            // StringValueSetterを追加
            textObject.AddComponent<StringValueSetter>();
            
            // TextMeshProUGUIを追加
            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.SetText("New Text");

            // 名前を連番にする
            GameObjectUtility.EnsureUniqueNameForSibling(textObject);
            
            // 選択
            Selection.activeGameObject = textObject;
        }

        private static void SetLayer(Transform target, int layer)
        {
            target.gameObject.layer = layer;
            foreach (Transform child in target)
            {
                SetLayer(child, layer);
            }
        }
    }
}