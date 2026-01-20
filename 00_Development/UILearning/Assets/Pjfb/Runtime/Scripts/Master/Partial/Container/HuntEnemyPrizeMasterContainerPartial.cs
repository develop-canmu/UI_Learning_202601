
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    public partial class HuntEnemyPrizeMasterContainer : MasterContainerBase<HuntEnemyPrizeMasterObject> {
        long GetDefaultKey(HuntEnemyPrizeMasterObject masterObject){
            return masterObject.id;
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        /// <returns></returns>
        public HuntEnemyPrizeMasterObject FindDataWithType(int mHuntEnemyId, int type){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.FirstOrDefault(item => item.mHuntEnemyId == mHuntEnemyId && item.type == type);
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HuntEnemyPrizeMasterObject> FindGeneralRewardList(long mHuntId, long difficulty, long rarity){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.Where(item => 
                item.mHuntId == mHuntId &&
                item.difficulty == difficulty && 
                item.rarity == rarity && 
                item.type >= (int) HuntEnemyPrizeMasterObject.Type.General &&
                item.battleResult == 1);
        }
        
        public IEnumerable<HuntEnemyPrizeMasterObject> FindEventRewardList(long mHuntId, long difficulty, long rarity){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.Where(item => 
                item.mHuntId == mHuntId &&
                item.difficulty == difficulty && 
                item.rarity == rarity && 
                (item.type == (int) HuntEnemyPrizeMasterObject.Type.FirstTime || item.type >= (int) HuntEnemyPrizeMasterObject.Type.General) &&
                item.battleResult == 1);
        }
        
        public IEnumerable<HuntEnemyPrizeMasterObject> FindChoiceReward(long mHuntId, long difficulty, long rarity, long choiceNumber){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.Where(item => 
                item.mHuntId == mHuntId &&
                item.difficulty == difficulty && 
                item.rarity == rarity && 
                (item.choiceNumber == choiceNumber || item.choiceNumber == 0) && 
                item.type == (int)HuntEnemyPrizeMasterObject.Type.Choice);
        }
    }
}
