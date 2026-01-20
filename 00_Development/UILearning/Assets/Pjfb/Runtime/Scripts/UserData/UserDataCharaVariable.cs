using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.UserData {
    
    public partial class UserDataCharaVariable {
        public long id{get;private set;}
		public long masterId{get;private set;}
		public long charaId{get;private set;}
		public string visualKey{get;private set;}
		public long hp{get;private set;}
		public long mp{get;private set;}
		public long atk{get;private set;}
		public long def{get;private set;}
		public long spd{get;private set;}
		public long tec{get;private set;}
		public long param1{get;private set;}
		public long param2{get;private set;}
		public long param3{get;private set;}
		public long param4{get;private set;}
		public long param5{get;private set;}
		public long combatPower{get;private set;}
		public long rank{get;private set;}
		public bool isLocked{get;private set;}
		public WrapperIntList[] abilityList{get;private set;}
		public TrainingSupport[] supportDetailJson{get;private set;}
		public CombinationOpenedMinimum[] comboBuffList{get;private set;}
		public long mTrainingScenarioId{get;private set;}

        public UserDataCharaVariable( CharaVariableMinimum chara ){
            this.id = chara.id;
            this.masterId = chara.uMasterId;
            this.charaId = chara.mCharaId;
            this.visualKey = chara.visualKey;
            this.hp = chara.hp;
            this.mp = chara.mp;
            this.atk = chara.atk;
            this.def = chara.def;
            this.spd = chara.spd;
            this.tec = chara.tec;
            this.param1 = chara.param1;
            this.param2 = chara.param2;
            this.param3 = chara.param3;
            this.param4 = chara.param4;
            this.param5 = chara.param5;
            this.combatPower = chara.combatPower;
            this.rank = chara.rank;
            this.isLocked = chara.isLocked;
            this.abilityList = chara.abilityList;
            this.supportDetailJson = chara.supportDetailJson;
            this.comboBuffList = chara.comboBuffList;
            this.mTrainingScenarioId = chara.mTrainingScenarioId;
        }
    }


    public class UserDataCharaVariableContainer : UserDataContainer<long, UserDataCharaVariable> {
        
        /// <summary>
        /// データ更新
        /// </summary>
        public void Update( CharaVariableMinimum[] charaVariables ){
            if( charaVariables == null ) {
               return; 
            } 
            
            foreach ( var chara in charaVariables) {
                var userChara = new UserDataCharaVariable(chara);
                Update(userChara.id, userChara);
            }
        }   

        /// <summary>
        /// データ削除
        /// </summary>
        public void Remove(long[] ids) {
            foreach ( var id in ids) {
                Remove(id);
            }
        }
    }
}
