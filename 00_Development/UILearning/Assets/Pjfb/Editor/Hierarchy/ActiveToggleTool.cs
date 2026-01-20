using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ActiveToggleTool
{
    private class StartUpData : ScriptableSingleton<StartUpData>
    {
        private int _count;
        public bool _isChecked;

        public bool IsStartUp()
            => _count++ == 0;
    }
    
    private const string MenuPath = "GameObject/IsShowActiveToggle";
    private const int WIDTH = 16;
    private const int OFFSET = 35;

    static ActiveToggleTool()
    {
        if (!StartUpData.instance.IsStartUp()) return;
        EditorApplication.delayCall += Load;
    }

    private static void Load()
    {
        StartUpData.instance._isChecked = !string.IsNullOrEmpty(EditorUserSettings.GetConfigValue(MenuPath));
        Menu.SetChecked(MenuPath, StartUpData.instance._isChecked);
        EditorApplication.delayCall -= Load;
    }
    
    [MenuItem(MenuPath)]
    private static void IsShowActiveToggle()
    {
        StartUpData.instance._isChecked = !StartUpData.instance._isChecked;
        Menu.SetChecked(MenuPath, StartUpData.instance._isChecked);
        EditorUserSettings.SetConfigValue(MenuPath, StartUpData.instance._isChecked ? "true" : null);
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }
    
    private static void OnGUI(int instanceID, Rect selectionRect)
    {
        var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null || !StartUpData.instance._isChecked)
        {
            return;
        }

        var pos = selectionRect;
        pos.x = pos.xMax - OFFSET;
        pos.width = WIDTH;

        bool active = GUI.Toggle(pos, go.activeSelf, string.Empty);
        if (active == go.activeSelf)
        {
            return;
        }

        Undo.RecordObject(go, $"{(active ? "Activate" : "Deactivate")} GameObject '{go.name}'");
        go.SetActive(active);
        EditorUtility.SetDirty(go);
    }
}