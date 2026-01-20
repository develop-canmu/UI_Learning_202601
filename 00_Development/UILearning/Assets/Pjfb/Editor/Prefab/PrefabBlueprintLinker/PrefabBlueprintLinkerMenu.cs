using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabBlueprintLinkerMenu : EditorWindow
{
    private const int RightAndLeftSpace = 60;
    private const int URLButtonTopAndBottomSpace = 5;
    private const int TagFieldWidth = 150;
    private const int SearchBoxTopAndBottomSpace = 20;
    private static string[] options;
    private Vector2 scrollPosition = Vector2.zero;
    private static float selectionRectY = float.MinValue;
    private static List<Object> objectList = new List<Object>();

    private class PrefabBlueprintLinkerLog
    {
        public string Log;
        public LogType Type;

        public PrefabBlueprintLinkerLog(string log, LogType type)
        {
            Log = log;
            Type = type;
        }
        
        public enum LogType
        {
            Information,
            Warning,
            Error,
        }
    }
    
    private class PrefabBlueprintLinkerSettings : ScriptableSingleton<PrefabBlueprintLinkerSettings>
    {
        public List<PrefabBlueprintLinker.PrefabAndUrls> PrefabAndUrlsList = new List<PrefabBlueprintLinker.PrefabAndUrls>();
        public List<PrefabBlueprintLinker.ChangePrefabAndUrls> ChangePrefabAndUrlsList = new List<PrefabBlueprintLinker.ChangePrefabAndUrls>();
        public List<PrefabBlueprintLinkerLog> LogList = new List<PrefabBlueprintLinkerLog>();
    }

    static PrefabBlueprintLinkerMenu()
    {
        //バッチモードかビルド中は実行させず、ビルドマシンでエラーが発生させない対応
        if (Application.isBatchMode || BuildPipeline.isBuildingPlayer)
        {
            return;
        }
        
        EditorApplication.delayCall = () =>
        {
            if (!ReadSettings() || PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList.Count <= 0)
            {
                PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList.Add(new PrefabBlueprintLinker.PrefabAndUrls());
                PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueID = Guid.NewGuid().ToString();
                PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs[0] = Guid.NewGuid().ToString();
                PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueID, PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs[^1], null, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.List_Added)));
                WriteSettings();
            }
            else
            {
                PrefabBlueprintLinker.instance.AddUserSaveData(in PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList,ref PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList);
            }
            options = PrefabBlueprintLinker.instance.ReadTagList();
        };
    }

    private static bool ReadSettings()
    {
        string path = $"{PrefabBlueprintLinker.instance.GetCurrentPath()}/URLAndTagSettings.json";
        if (!File.Exists(path)) return false;
        PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList = JsonUtility.FromJson<PrefabBlueprintLinker.ChangePrefabAndUrlsListWrapper>(File.ReadAllText(path)).ChangePrefabAndUrlsList;
        return true;
    }
    
    private static void WriteSettings()
    {
        string path = $"{PrefabBlueprintLinker.instance.GetCurrentPath()}/URLAndTagSettings.json";
        File.WriteAllText(path,JsonUtility.ToJson(new PrefabBlueprintLinker.ChangePrefabAndUrlsListWrapper(PrefabBlueprintLinker.instance.CompChangePrefabAndUrlsList(in PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList))));
    }
    
    private void DrawPrefabAndUrlsUI(Action<string, string, string, string, PrefabBlueprintLinker.ActionType> valueChange = null)
    {
        GUILayout.Space(SearchBoxTopAndBottomSpace);
        Rect rect = EditorGUILayout.BeginHorizontal();
        rect.x += RightAndLeftSpace - 10;
        rect.width -= RightAndLeftSpace * 2 - 20;
        rect.y -= 2;

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(RightAndLeftSpace);
            string textField = EditorGUILayout.TextField("URL", PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i], GUILayout.ExpandWidth(true));
            if (PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i] != textField)
            {
                PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i] = textField;
                if (string.IsNullOrEmpty(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i])) PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i] = "";
                valueChange?.Invoke(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueID, PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs[i], PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i], PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i].ToString(), PrefabBlueprintLinker.ActionType.URL_Changed);
            }
            int dropdownField = EditorGUILayout.Popup(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i], options, GUILayout.MaxWidth(TagFieldWidth));
            if (PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i] != dropdownField)
            {
                PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i] = dropdownField;
                valueChange?.Invoke(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueID, PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs[i], PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i], PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i].ToString(), PrefabBlueprintLinker.ActionType.Tag_Changed);
            }

            if (GUILayout.Button("Open", GUILayout.MaxWidth(80f)))
                Application.OpenURL(new Uri(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i]).AbsoluteUri);
            GUILayout.Space(RightAndLeftSpace);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(URLButtonTopAndBottomSpace);
        
        rect.yMax = EditorGUILayout.BeginHorizontal().yMax + URLButtonTopAndBottomSpace;
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.MaxWidth(24f)))
        {
            PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs.Add(Guid.NewGuid().ToString());
            PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls.Add("");
            PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags.Add(0);
            valueChange?.Invoke(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueID, PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs[^1], null, null, PrefabBlueprintLinker.ActionType.URLAndTag_Added);
        }

        if (GUILayout.Button("-", GUILayout.MaxWidth(24f)) && PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs.Count > 1)
        {
            valueChange?.Invoke(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueID, PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs[^1], null, null, PrefabBlueprintLinker.ActionType.URLAndTag_Removed);
            PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs.RemoveAt(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].UniqueIDs.Count - 1);
            PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls.RemoveAt(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls.Count - 1);
            PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags.RemoveAt(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags.Count - 1);
        }
        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(URLButtonTopAndBottomSpace);

        EditorGUI.DrawRect(rect, new Color32(255,255,255,10));
    }
    
    void OnGUI()
    {
        DrawPrefabAndUrlsUI((MainUniqueId, SubUniqueId, Value1, Value2, actionType) =>
        {
            PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(MainUniqueId, SubUniqueId, Value1, Value2, DateTime.Now.ToString("yyyyMMddHHmmssfff"), actionType)));
            WriteSettings();
        });
        
        GUILayout.Space(SearchBoxTopAndBottomSpace);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Prefab Add", GUILayout.MaxWidth(200f)))
        {
            AddPrefab();
        }

        if (GUILayout.Button("Prefab All Select", GUILayout.MaxWidth(200f)))
        {
            PrefabAllSelect();
        }
        
        if(GUILayout.Button("Log Clear", GUILayout.MaxWidth(200f)))
        {
            PrefabBlueprintLinkerSettings.instance.LogList.Clear();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(SearchBoxTopAndBottomSpace);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.BeginVertical();
        foreach (var log in PrefabBlueprintLinkerSettings.instance.LogList)
        {
            Rect rect = EditorGUILayout.BeginHorizontal();
            rect.x += RightAndLeftSpace - 10;
            rect.width -= RightAndLeftSpace * 2 - 20;
            GUILayout.Space(RightAndLeftSpace);
            EditorGUILayout.LabelField(log.Log);
            switch (log.Type)
            {
                case PrefabBlueprintLinkerLog.LogType.Information:
                    EditorGUI.DrawRect(rect, new Color32(255,255,255,10));
                    break;
                case PrefabBlueprintLinkerLog.LogType.Warning:
                    EditorGUI.DrawRect(rect, new Color32(255, 255, 0,15));
                    break;
            }
            GUILayout.Space(RightAndLeftSpace);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
    
    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        //1回のみのループにする
        if (selectionRect.y < selectionRectY)
        {
            List<Object> prefabList = new List<Object>();
            List<string> GUIDList = new List<string>();

            foreach (var o in objectList)
            {
                string GUID = GetGUID(o);
                if (!string.IsNullOrEmpty(GUID))
                {
                    if (GUIDList.Any(val => val == GUID)) continue;
                    prefabList.Add(o);
                    GUIDList.Add(GUID);
                }
            }

            //フォーカスを当てる
            objectList = prefabList;
            Selection.objects = prefabList.ToArray();
            selectionRectY = float.MinValue;
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
            return;
        }

        selectionRectY = selectionRect.y;
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj != null)
        {
            objectList.Add(obj);
        }
    }
    
    [MenuItem("GameObject/PrefabBlueprintLinker/Settings and Launcher", false, int.MinValue)]
    [MenuItem("Assets/PrefabBlueprintLinker/Settings and Launcher", false, int.MinValue)]
    private static void URLAndTagSettingsWindow()
    {
        options = PrefabBlueprintLinker.instance.ReadTagList();
        var window = CreateInstance<PrefabBlueprintLinkerMenu>();
        window.minSize = new Vector2(500, 100);
        window.titleContent = new GUIContent("Settings and Launcher");
        window.ShowUtility();
    }
    
    [MenuItem("GameObject/PrefabBlueprintLinker/Prefab All Select", false, int.MinValue)]
    private static void PrefabAllSelect()
    {
        objectList = new List<Object>();
        //Hierarchyウィンドウ取得し、フォーカスを当てる
        GUI.FocusControl("");
        Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        GetWindow(type);
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }
    
    [MenuItem("GameObject/PrefabBlueprintLinker/Add", true)]
    [MenuItem("Assets/PrefabBlueprintLinker/Add", true)]
    private static bool AddValidation()
    {
        if (Selection.objects.Length <= 0 && objectList.Count > 0)
        {
            Selection.objects = objectList.ToArray();
            objectList = new List<Object>();
        }
        bool selectAndSettingsFlag = Selection.objects.Length > 0 && PrefabBlueprintLinkerSettings.instance.ChangePrefabAndUrlsList.Count > 0 && !string.IsNullOrEmpty(PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[0]);
        if (!selectAndSettingsFlag) return false;
        
        foreach (var gameObject in Selection.objects)
        {
            string GUID = GetGUID(gameObject);
            if (!string.IsNullOrEmpty(GUID))
            {
                return true;
            }
        }
        return false;
    }

    [MenuItem("GameObject/PrefabBlueprintLinker/Add", false, int.MinValue)]
    [MenuItem("Assets/PrefabBlueprintLinker/Add", false, int.MinValue)]
    private static void AddPrefab()
    {
        foreach (var o in Selection.objects)
        {
            string GUID = GetGUID(o);
            if (string.IsNullOrEmpty(GUID)) continue;
            
            int index = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FindIndex(val => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(val.Prefab)) == GUID);

            //無ければ新規で作成
            if (index == -1)
            {
                //リスト作成
                PrefabBlueprintLinker.instance.PrefabAndUrlsList.Add(new PrefabBlueprintLinker.PrefabAndUrls());
                PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID = Guid.NewGuid().ToString();
                PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueIDs[^1] = Guid.NewGuid().ToString();
                PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueIDs[^1], null, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.List_Added)));

                //Object設定
                PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Prefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID),typeof(Object));
                PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID, null, GUID, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.Object_Changed)));

                //URLとタグの変更
                PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Urls[^1] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[0];
                PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Tags[^1] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[0];
                PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueIDs[^1], PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Urls[^1], PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Tags[^1].ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.URL_Changed)));
                
                for (int i = 1; i < PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls.Count; i++)
                {
                    //URL作成
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueIDs.Add(Guid.NewGuid().ToString());
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Urls.Add("");
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Tags.Add(0);
                    PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueIDs[^1], null, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.URLAndTag_Added)));

                    //URLとタグの変更
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Urls[^1] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i];
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Tags[^1] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i];
                    PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueIDs[^1], PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Urls[^1], PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Tags[^1].ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.URL_Changed)));
                }
                
                PrefabBlueprintLinkerSettings.instance.LogList.Add(new PrefabBlueprintLinkerLog($"リストに新規で{PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].Prefab.name}を追加しました。", PrefabBlueprintLinkerLog.LogType.Information));
                Debug.Log(PrefabBlueprintLinkerSettings.instance.LogList[^1].Log);
                PrefabBlueprintLinkerSettings.instance.LogList[^1].Log = DateTime.Now.ToString("[HH:mm:ss] ") + PrefabBlueprintLinkerSettings.instance.LogList[^1].Log;
            }
            //GUIDが一致するものがあればそこにURLとタグを追加
            else
            {
                bool changeFlag = false;
                for (int i = 0; i < PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls.Count; i++)
                {
                    //URLが既に登録済みの場合はタグだけ書き換える
                    int index2 = PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Urls.FindIndex(val => val == PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i]);
                    if (index2 != -1)
                    {
                        if(PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Tags[index2] == PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i]) continue;
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Tags[index2] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i];
                        PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueIDs[index2], PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Urls[index2], PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Tags[index2].ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.URL_Changed)));
                        changeFlag = true;
                        continue;
                    }
                    
                    //URL作成
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueIDs.Add(Guid.NewGuid().ToString());
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Urls.Add("");
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Tags.Add(0);
                    PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueIDs[^1], null, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.URLAndTag_Added)));

                    //URLとタグの変更
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Urls[^1] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Urls[i];
                    PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Tags[^1] = PrefabBlueprintLinkerSettings.instance.PrefabAndUrlsList[0].Tags[i];
                    PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueID, PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].UniqueIDs[^1], PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Urls[^1], PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Tags[^1].ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.URL_Changed)));
                    changeFlag = true;
                }

                if (changeFlag)
                {
                    PrefabBlueprintLinkerSettings.instance.LogList.Add(new PrefabBlueprintLinkerLog($"{PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Prefab.name}に変更を適用しました。", PrefabBlueprintLinkerLog.LogType.Information));
                    Debug.Log(PrefabBlueprintLinkerSettings.instance.LogList[^1].Log);
                }
                else
                {
                    PrefabBlueprintLinkerSettings.instance.LogList.Add(new PrefabBlueprintLinkerLog($"{PrefabBlueprintLinker.instance.PrefabAndUrlsList[index].Prefab.name}にて指定したURLは既に登録済みのため、変更は実行しませんでした。", PrefabBlueprintLinkerLog.LogType.Warning));
                    Debug.LogWarning(PrefabBlueprintLinkerSettings.instance.LogList[^1].Log);
                }
                PrefabBlueprintLinkerSettings.instance.LogList[^1].Log = DateTime.Now.ToString("[HH:mm:ss] ") + PrefabBlueprintLinkerSettings.instance.LogList[^1].Log;
            }

            PrefabBlueprintLinker.instance.WriteSaveData();            
        }
    }
    
    private static string GetGUID(Object target)
    {
        string GUID;
        
        //1. 選択したオブジェクトのGUIDを取る
        if (EditorUtility.IsPersistent(target))
        {
            GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target));
            return GUID;
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
                    return GUID;
                }
                
                if(EditorUtility.IsPersistent(prefab))
                {
                    GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                    return GUID;
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
                return GUID;
            }

            //3. プレハブモードで開いているオブジェクトのGUIDを取る
            if (prefabStage != null)
            {
                GUID = AssetDatabase.AssetPathToGUID(prefabStage.assetPath);
                return GUID;
            }
        }

        return null;
    }
    
    private static T DeepCopy<T>(T src)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, src);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }
}