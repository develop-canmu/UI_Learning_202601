using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.API {
    public class DefaultAPIHandler : IAPIHandler {

        /// <summary>
        /// 接続開始
        /// </summary>
        void IAPIHandler.OnBeginConnect( IAPIRequest request ){
        }

        /// <summary>
        /// 接続終了
        /// </summary>
        void IAPIHandler.OnFinishConnect( IAPIRequest request, bool isError ){
        }
    }
}