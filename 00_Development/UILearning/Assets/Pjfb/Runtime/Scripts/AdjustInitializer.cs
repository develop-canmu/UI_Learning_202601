using System;
using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using UnityEngine;

namespace Pjfb
{
    public class AdjustInitializer : MonoBehaviour
    {
        [SerializeField] private Adjust adjust;

        public void InitializeAdjust()
        {
#if PJFB_REL
            adjust.environment = AdjustEnvironment.Production;
#else 
            adjust.environment = AdjustEnvironment.Sandbox;

#endif
            var appToken = "gejqe0tpzz7k";
            
            var adjustConfig = new AdjustConfig(appToken, adjust.environment);
            
            // ここらへんの設定は不要と思われる.
            // バックグラウンドで進めさせて〜的な仕組み(時間経過とか)が特にないため.
            // adjustConfig.setSendInBackground(adjust.sendInBackground);
            // 起動フローでのデータ取得とかもあるはずなので, 使うことはないと思われる.
            // adjustConfig.setLaunchDeferredDeeplink(adjust.launchDeferredDeeplink);
            
            adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
            adjustConfig.setEventBufferingEnabled(adjust.eventBuffering);
#if UNITY_IOS
            adjustConfig.setAppSecret(1, 1216819274, 1603073183, 373570760, 2311773);
#elif UNITY_ANDROID
            adjustConfig.setAppSecret(2, 317080720, 521808215, 1697329050, 1513850301);
#endif

            // ATTポップアップ
            Adjust.requestTrackingAuthorizationWithCompletionHandler((status) =>
            {
                CruFramework.Logger.Log("Adjust ATT : " + status);
            });
            
            Adjust.start(adjustConfig);
        }
    }
}