using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Utility;
using TMPro;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace Pjfb
{
    //// <summary> マテリアルインスタンスを作成しプロパティを変更する </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteAlways]
    public class TextMeshFontMaterialPropertySetter : MonoBehaviour
    {
        // インスタンスの元になるマテリアル
        [SerializeField]
        [HideInInspector]
        private Material baseMaterialPreset = null;

        // 変更されたマテリアルのプロパティ
        [SerializeField] 
        [HideInInspector]
        private List<MaterialPropertyInfo> changePropertyList = new List<MaterialPropertyInfo>();
        
        // 動的に生成されたマテリアルインスタンス
        private Material materialInstance = null;

        private TextMeshProUGUI tmp = null;

        // マテリアルプロパティの変更はUnityEditor実行時のみに限定
        private void OnEnable()
        {
#if UNITY_EDITOR
            // フォントマテリアルが変更された際に呼ばれるイベントの登録
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT.Add(FontMaterialPropertyChanged);
#endif
            
            if (tmp == null)
            {
                tmp = GetComponent<TextMeshProUGUI>();
            }

            // マテリアルインスタンス割り当て時に元のセットされていたMaterialPreset情報が上書きされるのでこっちで保存しておく
            if (baseMaterialPreset == null)
            {
                baseMaterialPreset = tmp.fontSharedMaterial;
            }

            // マテリアルのインスタンスが生成されてないなら取得
            if (materialInstance == null)
            {
                // 内部で動的にインスタンスマテリアルを生成している
                materialInstance = new Material(baseMaterialPreset);
                materialInstance.shaderKeywords = baseMaterialPreset.shaderKeywords;
                materialInstance.name += " (Instance)";
                // プロパティIdのセット
                ShaderUtilities.GetShaderPropertyIDs();
                SetMaterialProperty();
            }
            
            tmp.fontSharedMaterial = materialInstance;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            // イベント削除
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT.Remove(FontMaterialPropertyChanged);
#endif
            
            if (tmp == null) return;
            // 元のマテリアルPresetにセットする
            tmp.fontSharedMaterial = baseMaterialPreset;
        }

#if UNITY_EDITOR
        private void Update()
        {
            // セットされたマテリアルが変更されたかどうか(マテリアルインスタンスの場合は無視する)
            if (baseMaterialPreset == tmp.fontSharedMaterial || materialInstance == tmp.fontSharedMaterial)
            {
                return;
            }

            // 新しくセットされたマテリアルを保存
            baseMaterialPreset = tmp.fontSharedMaterial;
            ReleaseMaterialInstance();
            materialInstance = tmp.fontMaterial;
            changePropertyList.Clear();
        }
#endif
        
        //// <summary> 変更されたマテリアルプロパティのセット </summary>
        private void SetMaterialProperty()
        {
            foreach (MaterialPropertyInfo propertyInfo in changePropertyList)
            {
                materialInstance.SetMaterialProperty(propertyInfo);
            }
        }

#if UNITY_EDITOR
        //// <summary> フォントマテリアルのプロパティ変更時のコールバック処理 </summary>
        private void FontMaterialPropertyChanged(bool isChanged, Material mat)
        {
            if (isChanged == false) return;

            // 変更されたマテリアルが違うならリターン
            if (mat != materialInstance)
            {
                return;
            }

            // 変更されたマテリアルプロパティを取得
            changePropertyList = MaterialUtility.CompareMaterialProperty(baseMaterialPreset, materialInstance, TMPMaterialPropertyUtility.exceptPropertyList);
            
            // キーワードの変更差分
            HashSet<string> diffKeyword = new HashSet<string>();
            
            foreach (string keyword in baseMaterialPreset.shaderKeywords)
            {
                if (mat.shaderKeywords.Contains(keyword) == false)
                {
                    diffKeyword.Add(keyword);
                }
            }
            foreach (string keyword in mat.shaderKeywords)
            {
                if (baseMaterialPreset.shaderKeywords.Contains(keyword) == false)
                {
                    diffKeyword.Add(keyword);
                }
            }

            // キーワードが変更されているならエラーを出す
            if (diffKeyword.Count > 0)
            {
                // シェーダーキーワードの変更は動的に変更できないパラメータがあるのでマテリアルプリセットを変更して対応する想定
                CruFramework.Logger.LogError("シェーダーキーワードが元のマテリアルから変更されています\n 変更する場合は変更したいマテリアルプリセットを適用してから編集してください");
                CruFramework.Logger.LogError($"変更されたシェーダーキーワード:{string.Join(",", diffKeyword)}");
            }
            
            EditorUtility.SetDirty(this);
        }
#endif

        //// <summary> 作成したマテリアルインスタンスを破棄 </summary>
        private void ReleaseMaterialInstance()
        {
            if (materialInstance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(materialInstance);
                }
                else
                {
                    DestroyImmediate(materialInstance);
                }

                // 明示的にNull代入
                materialInstance = null;
            }
        }

        private void OnDestroy()
        {
            // 生成されたマテリアルのインスタンスはオブジェクトが破棄されても残るので削除する
            // UnityEngine.Objectを継承しているクラスはアンマネージドリソースなので
            ReleaseMaterialInstance();
        }
    }
}