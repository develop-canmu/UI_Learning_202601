using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Pjfb.Storage;
using UnityEngine;

namespace Pjfb.Master {
    
    public abstract class MasterContainerBase<KType, VType> : IMasterContainer where VType : class, IMasterObject {
        
        abstract public string masterName{get;}
        abstract public Type valueObjectType{get;}


        public Dictionary<KType, VType>.ValueCollection values => _dataDictionary.Values;

        protected Dictionary<KType, VType> _dataDictionary = new Dictionary<KType, VType>();

        //データ更新
        public abstract void UpdateLocalData( byte[] byteArray );
        public abstract void UpdateLocalData(CryptoStream cryptoStream);
        public abstract void UpdateLocalData( IMasterValueObject json );
        public abstract void DeleteLocalData( IMasterValueObject json );
        
        //json化
        public abstract byte[] ToByteArray();
        
        // ローカルマスタへのセーブ
        public abstract void SaveLocalMaster(CryptoStream cryptoStream);

        /// <summary>
        /// マスター読み込み
        /// </summary>
        public void LoadMaster( string directoryPath, string IV, string key ){

            var fileName = Storage.IO.CreateHashFileName(masterName);
            var savePath = System.IO.Path.Combine(directoryPath, fileName);
            if( !Storage.IO.ExistsFile(savePath) ) {
                return;
            }
#if PJFB_MASTER_NONE_ENCRYPT            
            byte[] data = Storage.IO.ReadBytes(savePath);
#else       
            using CryptoStreamWrapper data = Storage.Crypt.ReadCryptoStream(savePath, IV, key);
#endif 
            UpdateLocalData(data);
        }

        /// <summary>
        /// マスター書き込み
        /// </summary>
        public void SaveMaster( string directoryPath, string IV, string key ){
            var fileName = Storage.IO.CreateHashFileName(masterName);
            var savePath = System.IO.Path.Combine(directoryPath, fileName);
            
#if PJFB_MASTER_NONE_ENCRYPT
            byte[] byteArray = ToByteArray();
            Storage.IO.WriteBytes( savePath, byteArray );
#else
            using CryptoStreamWrapper stream = Storage.Crypt.GetCryptoStreamWrite(savePath, IV, key);
            SaveLocalMaster(stream);
#endif
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        /// <returns></returns>
        public VType FindData( KType key ){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null : target master = " + masterName );
                return null;
            }

            if( !_dataDictionary.ContainsKey(key) ) {
                CruFramework.Logger.LogError( "not find data : " + key + " : target master = " + masterName );
                return null;
            }

            return _dataDictionary[key];
        }

        /// <summary>
        /// データが存在しているか
        /// </summary>
        public bool Contains( KType key ){
            return _dataDictionary.ContainsKey(key);
        }

        //データのクリア        
        protected void ClearDataDictionary(){
            _dataDictionary.Clear();
            _dataDictionary = null;
        }

    }

    public abstract class MasterContainerBase<VType> : MasterContainerBase<long,VType> where VType : class, IMasterObject {
        
    }

}