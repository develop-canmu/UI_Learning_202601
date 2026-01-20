using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;

namespace Pjfb
{
    public class PrefabTemplateEditor
    {
        [MenuItem("GameObject/UI/Pjfb/Prefab/Template")]
        public static void CreateTemplate()
        {
        
            // プレハブ
            RectTransform prefab = CreateGameObject(Selection.activeGameObject.transform, "PrefabTemplate", false);
            // Root
            RectTransform root = CreateGameObject(prefab, "Root");
            
            // Back
            RectTransform back = CreateGameObject(root, "Back");
            CreateLayers(back);
            // Middle
            RectTransform middle = CreateGameObject(root, "Middle");
            CreateLayers(middle);
            // Front
            RectTransform front = CreateGameObject(root, "Front");
            CreateLayers(front);
            
            // 作ったGameObjectを選択
            Selection.activeGameObject = prefab.gameObject;
        }
        
        
        private static RectTransform CreateGameObject(Transform parent, string name, bool useAnchor = true)
        {
            GameObject obj = new GameObject(name);
            
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.SetParent(parent);
            // スケール初期化
            rectTransform.localScale = Vector3.one;
            // 座標初期化
            rectTransform.localPosition = Vector3.zero;
            // サイズを親と同じにする
            rectTransform.sizeDelta = ((RectTransform)parent).rect.size;
            
            if(useAnchor)
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.offsetMax = new Vector2(0, 0);
            }
            
            return rectTransform;
        }
        
        public static void CreateLayers(RectTransform parent)
        {
            CreateGameObject(parent, "Top");
            CreateGameObject(parent, "Center");
            CreateGameObject(parent, "Bottom");
        }
    }
}