using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace Pjfb.Storage {

    public class LocalSaveManager : CruFramework.Utils.Singleton<LocalSaveManager> {

        string saveDataFileName
        {
            get
            {
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
                switch (AppEnvironment.CurrentEnvironment)
                {
                    case AppEnvironment.EnvironmentEnum.DEV:
                    {
                        return "saveDataDev";
                    }
                    case AppEnvironment.EnvironmentEnum.EXA:
                    {
                        return "saveDataExa";
                    }
                    case AppEnvironment.EnvironmentEnum.PLN:
                    {
                        return "saveDataPln";
                    }
                    case AppEnvironment.EnvironmentEnum.COP:
                    {
                        // コンパチテストで本番と同等にするため、揃える
                        return "saveData";
                    }
                    case AppEnvironment.EnvironmentEnum.STG:
                    {
                        return "saveDataStg";
                    }
                    case AppEnvironment.EnvironmentEnum.PROD:
                    {
                        return "saveDataRel";
                    }
                    case AppEnvironment.EnvironmentEnum.MAN:
                    {
                        return "saveDataMan";
                    }
                    case AppEnvironment.EnvironmentEnum.POL:
                    {
                        return "saveDataPol";
                    }
                }
#endif
                return "saveData";
            }
        }

        string immutableSaveDataFileName
        {
            get
            {
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
                switch (AppEnvironment.CurrentEnvironment)
                {
                    case AppEnvironment.EnvironmentEnum.DEV:
                    {
                        return "immutablesSveDataDev";
                    }
                    case AppEnvironment.EnvironmentEnum.EXA:
                    {
                        return "immutablesSveDataExa";
                    }
                    case AppEnvironment.EnvironmentEnum.PLN:
                    {
                        return "immutablesSveDataPln";
                    }
                    case AppEnvironment.EnvironmentEnum.COP:
                    {
                        // コンパチテストで本番と同等にするため、揃える
                        return "immutablesSveData";
                    }
                    case AppEnvironment.EnvironmentEnum.STG:
                    {
                        return "immutablesSveDataStg";
                    }
                    case AppEnvironment.EnvironmentEnum.PROD:
                    {
                        return "immutablesSveDataRel";
                    }
                    case AppEnvironment.EnvironmentEnum.MAN:
                    {
                        return "immutablesSveDataMan"; 
                    }
                    case AppEnvironment.EnvironmentEnum.POL:
                    {
                        return "immutablesSveDataPol"; 
                    }
                }
#endif
                return "immutablesSveData";
            }
        }

        const string saveIV = @"tQ8j9u5XJ5eD7tCZ";
        const string saveKey = @"wF7BTxfdzQ7dUnhei3ADbsqkE3hdjHRt";
        const string immutableDataIV = @"Nf6yTnxLkJ9R8dfV";
        const string immutableDataKey = @"Yr6MJCatQj6gtnXUA8ybdkxFV8kh4BSd";

        public static SaveData saveData => Instance._saveData;
        public static ImmutableSaveData immutableData => Instance._immutableData;


        SaveData _saveData = new SaveData();
        ImmutableSaveData _immutableData = new ImmutableSaveData("");

        
        protected override void Init(){
            LoadData();
            LoadImmutableData();
        }

        /// <summary>
        /// セーブデータ保存
        /// </summary>
        public void SaveData(){
            try {
                IO.WriteEncryptFileToPersistent( HashSaveFileName(), _saveData, saveIV, saveKey);
            } catch(System.Exception e) {
                CruFramework.Logger.LogError(" Save data Error : " + e.Message);
            }
        }


        /// <summary>
        /// Immutableなセーブデータ保存
        /// </summary>
        public void SaveImmutableData( ImmutableSaveData data ){
            try {
                _immutableData = data;
                IO.WriteEncryptFileToPersistent( HashImmutableSaveFileName(), _immutableData, immutableDataIV, immutableDataKey);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError(" SaveImmutableData error : " + e.Message);
                throw e;
            }
        }

        
        /// <summary>
        /// セーブデータ読み込み
        /// </summary>
        public void LoadData(){
            if( !IO.ExistsFileInPersistent(HashSaveFileName()) ) {
                _saveData = new SaveData();    
                return;
            }
            try {
                _saveData = IO.ReadEncryptFileFromPersistent<SaveData>(HashSaveFileName(), saveIV, saveKey);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError(" LoadData error : " + e.Message);
                _saveData = new SaveData();
            }
            
        }

        /// <summary>
        /// Immutableなセーブデータ読み込み
        /// </summary>
        public void LoadImmutableData(){
            if( !IO.ExistsFileInPersistent(HashImmutableSaveFileName()) ) {
                _immutableData = new ImmutableSaveData("");   
                return; 
            }
            try {
                _immutableData = IO.ReadEncryptFileFromPersistent<ImmutableSaveData>(HashImmutableSaveFileName(), immutableDataIV, immutableDataKey);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError(" LoadImmutableData error : " + e.Message);
                throw e;
            }
            
        }

        /// <summary>
        /// セーブデータ削除
        /// </summary>
        public void DeleteData(){
            IO.DeletePersistentFile(HashSaveFileName());
            _saveData = new SaveData();
            // PlayerPrefsも一緒に削除
            ObscuredPrefs.DeleteAll();
        }

        /// <summary>
        /// Immutableなセーブデータ削除
        /// </summary>
        public void DeleteImmutableData(){
            IO.DeletePersistentFile(HashImmutableSaveFileName());
            _immutableData = new ImmutableSaveData("");
        }

        string HashSaveFileName( ){
            return IO.CreateHashFileName(saveDataFileName);
        }

        string HashImmutableSaveFileName(){
            return IO.CreateHashFileName(immutableSaveDataFileName);
        }
    }
}