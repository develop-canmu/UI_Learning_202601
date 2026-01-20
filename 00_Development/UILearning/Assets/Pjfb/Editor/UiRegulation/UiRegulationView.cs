using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Pjfb.Extensions;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

namespace Pjfb.UiRegulation
{
    public class ObjectState
    {
        public string path;
        public bool lockedPosition;
        public bool lockedSizeWidth;
        public bool lockedSizeHeight;
    }
    
    public class UiRegulationView : EditorWindow
    {
        
        private UiRegulationType checkerType;
        private Vector2 scrollPosition = Vector2.zero;
        
        private List<string> missingList = new();
        private List<string> unnecessaryList = new();
        private List<string> anchorErrorList = new();
        private List<string> errorPositionList = new();
        private List<string> errorScaleList = new();
        
        private bool missingListOpen = false;
        private bool unnecessaryListOpen = false;
        private bool anchorErrorListOpen = false;
        private bool errorPositionListOpen = false;
        private bool errorScaleListOpen = false;
        
        private bool isCheck = false;
        private string rootName = "";

        [MenuItem("Pjfb/UITools/UiRegulationView")]
        private static void OpenView()
        {
            var window = (UiRegulationView)GetWindow(typeof(UiRegulationView));
            window.Show();
        }

        public void OnGUI()
        {

            checkerType = (UiRegulationType)EditorGUILayout.EnumPopup("Ui種別", checkerType);

            if (GUILayout.Button("check"))
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    var root = stage.prefabContentsRoot;
                    rootName = root.name;
                    UiRegulationCheck(checkerType);
                    isCheck = true;
                }
                else
                {
                    isCheck = false;
                }
            }

            if (!isCheck)
            {
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var textStyle = new GUIStyle();
            textStyle.fontSize = 20;
            textStyle.normal.textColor = Color.white;
            
            EditorGUILayout.LabelField($"{rootName}のチェック結果",textStyle);

            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("■レギュレーション構造の確認");
            
            EditorGUI.indentLevel = 1;
            missingListOpen = EditorGUILayout.Foldout(missingListOpen, $"必須オブジェクトの不足:{missingList.Count}件");
            if(missingListOpen)
            {
                EditorGUI.indentLevel++;
                missingList.ForEach(value =>
                {
                    EditorGUILayout.LabelField(value);
                });
            }

            EditorGUI.indentLevel = 1;
            unnecessaryListOpen = EditorGUILayout.Foldout(unnecessaryListOpen, $"レギュレーション違反のオブジェクト配置:{unnecessaryList.Count}件");
            if(unnecessaryListOpen)
            {
                EditorGUI.indentLevel++;
                unnecessaryList.ForEach(value =>
                {
                    EditorGUILayout.LabelField(value);
                });
            } 
            
            EditorGUI.indentLevel = 1;
            anchorErrorListOpen = EditorGUILayout.Foldout(anchorErrorListOpen, $"全方位ストレッチの設定漏れ:{anchorErrorList.Count}件");
            if(anchorErrorListOpen)
            {
                EditorGUI.indentLevel++;
                anchorErrorList.ForEach(value =>
                {
                    EditorGUILayout.LabelField(value);
                });
            } 
            
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("■オブジェクトのRectTransform確認");

            EditorGUI.indentLevel = 1;
            errorPositionListOpen = EditorGUILayout.Foldout(errorPositionListOpen, $"座標、サイズに奇数または実数が指定されています:{errorPositionList.Count}件");
            if(errorPositionListOpen)
            {
                EditorGUI.indentLevel++;
                errorPositionList.ForEach(value =>
                {
                    EditorGUILayout.LabelField(value);
                });
            } 
            
            EditorGUI.indentLevel = 1;
            errorScaleListOpen = EditorGUILayout.Foldout(errorScaleListOpen, $"scaleに1以外が指定されています:{errorScaleList.Count}件");
            if(errorScaleListOpen)
            {
                EditorGUI.indentLevel++;
                errorScaleList.ForEach(value =>
                {
                    EditorGUILayout.LabelField(value);
                });
            } 
            
            EditorGUILayout.EndScrollView();
        }
        
        
        
        private void UiRegulationCheck(UiRegulationType type)
        {
            
            var settingPath = "Assets/Pjfb/Editor/UiRegulation/UiRegulationSetting.asset";
            UiRegulationSetting setting = AssetDatabase.LoadAssetAtPath<UiRegulationSetting>(settingPath);
            var regulationData = setting.RegulationDataList.FirstOrDefault(data => data.type == type);

            if (regulationData == null)
            {
                CruFramework.Logger.LogError("UiRegulationSetting.assetが見つかりません。エンジニアに確認してください。");
                return;
            }

            regulationData.setting.ShowRoot();

            // 現在のPrefabMode の Stage を取得
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            
            // Root のオブジェクトを取得
            var root = stage.prefabContentsRoot;
            
            missingList.Clear();
            unnecessaryList.Clear();
            anchorErrorList.Clear();
            errorPositionList.Clear();
            errorScaleList.Clear();
            
            regulationData.setting.CheckRegulation(root.transform, missingList, unnecessaryList, anchorErrorList);
            
            // 必須オブジェクトの不足
            if (missingList.Count > 0)
            {
                missingList.ForEach(path => EditorGUILayout.LabelField(path));
            }

            // 余計なオブジェクの表示
            if (unnecessaryList.Count > 0)
            {
                unnecessaryList.ForEach(path => EditorGUILayout.LabelField(path));
            }
            
            // 全方位ストレッチになっていないオブジェクトの表示
            if (anchorErrorList.Count > 0)
            {
                anchorErrorList.ForEach(path => EditorGUILayout.LabelField(path));
            }

            var transformList = root.GetComponentsInChildren<RectTransform>(true);

            transformList.ForEach(obj =>
            {
                var objState = GetObjectState(obj.gameObject);
                if (IsDecimalPosition(obj.anchoredPosition,objState.lockedPosition, objState.lockedPosition) ||
                    IsOddNumberPosition(obj.anchoredPosition,objState.lockedPosition, objState.lockedPosition) ||
                    IsDecimalPosition(obj.sizeDelta,objState.lockedSizeWidth, objState.lockedSizeHeight) ||
                    IsOddNumberPosition(obj.sizeDelta,objState.lockedSizeWidth, objState.lockedSizeHeight))
                {
                    errorPositionList.Add(objState.path);
                }

                if (IsErrorScale(obj.localScale))
                {
                    errorScaleList.Add(objState.path);
                }
                
            });

        }
        
        private bool IsDecimalPosition(Vector3 v, bool lockedWidth, bool lockedHeight)
        {
            var x = IsDecimalValue(v.x) && !lockedWidth;
            var y = IsDecimalValue(v.y) && !lockedHeight;
            var z = IsDecimalValue(v.z);
            return x || y || z;
        }

        private bool IsErrorScale(Vector3 v)
        {
            var x = !Mathf.Approximately(v.x, 1f);
            var y = !Mathf.Approximately(v.y, 1f);
            var z = !Mathf.Approximately(v.z, 1f);
            return x || y || z;
        }
        
        private bool IsOddNumberPosition(Vector3 v, bool lockedWidth, bool lockedHeight)
        {
            var x = Mathf.FloorToInt(v.x) % 2 != 0 && !lockedWidth;
            var y = Mathf.FloorToInt(v.y) % 2 != 0 && !lockedHeight;
            var z = Mathf.FloorToInt(v.z) % 2 != 0;
            return x || y || z;
        }

        private bool IsDecimalValue(float value)
        {
            var fValue = value - Mathf.FloorToInt(value);
            return !Mathf.Approximately(fValue, 0);
        }

        private ObjectState GetObjectState(GameObject obj)
        {
            var path = new StringBuilder(obj.transform.name);
            var current = obj.transform.parent;
            var objState = new ObjectState();
            while (current != null)
            {
                if (current.name.Contains("(Environment)")) break;
                
                var vl = current.GetComponent<VerticalLayoutGroup>();
                if (vl != null)
                {
                    objState.lockedPosition = true;
                    if (vl.childControlHeight) objState.lockedSizeHeight = true;
                    if (vl.childControlWidth) objState.lockedSizeWidth = true;
                }
                
                var hl = current.GetComponent<HorizontalLayoutGroup>();
                if (hl != null)
                {
                    objState.lockedPosition = true;
                    if (hl.childControlHeight) objState.lockedSizeHeight = true;
                    if (hl.childControlWidth) objState.lockedSizeWidth = true;
                }
                
                path.Insert(0, current.name + "/");
                current = current.parent;
            }
            objState.path = path.ToString();
            return objState;
        }
    }
}