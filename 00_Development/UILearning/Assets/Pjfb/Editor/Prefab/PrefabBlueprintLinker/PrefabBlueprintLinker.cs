using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabBlueprintLinker : ScriptableSingleton<PrefabBlueprintLinker>
{
    public List<PrefabAndUrls> PrefabAndUrlsList = new List<PrefabAndUrls>();
    public List<ChangePrefabAndUrls> ChangePrefabAndUrlsList = new List<ChangePrefabAndUrls>();
    private SaveData saveData = new SaveData();

    public enum ActionType
    {
        None,
        List_Added,
        List_Removed,
        URLAndTag_Added,
        URLAndTag_Removed,
        Object_Changed,
        URL_Changed,
        Tag_Changed,
    }

    private class SaveData
    {
        public List<GuidAndUrls> GuidAndUrls;
    }
    
    [Serializable]
    public class GuidAndUrls
    {
        public string UniqueID = "";
        public string GUID = "";
        public List<string> UniqueIDs = new List<string>(){""};
        public List<string> Urls = new List<string>(){""}; 
        public List<int> Tags = new List<int>(){0};
        public string AddTime = "";
        public string ChangeTime = "";

        public PrefabAndUrls ToPrefabAndUrls()
        {
            PrefabAndUrls prefabAndUrls = new PrefabAndUrls();
            prefabAndUrls.UniqueID = UniqueID;
            prefabAndUrls.Prefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID),typeof(Object));
            prefabAndUrls.UniqueIDs = UniqueIDs;
            prefabAndUrls.Urls = Urls;
            prefabAndUrls.Tags = Tags;
            return prefabAndUrls;
        }
    }
    
    public class PrefabAndUrls
    {
        public string UniqueID = "";
        public Object Prefab = null;
        public List<string> UniqueIDs = new List<string>(){""};
        public List<string> Urls = new List<string>(){""};
        public List<int> Tags = new List<int>(){0};
        public string AddTime = "";
        public string ChangeTime = "";
        public bool isCheck = true;
        public bool isDelete = false;

        public GuidAndUrls ToGuidAndUrls()
        {
            GuidAndUrls guidAndUrls = new GuidAndUrls();
            guidAndUrls.UniqueID = UniqueID;
            guidAndUrls.GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Prefab));
            guidAndUrls.UniqueIDs = UniqueIDs;
            guidAndUrls.Urls = Urls;
            guidAndUrls.Tags = Tags;
            guidAndUrls.AddTime = AddTime;
            guidAndUrls.ChangeTime = ChangeTime;
            return guidAndUrls;
        }
    }

    [Serializable]
    public class GuidAndUrlsWrapper
    {
        public GuidAndUrls GuidAndUrls;

        public GuidAndUrlsWrapper(GuidAndUrls guidAndUrls)
        {
            GuidAndUrls = guidAndUrls;
        }
    }
    
    [Serializable]
    public class ChangePrefabAndUrlsListWrapper
    {
        public List<ChangePrefabAndUrls> ChangePrefabAndUrlsList;

        public ChangePrefabAndUrlsListWrapper(List<ChangePrefabAndUrls> changePrefabAndUrlsList)
        {
            ChangePrefabAndUrlsList = changePrefabAndUrlsList;
        }
    }
    
    [Serializable]
    public class ChangePrefabAndUrls
    {
        public string UserName;
        public string MainUniqueId;
        public string SubUniqueId;
        public string Value1;
        public string Value2;
        public string ChangeTime;
        public ActionType ChangeType;

        public ChangePrefabAndUrls(string mainUniqueId, string subUniqueId, string value1, string value2, string changeTime, ActionType changeType)
        {
            UserName = Environment.UserName;
            MainUniqueId = mainUniqueId;
            SubUniqueId = subUniqueId;
            Value1 = value1;
            Value2 = value2;
            ChangeTime = changeTime;
            ChangeType = changeType;
        }
    }

    public void WriteSaveData()
    {
        File.WriteAllText($"{GetCurrentPath()}/UserData/{Environment.UserName}.json",JsonUtility.ToJson(new ChangePrefabAndUrlsListWrapper(CompChangePrefabAndUrlsList(in ChangePrefabAndUrlsList))));
    }

    public List<ChangePrefabAndUrls> CompChangePrefabAndUrlsList(in List<ChangePrefabAndUrls> changePrefabAndUrlsList)
    {
        List<ChangePrefabAndUrls> myChangePrefabAndUrlsList = changePrefabAndUrlsList.Where(val => val.UserName == Environment.UserName).ToList();

        //同じIDのChangeは最新のみを保存する
        List<ChangePrefabAndUrls> changeList = myChangePrefabAndUrlsList.Where(val => val.ChangeType != ActionType.List_Added && val.ChangeType != ActionType.List_Removed && val.ChangeType != ActionType.URLAndTag_Added && val.ChangeType != ActionType.URLAndTag_Removed).ToList();
        changeList = changeList.OrderByDescending(val => val.ChangeTime).ToList();
        List<ChangePrefabAndUrls> newChangeList = new List<ChangePrefabAndUrls>();
        foreach (var changePrefabAndUrls in changeList)
        {
            if(newChangeList.Any(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId && val.SubUniqueId == changePrefabAndUrls.SubUniqueId)) continue;
            
            newChangeList.Add(changePrefabAndUrls);
        }
        
        foreach (var changePrefabAndUrls in newChangeList)
        {
            myChangePrefabAndUrlsList.RemoveAll(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId && val.SubUniqueId == changePrefabAndUrls.SubUniqueId && val.ChangeType != ActionType.List_Added && val.ChangeType != ActionType.List_Removed && val.ChangeType != ActionType.URLAndTag_Added && val.ChangeType != ActionType.URLAndTag_Removed);
        }
        
        myChangePrefabAndUrlsList.AddRange(newChangeList);
        
        //Add、Removeのペアを削除
        //ListのAdd、Removeのペアを抽出
        List<ChangePrefabAndUrls> addRemoveList = myChangePrefabAndUrlsList.Where(val => val.ChangeType == ActionType.List_Added || val.ChangeType == ActionType.List_Removed).ToList();
        List<string> duplicateList1 = addRemoveList.GroupBy(val => val.MainUniqueId).Where(val => val.Count() > 1).Select(val => val.Key).ToList();
        
        //URLタグのAdd、Removeのペアを抽出
        addRemoveList = myChangePrefabAndUrlsList.Where(val => val.ChangeType == ActionType.URLAndTag_Added || val.ChangeType == ActionType.URLAndTag_Removed).ToList();
        List<string> duplicateList2 = addRemoveList.GroupBy(val => val.SubUniqueId).Where(val => val.Count() > 1).Select(val => val.Key).ToList();

        //ペア削除
        foreach (var s in duplicateList1)
        {
            myChangePrefabAndUrlsList.RemoveAll(val => val.MainUniqueId == s);
        }
        
        foreach (var s in duplicateList2)
        {
            myChangePrefabAndUrlsList.RemoveAll(val => val.SubUniqueId == s);
        }
        
        //ListのRemoveがある場合、その要素に対する操作を削除する
        List<ChangePrefabAndUrls> removeList = myChangePrefabAndUrlsList.Where(val => val.ChangeType == ActionType.List_Removed).ToList();
        changeList = new List<ChangePrefabAndUrls>();
        
        foreach (var changePrefabAndUrls in removeList)
        {
            changeList.AddRange(myChangePrefabAndUrlsList.Where(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId && val.ChangeType != ActionType.List_Added && val.ChangeType != ActionType.List_Removed).ToList());
        }
        
        foreach (var changePrefabAndUrls in changeList)
        {
            myChangePrefabAndUrlsList.RemoveAll(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId && val.ChangeType != ActionType.List_Added && val.ChangeType != ActionType.List_Removed);
        }
        
        //URLタグのRemoveがある場合、その要素に対する操作を削除する
        removeList = myChangePrefabAndUrlsList.Where(val => val.ChangeType == ActionType.URLAndTag_Removed).ToList();
        changeList = new List<ChangePrefabAndUrls>();
        
        foreach (var changePrefabAndUrls in removeList)
        {
            changeList.AddRange(myChangePrefabAndUrlsList.Where(val => val.SubUniqueId == changePrefabAndUrls.SubUniqueId && (val.ChangeType == ActionType.Object_Changed || val.ChangeType == ActionType.URL_Changed || val.ChangeType == ActionType.Tag_Changed)).ToList());
        }
        
        foreach (var changePrefabAndUrls in changeList)
        {
            myChangePrefabAndUrlsList.RemoveAll(val => val.MainUniqueId == changePrefabAndUrls.MainUniqueId && (val.ChangeType == ActionType.Object_Changed || val.ChangeType == ActionType.URL_Changed || val.ChangeType == ActionType.Tag_Changed));
        }
        
        myChangePrefabAndUrlsList = myChangePrefabAndUrlsList.OrderBy(val => val.ChangeTime).ToList();

        return myChangePrefabAndUrlsList;
    }

    public void MergeSaveData()
    {
        saveData = new SaveData();
        saveData.GuidAndUrls = new List<GuidAndUrls>();
        PrefabAndUrlsList = PrefabAndUrlsList.OrderBy(val => val.AddTime).ToList();
        foreach (var prefabAndUrls in PrefabAndUrlsList)
            saveData.GuidAndUrls.Add(prefabAndUrls.ToGuidAndUrls());
        File.WriteAllText(GetJsonPath(),JsonUtility.ToJson(saveData));
        
        DirectoryInfo directory = new DirectoryInfo($"{GetCurrentPath()}/UserData/");
 
        foreach (FileInfo file in directory.GetFiles()) {
            file.Delete();
        }
 
        foreach (DirectoryInfo dir in directory.GetDirectories()) {
            dir.Delete(true);
        }
    }

    public void ReadSaveData()
    {
        string path = GetJsonPath();
        if (File.Exists(path))
        {
            saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            PrefabAndUrlsList = new List<PrefabAndUrls>();
            foreach (var guidAndUrl in saveData.GuidAndUrls)
            {
                PrefabAndUrlsList.Add(guidAndUrl.ToPrefabAndUrls());
            }
        }
    }

    public void ReadUserSaveData()
    {
        string[] jsonPathArray = Directory.GetFiles($"{GetCurrentPath()}/UserData/");
        jsonPathArray = jsonPathArray.Where(val => Path.GetExtension(val) == ".json").ToArray();
        ChangePrefabAndUrlsList = new List<ChangePrefabAndUrls>();

        foreach (var jsonPath in jsonPathArray)
        {
            if (File.Exists(jsonPath))
            {
                ChangePrefabAndUrlsList.AddRange(JsonUtility.FromJson<ChangePrefabAndUrlsListWrapper>(File.ReadAllText(jsonPath)).ChangePrefabAndUrlsList);
            }
        }
        ChangePrefabAndUrlsList = ChangePrefabAndUrlsList.OrderBy(val => val.ChangeTime).ToList();
    }
    
    public void AddUserSaveData(in List<ChangePrefabAndUrls> changePrefabAndUrlsList,ref List<PrefabAndUrls> prefabAndUrlsList)
    {
        int index = -1;
        foreach (var changePrefabAndUrls in changePrefabAndUrlsList)
        {
            switch (changePrefabAndUrls.ChangeType)
            {
                case ActionType.List_Added:
                    PrefabAndUrls prefabAndUrls = new PrefabAndUrls();
                    prefabAndUrls.UniqueID = changePrefabAndUrls.MainUniqueId;
                    prefabAndUrls.UniqueIDs[0] = changePrefabAndUrls.SubUniqueId;
                    prefabAndUrls.AddTime = changePrefabAndUrls.ChangeTime;
                    prefabAndUrls.ChangeTime = changePrefabAndUrls.ChangeTime;
                    prefabAndUrlsList.Add(prefabAndUrls);
                    break;
                case ActionType.List_Removed:
                    index = prefabAndUrlsList.FindIndex(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
                    if (index != -1)
                        prefabAndUrlsList.RemoveAt(index);
                    break;
                case ActionType.URLAndTag_Added:
                    index = prefabAndUrlsList.FindIndex(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
                    if (index != -1)
                    {
                        prefabAndUrlsList[index].UniqueIDs.Add(changePrefabAndUrls.SubUniqueId);
                        prefabAndUrlsList[index].Urls.Add("");
                        prefabAndUrlsList[index].Tags.Add(0);
                        prefabAndUrlsList[index].ChangeTime = changePrefabAndUrls.ChangeTime;
                    }
                    break;
                case ActionType.URLAndTag_Removed:
                    index = prefabAndUrlsList.FindIndex(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
                    if (index != -1)
                    {
                        prefabAndUrlsList[index].UniqueIDs.RemoveAt(prefabAndUrlsList[index].UniqueIDs.Count - 1);
                        prefabAndUrlsList[index].Urls.RemoveAt(prefabAndUrlsList[index].Urls.Count - 1);
                        prefabAndUrlsList[index].Tags.RemoveAt(prefabAndUrlsList[index].Tags.Count - 1);
                        prefabAndUrlsList[index].ChangeTime = changePrefabAndUrls.ChangeTime;
                    }
                    break;
                case ActionType.Object_Changed:
                    index = prefabAndUrlsList.FindIndex(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
                    if (index != -1)
                    {
                        prefabAndUrlsList[index].Prefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(changePrefabAndUrls.Value1),typeof(Object));
                        prefabAndUrlsList[index].ChangeTime = changePrefabAndUrls.ChangeTime;
                    }
                    break;
                case ActionType.URL_Changed:
                    index = prefabAndUrlsList.FindIndex(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
                    if (index != -1)
                    {
                        int index2 = prefabAndUrlsList[index].UniqueIDs
                            .FindIndex(val => val == changePrefabAndUrls.SubUniqueId);
                        if (index2 != -1)
                        {
                            prefabAndUrlsList[index].Urls[index2] = changePrefabAndUrls.Value1;
                            prefabAndUrlsList[index].Tags[index2] = int.Parse(changePrefabAndUrls.Value2);
                            prefabAndUrlsList[index].ChangeTime = changePrefabAndUrls.ChangeTime;
                        }
                    }
                    break;
                case ActionType.Tag_Changed:
                    index = prefabAndUrlsList.FindIndex(val => val.UniqueID == changePrefabAndUrls.MainUniqueId);
                    if (index != -1)
                    {
                        int index2 = prefabAndUrlsList[index].UniqueIDs
                            .FindIndex(val => val == changePrefabAndUrls.SubUniqueId);
                        if (index2 != -1)
                        {
                            prefabAndUrlsList[index].Urls[index2] = changePrefabAndUrls.Value1;
                            prefabAndUrlsList[index].Tags[index2] = int.Parse(changePrefabAndUrls.Value2);
                            prefabAndUrlsList[index].ChangeTime = changePrefabAndUrls.ChangeTime;
                        }
                    }
                    break;
            }
        }
    }
    
    public string GetCurrentPath()
    {
        string[] assetsGUIDs = AssetDatabase.FindAssets("PrefabBlueprintLinker");
        foreach (var assetsGUID in assetsGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetsGUID);
            //このC#ファイルの場合
            if (Path.GetExtension(path) == ".cs")
            {
                path = Path.GetDirectoryName(path);
                Regex regex = new Regex(".*?/Assets");
                path = regex.Replace(path, "", 1);
                regex = new Regex("^Assets");
                return Application.dataPath + regex.Replace(path, "", 1);
            }
        }
        return "";
    }
    
    private string GetJsonPath()
    {
        string currentPath = GetCurrentPath();
        if (currentPath != "") return currentPath + "/PrefabBlueprintLinker.json";
        return "";
    }
    
    private string GetTagListFilePath()
    {
        string currentPath = GetCurrentPath();
        if (currentPath != "") return currentPath + "/TagList.txt";
        return "";
    }
    
    public string[] ReadTagList()
    {
        string path = GetTagListFilePath();
        if (File.Exists(path))
        {
           string lines = File.ReadAllText(path);
           return lines.Split("\n");
        }
        return null;
    }
}