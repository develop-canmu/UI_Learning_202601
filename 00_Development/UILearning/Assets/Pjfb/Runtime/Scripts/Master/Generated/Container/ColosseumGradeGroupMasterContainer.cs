//
// This file is auto-generated
//

using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using MessagePack;

namespace Pjfb.Master {

    public partial class ColosseumGradeGroupMasterContainer : MasterContainerBase<ColosseumGradeGroupMasterObject> {

        [MessagePackObject]
        public class SaveClass{
            [Key(0)]
            public List<ColosseumGradeGroupMasterValueObject> dataList = null;
        }


        override public string masterName => "ColosseumGradeGroup";   
        override public System.Type valueObjectType => typeof(ColosseumGradeGroupMasterValueObject);   


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
            var masterValueObject = vo as ColosseumGradeGroupMasterValueObject;
            if( masterValueObject == null ) {
                CruFramework.Logger.LogError( "update data type error : " + vo + " to " + typeof(ColosseumGradeGroupMasterValueObject) );
                return;
            }
            var masterObject = new ColosseumGradeGroupMasterObject(masterValueObject);
            _dataDictionary[GetDefaultKey(masterObject)] = masterObject;
        }

        public override void DeleteLocalData(IMasterValueObject vo){
            var masterValueObject = vo as ColosseumGradeGroupMasterValueObject;
            if( masterValueObject == null ) {
                CruFramework.Logger.LogError( "update data type error : " + vo + " to " + typeof(ColosseumGradeGroupMasterValueObject) );
                return;
            }
            var masterObject = new ColosseumGradeGroupMasterObject(masterValueObject);
            var key = GetDefaultKey(masterObject);
            if( _dataDictionary.ContainsKey(key) ) {
                _dataDictionary.Remove(key);
            }
        }

        public override byte[] ToByteArray(){
            var saveInstance = new SaveClass();
            var dataList = new List<ColosseumGradeGroupMasterValueObject>();
            foreach( var value in values ){
                dataList.Add( value.rawData );
            }
            saveInstance.dataList = dataList;
            
            byte[] byteArray = MessagePackSerializer.Serialize(saveInstance);
            return byteArray;
        }

        public override void SaveLocalMaster(CryptoStream cryptoStream){
            var saveInstance = new SaveClass();
            var dataList = new List<ColosseumGradeGroupMasterValueObject>();
            foreach( var value in values ){
                dataList.Add( value.rawData );
            }
            saveInstance.dataList = dataList;
                       
            MessagePackSerializer.Serialize(cryptoStream, saveInstance);
        }
       
    }
}
