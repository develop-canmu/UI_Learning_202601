
namespace Pjfb.Master {

    public partial class TrainingPointStatusLevelMasterContainer : MasterContainerBase<TrainingPointStatusLevelMasterObject> {
        long GetDefaultKey(TrainingPointStatusLevelMasterObject masterObject){
            return masterObject.id;
        }
    }
}
