using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.UserData {
    
    public class UserDataCharaPiece {
        public long charaId{get;private set;}
		public long value{get;private set;}
        public UserDataCharaPiece( long charaId, long value ){
            this.charaId = charaId;
            this.value = value;
        }
    }


    public class UserDataCharaPieceContainer : UserDataContainer<long, UserDataCharaPiece> {
        
        /// <summary>
        /// データ更新
        /// </summary>
        public void Update( NativeApiCharaPiece[] charaPieces ){
            if( charaPieces == null ) {
               return; 
            } 
            
            foreach ( var charaPiece in charaPieces) {
                var userChara = new UserDataCharaPiece(charaPiece.mCharaId, charaPiece.value);
                Update(userChara.charaId, userChara);
            }
            
            ForeachHandler((handler)=>{ handler.OnUpdatedData(); });
        }   
    }
}