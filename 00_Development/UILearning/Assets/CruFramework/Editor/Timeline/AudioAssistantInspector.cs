using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEditor;
using UnityEditor.UIElements;
using CruFramework.Timeline;
using CruFramework.Audio;

namespace CruFramework.Editor.Timeline
{
    [CustomEditor(typeof(AudioAssistant))]
    public class AudioAssistantInspector : UnityEditor.Editor
    {
        private static readonly string CustomAudioGroupName = "Custom";
        
        private class ListViewItem : VisualElement
        {
            public TextField trackNameField = null;
            public PopupField<string> audioGroupPopup = null;
            public TextField audioGroupField = null;

            public ListViewItem(List<string> audioGroupList)
            {
                style.flexDirection = FlexDirection.Column;
                // トラック名
                trackNameField = new TextField("TrackName");
                Add(trackNameField);
                // 選択ポップアップ
                audioGroupPopup = new PopupField<string>("AudioGroup", audioGroupList, 0);
                audioGroupPopup.RegisterValueChangedCallback(evt =>
                {
                    audioGroupField.value = evt.newValue;
                    audioGroupField.style.display = evt.newValue == CustomAudioGroupName ? DisplayStyle.Flex : DisplayStyle.None;
                });
                Add(audioGroupPopup);
                // オーディオグループ名
                audioGroupField = new TextField();
                audioGroupField.style.display = DisplayStyle.None;
                Add(audioGroupField);
            }
        }
        
        private List<string> audioGroupList = new List<string>() 
        { 
            AudioGroup.BGM.Name, 
            AudioGroup.SE.Name, 
            AudioGroup.Voice.Name, 
            CustomAudioGroupName 
        };
        
        private AudioAssistant assistant = null;
        private ListView listView = null;
        private ObjectField objectField = null;
        
        public override VisualElement CreateInspectorGUI()
        {
            assistant = (AudioAssistant)target;
            VisualElement rootVisualElement = new VisualElement();
            
            objectField = new ObjectField("PlayableDirector");
            objectField.objectType = typeof(PlayableDirector);
            objectField.value = assistant.playableDirector;
            objectField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(assistant, "Change PlayableDirector");
                assistant.playableDirector = (PlayableDirector)evt.newValue;
                EditorUtility.SetDirty(assistant);
            });
            rootVisualElement.Add(objectField);
            
            listView = new ListView();
            listView.headerTitle = "BindingDatas";
            listView.showFoldoutHeader = true;
            listView.showBoundCollectionSize = false;
            // 追加、削除
            listView.showAddRemoveFooter = true;
            // 入れ替え
            listView.reorderable = true;
            listView.reorderMode = ListViewReorderMode.Animated;
            // データ
            listView.itemsSource = assistant.bindingDatas;
            // 高さを可変
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            // 要素の生成
            listView.makeItem = () =>
            {
                ListViewItem item = new ListViewItem(audioGroupList);
                return item;
            };
            // 要素の更新
            listView.bindItem = (e, i) =>
            {
                ListViewItem item = (ListViewItem)e;
                // トラック名
                item.trackNameField.value = assistant.bindingDatas[i].trackName;
                // オーディオグループ名
                item.audioGroupField.value = assistant.bindingDatas[i].audioGroup;
                item.audioGroupPopup.value = audioGroupList.Contains(item.audioGroupField.value) ? item.audioGroupField.value : CustomAudioGroupName;
                
                item.trackNameField.UnregisterCallback<ChangeEvent<string>, int>(OnTrackNameChanged);
                item.trackNameField.RegisterCallback<ChangeEvent<string>, int>(OnTrackNameChanged, i);
                
                item.audioGroupField.UnregisterCallback<ChangeEvent<string>, int>(OnAudioGroupChanged);
                item.audioGroupField.RegisterCallback<ChangeEvent<string>, int>(OnAudioGroupChanged, i);
            };
            // 要素の追加
            listView.itemsAdded += indexes =>
            {
                Undo.RecordObject(assistant, "Add");
                foreach (int index in indexes)
                {
                    ArrayUtility.Add(ref assistant.bindingDatas, new AudioAssistant.BindingData());
                }
                EditorUtility.SetDirty(assistant);
            };
            // 要素の削除
            listView.itemsRemoved += indexes =>
            {
                Undo.RecordObject(assistant, "Remove");
                foreach (int index in indexes)
                {
                    ArrayUtility.RemoveAt(ref assistant.bindingDatas, index);
                }
                EditorUtility.SetDirty(assistant);
            };
            
            listView.RefreshItems();
            rootVisualElement.Add(listView);
            
            return rootVisualElement;
        }

        /// <summary>トラック名変更通知</summary>
        private void OnTrackNameChanged(ChangeEvent<string> evt, int index)
        {
            Undo.RecordObject(assistant, "Change TrackName");
            assistant.bindingDatas[index].trackName = evt.newValue;
            EditorUtility.SetDirty(assistant);
        }
        
        /// <summary>オーディオグループ名変更通知</summary>
        private void OnAudioGroupChanged(ChangeEvent<string> evt, int index)
        {
            Undo.RecordObject(assistant, "Change AudioGroup");
            assistant.bindingDatas[index].audioGroup = evt.newValue;
            EditorUtility.SetDirty(assistant);
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedoCallback;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoCallback;
        }

        private void UndoRedoCallback()
        {
            objectField.value = assistant.playableDirector;
            listView.itemsSource = assistant.bindingDatas;
            listView.RefreshItems();
        }
    }
}
