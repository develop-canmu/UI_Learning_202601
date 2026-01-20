
namespace Pjfb.Master {

    public partial class RankingClientPreviewMasterContainer : MasterContainerBase<RankingClientPreviewMasterObject> {
        long GetDefaultKey(RankingClientPreviewMasterObject masterObject){
            return masterObject.id;
        }
    }
}
