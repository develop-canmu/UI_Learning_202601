
namespace Pjfb.Master {

    public partial class DeckFormatUseMasterContainer : MasterContainerBase<DeckFormatUseMasterObject> {
        long GetDefaultKey(DeckFormatUseMasterObject masterObject){
            return masterObject.id;
        }
        
        public DeckFormatUseMasterObject FindData(DeckType type)
        {
            foreach(DeckFormatUseMasterObject value in values)
            {
                if(value.useType == (int)type)return value;
            }
            return null;
        }
    }
}
