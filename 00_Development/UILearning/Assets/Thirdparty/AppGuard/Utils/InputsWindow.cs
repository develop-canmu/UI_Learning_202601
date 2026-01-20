using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ThirdParty.AppGuard.Editor
{
    public class InputsWindow : EditorWindow
    {
        private Action<Dictionary<string, string>> OnInputAction { get; set; }

        private string Message { get; set; }
        private Dictionary<string, string> Elements { get; set; }
        private Dictionary<string, string> Results { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> OnDestroyResults { get; set; }

        private bool IsInitialize { get; set; }


        public static void Open(string title, Dictionary<string, string> elements, Action<Dictionary<string, string>> onInput)
        {
            // TODO：Element Key GUI Type (ex. text/directory/bool)
            var win = new InputsWindow()
            {
                OnInputAction = onInput,
                Message = "項目を入力してください",
                Elements = elements,
                IsInitialize = false

            };
            win.titleContent.text = title;
            win.Show();
        }


        #region EditorWindow Event

        private void OnDestroy()
        {
            OnInputAction?.Invoke(OnDestroyResults);
            OnInputAction = null;
        }

        private void OnGUI()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
            {
                OnDestroyResults = Results;
                Close();
            }

            GUILayout.Label(Message);

            GUILayout.Space(10f);

            GUI.SetNextControlName("FocusField");
            foreach (var element in Elements)
            {
                var key = element.Key;
                if (!Results.ContainsKey(key))
                {
                    Results.Add(key, element.Value);
                }
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(key, GUILayout.Width(150f));
                    Results[key] = GUILayout.TextField(Results[key]);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
            }

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("OK", GUILayout.Height(20f)))
                {
                    OnDestroyResults = Results;
                    Close();
                }

                if (GUILayout.Button("CANCEL", GUILayout.Height(20f)))
                {
                    OnDestroyResults = null;
                    Close();
                }
            }
            GUILayout.EndHorizontal();


            if (!IsInitialize)
            {
                EditorGUI.FocusTextInControl("FocusField");
            }
            IsInitialize = true;
        }

        #endregion
    }
}