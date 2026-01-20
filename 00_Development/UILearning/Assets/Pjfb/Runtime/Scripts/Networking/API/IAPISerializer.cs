using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Networking.API {
    public interface IAPISerializer<PostType, ResponseType> 
    where PostType : class, IPostData
    where ResponseType : class, IResponseData, new() {
        /// <summary>
        /// 送信可能なbyte配列への変更
        /// </summary>
        byte[] Serialize(RootPostData<PostType> data);
        byte[] SerializeWithCrypt(RootPostData<PostType> data, string key);
        
        /// <summary>
        /// byte配列をレスポンス型に変更
        /// </summary>
        RootResponseData<ResponseType> Deserialize(byte[] data);
        RootResponseData<ResponseType> DeserializeWithCrypt(byte[] data, string key);
    }
}