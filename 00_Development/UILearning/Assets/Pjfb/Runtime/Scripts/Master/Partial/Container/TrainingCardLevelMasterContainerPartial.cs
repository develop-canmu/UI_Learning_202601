
namespace Pjfb.Master {

    public partial class TrainingCardLevelMasterContainer : MasterContainerBase<TrainingCardLevelMasterObject> {
        long GetDefaultKey(TrainingCardLevelMasterObject masterObject){
            return masterObject.id;
        }
        
        public TrainingCardLevelMasterObject FindData(long trainingCardId, long level)
        {
            foreach(var value in values)
            {
				if(value.mTrainingCardId == trainingCardId && value.level == level)
                {
                    return value;
                }
            }
            return null;
        }
    }
}
