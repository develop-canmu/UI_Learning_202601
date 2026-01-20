
namespace Pjfb.Master {

    public partial class SaleIntroductionMasterContainer : MasterContainerBase<SaleIntroductionMasterObject> {
        long GetDefaultKey(SaleIntroductionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
