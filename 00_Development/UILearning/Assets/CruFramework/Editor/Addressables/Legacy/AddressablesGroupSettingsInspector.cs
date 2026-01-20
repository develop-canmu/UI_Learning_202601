#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.AddressableAssets;

namespace CruFramework.Editor.Addressables
{
    [CustomEditor(typeof(AddressablesGroupSettings))]
    internal class AddressablesGroupSettingsInspector : UnityEditor.Editor
    {
        private AddressablesGroupSettings groupSettings = null;

        private EnumField groupTypeField = null;
        private EnumField packingModeField = null;
        private EnumField addressRootField = null;
        private TextField addressRootOverrideField = null;
        private IntegerField requestTimeoutField = null;
        private IntegerField httpRedirectLimitField = null;
        private IntegerField retryCountField = null;

        public override VisualElement CreateInspectorGUI()
        {
            groupSettings = (AddressablesGroupSettings)target;
            VisualElement rootVisualElement = new VisualElement();

            // グループタイプ
            groupTypeField = new EnumField(nameof(groupSettings.GroupType), groupSettings.GroupType);
            groupTypeField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change GroupType");
                groupSettings.GroupType = (AddressablesGroupType)evt.newValue;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(groupTypeField);

            // パッキングモード
            packingModeField = new EnumField(nameof(groupSettings.PackingMode), groupSettings.PackingMode);
            packingModeField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change PackingMode");
                groupSettings.PackingMode = (AddressablesBundlePackingMode)evt.newValue;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(packingModeField);

            // アドレスルート
            addressRootField = new EnumField(nameof(groupSettings.AddressRoot), groupSettings.AddressRoot);
            addressRootField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change AddressRoot");
                groupSettings.AddressRoot = (AddressablesAddressRoot)evt.newValue;
                addressRootOverrideField.style.display = groupSettings.AddressRoot == AddressablesAddressRoot.CurrentDirectory ? DisplayStyle.Flex : DisplayStyle.None;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(addressRootField);

            // アドレスルートの上書き
            addressRootOverrideField = new TextField(nameof(groupSettings.AddressRootOverride));
            addressRootOverrideField.value = groupSettings.AddressRootOverride;
            addressRootOverrideField.style.display = groupSettings.AddressRoot == AddressablesAddressRoot.CurrentDirectory ? DisplayStyle.Flex : DisplayStyle.None;
            addressRootOverrideField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change AddressRootOverride");
                groupSettings.AddressRootOverride = evt.newValue;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(addressRootOverrideField);
            
            // タイムアウト
            requestTimeoutField = new IntegerField(nameof(groupSettings.RequestTimeout));
            requestTimeoutField.value = groupSettings.RequestTimeout;
            requestTimeoutField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change RequestTimeout");
                groupSettings.RequestTimeout = evt.newValue;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(requestTimeoutField);
            
            // リダイレクト
            httpRedirectLimitField = new IntegerField(nameof(groupSettings.HttpRedirectLimit));
            httpRedirectLimitField.value = groupSettings.HttpRedirectLimit;
            httpRedirectLimitField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change HttpRedirectLimit");
                groupSettings.HttpRedirectLimit = evt.newValue;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(httpRedirectLimitField);
            
            // リトライ
            retryCountField = new IntegerField(nameof(groupSettings.RetryCount));
            retryCountField.value = groupSettings.RetryCount;
            retryCountField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(groupSettings, "Change RetryCount");
                groupSettings.RetryCount = evt.newValue;
                EditorUtility.SetDirty(groupSettings);
            });
            rootVisualElement.Add(retryCountField);

            // ラベル追加されたか検知できないのでIMGUIで記述
            IMGUIContainer labelContainer = new IMGUIContainer(() =>
            {
                using (EditorGUILayout.HorizontalScope scope = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Labels");
                    GUILayout.FlexibleSpace();
                    // アドレッサブルのラベルウィンドウ表示
                    if (GUILayout.Button("Edit Labels", GUILayout.Width(80)))
                    {
                        OpenLabelWindow();
                    }
                }

                using (EditorGUILayout.VerticalScope scope = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUI.indentLevel++;
                    // アドレッサブルで定義されてるラベルを取得
                    List<string> labels = AddressableAssetSettingsDefaultObject.Settings.GetLabels();
                    foreach (string label in labels)
                    {
                        using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
                        {
                            bool enable = EditorGUILayout.ToggleLeft(label, groupSettings.Labels.Contains(label));
                            if (check.changed)
                            {
                                Undo.RecordObject(groupSettings, "Change Labels");
                                if (enable)
                                {
                                    if (!groupSettings.Labels.Contains(label))
                                    {
                                        groupSettings.Labels.Add(label);
                                    }
                                }
                                else
                                {
                                    if (groupSettings.Labels.Contains(label))
                                    {
                                        groupSettings.Labels.Remove(label);
                                    }
                                }
                                EditorUtility.SetDirty(groupSettings);
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            });
            labelContainer.style.marginLeft = 4;
            rootVisualElement.Add(labelContainer);

            return rootVisualElement;
        }

        private void OpenLabelWindow()
        {
            EditorWindow.GetWindow<LabelWindow>(true).Intialize(AddressableAssetSettingsDefaultObject.Settings);
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoCallback;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoCallback;
        }

        private void OnUndoRedoCallback()
        {
            groupTypeField.value = groupSettings.GroupType;
            packingModeField.value = groupSettings.PackingMode;
            addressRootField.value = groupSettings.AddressRoot;
            addressRootOverrideField.value = groupSettings.AddressRootOverride;
            requestTimeoutField.value = groupSettings.RequestTimeout;
            httpRedirectLimitField.value = groupSettings.HttpRedirectLimit;
            retryCountField.value = groupSettings.RetryCount;
        }
    }
}

#endif