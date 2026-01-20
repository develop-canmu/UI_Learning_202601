using System;

namespace Pjfb.Master {
    public interface IMasterContainer {
        //マスタ名
        string masterName{get;}
        Type valueObjectType{get;}

        //マスタ読み込み
        void LoadMaster( string directoryPath, string IV, string key );
        //マスタ保存
        void SaveMaster( string directoryPath, string IV, string key );

        //ローカルデータの更新
        public void UpdateLocalData( byte[] byteArray );
        public void UpdateLocalData( IMasterValueObject json );
        public void DeleteLocalData( IMasterValueObject json );
    }
}