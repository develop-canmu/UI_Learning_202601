using UnityEngine;

namespace Pjfb.Networking.API {
    public class JsonSerializer<PostType, ResponseType> : IAPISerializer<PostType, ResponseType>
    where PostType : class, IPostData
    where ResponseType : class, IResponseData, new() {
        
        /// <summary>
        /// 送信可能なbyte配列への変更
        /// </summary>
        public byte[] Serialize(RootPostData<PostType> data) {
            var json = JsonUtility.ToJson(data);       
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        public byte[] SerializeWithCrypt(RootPostData<PostType> data, string key) {
                var guid = System.Guid.NewGuid();
                var seedByte = System.Text.Encoding.UTF8.GetBytes(guid.ToString());
                var iv = System.BitConverter.ToString( Pjfb.Storage.Crypt.Sha256Hash(seedByte)).ToLower().Replace("-","").Substring(0,16);

                var dataJson = JsonUtility.ToJson(data);       
                
                var cryptRoot = new RootEncryptPostData();
                var cryptData = Pjfb.Storage.Crypt.Encrypt(dataJson, iv, key);
                cryptRoot.iv = iv;
                cryptRoot.data = System.Convert.ToBase64String(cryptData);
                var postJson = JsonUtility.ToJson(cryptRoot);       

                return System.Text.Encoding.UTF8.GetBytes(postJson);

        }

        
        /// <summary>
        /// 送信可能なbyte配列への変更
        /// </summary>
        public RootResponseData<ResponseType> Deserialize(byte[] data){
            var json = System.Text.Encoding.UTF8.GetString(data);
            var response = JsonUtility.FromJson<RootResponseData<ResponseType>>(json);
            return response;
        }

        public RootResponseData<ResponseType> DeserializeWithCrypt(byte[] data, string key){
            var rawJson = System.Text.Encoding.UTF8.GetString(data);
            var cryptRoot = JsonUtility.FromJson<RootEncryptResponseData>(rawJson);    
            var cryptBytes = System.Convert.FromBase64String(cryptRoot.data.data);   
            var decryptJson = Pjfb.Storage.Crypt.Decrypt(cryptBytes, cryptRoot.data.iv, key);
            ResponseType response = null;
            // レスポンスが空の場合があるためチェックする
            if( string.IsNullOrEmpty(decryptJson) ||  decryptJson != "[]" ){
                response = JsonUtility.FromJson<ResponseType>(decryptJson);
            } else {
                response = new ResponseType();
            }
            var rootResponse = new RootResponseData<ResponseType>();
            rootResponse.SerRootResponseData(cryptRoot);
            rootResponse.data = response;
            return rootResponse;
        }
      
    }
}