using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.App {
    
    static public class APIUtility {

        public static readonly int errorTypeOffset = 1000;

        /// <summary>
        /// ワンタイムトークンの取得
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="T"></typeparam>
        static public async UniTask<UserGenerateOneTimeTokenAPIResponse> ConnectOneTimeTokeRequest(){
            var request = new UserGenerateOneTimeTokenAPIRequest();
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        /// <summary>
        /// エラータイプ計算
        /// </summary>
        static public ErrorModalType CalcErrorType( long errorCode ){
            var errorType =  errorCode / errorTypeOffset;
            return (ErrorModalType)errorType;
        }


        /// <summary>
        /// デバイス情報の作成
        /// </summary>
        static public DeviceDeviceInfo CreateDeviceInfo(){
            var info = new DeviceDeviceInfo();
            info.deviceModel = SystemInfo.deviceModel;
            info.deviceName = SystemInfo.deviceName;
            info.deviceOs = SystemInfo.operatingSystem;
            return info;
        }

    }
}