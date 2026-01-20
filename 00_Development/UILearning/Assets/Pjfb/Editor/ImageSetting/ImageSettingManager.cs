using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pjfb.Editor.ImageSetting
{
    [CreateAssetMenu(menuName = "Pjfb/ImageSetting/Create setting manager", fileName = nameof(ImageSettingManager))]
    public class ImageSettingManager : ScriptableObject
    {
        public List<ImageSettingData> settingDataList = new ();

        [MenuItem("Pjfb/ImageSettingManager")]
        public static void OnClickMenu()
        {
            var path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(CreateInstance<ImageSettingManager>()));
            var selectObject = AssetDatabase.LoadAssetAtPath<Object>(path.Replace(".cs", ".asset"));
            Selection.objects = new [] {selectObject};
        }
    }

    [Serializable]
    public class ImageSettingData
    {
        public string name = "_";
        public TextureFormat format;
        public DefaultAsset targetFolder;
    }

    [Serializable]
    public class TextureFormat
    {
        public TextureImporterType TextureImporterType = TextureImporterType.Sprite;
        public int MaxSize = 2048; // 圧縮サイズ
        public TextureImporterFormat TextureImporterFormat = TextureImporterFormat.ASTC_6x6; // 圧縮形式
        public bool AlphaIsTransparency = false;
        public bool ReadWriteAccess = false;
        public SpriteMeshType SpriteMeshType = SpriteMeshType.FullRect;
        public bool IsStrictSpriteMode = true;
        public SpriteImportMode SpriteImportMode = SpriteImportMode.Single;
    }

    [CustomEditor(typeof(ImageSettingManager))]
    public class ImageSettingEditor : UnityEditor.Editor
    {
        private SerializedProperty settingDataList;

        private void Awake()
        {
            settingDataList = serializedObject.FindProperty(nameof(ImageSettingManager.settingDataList));
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ImageSettingManager)target), typeof(ImageSettingManager), false);
            GUI.enabled = true;
            if (GUILayout.Button("Apply Settings")) OnClickApplyButton();
            EditorGUILayout.PropertyField(settingDataList, true);
            if (GUILayout.Button("Save")) OnClickSaveButton();
        }

        private void OnClickSaveButton()
        {
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void OnClickApplyButton()
        {
            ApplyImageSetting(target);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ApplyImageSetting(Object target)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(ImageSettingManager)}.{nameof(OnClickApplyButton)}\n");
            var data = target as ImageSettingManager;

            // 差分を監視
            bool isChangedFiles = false;
            
            try
            {
                // アセットデータベースの割り込みを一時停止
                AssetDatabase.StartAssetEditing();
                
                // 該当するディレクトリの条件を適用。必要に応じてスキップ
                foreach (ImageSettingData aSetting in data.settingDataList)
                {
                    string settingFolderPath = GetAssetPath(aSetting.targetFolder);
                    var assetGuids = AssetDatabase.FindAssets("t:texture2D", new[] { settingFolderPath });
                    int count = assetGuids.Length;
                    for (int index = 0; index < assetGuids.Length; index++)
                    {
                        // 割り込みがあったか
                        bool isInterrupt = EditorUtility.DisplayCancelableProgressBar($"Applying Texture Format", $"[{aSetting.name}] {index} / {count}", index / (float)count);

                        if (isInterrupt)
                        {
                            EditorUtility.ClearProgressBar();
                            return;
                        }

                        string guid = assetGuids[index];
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        // アセットのフォルダパス
                        string searchAssetPath = Path.GetDirectoryName(path)?.Replace("\\", "/");
                        
                        // より深い階層で上書き設定がないか確認
                        /*
                         * searchPath : Remote/Images/
                         * contains: Remote/Images/AbilityImages/
                         */
                        if (HasOtherSettingOverride(data.settingDataList, aSetting, searchAssetPath))
                        {
                            continue;
                        }
                        
                        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (textureImporter != null)
                        { 
                            bool changed = SetTextureImporterFormat(textureImporter, aSetting.format); 
                            if (changed) 
                            { 
                                sb.AppendLine($"> {path}\n"); 
                            }

                            isChangedFiles |= changed;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                // 何かあれば
                CruFramework.Logger.Log(exception);
            }
            finally
            {
                // AssetDatabaseを戻す
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }
            
            CruFramework.Logger.Log(sb.ToString());

            if (isChangedFiles == false)
            {
                CruFramework.Logger.Log("差分はありませんでした");
                return;
            }
            
            // ログをファイルに書き出して保存するダイアログを表示
            string logContent = sb.ToString();
            string defaultName = $"{nameof(ImageSettingManager)}_ApplyLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string savePath = EditorUtility.SaveFilePanel("差分リストを保存", Application.dataPath, defaultName, "txt");
            if (!string.IsNullOrEmpty(savePath))
            {
                File.WriteAllText(savePath, logContent, Encoding.UTF8);
                EditorUtility.RevealInFinder(savePath);
            }
        }
        
        public static bool SetTextureImporterFormat(TextureImporter ti, TextureFormat format)
        {
            bool dirty = false;
            
            TextureImporterSettings textureSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(textureSettings);
            
            if(textureSettings.textureType != format.TextureImporterType)
            {
                textureSettings.textureType = format.TextureImporterType;
                dirty = true;
            }
            if(textureSettings.mipmapEnabled)
            {
                textureSettings.mipmapEnabled = false;
                dirty = true;
            }
            if(textureSettings.alphaIsTransparency != format.AlphaIsTransparency)
            {
                textureSettings.alphaIsTransparency = format.AlphaIsTransparency;
                dirty = true;
            }
            if(textureSettings.readable != format.ReadWriteAccess)
            {
                textureSettings.readable = format.ReadWriteAccess;
                dirty = true;
            }
            if(textureSettings.spriteMeshType != format.SpriteMeshType)
            {
                textureSettings.spriteMeshType = format.SpriteMeshType;
                dirty = true;
            }
            // SpriteModeを強制する設定の時のみ
            if(format.IsStrictSpriteMode && textureSettings.spriteMode != (int)format.SpriteImportMode)
            {
                textureSettings.spriteMode = (int)format.SpriteImportMode;
                dirty = true;
            }
            
            
            var androidPlatformSettings = ti.GetPlatformTextureSettings("Android");
            if(!androidPlatformSettings.overridden)
            {
                androidPlatformSettings.overridden = true;
                dirty = true;
            }
            if(androidPlatformSettings.name != "Android")
            {
                androidPlatformSettings.name = "Android";
                dirty = true;
            }
            if(androidPlatformSettings.format != format.TextureImporterFormat)
            {
                androidPlatformSettings.format = format.TextureImporterFormat;
                dirty = true;
            }
            if(androidPlatformSettings.maxTextureSize != format.MaxSize)
            {
                androidPlatformSettings.maxTextureSize = format.MaxSize;
                dirty = true;
            }
            
            var iosPlatformSettings = ti.GetPlatformTextureSettings("iPhone");
            if(!iosPlatformSettings.overridden)
            {
                iosPlatformSettings.overridden = true;
                dirty = true;
            }
            if(iosPlatformSettings.name != "iOS" && iosPlatformSettings.name != "iPhone")
            {
                iosPlatformSettings.name = "iOS";
                dirty = true;
            }
            if(iosPlatformSettings.format != format.TextureImporterFormat)
            {
                iosPlatformSettings.format = format.TextureImporterFormat;
                dirty = true;
            }
            if(iosPlatformSettings.maxTextureSize != format.MaxSize)
            {
                iosPlatformSettings.maxTextureSize = format.MaxSize;
                dirty = true;
            }
            
            if(dirty)
            {
                ti.SetTextureSettings(textureSettings);
                ti.SetPlatformTextureSettings(androidPlatformSettings);
                ti.SetPlatformTextureSettings(iosPlatformSettings);
                ti.SaveAndReimport();
            }
            
            return dirty;
        }

        private static string GetMetaFilePath(Object obj)
        {
            return GetMetaFilePath(GetAssetPath(obj));
        }

        private static string GetMetaFilePath(string assetPath)
        {
            return AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
        }

        private static string GetAssetPath(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj.GetInstanceID());
        }

        private static string ReadFileToEnd(string path)
        {
            using var settingStreamReader = new StreamReader(path);
            var retVal = settingStreamReader.ReadToEnd();
            settingStreamReader.Close();
            settingStreamReader.Dispose();

            return retVal;
        }

        private static string GetGuid(string metaContent)
        {
            const string guidFormat = "guid: ";
            const int guidCount = 32;

            var guidStartIndex = metaContent.IndexOf(guidFormat, StringComparison.Ordinal) + guidFormat.Length;
            return metaContent.Substring(guidStartIndex, guidCount);
        }

        /// <summary> より下層で上書きしているPathがないか </summary>
        private static bool HasOtherSettingOverride(List<ImageSettingData> settingList, ImageSettingData currentSetting, string searchAssetPath)
        {
            // 比較対象の設定の深さ
            int currentSettingDepth = GetAssetPath(currentSetting.targetFolder).Split("/").Length;
            
            string pathWithSeparator = $"{searchAssetPath}/";
            // 他の設定で上書きがないか見て回る
            foreach (ImageSettingData data in settingList)
            {
                string dataAssetPath = GetAssetPath(data.targetFolder);

                // ディレクトリの深さを計算
                int dataDepth = dataAssetPath.Split("/").Length;
                
                // より下層で一致するパスがある。ディレクトリ自体は完全一致させたいのでセパレータ込みで確認
                if (pathWithSeparator.StartsWith($"{dataAssetPath}/") && currentSettingDepth < dataDepth)
                {
                    return true;
                }
            }

            return false;
        }
    }
    
#if !DISABLE_IMAGE_IMPORTER
    public class ImageImporter : AssetPostprocessor
    {
        private class ImageSettingDepthData
        {
            public int Depth { get; }
            public ImageSettingData Data { get; }

            public ImageSettingDepthData(int depth, ImageSettingData data)
            {
                Depth = depth;
                Data = data;
            }
        }
        
        private static ImageSettingManager setting = null;
#if UNITY_6000_0_58
        private static Queue<string> reimportQueue = new();
#endif
        
        private void OnPreprocessTexture()
        {
            TextureImporter ti = assetImporter as TextureImporter;
            if (ti == null) return;
            if (setting == default)
            {
                var findAssets = AssetDatabase.FindAssets($"{nameof(ImageSettingManager)}");
                var path = "";
                foreach (var guid in findAssets)
                {
                    path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.ToLower().EndsWith(".asset"))
                    {
                        setting = AssetDatabase.LoadAssetAtPath<ImageSettingManager>(path);
                        break;
                    }
                }
            }

            if (setting == default) return;
            
#if UNITY_6000_0_58
            // 初回インポート時(metaファイルがない時)のみ、再インポートをかけてmetaファイルを更新する
            if (ti.importSettingsMissing)
            {
                ImageImporter.reimportQueue.Enqueue(ti.assetPath);
            }
#endif
            string assetPath = ti.assetPath;

            ImageSettingDepthData currentSetting = null;
            int currentDepth = 0;
            
            foreach (ImageSettingData data in setting.settingDataList)
            {
                // 一致するものを控えて、splitしてdepthを算出し、一番深いものを優先して適用する
                string targetFolderPath = AssetDatabase.GetAssetPath(data.targetFolder);
                
                // 部分一致を避けるためフォルダパスの区切りを入れる
                string targetPathWithSeparator = $"{targetFolderPath}/";

                // アセットパスがターゲットフォルダ内に存在しない
                if (!assetPath.StartsWith(targetPathWithSeparator)) continue;
                
                // パスの深度を計算（'/'の数で判定）
                int depth = targetFolderPath.Split('/').Length;
                
                // 今の設定
                ImageSettingDepthData setting = new ImageSettingDepthData(depth, data);
                // 選択中の設定より深い階層がある
                if (depth > currentDepth)
                {
                    currentDepth = depth;
                    currentSetting = setting;
                }
            }

            // フォルダに設定があるとき
            if (currentDepth > 0)
            {
                // 最終的に最も深い階層の設定を適用
                ImageSettingEditor.SetTextureImporterFormat(ti, currentSetting.Data.format);
            }
        }
        
#if UNITY_6000_0_58
        // バージョン固有バグの回避処理。一応次乗り換える時には直っている想定。。
        // https://issuetracker-mig.prd.it.unity3d.com/issues/namefileidtable-entry-changes-in-the-meta-file-when-reimporting-a-texture-while-the-default-behaviour-mode-is-set-to-2d
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // テクスチャだけReimportかける
            for (int i = 0; i < reimportQueue.Count; i++)
            {
                string path = reimportQueue.Dequeue();
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset is Texture2D)
                {
                    // 再インポートをかけてmetaファイルを更新する
                    AssetDatabase.ImportAsset(path);
                }
            }
        }
#endif
        
    }
#endif
}