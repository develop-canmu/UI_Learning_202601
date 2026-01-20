
namespace Pjfb.Master {

    public partial class PointMasterContainer : MasterContainerBase<PointMasterObject> {
        long GetDefaultKey(PointMasterObject masterObject){
            return masterObject.id;
        }

        public enum PointVisibleType
        {
            Normal = 1, // 持っていれば表示
            Display = 2, // 常に表示
            Hidden = 3, // 非表示
        }
    }
}
