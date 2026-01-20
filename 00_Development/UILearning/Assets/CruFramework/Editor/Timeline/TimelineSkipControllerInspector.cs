using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using CruFramework.Timeline;
using UnityEngine.Playables;

namespace CruFramework.Editor.Timeline
{
    [CustomEditor(typeof(TimelineSkipController))]
    public class TimelineSkipControllerInspector : UnityEditor.Editor
    {
        private class ListViewItem : VisualElement
        {
            public IntegerField frameField = null;
            public Button skipButton = null;
            
            public ListViewItem()
            {
                style.flexDirection = FlexDirection.Row;
                
                frameField = new IntegerField();
                frameField.style.flexGrow = 1.0f;
                frameField.isDelayed = true;
                Add(frameField);
                
                skipButton = new Button();
                skipButton.text = "Skip";
                Add(skipButton);
            }
        }
        
        private TimelineSkipController controller = null;
        private ListView listView = null;
        private ObjectField objectField = null;

        public override VisualElement CreateInspectorGUI()
        {
            controller = (TimelineSkipController)target;
            VisualElement rootVisualElement = new VisualElement();
            
            objectField = new ObjectField("PlayableDirector");
            objectField.objectType = typeof(PlayableDirector);
            objectField.value = controller.playableDirector;
            objectField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(controller, "Change PlayableDirector");
                controller.playableDirector = (PlayableDirector)evt.newValue;
                EditorUtility.SetDirty(controller);
            });
            rootVisualElement.Add(objectField);
            
            Button skipButton = new Button();
            skipButton.text = "Skip";
            skipButton.style.height = EditorGUIUtility.singleLineHeight + 4;
            skipButton.clicked += () =>
            {
                controller.Skip();
            };
            rootVisualElement.Add(skipButton);
            
            listView = new ListView();
            listView.headerTitle = "Frames";
            listView.showFoldoutHeader = true;
            listView.showBoundCollectionSize = false;
            // 追加、削除
            listView.showAddRemoveFooter = true;
            // データ
            listView.itemsSource = controller.frames;
            // 高さ
            listView.fixedItemHeight = EditorGUIUtility.singleLineHeight + 4;
            // 要素の生成
            listView.makeItem = () =>
            {
                ListViewItem item = new ListViewItem();
                return item;
            };
            listView.bindItem = (e, i) =>
            {
                ListViewItem item = (ListViewItem)e;
                item.frameField.value = controller.frames[i];
                
                item.frameField.UnregisterCallback<ChangeEvent<int>, int>(OnFrameChanged);
                item.frameField.RegisterCallback<ChangeEvent<int>, int>(OnFrameChanged, i);
                
                item.skipButton.UnregisterCallback<ClickEvent, int>(OnClickSkipButton);
                item.skipButton.RegisterCallback<ClickEvent, int>(OnClickSkipButton, i);
            };
            
            // 要素の追加
            listView.itemsAdded += indexes =>
            {
                Undo.RecordObject(controller, "Add");
                foreach (int index in indexes)
                {
                    ArrayUtility.Add(ref controller.frames, 0);
                }
                Array.Sort(controller.frames);
                listView.RefreshItems();
                EditorUtility.SetDirty(controller);
            };
            // 要素の削除
            listView.itemsRemoved += indexes =>
            {
                Undo.RecordObject(controller, "Remove");
                foreach (int index in indexes)
                {
                    ArrayUtility.RemoveAt(ref controller.frames, index);
                }
                EditorUtility.SetDirty(controller);
            };
            
            listView.RefreshItems();
            rootVisualElement.Add(listView);
            
            return rootVisualElement;
        }
        
        private void OnFrameChanged(ChangeEvent<int> evt, int index)
        {
            Undo.RecordObject(controller, "Change Frame");
            controller.frames[index] = evt.newValue;
            Array.Sort(controller.frames);
            listView.RefreshItems();
            EditorUtility.SetDirty(controller);
        }
        
        private void OnClickSkipButton(ClickEvent evt, int index)
        {
            controller.Skip(controller.frames[index]);
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
            objectField.value = controller.playableDirector;
            listView.itemsSource = controller.frames;
            listView.RefreshItems();
        }
    }
}
