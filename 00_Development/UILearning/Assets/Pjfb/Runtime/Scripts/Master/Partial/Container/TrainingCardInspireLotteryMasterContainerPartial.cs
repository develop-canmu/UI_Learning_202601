
namespace Pjfb.Master {

    public partial class TrainingCardInspireLotteryMasterContainer : MasterContainerBase<TrainingCardInspireLotteryMasterObject> {
        long GetDefaultKey(TrainingCardInspireLotteryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
