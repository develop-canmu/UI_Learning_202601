using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.API {
    public interface IAPIHandler {

        /// <summary>
        /// 接続開始
        /// </summary>
        void OnBeginConnect( IAPIRequest request );

        /// <summary>
        /// 接続終了
        /// </summary>
        void OnFinishConnect( IAPIRequest request, bool isError );
    }
}