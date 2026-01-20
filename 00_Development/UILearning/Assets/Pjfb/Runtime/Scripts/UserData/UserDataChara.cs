using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.UserData {
    
    public partial class UserDataChara {
        public long id{get;private set;}
		public long masterId{get;private set;}
		public long charaSkinId{get;private set;}
		public long combatPower{get;private set;}
		public long charaId{get;private set;}
		public long level{get;private set;}
		public long newLiberationLevel{get;private set;}

        public UserDataChara( CharaV2Base chara ){
            this.id = chara.id;
            this.masterId = chara.uMasterId;
            this.charaSkinId = chara.mCharaSkinId;
            this.combatPower = chara.combatPower;
            this.charaId = chara.mCharaId;
            this.level = chara.level;
            this.newLiberationLevel = chara.newLiberationLevel;
        }
    }


    public class UserDataCharaContainer : UserDataContainer<long, UserDataChara> {
        
        
        /// <summary>
        /// データ更新
        /// </summary>
        public void Update( CharaV2Base[] charas ){
            if( charas == null ) {
               return; 
            } 
            
            foreach ( var chara in charas) {
                var userChara = new UserDataChara(chara);
                Update(userChara.id, userChara);
            }

            ForeachHandler((handler)=>{ handler.OnUpdatedData(); });
        }

        /// <summary>
        /// mCharaIdをキー、UserDataCharaをバリューとして、データ更新
        /// </summary>
        public void UpdateByMCharaId(CharaV2Base[] charas)
        {
            if (charas == null)
            {
                return;
            }

            foreach (var chara in charas)
            {
                var userDataChara = new UserDataChara(chara);
                
                // サポート器具はmCharaIdの重複が発生するためスキップ
                if (userDataChara.CardType == CardType.SupportEquipment)
                {
                    CruFramework.Logger.LogWarning($"mCharaId:{userDataChara.charaId} はサポート器具だったためスキップ");
                    continue;
                }
                
                // mCharaIdをキーにして更新
                Update(userDataChara.charaId, userDataChara);
            }
        }

        public bool HaveCharacterWithMasterCharaId( long masterCharaId ){
            return data.Any(itr => itr.Value.charaId == masterCharaId );
        }
    }
}