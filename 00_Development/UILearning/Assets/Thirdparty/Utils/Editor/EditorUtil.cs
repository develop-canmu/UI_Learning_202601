using System;
using System.IO;
using System.Linq;


// ReSharper disable once CheckNamespace
namespace ThirdParty.Utils.Editor
{
    public class EditorUtil
    {
        public static string GetMonoScriptPath(Type type)
        {
            var guidList = UnityEditor.AssetDatabase.FindAssets($"{type.Name} t:monoscript");
            if (guidList.Length == 0)
            {
                return string.Empty;
            }

            var path = string.Empty;
            foreach (var guid in guidList)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(assetPath);
                var monoClass = asset.GetClass();
                if (monoClass == null)
                {
                    continue;
                }

                var monoNameSpace = monoClass.Namespace;
                var monoClassName = monoClass.Name;
                var monoFullName = !string.IsNullOrEmpty(monoNameSpace) ? $"{monoNameSpace}.{monoClassName}" : monoClassName;
                if (monoFullName != type.FullName)
                {
                    continue;
                }

                path = assetPath;
                break;
            }

            return path;
        }

        public static void CreateDirectoryRecursively(string path, char splitChar)
        {
            if (UnityEditor.AssetDatabase.IsValidFolder(path)) return;

            if (path[path.Length - 1] == splitChar)
            {
                path = path.Substring(0, path.Length - 1);
            }

            var names = path.Split(splitChar);
            for (var i = 1; i < names.Length; i++)
            {
                var target = string.Join(splitChar.ToString(), names.Take(i + 1).ToArray());
                if (!UnityEditor.AssetDatabase.IsValidFolder(target))
                {
                    var parent = string.Join(splitChar.ToString(), names.Take(i).ToArray());
                    var folder = names[i];
                    UnityEditor.AssetDatabase.CreateFolder(parent, folder);
                }
            }
        }

        public static void WriteAsset(UnityEngine.Object target, string path)
        {
            var dir = Path.GetDirectoryName(path);
            CreateDirectoryRecursively(dir, Path.AltDirectorySeparatorChar);
            CreateDirectoryRecursively(dir, Path.DirectorySeparatorChar);

            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
            {
                UnityEditor.AssetDatabase.CreateAsset(target, path);
            }

            UnityEditor.EditorUtility.SetDirty(target);

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
        }

        public static T ReadAsset<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static void UpdateAsset(UnityEngine.Object target)
        {
            UnityEditor.EditorUtility.SetDirty(target);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public static void DeleteAsset(string path)
        {
            UnityEditor.AssetDatabase.DeleteAsset(path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public static void WriteText(string text, string path, bool isOverWrite = false)
        {
            var fi = new FileInfo(path);
            if (fi.Exists && !isOverWrite)
            {
                return;
            }

            var sw = fi.CreateText();
            sw.Write(text); // 未改行
            sw.Flush();
            sw.Close();
        }

        public static string ReadText(string path)
        {
            var fi = new FileInfo(path);
            if (!fi.Exists)
            {
                return null;
            }

            var sr = new StreamReader(fi.OpenRead());
            var returnString = "";
            while (sr.Peek() != -1)
            {
                returnString += sr.ReadLine() + "\n";
            }

            sr.Close();
            return returnString;
        }
    }
}