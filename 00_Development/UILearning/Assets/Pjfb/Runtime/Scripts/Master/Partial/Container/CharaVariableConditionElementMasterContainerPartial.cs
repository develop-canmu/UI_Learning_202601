
namespace Pjfb.Master {

    public partial class CharaVariableConditionElementMasterContainer : MasterContainerBase<CharaVariableConditionElementMasterObject> {
        long GetDefaultKey(CharaVariableConditionElementMasterObject masterObject){
            return masterObject.id;
        }
        
        
        public CharaVariableConditionElementMasterObject FinDataByCharaVariableConditionId(long id)
        {
            foreach(CharaVariableConditionElementMasterObject value in values)
            {
                if(value.mCharaVariableConditionId == id)return value;
            }
            return null;
        }
    }
}
