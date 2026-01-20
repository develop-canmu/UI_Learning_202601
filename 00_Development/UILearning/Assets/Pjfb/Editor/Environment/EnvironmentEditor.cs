using System;
using System.Collections.Generic;
using System.Reflection;
using Pjfb.Master;
using UnityEditor;
using UnityEditor.Build;

namespace Pjfb.Editor.Environment
{
    public static class EnvironmentEditor
    {
        private const string baseMenuItemPath = "Pjfb/Environment/";
        private const string baseSymbol = "PJFB_";

        // 環境のシンボルリスト
        private static List<string> environmentList = new ();
        
        private static AppEnvironment.EnvironmentEnum currentEnvironment;
        
        /// <summary>環境のシンボルリスト作成</summary>
        private static void CreateEnvironmentList()
        {
            environmentList.Clear();
            foreach (AppEnvironment.EnvironmentEnum value in Enum.GetValues(typeof(AppEnvironment.EnvironmentEnum)))
            {
                if (value == AppEnvironment.EnvironmentEnum.PROD) continue;
                environmentList.Add($"{baseSymbol}{value.ToString()}");
            }
        }
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.delayCall += CreateEnvironmentItem;
            currentEnvironment = AppEnvironment.CurrentEnvironment;
            CreateEnvironmentList();
        }

        #region Menu用
        /// <summary>Menuにアイテム追加する</summary>
        private static void CreateEnvironmentItem()
        {
            foreach (AppEnvironment.EnvironmentEnum value in Enum.GetValues(typeof(AppEnvironment.EnvironmentEnum)))
            {
                if(value == AppEnvironment.EnvironmentEnum.PROD) continue;
                AddMenuItem(baseMenuItemPath + value, "", AppEnvironment.CurrentEnvironment == value, (int)value, () => ChangeEnvironment(value), null);
            }
            EditorApplication.delayCall -= CreateEnvironmentItem;
            ItemUpdate();
        }

        /// <summary>アイテム追加</summary>
        private static void AddMenuItem(string name, string shortcut, bool isChecked, int priority, Action execute, Func<bool> validate)
        {
            var addMenuItemMethod = typeof(UnityEditor.Menu).GetMethod("AddMenuItem", BindingFlags.Static | BindingFlags.NonPublic);
            addMenuItemMethod?.Invoke(null, new object[] { name, shortcut, isChecked, priority, execute, validate });
        }
        
        /// <summary>見た目更新</summary>
        private static void ItemUpdate()
        {
            var internalUpdateAllMenus = typeof(EditorUtility).GetMethod("Internal_UpdateAllMenus", BindingFlags.Static | BindingFlags.NonPublic);
            internalUpdateAllMenus?.Invoke(null, null);

            var shortcutIntegrationType = Type.GetType("UnityEditor.ShortcutManagement.ShortcutIntegration, UnityEditor.CoreModule");
            var instanceProp = shortcutIntegrationType?.GetProperty("instance", BindingFlags.Static | BindingFlags.Public);
            var instance = instanceProp?.GetValue(null);
            var rebuildShortcutsMethod = instance?.GetType().GetMethod("RebuildShortcuts", BindingFlags.Instance | BindingFlags.NonPublic);
            rebuildShortcutsMethod?.Invoke(instance, null);
        }
        #endregion

        /// <summary>環境変更</summary>
        private static void ChangeEnvironment(AppEnvironment.EnvironmentEnum targetEnvironment)
        {
            if (currentEnvironment == targetEnvironment) return;
            
            // シンボル取得
            string symbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
            string[] symbolsArray = symbols.Split(';');
            // シンボルの入れ替え
            for (int i = 0; i < symbolsArray.Length; i++)
            {
                if (!environmentList.Contains(symbolsArray[i])) continue;
                symbolsArray[i] = $"{baseSymbol}{targetEnvironment.ToString()}";
            }
            // 新しいシンボル
            string newSymbols = string.Join(";", symbolsArray);
            // シンボルセット
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), newSymbols);

            if (currentEnvironment != targetEnvironment)
            {
                // 環境を変えたらマスタ削除
                MasterManager.Instance.DeleteMaster();
                // 現在の環境更新
                currentEnvironment = targetEnvironment;
            }
        }
    }
}