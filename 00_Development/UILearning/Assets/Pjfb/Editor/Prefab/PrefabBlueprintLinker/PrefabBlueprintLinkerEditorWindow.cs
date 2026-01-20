using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabBlueprintLinkerEditorWindow : EditorWindow
{
    private string searchWord = "";
    private Vector2 scrollPosition = Vector2.zero;
    private string errorMessage = "";
    private GUIStyle style;
    private GUIStyle style2;
    private const int RightAndLeftSpace = 60;
    private const int PrefabBottomSpace = 5;
    private const int SearchBoxTopAndBottomSpace = 20;
    private const int URLButtonTopAndBottomSpace = 5;
    private const int PrefabAndUrlsSpace = 15;
    private const int ShowErrorMessageTime = 5000;
    private const int TagFieldWidth = 150;
    private string[] options;
    private ViewType viewType = ViewType.Normal;
    private List<List<PrefabBlueprintLinker.PrefabAndUrls>> noConflictList = new List<List<PrefabBlueprintLinker.PrefabAndUrls>>();
    private List<List<PrefabBlueprintLinker.PrefabAndUrls>> dataErrorList = new List<List<PrefabBlueprintLinker.PrefabAndUrls>>();
    private List<List<PrefabBlueprintLinker.PrefabAndUrls>> conflictList = new List<List<PrefabBlueprintLinker.PrefabAndUrls>>();

    public enum ViewType{
        Normal,
        Review
    }
    
    [MenuItem("Tools/PrefabBlueprintLinker")]
    static void Open()
    {
        var window = GetWindow<PrefabBlueprintLinkerEditorWindow>();
        window.titleContent = new GUIContent("PrefabBlueprintLinker");
        PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList = new List<PrefabBlueprintLinker.ChangePrefabAndUrls>();
        PrefabBlueprintLinker.instance.PrefabAndUrlsList = new List<PrefabBlueprintLinker.PrefabAndUrls>();
        PrefabBlueprintLinker.instance.ReadSaveData();
        PrefabBlueprintLinker.instance.ReadUserSaveData();
        PrefabBlueprintLinker.instance.AddUserSaveData(in PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList,ref PrefabBlueprintLinker.instance.PrefabAndUrlsList);
        PrefabBlueprintLinker.instance.PrefabAndUrlsList = PrefabBlueprintLinker.instance.PrefabAndUrlsList.OrderBy(val => val.AddTime).ToList();
    }
    
    void Awake()
    {
        style = new GUIStyle(EditorStyles.label);
        style.richText = true;
        style2 = new GUIStyle(EditorStyles.label);
        style2.richText = true;
        style2.alignment = TextAnchor.MiddleCenter;
        PrefabBlueprintLinker.instance.ReadSaveData();
        PrefabBlueprintLinker.instance.ReadUserSaveData();
        PrefabBlueprintLinker.instance.AddUserSaveData(in PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList,ref PrefabBlueprintLinker.instance.PrefabAndUrlsList);
        PrefabBlueprintLinker.instance.PrefabAndUrlsList = PrefabBlueprintLinker.instance.PrefabAndUrlsList.OrderBy(val => val.AddTime).ToList();
        options = PrefabBlueprintLinker.instance.ReadTagList();
    }
    
    void OnGUI()
    {
        if (viewType == ViewType.Normal)
        {
            DrawSearchAndButtons();

            if (errorMessage != "")
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(RightAndLeftSpace);
                EditorGUILayout.LabelField($"<color=red>{errorMessage}</color>",style);
                GUILayout.Space(RightAndLeftSpace);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(SearchBoxTopAndBottomSpace);
            }
        
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = PrefabBlueprintLinker.instance.PrefabAndUrlsList.Count - 1; i >= 0; i--)
            {
                if (PrefabBlueprintLinker.instance.PrefabAndUrlsList[i].Prefab == null || PrefabBlueprintLinker.instance.PrefabAndUrlsList[i].Prefab.name.Contains(searchWord) || PrefabBlueprintLinker.instance.PrefabAndUrlsList[i].Urls.Contains(searchWord))
                {
                     DrawPrefabAndUrlsUI(PrefabBlueprintLinker.instance.PrefabAndUrlsList, i,(MainUniqueId, SubUniqueId, Value1, Value2, actionType) =>
                    {
                        PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(DeepCopy(new PrefabBlueprintLinker.ChangePrefabAndUrls(MainUniqueId, SubUniqueId, Value1, Value2, DateTime.Now.ToString("yyyyMMddHHmmssfff"), actionType)));
                        PrefabBlueprintLinker.instance.WriteSaveData();
                    });
                }
            }

            EditorGUILayout.EndScrollView();
        }
        else if (viewType == ViewType.Review)
        {
            GUILayout.Space(SearchBoxTopAndBottomSpace);
            EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("<size=20>レビューモード</size>",style2,GUILayout.MaxWidth(300));
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(URLButtonTopAndBottomSpace);
                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("マージする要素にチェックを入れてください。",style2,GUILayout.MaxWidth(300));
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("緑が追加、赤が削除です。",style2,GUILayout.MaxWidth(300));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(URLButtonTopAndBottomSpace);
                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("マージ", GUILayout.MaxWidth(80f)))
                    {
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList = new List<PrefabBlueprintLinker.PrefabAndUrls>();
                        PrefabBlueprintLinker.instance.ReadSaveData();
                        List<PrefabBlueprintLinker.PrefabAndUrls> mergeList = new List<PrefabBlueprintLinker.PrefabAndUrls>();
                        mergeList.AddRange(noConflictList.SelectMany(val => val));
                        mergeList.AddRange(dataErrorList.SelectMany(val => val));
                        mergeList.AddRange(conflictList.SelectMany(val => val));
                        mergeList.RemoveAll(val => val.isCheck == false);
                        mergeList = mergeList.OrderBy(val => val.AddTime).ToList();
                        foreach (var prefabAndUrls in mergeList)
                        {
                            int index = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FindIndex(val => val.UniqueID == prefabAndUrls.UniqueID);
                            if (index != -1)
                                PrefabBlueprintLinker.instance.PrefabAndUrlsList[index] = prefabAndUrls;
                            else
                                PrefabBlueprintLinker.instance.PrefabAndUrlsList.Add(prefabAndUrls);
                        }
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList.RemoveAll(val => val.isDelete);
                        PrefabBlueprintLinker.instance.MergeSaveData();
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList = new List<PrefabBlueprintLinker.PrefabAndUrls>();
                        PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList = new List<PrefabBlueprintLinker.ChangePrefabAndUrls>();
                        PrefabBlueprintLinker.instance.ReadSaveData();
                        PrefabBlueprintLinker.instance.ReadUserSaveData();
                        PrefabBlueprintLinker.instance.AddUserSaveData(in PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList,ref PrefabBlueprintLinker.instance.PrefabAndUrlsList);
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList = PrefabBlueprintLinker.instance.PrefabAndUrlsList.OrderBy(val => val.AddTime).ToList();
                        viewType = ViewType.Normal;
                    }
                    if (GUILayout.Button("戻る", GUILayout.MaxWidth(80f)))
                    {
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList = new List<PrefabBlueprintLinker.PrefabAndUrls>();
                        PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList = new List<PrefabBlueprintLinker.ChangePrefabAndUrls>();
                        PrefabBlueprintLinker.instance.ReadSaveData();
                        PrefabBlueprintLinker.instance.ReadUserSaveData();
                        PrefabBlueprintLinker.instance.AddUserSaveData(in PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList,ref PrefabBlueprintLinker.instance.PrefabAndUrlsList);
                        PrefabBlueprintLinker.instance.PrefabAndUrlsList = PrefabBlueprintLinker.instance.PrefabAndUrlsList.OrderBy(val => val.AddTime).ToList();
                        viewType = ViewType.Normal;
                    }
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(SearchBoxTopAndBottomSpace);
            EditorGUILayout.EndVertical();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("----------競合しています----------",style2,GUILayout.MaxWidth(300));
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            for (int i = conflictList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < conflictList[i].Count; j++)
                {
                    conflictList[i][j] = DrawPrefabAndUrlsUI(conflictList[i][j]);
                }
                GUILayout.Space(PrefabAndUrlsSpace);                 
            }
            
            EditorGUILayout.BeginVertical();
                GUILayout.Space(PrefabAndUrlsSpace);
                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("----------データの未登録や重複があります----------",style2,GUILayout.MaxWidth(300));
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            for (int i = dataErrorList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < dataErrorList[i].Count; j++)
                {
                    dataErrorList[i][j] = DrawPrefabAndUrlsUI(dataErrorList[i][j]);
                }
                GUILayout.Space(PrefabAndUrlsSpace);                 
            }
            
            EditorGUILayout.BeginVertical();
            GUILayout.Space(PrefabAndUrlsSpace);
                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("----------競合はありません----------",style2,GUILayout.MaxWidth(300));
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            for (int i = noConflictList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < noConflictList[i].Count; j++)
                {
                    noConflictList[i][j] = DrawPrefabAndUrlsUI(noConflictList[i][j]);
                }
                GUILayout.Space(PrefabAndUrlsSpace);                 
            }

            EditorGUILayout.EndScrollView();
        }
    }
    
    private void DrawSearchAndButtons()
    {
        GUILayout.Space(SearchBoxTopAndBottomSpace);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(RightAndLeftSpace);
        searchWord = EditorGUILayout.TextField("SearchWord", searchWord, GUILayout.ExpandWidth(true));
        if (string.IsNullOrEmpty(searchWord))
        {
            searchWord = "";
        }

        if (GUILayout.Button("+", GUILayout.MaxWidth(24f)))
        {
            if (string.IsNullOrEmpty(searchWord))
            {
                PrefabBlueprintLinker.PrefabAndUrls prefabAndUrls = new PrefabBlueprintLinker.PrefabAndUrls();
                prefabAndUrls.UniqueID = Guid.NewGuid().ToString();
                prefabAndUrls.UniqueIDs[0] = Guid.NewGuid().ToString();
                PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(new PrefabBlueprintLinker.ChangePrefabAndUrls(prefabAndUrls.UniqueID, prefabAndUrls.UniqueIDs[0], null, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.List_Added));
                PrefabBlueprintLinker.instance.PrefabAndUrlsList.Add(prefabAndUrls);
                PrefabBlueprintLinker.instance.WriteSaveData();
            }
            else
            {
                errorMessage = "エラー：検索ボックスを空にしてからリスト追加をしてください。";
                Debug.LogError(errorMessage);
                WaitForSecondsAction(() => errorMessage = "", ShowErrorMessageTime).Forget();
            }
        }

        if (GUILayout.Button("-", GUILayout.MaxWidth(24f)) && PrefabBlueprintLinker.instance.PrefabAndUrlsList.Count > 0)
        {
            if (string.IsNullOrEmpty(searchWord))
            {
                PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Add(new PrefabBlueprintLinker.ChangePrefabAndUrls(PrefabBlueprintLinker.instance.PrefabAndUrlsList[^1].UniqueID, null, null, null, DateTime.Now.ToString("yyyyMMddHHmmssfff"), PrefabBlueprintLinker.ActionType.List_Removed));
                PrefabBlueprintLinker.instance.PrefabAndUrlsList.RemoveAt(PrefabBlueprintLinker.instance.PrefabAndUrlsList.Count - 1);
                PrefabBlueprintLinker.instance.WriteSaveData();
            }
            else
            {
                errorMessage = "エラー：検索ボックスを空にしてからリスト削除をしてください。";
                Debug.LogError(errorMessage);
                WaitForSecondsAction(() => errorMessage = "", ShowErrorMessageTime).Forget();
            }
        }

        if (GUILayout.Button("レビュー", GUILayout.MaxWidth(80f)))
        {
            viewType = ViewType.Review;
            //競合、重複、問題無しのデータ分けをする
            PrefabBlueprintLinker.instance.ReadSaveData();
            PrefabBlueprintLinker.instance.ReadUserSaveData();
            
            //競合リスト取得
            GetConflictList(out conflictList);

            //重複、競合無しリスト取得
            GetNoConflictList(in conflictList,out dataErrorList,out noConflictList);
        }

        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(SearchBoxTopAndBottomSpace);
    }
    
    private void DrawPrefabAndUrlsUI(List<PrefabBlueprintLinker.PrefabAndUrls> prefabAndUrlsList, int index, System.Action<string, string, string, string, PrefabBlueprintLinker.ActionType> valueChange = null)
    {
        Rect rect = EditorGUILayout.BeginHorizontal();
        rect.x += 30;
        rect.width -= 60;
        rect.y -= 2;
        EditorGUI.DrawRect(rect,new Color32(255,255,255,30));
        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.BeginVertical();
        Object objectField = null;
        //Object選択時にSelect Objectウィンドウを開いた場合、ExitGUIExceptionが発生するためtry-catchする
        try
        {
            objectField = EditorGUILayout.ObjectField("Prefab", prefabAndUrlsList[index].Prefab, typeof(Object), false);
        }
        catch (Exception ex) 
        {
            if (!ShouldRethrowException(ex))
                throw;
        }
        if (objectField != null && prefabAndUrlsList[index].Prefab != objectField)
        {
            PrefabBlueprintLinker.PrefabAndUrls prefabAndUrls = prefabAndUrlsList.FirstOrDefault(val => val.Prefab == objectField);
            if (prefabAndUrls == null)
            {
                prefabAndUrlsList[index].Prefab = objectField;
                valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, null, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objectField)), null, PrefabBlueprintLinker.ActionType.Object_Changed);
            }
            else
            {
                searchWord = prefabAndUrls.Prefab.name;
                valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, null, null, null, PrefabBlueprintLinker.ActionType.List_Removed);
                prefabAndUrlsList.RemoveAt(index);
                GUILayout.Space(PrefabBottomSpace);
                EditorGUILayout.EndVertical();
                GUILayout.Space(RightAndLeftSpace);
                EditorGUILayout.EndHorizontal();
                //オブジェクト選択ウィンドウを閉じる
                if(focusedWindow.titleContent.text == "Select Object") focusedWindow.Close();
                errorMessage = "エラー：既に登録済みのPrefabです、下記プレハブにデータ入力をしてください。";
                Debug.LogError(errorMessage);
                WaitForSecondsAction(() => errorMessage = "", ShowErrorMessageTime).Forget();
            }
        }
        GUILayout.Space(PrefabBottomSpace);
        EditorGUILayout.EndVertical();
        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < prefabAndUrlsList[index].Urls.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(RightAndLeftSpace);
            string textField = EditorGUILayout.TextField("URL", prefabAndUrlsList[index].Urls[i], GUILayout.ExpandWidth(true));
            if (prefabAndUrlsList[index].Urls[i] != textField)
            {
                prefabAndUrlsList[index].Urls[i] = textField;
                if (string.IsNullOrEmpty(prefabAndUrlsList[index].Urls[i]))prefabAndUrlsList[index].Urls[i] = "";
                valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, prefabAndUrlsList[index].UniqueIDs[i], prefabAndUrlsList[index].Urls[i], prefabAndUrlsList[index].Tags[i].ToString(), PrefabBlueprintLinker.ActionType.URL_Changed);
            }
            int dropdownField = EditorGUILayout.Popup(prefabAndUrlsList[index].Tags[i], options, GUILayout.MaxWidth(TagFieldWidth));
            if (prefabAndUrlsList[index].Tags[i] != dropdownField)
            {
                prefabAndUrlsList[index].Tags[i] = dropdownField;
                valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, prefabAndUrlsList[index].UniqueIDs[i], prefabAndUrlsList[index].Urls[i], prefabAndUrlsList[index].Tags[i].ToString(), PrefabBlueprintLinker.ActionType.Tag_Changed);
            }

            if (GUILayout.Button("Open", GUILayout.MaxWidth(80f)))
                Application.OpenURL(new Uri(prefabAndUrlsList[index].Urls[i]).AbsoluteUri);
            GUILayout.Space(RightAndLeftSpace);
            EditorGUILayout.EndHorizontal();
        }
        
        GUILayout.Space(URLButtonTopAndBottomSpace);
        
        rect.yMax = EditorGUILayout.BeginHorizontal().yMax + URLButtonTopAndBottomSpace;
        GUILayout.Space(RightAndLeftSpace);
        if (GUILayout.Button("X", GUILayout.MaxWidth(24f)))
        {
            valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, null, null, null, PrefabBlueprintLinker.ActionType.List_Removed);
            prefabAndUrlsList.RemoveAt(index);
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.MaxWidth(24f)))
        {
            prefabAndUrlsList[index].UniqueIDs.Add(Guid.NewGuid().ToString());
            prefabAndUrlsList[index].Urls.Add("");
            prefabAndUrlsList[index].Tags.Add(0);
            valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, prefabAndUrlsList[index].UniqueIDs[^1], null, null, PrefabBlueprintLinker.ActionType.URLAndTag_Added);
        }

        if (GUILayout.Button("-", GUILayout.MaxWidth(24f)) && prefabAndUrlsList[index].UniqueIDs.Count > 1)
        {
            valueChange?.Invoke(prefabAndUrlsList[index].UniqueID, prefabAndUrlsList[index].UniqueIDs[^1], null, null, PrefabBlueprintLinker.ActionType.URLAndTag_Removed);
            prefabAndUrlsList[index].UniqueIDs.RemoveAt(prefabAndUrlsList[index].UniqueIDs.Count - 1);
            prefabAndUrlsList[index].Urls.RemoveAt(prefabAndUrlsList[index].Urls.Count - 1);
            prefabAndUrlsList[index].Tags.RemoveAt(prefabAndUrlsList[index].Tags.Count - 1);
        }
        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(URLButtonTopAndBottomSpace);
        
        EditorGUI.DrawRect(rect, new Color32(255,255,255,10));
        
        GUILayout.Space(PrefabAndUrlsSpace);
    }
    
    private PrefabBlueprintLinker.PrefabAndUrls DrawPrefabAndUrlsUI(PrefabBlueprintLinker.PrefabAndUrls prefabAndUrls)
    {
        Rect rect = EditorGUILayout.BeginHorizontal();
        rect.x += 50;
        rect.width -= 100;
        rect.y -= 2;
        EditorGUI.DrawRect(rect,new Color32(255,255,255,30));
        GUILayout.Space(30);
        prefabAndUrls.isCheck = EditorGUILayout.Toggle("", prefabAndUrls.isCheck,GUILayout.MaxWidth(-3));
        GUILayout.Space(RightAndLeftSpace - 30);
        EditorGUILayout.BeginVertical();
        Object objectField = null;
        //Object選択時にSelect Objectウィンドウを開いた場合、ExitGUIExceptionが発生するためtry-catchする
        try
        {
            objectField = EditorGUILayout.ObjectField("Prefab", prefabAndUrls.Prefab, typeof(Object), false);
        }
        catch (Exception ex) 
        {
            if (!ShouldRethrowException(ex))
                throw;
        }
        if (objectField != null && prefabAndUrls.Prefab != objectField)
        {
            prefabAndUrls.Prefab = objectField;
        }
        GUILayout.Space(PrefabBottomSpace);
        EditorGUILayout.EndVertical();
        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < prefabAndUrls.Urls.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(RightAndLeftSpace);
            string textField = EditorGUILayout.TextField("URL", prefabAndUrls.Urls[i], GUILayout.ExpandWidth(true));
            if (prefabAndUrls.Urls[i] != textField)
            {
                prefabAndUrls.Urls[i] = textField;
                if (string.IsNullOrEmpty(prefabAndUrls.Urls[i]))prefabAndUrls.Urls[i] = "";
            }
            prefabAndUrls.Tags[i] = EditorGUILayout.Popup(prefabAndUrls.Tags[i], options, GUILayout.MaxWidth(TagFieldWidth));

            if (GUILayout.Button("Open", GUILayout.MaxWidth(80f)))
                Application.OpenURL(new Uri(prefabAndUrls.Urls[i]).AbsoluteUri);
            GUILayout.Space(RightAndLeftSpace);
            EditorGUILayout.EndHorizontal();
        }
        
        GUILayout.Space(URLButtonTopAndBottomSpace);
        
        rect.yMax = EditorGUILayout.BeginHorizontal().yMax + URLButtonTopAndBottomSpace;
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.MaxWidth(24f)))
        {
            prefabAndUrls.UniqueIDs.Add(Guid.NewGuid().ToString());
            prefabAndUrls.Urls.Add("");
            prefabAndUrls.Tags.Add(0);
        }

        if (GUILayout.Button("-", GUILayout.MaxWidth(24f)) && prefabAndUrls.UniqueIDs.Count > 1)
        {
            prefabAndUrls.UniqueIDs.RemoveAt(prefabAndUrls.UniqueIDs.Count - 1);
            prefabAndUrls.Urls.RemoveAt(prefabAndUrls.Urls.Count - 1);
            prefabAndUrls.Tags.RemoveAt(prefabAndUrls.Tags.Count - 1);
        }
        GUILayout.Space(RightAndLeftSpace);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(URLButtonTopAndBottomSpace);

        if (prefabAndUrls.isCheck)
        {
            if(prefabAndUrls.isDelete) EditorGUI.DrawRect(rect, new Color32(255,0,0,10));
            else EditorGUI.DrawRect(rect, new Color32(0,255,0,10));
        }
        else
        {
            EditorGUI.DrawRect(rect, new Color32(0,0,0,100));
        }

        return prefabAndUrls;
    }
    
    private async UniTask WaitForSecondsAction(Action action,int milliSecond)
    {
        await Task.Delay(milliSecond);
        action?.Invoke();
    }
    
    //https://caitsithware.com/wordpress/archives/2284#GenericMenuPopupWindowShowExitGUIException
    private bool ShouldRethrowException(Exception exception)
    {
        while (exception is TargetInvocationException && exception.InnerException != null)
            exception = exception.InnerException;
        return exception is ExitGUIException;
    }
    
    private T DeepCopy<T>(T src)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, src);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }

    private void GetConflictList(out List<List<PrefabBlueprintLinker.PrefabAndUrls>> conflictPrefabAndUrlsList)
    {
        conflictPrefabAndUrlsList = new List<List<PrefabBlueprintLinker.PrefabAndUrls>>();
        
        foreach (var changePrefabAndUrls in PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList)
        {
            //競合登録済みはスキップ
            if(conflictPrefabAndUrlsList.SelectMany(val => val).Any(val => val.UniqueID == changePrefabAndUrls.MainUniqueId)) continue;
            
            if(GetListConflict(ref conflictPrefabAndUrlsList, changePrefabAndUrls)) continue;
            GetURLAndTagConflict(ref conflictPrefabAndUrlsList, changePrefabAndUrls);
        }
    }

    private bool GetListConflict(ref List<List<PrefabBlueprintLinker.PrefabAndUrls>> conflictPrefabAndUrlsList, PrefabBlueprintLinker.ChangePrefabAndUrls changePrefabAndUrls)
    {
        List<PrefabBlueprintLinker.ChangePrefabAndUrls> sameMainUniqueIdList = PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Where(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId).ToList();

        //リスト削除競合あり
        if(sameMainUniqueIdList.Any(val => val.ChangeType == PrefabBlueprintLinker.ActionType.List_Removed) && sameMainUniqueIdList.Count > 1)
        {
            //削除操作を除外
            sameMainUniqueIdList = sameMainUniqueIdList.Where(val => val.ChangeType != PrefabBlueprintLinker.ActionType.List_Removed).ToList();
            conflictPrefabAndUrlsList.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
            List<PrefabBlueprintLinker.PrefabAndUrls> tmp = new List<PrefabBlueprintLinker.PrefabAndUrls>();
            tmp.Add(PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.UniqueID == sameMainUniqueIdList[0].MainUniqueId));
            //既にあるなら引っ張る
            if (tmp[0] != null)
                tmp[0] = DeepCopy(new PrefabBlueprintLinker.GuidAndUrlsWrapper(tmp[0].ToGuidAndUrls())).GuidAndUrls.ToPrefabAndUrls();
            //ないならば新規
            else
                tmp = new List<PrefabBlueprintLinker.PrefabAndUrls>();
                
            PrefabBlueprintLinker.instance.AddUserSaveData(in sameMainUniqueIdList,ref tmp);
            tmp[0].isCheck = true;
            tmp[0].isDelete = true;
            conflictPrefabAndUrlsList[^1].Add(tmp[0]);
            //リスト削除競合とURLタグ競合が被った場合の処理
            GetURLAndTagConflict(ref conflictPrefabAndUrlsList, changePrefabAndUrls, true);
            return true;
        }
        return false;
    }
    
    private bool GetURLAndTagConflict(ref List<List<PrefabBlueprintLinker.PrefabAndUrls>> conflictPrefabAndUrlsList, PrefabBlueprintLinker.ChangePrefabAndUrls changePrefabAndUrls, bool isList = false)
    {
        //リスト内操作の抜き出し
        List<PrefabBlueprintLinker.ChangePrefabAndUrls> sameMainUniqueIdList = PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Where(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId && val.ChangeType != PrefabBlueprintLinker.ActionType.List_Added && val.ChangeType != PrefabBlueprintLinker.ActionType.List_Removed).ToList();

        //URLタグリストに削除あり
        if (sameMainUniqueIdList.Any(val =>
                val.ChangeType == PrefabBlueprintLinker.ActionType.URLAndTag_Removed) &&
            sameMainUniqueIdList.Any(val =>
                val.ChangeType == PrefabBlueprintLinker.ActionType.URL_Changed ||
                val.ChangeType == PrefabBlueprintLinker.ActionType.Tag_Changed))
        {
            //リストを削除する場合と削除しない場合を作る
            if(!isList)conflictPrefabAndUrlsList.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
            List<PrefabBlueprintLinker.PrefabAndUrls> del = new List<PrefabBlueprintLinker.PrefabAndUrls>();
            List<PrefabBlueprintLinker.PrefabAndUrls> noDel = new List<PrefabBlueprintLinker.PrefabAndUrls>();
            
            //削除する場合
            del.Add(PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.UniqueID == sameMainUniqueIdList[0].MainUniqueId));
            //既にあるなら引っ張る
            if (del[0] != null)
                del[0] = DeepCopy(new PrefabBlueprintLinker.GuidAndUrlsWrapper(del[0].ToGuidAndUrls())).GuidAndUrls.ToPrefabAndUrls();
            else if(isList)
                del[0] = DeepCopy(new PrefabBlueprintLinker.GuidAndUrlsWrapper(conflictPrefabAndUrlsList[^1][^1].ToGuidAndUrls())).GuidAndUrls.ToPrefabAndUrls();
            //ないならば新規
            else
                del = new List<PrefabBlueprintLinker.PrefabAndUrls>();
                
            PrefabBlueprintLinker.instance.AddUserSaveData(in sameMainUniqueIdList,ref del);
                
            //削除しない場合
            noDel.Add(PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.UniqueID == sameMainUniqueIdList[0].MainUniqueId));
            //既にあるなら引っ張る
            if (noDel[0] != null)
                noDel[0] = DeepCopy(new PrefabBlueprintLinker.GuidAndUrlsWrapper(noDel[0].ToGuidAndUrls())).GuidAndUrls.ToPrefabAndUrls();
            else if(isList)
                noDel[0] = DeepCopy(new PrefabBlueprintLinker.GuidAndUrlsWrapper(conflictPrefabAndUrlsList[^1][^1].ToGuidAndUrls())).GuidAndUrls.ToPrefabAndUrls();
            //ないならば新規
            else
                noDel = new List<PrefabBlueprintLinker.PrefabAndUrls>();
            
            PrefabBlueprintLinker.instance.AddUserSaveData(sameMainUniqueIdList.Where(val => val.ChangeType != PrefabBlueprintLinker.ActionType.URLAndTag_Removed).ToList(),ref noDel);

            if (!isList)
            {
                del[0].isCheck = true;
                noDel[0].isCheck = false;
                conflictPrefabAndUrlsList[^1].Add(del[0]);
                conflictPrefabAndUrlsList[^1].Add(noDel[0]);
            }
            else
            {
                noDel[0].isCheck = false;
                conflictPrefabAndUrlsList[^1].Add(noDel[0]);
            }
            return true;
        }
        return false;
    }

    private void GetNoConflictList(in List<List<PrefabBlueprintLinker.PrefabAndUrls>> conflictPrefabAndUrlsList,out List<List<PrefabBlueprintLinker.PrefabAndUrls>> dataErrorPrefabAndUrls,out List<List<PrefabBlueprintLinker.PrefabAndUrls>> noConflictPrefabAndUrlsList)
    {
        //競合無し
        noConflictPrefabAndUrlsList = new List<List<PrefabBlueprintLinker.PrefabAndUrls>>();
        dataErrorPrefabAndUrls = new List<List<PrefabBlueprintLinker.PrefabAndUrls>>();
        List<PrefabBlueprintLinker.ChangePrefabAndUrls> noConflictChangePrefabAndUrls = PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList;
        foreach (var prefabAndUrls in conflictList.SelectMany(val => val))
        {
            noConflictChangePrefabAndUrls.RemoveAll(val => val.MainUniqueId == prefabAndUrls.UniqueID);
        }
        //既にあるプレハブをゲット
        List<PrefabBlueprintLinker.PrefabAndUrls> noConflictListTmp = new List<PrefabBlueprintLinker.PrefabAndUrls>();
        foreach (var noConflictChangePrefabAndUrl in noConflictChangePrefabAndUrls)
        {
            if(noConflictListTmp.Any(val => val.UniqueID == noConflictChangePrefabAndUrl.MainUniqueId))continue;
            PrefabBlueprintLinker.PrefabAndUrls prefabAndUrls =
                PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val =>
                    val.UniqueID == noConflictChangePrefabAndUrl.MainUniqueId);
            if(prefabAndUrls != null) noConflictListTmp.Add(DeepCopy(new PrefabBlueprintLinker.GuidAndUrlsWrapper(prefabAndUrls.ToGuidAndUrls())).GuidAndUrls.ToPrefabAndUrls());
        }
        PrefabBlueprintLinker.instance.AddUserSaveData(in noConflictChangePrefabAndUrls,ref noConflictListTmp);

        //データ重複あり
        foreach (var prefabAndUrls in noConflictListTmp)
        {
            //登録済みはスキップ
            if(conflictPrefabAndUrlsList.SelectMany(val => val).Any(val => val.UniqueID == prefabAndUrls.UniqueID) ||  noConflictPrefabAndUrlsList.SelectMany(val => val).Any(val => val.UniqueID == prefabAndUrls.UniqueID) || dataErrorList.SelectMany(val => val).Any(val => val.UniqueID == prefabAndUrls.UniqueID)) continue;

            //オブジェクト未登録
            if (String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefabAndUrls.Prefab))))
            {
                dataErrorPrefabAndUrls.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
                dataErrorPrefabAndUrls[^1].Add(prefabAndUrls);
                continue;
            }
            
            //URLが空
            if (prefabAndUrls.Urls.Any(string.IsNullOrEmpty))
            {
                dataErrorPrefabAndUrls.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
                dataErrorPrefabAndUrls[^1].Add(prefabAndUrls);
                continue;
            }
            
            //オブジェクト重複
            int index = noConflictListTmp.FindIndex(val => val.UniqueID != prefabAndUrls.UniqueID && AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefabAndUrls.Prefab)) == AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(val.Prefab)));
            if (index != -1)
            {
                dataErrorPrefabAndUrls.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
                dataErrorPrefabAndUrls[^1].Add(prefabAndUrls);
                dataErrorPrefabAndUrls[^1].Add(noConflictListTmp[index]);
                continue;
            }
            
            //URL重複
            if (prefabAndUrls.Urls.GroupBy(val => val).Any(val => val.Count() > 1))
            {
                dataErrorPrefabAndUrls.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
                dataErrorPrefabAndUrls[^1].Add(prefabAndUrls);
                continue;
            }
            
            //データ的に問題無し
            noConflictPrefabAndUrlsList.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
            noConflictPrefabAndUrlsList[^1].Add(prefabAndUrls);
        }
        
        //削除のみ操作を競合無しリストに追加
        List<PrefabBlueprintLinker.ChangePrefabAndUrls> sameMainUniqueIdList = PrefabBlueprintLinker.instance.ChangePrefabAndUrlsList.Where(val => val.ChangeType == PrefabBlueprintLinker.ActionType.List_Removed).ToList();
        
        foreach (var changePrefabAndUrls in sameMainUniqueIdList)
        {
            //登録済みはスキップ
            if(noConflictPrefabAndUrlsList.SelectMany(val => val).Any(val => val.UniqueID == changePrefabAndUrls.MainUniqueId)) continue;
            
            //データ的に問題無し
            noConflictPrefabAndUrlsList.Add(new List<PrefabBlueprintLinker.PrefabAndUrls>());
            PrefabBlueprintLinker.PrefabAndUrls prefabAndUrls = PrefabBlueprintLinker.instance.PrefabAndUrlsList.FirstOrDefault(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
            if(prefabAndUrls == null) continue;
            prefabAndUrls.isCheck = true;
            prefabAndUrls.isDelete = true;
            noConflictPrefabAndUrlsList[^1].Add(prefabAndUrls);
        }
    }
}