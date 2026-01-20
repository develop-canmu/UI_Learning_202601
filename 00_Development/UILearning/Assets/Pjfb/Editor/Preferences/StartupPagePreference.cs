using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pjfb.Editor.Preference
{
    public static class StartupPagePreference
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new SettingsProvider("AppPreferences/", SettingsScope.User)
            {
                label = "StartupPage",
                activateHandler = (searchContext, rootElement) =>
                {
                    // 有効か
                    Toggle enableToggle = new Toggle("Enable");
                    // 値セット
                    if(Boolean.TryParse(EditorUserSettings.GetConfigValue(AppManager.StartupPageEnableKey), out bool enable))
                    {
                        enableToggle.value = enable;
                    }
                    // 変更時に保存
                    enableToggle.RegisterValueChangedCallback(evt =>
                    {
                        EditorUserSettings.SetConfigValue(AppManager.StartupPageEnableKey, evt.newValue.ToString());
                        AssetDatabase.SaveAssets();
                    });
                    rootElement.Add(enableToggle);
                    
                    // 起動ページ
                    EnumField pageTypeField = new EnumField("Startup Page", PageType.Title);
                    // 値セット
                    if(Enum.TryParse(EditorUserSettings.GetConfigValue(AppManager.StartupPagePageTypeKey), out PageType pageType))
                    {
                        pageTypeField.value = pageType;
                    }
                    // 変更時に保存
                    pageTypeField.RegisterValueChangedCallback(evt =>
                    {
                        EditorUserSettings.SetConfigValue(AppManager.StartupPagePageTypeKey, evt.newValue.ToString());
                        AssetDatabase.SaveAssets();
                    });
                    rootElement.Add(pageTypeField);
                },
            };
            return provider;
        }
    }
}
