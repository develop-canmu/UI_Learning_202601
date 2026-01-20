
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Pjfb.Networking.HTTP {

    public enum HTTPResult : int{
        None,

        Success,    //成功
        Error,      //エラー
        Abort,    //キャンセル
        TimeOut,      //エラー
        NoneConnectNetwork, //ネットワークに未接続  
    }
}