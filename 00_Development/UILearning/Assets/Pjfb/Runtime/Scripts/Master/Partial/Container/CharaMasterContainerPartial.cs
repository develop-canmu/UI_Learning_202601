using System;
using System.Linq;
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class CharaMasterContainer : MasterContainerBase<CharaMasterObject> {
        /// <summary>
        /// 作成後のデータ変換
        /// </summary>
        public void RemoveMainCharaId( long mainCharaId1, long mainCharaId2 ){
            if( _dataDictionary.ContainsKey(mainCharaId1) ) {
                _dataDictionary.Remove(mainCharaId1);
            }
            
            if( _dataDictionary.ContainsKey(mainCharaId2) ) {
                _dataDictionary.Remove(mainCharaId2);
            }
        }


        long GetDefaultKey(CharaMasterObject masterObject){
            return masterObject.id;
        }

    }
}
