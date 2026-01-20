using UnityEditor;
using Pjfb.Storage;
using UnityEngine;

namespace Pjfb
{
    public class LocalSaveEditor : EditorWindow
    {
        [MenuItem("Pjfb/LocalSave/Delete Data", false, 100)]
        public static void Delete() {
            
            if(EditorUtility.DisplayDialog("データ削除", "データを削除しますか？", "Yes", "No"))
            {
                LocalSaveManager.Instance.DeleteImmutableData();
                LocalSaveManager.Instance.DeleteData();
            }
        } 
        
        [MenuItem("Pjfb/LocalSave/Delete PlayerPrefs Data", false, 101)]
        public static void DeleteSaveData() {
            
            if(EditorUtility.DisplayDialog("PlayerPrefs削除", "PlayerPrefsを全て削除しますか？", "Yes", "No"))
            {
                LocalSaveManager.Instance.DeleteData();
            }
        } 
    }
}