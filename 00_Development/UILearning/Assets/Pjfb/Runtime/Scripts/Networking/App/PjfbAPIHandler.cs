using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.API;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.App {
    
    public class PjfbAPIHandler : IAPIHandler
    {
        
#if UNITY_EDITOR
        protected  virtual void OnBeginConnect(IAPIRequest request)
        {
        }
        
        protected  virtual void OnFinishConnect(IAPIRequest request, bool isError)
        {
        }
#endif
        
        /// <summary>
        /// 接続開始
        /// </summary>
        void IAPIHandler.OnBeginConnect( IAPIRequest request ){
            // タッチガードとローディング表示
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            AppManager.Instance.UIManager.System.Loading.Show();
            
#if UNITY_EDITOR
            OnBeginConnect(request);
#endif

#if PJFB_DEV
            // APILogger.WriteLog(request);
#endif                            
        }

        /// <summary>
        /// 接続終了
        /// </summary>
        void IAPIHandler.OnFinishConnect( IAPIRequest request, bool isError ){
            // タッチガードとローディングを非表示
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
#if UNITY_EDITOR
            OnFinishConnect(request, isError);
#endif
        }
    }
}