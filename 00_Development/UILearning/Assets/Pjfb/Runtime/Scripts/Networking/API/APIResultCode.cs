using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.HTTP;

namespace Pjfb.Networking.API {
    
    public enum APIResultCode : int{
        None,

        Success,    //成功
        Cancel, //キャンセル
        TimeOut,      //エラー
        NoneConnectNetwork, //ネットワークに未接続  

        APIError, //APIのエラー
        HTTPError, //HTTP Error
        UnknownError, //原因不明な Error
    }


}