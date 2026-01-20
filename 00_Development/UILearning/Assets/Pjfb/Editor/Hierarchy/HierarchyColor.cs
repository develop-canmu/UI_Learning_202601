using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class HierarchyColor : EditorWindow
{
    private static Vector2 s_dropDownSize = new Vector2(200,100);
    private static GlobalObjectId s_currentGlobalObjectID;
    private static Object s_currentObject;
    private static int s_currentIndex;
    private const string MenuPath = "GameObject/HierarchyColor";
    private const int LineNum = 4;
    private const int ColNum = 10;
    private const int CellSize = 20;

    private class SaveData : ScriptableSingleton<SaveData>
    {
        public ColorData ColorData = new ColorData();
    }
    
    [Serializable]
    private class ColorData
    {
        public List<Color32> ColorList = new List<Color32>();
        public List<ColorLabel> ColorLabelList = new List<ColorLabel>();
        public bool IsChecked;
    }

    [Serializable]
    private class ColorLabel
    {
        public Color Color;
        public ulong TargetObjectID;
        public string AssetGUID;
        public ulong TargetPrefabID;
        public int NestCount;

        public ColorLabel(Color color, ulong targetObjectID, string assetGUID, ulong targetPrefabID, int nestCount)
        {
            Color = color;
            TargetObjectID = targetObjectID;
            AssetGUID = assetGUID;
            TargetPrefabID = targetPrefabID;
            NestCount = nestCount;
        }
    }

    static HierarchyColor()
    {
        LoadSaveData();
        EditorApplication.delayCall += Load;
        EditorApplication.quitting += SaveSaveData;
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI2;
        SetDefaultColor(SaveData.instance.ColorData.ColorList);
    }
    
    private static void Load()
    {
        SaveData.instance.ColorData.IsChecked = !string.IsNullOrEmpty(EditorUserSettings.GetConfigValue(MenuPath));
        Menu.SetChecked(MenuPath, SaveData.instance.ColorData.IsChecked);
        EditorApplication.delayCall -= Load;
    }
    
    [MenuItem(MenuPath)]
    private static void IsShowActiveToggle()
    {
        SaveData.instance.ColorData.IsChecked = !SaveData.instance.ColorData.IsChecked;
        Menu.SetChecked(MenuPath, SaveData.instance.ColorData.IsChecked);
        EditorUserSettings.SetConfigValue(MenuPath, SaveData.instance.ColorData.IsChecked ? "true" : null);
        EditorApplication.RepaintHierarchyWindow();
    }

    private static void LoadSaveData()
    {
        if (File.Exists(GetJsonPath()))
        {
            SaveData.instance.ColorData = JsonUtility.FromJson<ColorData>(File.ReadAllText(GetJsonPath()));
        }
    }
    
    private static void SaveSaveData()
    {
        //階層の深さで降順ソートを行い、浅い階層の色を優先して表示する
        SaveData.instance.ColorData.ColorLabelList = SaveData.instance.ColorData.ColorLabelList.OrderByDescending(label => label.NestCount).ToList();
        ColorData colorData = SaveData.instance.ColorData;
        File.WriteAllText(GetJsonPath(),JsonUtility.ToJson(colorData));
    }
    
    private static string GetJsonPath()
    {
        string path = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("HierarchyColor")[0])) + "/HierarchyColor.json";
        Regex regex = new Regex(".*?/Assets");
        path = regex.Replace(path, "", 1);
        regex = new Regex("^Assets");
        return Application.dataPath + regex.Replace(path, "", 1);
    }

    private static void OnGUI2( int instanceID, Rect rect )
    {
        if(!SaveData.instance.ColorData.IsChecked) return;

        GlobalObjectId globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(instanceID);
        
        rect.x = rect.xMax - 10;
        rect.y += 1;
        rect.width = 6;
        rect.height -= 2;
        
        //ヒエラルキー上で見えているオブジェクトのindex値を取得する
        int index = SaveData.instance.ColorData.ColorLabelList.FindIndex(label => label.TargetObjectID == globalObjectId.targetObjectId && label.AssetGUID == globalObjectId.assetGUID.ToString() && label.TargetPrefabID == globalObjectId.targetPrefabId);
        //ない場合はPrefabの元をたどってindex値を取得する
        if (index == -1)
        {
            var instanceObj = EditorUtility.InstanceIDToObject(instanceID);
            if (instanceObj != null)
            {
                var obj = PrefabUtility.GetCorrespondingObjectFromSource(instanceObj);
                while (obj != null)
                {
                    GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(obj);
                    index = SaveData.instance.ColorData.ColorLabelList.FindIndex(label => label.TargetObjectID == id.targetObjectId);
                    if(index != -1) break;
                    obj = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                }
            }
        }
        if (index != -1) EditorGUI.DrawRect( rect, SaveData.instance.ColorData.ColorLabelList[index].Color );
        else EditorGUI.DrawRect( new Rect(new Vector2(rect.x,rect.y + 4),new Vector2(rect.width,rect.height - 8)), Color.white );
        if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
        {
            s_currentGlobalObjectID = globalObjectId;
            s_currentObject = EditorUtility.InstanceIDToObject(instanceID);
            s_currentIndex = index;
            DropDown(Event.current.mousePosition, s_dropDownSize);
        }
    }
    
    private void OnGUI()
    {
        if(!SaveData.instance.ColorData.IsChecked) return;
        
        for (int y = 0; y < LineNum; y++)
        {
            for (int x = 0; x < ColNum; x++)
            {
                Rect rect = new Rect(CellSize * x, CellSize * y, CellSize, CellSize);
                EditorGUI.DrawRect( rect, SaveData.instance.ColorData.ColorList[x + (ColNum * y)]);
                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    int index = SaveData.instance.ColorData.ColorLabelList.FindIndex(label => label.TargetObjectID == s_currentGlobalObjectID.targetObjectId && label.AssetGUID == s_currentGlobalObjectID.assetGUID.ToString() && label.TargetPrefabID == s_currentGlobalObjectID.targetPrefabId);
                    //完全一致の場合には上書きする
                    if (index != -1) SaveData.instance.ColorData.ColorLabelList[index] = new ColorLabel(SaveData.instance.ColorData.ColorList[x + (ColNum * y)], s_currentGlobalObjectID.targetObjectId, s_currentGlobalObjectID.assetGUID.ToString(), s_currentGlobalObjectID.targetPrefabId, GetNestCount(s_currentObject));
                    //ない場合、Prefabの元に情報がある場合には追加する
                    else SaveData.instance.ColorData.ColorLabelList.Add(new ColorLabel(SaveData.instance.ColorData.ColorList[x + (ColNum * y)], s_currentGlobalObjectID.targetObjectId, s_currentGlobalObjectID.assetGUID.ToString(), s_currentGlobalObjectID.targetPrefabId, GetNestCount(s_currentObject)));
                    SaveSaveData();
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        if (GUI.Button(new Rect(0, 80, 50, 20), "色削除")) Remove(s_currentGlobalObjectID.targetObjectId, s_currentGlobalObjectID.assetGUID.ToString(), s_currentGlobalObjectID.targetPrefabId);
        if (GUI.Button(new Rect(50, 80, 75, 20), "色一括削除")) if(s_currentIndex != -1) RemoveColor(SaveData.instance.ColorData.ColorLabelList[s_currentIndex].Color);
        if (GUI.Button(new Rect(125, 80, 75, 20), "色全削除")) RemoveAll();
    }

    private static void DropDown(Vector2 popupPosition, Vector2 windowSize)
    {
        popupPosition = GUIUtility.GUIToScreenPoint(popupPosition);
        var window = CreateInstance<HierarchyColor>();
        window.ShowAsDropDown(new Rect(popupPosition, Vector2.zero), windowSize);
    }
    
    private static void Remove(ulong targetObjectId, string assetGUID, ulong targetPrefabId)
    {
        SaveData.instance.ColorData.ColorLabelList = SaveData.instance.ColorData.ColorLabelList.Where(label => label.TargetObjectID != targetObjectId && label.AssetGUID != assetGUID && label.TargetObjectID != targetPrefabId).ToList();
        SaveSaveData();
        EditorApplication.RepaintHierarchyWindow();
    }
    
    private static void RemoveColor(Color color)
    {
        SaveData.instance.ColorData.ColorLabelList = SaveData.instance.ColorData.ColorLabelList.Where(label => label.Color != color).ToList();
        SaveSaveData();
        EditorApplication.RepaintHierarchyWindow();
    }
    
    private static void RemoveAll()
    {
        SaveData.instance.ColorData.ColorLabelList.Clear();
        SaveSaveData();
        EditorApplication.RepaintHierarchyWindow();
    }
    
    private static int GetNestCount(Object obj)
    {
        int count = 0;
        if (obj == null) return count;
        var obj2= PrefabUtility.GetCorrespondingObjectFromSource(obj);
        while (obj2 != null)
        {
            count++;
            obj2 = PrefabUtility.GetCorrespondingObjectFromSource(obj2);
        }
        return count;
    }

    private static void SetDefaultColor(List<Color32> colorList)
    {
        colorList.Clear();
        colorList.Add(new Color32(255,255,255, 255));
        colorList.Add(new Color32(228,50,34, 255));
        colorList.Add(new Color32(230,84,39, 255));
        colorList.Add(new Color32(234,199,68, 255));
        colorList.Add(new Color32(152,217,81, 255));
        colorList.Add(new Color32(98,206,211, 255));
        colorList.Add(new Color32(74,147,223, 255));
        colorList.Add(new Color32(50,84,222, 255));
        colorList.Add(new Color32(157,49,222, 255));
        colorList.Add(new Color32(211,59,143, 255));
        colorList.Add(new Color32(222,222,222, 255));
        colorList.Add(new Color32(197,41,32, 255));
        colorList.Add(new Color32(198,63,31, 255));
        colorList.Add(new Color32(209,159,55, 255));
        colorList.Add(new Color32(111,177,63, 255));
        colorList.Add(new Color32(76,163,173, 255));
        colorList.Add(new Color32(56,116,182, 255));
        colorList.Add(new Color32(35,65,180, 255));
        colorList.Add(new Color32(121,34,180, 255));
        colorList.Add(new Color32(172,44,113, 255));
        colorList.Add(new Color32(132,132,132, 255));
        colorList.Add(new Color32(132,25,26, 255));
        colorList.Add(new Color32(132,35,17, 255));
        colorList.Add(new Color32(149,97,32, 255));
        colorList.Add(new Color32(62,113,37, 255));
        colorList.Add(new Color32(45,101,112, 255));
        colorList.Add(new Color32(31,69,114, 255));
        colorList.Add(new Color32(19,38,113, 255));
        colorList.Add(new Color32(73,18,113, 255));
        colorList.Add(new Color32(108,25,68, 255));
        colorList.Add(new Color32(23,23,23, 255));
        colorList.Add(new Color32(81,11,19, 255));
        colorList.Add(new Color32(81,17,6, 255));
        colorList.Add(new Color32(97,60,17, 255));
        colorList.Add(new Color32(34,72,21, 255));
        colorList.Add(new Color32(26,65,74, 255));
        colorList.Add(new Color32(18,46,71, 255));
        colorList.Add(new Color32(9,24,70, 255));
        colorList.Add(new Color32(44,7,70, 255));
        colorList.Add(new Color32(66,11,44, 255));
    }
}