
namespace Pjfb.Master {

    public partial class TrainingPointHandResetCostMasterContainer : MasterContainerBase<TrainingPointHandResetCostMasterObject> {
        long GetDefaultKey(TrainingPointHandResetCostMasterObject masterObject){
            return masterObject.id;
        }

        /// <summary> 引き直しが有効か </summary>
        public bool IsEnableHandReload(long trainingPointId)
        {
            foreach (var master in values)
            {
                // 引き直しに利用するポイントIdが一致するか
                if (master.mTrainingPointHandResetCostGroup == trainingPointId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
