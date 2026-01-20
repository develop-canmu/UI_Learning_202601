using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Pjfb
{
    public class CopyHierarchyPath : EditorWindow
    {
        private string keyword = "";

        [MenuItem("GameObject/HierarchyPath/Copy", false, int.MinValue)]
        private static void CopyPath()
        {
            var active = Selection.activeGameObject;
            if (active == null)
                return;
 
            var obj = active as GameObject;
            var builder = new StringBuilder(obj.transform.name);
            var current = obj.transform.parent;
 
            while (current != null)
            {
                builder.Insert(0, current.name + "/");
                current = current.parent;
            }
 
            GUIUtility.systemCopyBuffer = builder.ToString();
        }
        
        private static void SearchPath(string fullPath)
        {
            var path = fullPath.Split("/");
            var path2 = String.Join("/", path.Skip(1));
            List<GameObject> objs = GetRootObjects();
            foreach (var obj in objs)
            {
                if(path.All(str => obj.name != str))continue;
                
                var targetObj = obj.transform.root.Find(path2);
                if (targetObj != null)
                {
                    Selection.activeGameObject = targetObj.gameObject;
                    EditorGUIUtility.PingObject(targetObj);
                    return;
                }
            }
        }

        private static List<GameObject> GetRootObjects()
        {
            var obj = PrefabStageUtility.GetCurrentPrefabStage();
            if (obj != null) return new List<GameObject>(){obj.prefabContentsRoot};

            List<GameObject> objs = new List<GameObject>();
            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                foreach (var rootGameObject in scene.GetRootGameObjects())
                {
                    objs.Add(rootGameObject);
                }
            }

            return objs;
        }
        
        [MenuItem("GameObject/HierarchyPath/Search", false, int.MinValue)]
        private static void SearchModal()
        {
            var window = CreateInstance<CopyHierarchyPath>();
            window.minSize = new Vector2(500, 150);
            window.titleContent = new GUIContent("Search");
            window.ShowUtility();
        }
        
        void OnGUI()
        {
            keyword = EditorGUILayout.TextField("検索パス", keyword);
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "";
            }
            //実行ボタン
            if (GUILayout.Button("実行", GUILayout.MaxWidth(80f), GUILayout.Height(24f)))
            {
                SearchPath(keyword);
            }
        }
    }
}