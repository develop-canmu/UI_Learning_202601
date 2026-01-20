
namespace Pjfb.Master {

    public partial class SystemLockMasterContainer : MasterContainerBase<SystemLockMasterObject> {
        long GetDefaultKey(SystemLockMasterObject masterObject){
            return masterObject.id;
        }
        
        public SystemLockMasterObject FindDataBySystemNumber(long systemNumber)
        {
            foreach(SystemLockMasterObject v in values)
            {
                if(v.systemNumber == systemNumber)return v;
            }
            return null;
        }

        /// <summary>二つを比較して速く開放されるほうを渡す</summary>
        public SystemLockMasterObject GetFastReleaseSystemLock(long systemNumberA, long systemNumberB)
        {
            SystemLockMasterObject systemLockA = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(systemNumberA);
            SystemLockMasterObject systemLockB = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(systemNumberB);
            if (systemLockA.triggerType == systemLockB.triggerType)
            {
                return systemLockA.triggerValue <= systemLockB.triggerValue ? systemLockA : systemLockB;
            }
            return null;
        }
    }
}
