//
// This file is auto-generated
//

using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using MessagePack;

namespace Pjfb.Master {

    public partial class DailyMissionMasterContainer : MasterContainerBase<DailyMissionMasterObject> {

        [MessagePackObject]
        public class SaveClass{
            [Key(0)]
            public List<DailyMissionMasterValueObject> dataList = null;
        }


        override public string masterName => "DailyMission";   
        override public System.Type valueObjectType => typeof(DailyMissionMasterValueObject);   


        public override void UpdateLocalData(byte[] byteArray){
            var saveInstance = MessagePackSerializer.Deserialize<SaveClass>(byteArray);
            foreach( var data in saveInstance.dataList ){
                UpdateLocalData(data);
            }
        }
        
        public override void UpdateLocalData(CryptoStream cryptoStream){
            var saveInstance = MessagePackSerializer.Deserialize<SaveClass>(cryptoStream);
            foreach( var data in saveInstance.dataList ){
                UpdateLocalData(data);
            }
        }  

        public override void UpdateLocalData(IMasterValueObject vo){
            var masterValueObject = vo as DailyMissionMasterValueObject;
            if( masterValueObject == null ) {
                CruFramework.Logger.LogError( "update data type error : " + vo + " to " + typeof(DailyMissionMasterValueObject) );
                return;
            }
            var masterObject = new DailyMissionMasterObject(masterValueObject);
            _dataDictionary[GetDefaultKey(masterObject)] = masterObject;
        }

        public override void DeleteLocalData(IMasterValueObject vo){
            var masterValueObject = vo as DailyMissionMasterValueObject;
            if( masterValueObject == null ) {
                CruFramework.Logger.LogError( "update data type error : " + vo + " to " + typeof(DailyMissionMasterValueObject) );
                return;
            }
            var masterObject = new DailyMissionMasterObject(masterValueObject);
            var key = GetDefaultKey(masterObject);
            if( _dataDictionary.ContainsKey(key) ) {
                _dataDictionary.Remove(key);
            }
        }

        public override byte[] ToByteArray(){
            var saveInstance = new SaveClass();
            var dataList = new List<DailyMissionMasterValueObject>();
            foreach( var value in values ){
                dataList.Add( value.rawData );
            }
            saveInstance.dataList = dataList;
            
            byte[] byteArray = MessagePackSerializer.Serialize(saveInstance);
            return byteArray;
        }

        public override void SaveLocalMaster(CryptoStream cryptoStream){
            var saveInstance = new SaveClass();
            var dataList = new List<DailyMissionMasterValueObject>();
            foreach( var value in values ){
                dataList.Add( value.rawData );
            }
            saveInstance.dataList = dataList;
                       
            MessagePackSerializer.Serialize(cryptoStream, saveInstance);
        }
       
    }
}
