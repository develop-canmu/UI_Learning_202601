using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class PrefabBlueprintLinkerInspector : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private static Vector2 s_dropDownSize = new Vector2(300,100);
    private GUIStyle style;
    private static List<LinkInfo> linkInfoList;
    private static string[] options;

    private class LinkInfo
    {
        public string URL;
        public string Name;
    }

    static PrefabBlueprintLinkerInspector()
    {
        //バッチモードかビルド中は実行させず、ビルドマシンでエラーが発生させない対応
        if (Application.isBatchMode || BuildPipeline.isBuildingPlayer)
        {
            return;
        }

        PrefabBlueprintLinker.instance.ReadSaveData();
        PrefabBlueprintLinker.instance.ReadUserSaveData();
        PrefabBlueprintLinker.instance.AddUserSaveData(in PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList,ref PrefabBlueprintLinker.instance.PrefabAndUrlsList);
        options = PrefabBlueprintLinker.instance.ReadTagList();
        
        Editor.finishedDefaultHeaderGUI += editor =>
        {
            PrefabBlueprintLinker.GuidAndUrls guidAndUrls = GetGuidAndUrls(editor.target, PrefabBlueprintLinker.instance.PrefabAndUrlsList);
            bool isBlueprintButton = guidAndUrls != null && guidAndUrls.Urls.Any(val => !string.IsNullOrEmpty(val));
            if (isBlueprintButton && GUILayout.Button($"{AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guidAndUrls.GUID),typeof(Object)).name} Blueprint"))
            {
                linkInfoList = new List<LinkInfo>();
                for (int i = 0; i < guidAndUrls.Urls.Count; i++)
                {
                    LinkInfo info = new LinkInfo();
                    info.URL = guidAndUrls.Urls[i];
                    info.Name = $"{options[guidAndUrls.Tags[i]]} {new Uri(guidAndUrls.Urls[i]).Host}";
                    linkInfoList.Add(info);
                }
                DropDown(Event.current.mousePosition, s_dropDownSize);
            }
        };
    }

    private static PrefabBlueprintLinker.GuidAndUrls GetGuidAndUrls(Object target, List<PrefabBlueprintLinker.PrefabAndUrls> prefabAndUrlsList)
    {
        string GUID;
        
        //1. 選択したオブジェクトのGUIDを取る
        if (EditorUtility.IsPersistent(target))
        {
            GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target));
            PrefabBlueprintLinker.GuidAndUrls guidAndUrls = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.ToGuidAndUrls().GUID == GUID)?.ToGuidAndUrls();
            if (guidAndUrls != null) return guidAndUrls;
        }
        
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

        //プレハブモードではない場合
        if (prefabStage == null)
        {
            //2. 選択したオブジェクトのPrefab内PrefabのGUIDを取る
            //無ければPrefabのGUIDを取る
            Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(target);
            if (prefab != null)
            {
                Object nestedPrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                if (EditorUtility.IsPersistent(nestedPrefab))
                {
                    GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(nestedPrefab));
                    PrefabBlueprintLinker.GuidAndUrls guidAndUrls = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.ToGuidAndUrls().GUID == GUID)?.ToGuidAndUrls();
                    return guidAndUrls;
                }
                
                if(EditorUtility.IsPersistent(prefab))
                {
                    GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                    PrefabBlueprintLinker.GuidAndUrls guidAndUrls = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.ToGuidAndUrls().GUID == GUID)?.ToGuidAndUrls();
                    return guidAndUrls;
                }
            }            
        }
        //プレハブモードの場合
        else
        {
            //2. 選択したオブジェクトのPrefabのGUIDを取る
            Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(target);
            if (EditorUtility.IsPersistent(prefab))
            {
                GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                PrefabBlueprintLinker.GuidAndUrls guidAndUrls = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.ToGuidAndUrls().GUID == GUID)?.ToGuidAndUrls();
                if (guidAndUrls != null) return guidAndUrls;
            }

            //3. プレハブモードで開いているオブジェクトのGUIDを取る
            if (prefabStage != null)
            {
                GUID = AssetDatabase.AssetPathToGUID(prefabStage.assetPath);
                PrefabBlueprintLinker.GuidAndUrls guidAndUrls = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.ToGuidAndUrls().GUID == GUID)?.ToGuidAndUrls();
                if (guidAndUrls != null) return guidAndUrls;
            }
        }

        return null;
    }

    void Awake()
    {
        style = new GUIStyle(EditorStyles.miniButton);
        style.alignment = TextAnchor.MiddleLeft;
    }
    
    private static void DropDown(Vector2 popupPosition, Vector2 windowSize)
    {
        popupPosition = GUIUtility.GUIToScreenPoint(popupPosition);
        var window = CreateInstance<PrefabBlueprintLinkerInspector>();
        window.ShowAsDropDown(new Rect(popupPosition, Vector2.zero), windowSize);
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.BeginVertical();
        foreach (var linkInfo in linkInfoList)
        {
            if (GUILayout.Button(linkInfo.Name, style)) Application.OpenURL(new Uri(linkInfo.URL).AbsoluteUri);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndScrollView();
    }
}