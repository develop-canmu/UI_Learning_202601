using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.API {
    public interface IAPIConnecter {
        UniTask<APIResult> Connect( string baseURL, IAPIRequest request );
        void Abort( IAPIRequest request );
    }
}