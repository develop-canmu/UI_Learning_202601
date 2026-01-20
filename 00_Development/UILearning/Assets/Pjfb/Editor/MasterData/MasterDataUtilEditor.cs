using UnityEditor;
using Pjfb.Master;
using Pjfb.Storage;
namespace Pjfb
{
    public class MasterDataUtilEditor
    {
        [MenuItem("Pjfb/Master/Delete LocalData", false, 100)]
        public static void DeleteMaster() {
            var path = MasterManager.CreateDataDirectoryPath();
            Pjfb.Storage.IO.DeleteDirectory(path);
            LocalSaveManager.saveData.masterVersion = 0;
            LocalSaveManager.Instance.SaveData();
        }
        
    }
}