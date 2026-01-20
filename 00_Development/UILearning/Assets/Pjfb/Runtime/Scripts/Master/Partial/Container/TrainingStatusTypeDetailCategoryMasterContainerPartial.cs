
namespace Pjfb.Master {

    public partial class TrainingStatusTypeDetailCategoryMasterContainer : MasterContainerBase<TrainingStatusTypeDetailCategoryMasterObject> {
        long GetDefaultKey(TrainingStatusTypeDetailCategoryMasterObject masterObject){
            return masterObject.id;
        }

        //// <summary> CategoryIdとtargetTypeでマスタを特定する </summary>
        public TrainingStatusTypeDetailCategoryMasterObject FindCategory(long categoryId, long targetType)
        {
            foreach (TrainingStatusTypeDetailCategoryMasterObject master in MasterManager.Instance.trainingStatusTypeDetailCategoryMaster.values)
            {
                // 一致するレコードで絞り込み
                if (master.detailCategoryId == categoryId && master.targetType == targetType)
                {
                    return master;
                }
            }

            // データが見つからないならエラーログを出す
            CruFramework.Logger.LogError($"Not found TrainingStatusTypeDetailCategoryMaster CategoryId {categoryId}, targetType {targetType}");
            return null;
        }
    }
}
