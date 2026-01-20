
namespace Pjfb.Master {

    public partial class TrainingStatusTypeDetailMasterContainer : MasterContainerBase<TrainingStatusTypeDetailMasterObject> {
        long GetDefaultKey(TrainingStatusTypeDetailMasterObject masterObject){
            return masterObject.id;
        }

        // typeでの検索
        public TrainingStatusTypeDetailMasterObject FindType(long type)
        {
            foreach (TrainingStatusTypeDetailMasterObject master in MasterManager.Instance.trainingStatusTypeDetailMaster.values)
            {
                if (master.type == type)
                {
                    return master;
                }
            }

            // データが見つからないならエラーログを出す
            CruFramework.Logger.LogError($"Not found TrainingStatusTypeDetailMaster Type {type}");
            return null;
        }
    }
}
