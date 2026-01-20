
namespace Pjfb.Master {

    public partial class DeckExtraMasterContainer : MasterContainerBase<DeckExtraMasterObject> {
        long GetDefaultKey(DeckExtraMasterObject masterObject){
            return masterObject.id;
        }
        
        /// <summary>useTypeでの検索</summary>
        public DeckExtraMasterObject FindByUseType(long useType){
            foreach( DeckExtraMasterObject data in values ){
                if( data.useType == useType ){
                    return data;
                }
            }
            
            CruFramework.Logger.LogError($"useType {useType} is not found in {masterName}");
            
            return null;
        }
    }
}
