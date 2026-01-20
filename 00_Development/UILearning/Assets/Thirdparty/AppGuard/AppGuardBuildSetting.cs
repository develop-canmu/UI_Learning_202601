using System.Collections.Generic;
using DiresuUnity.Base;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace ThirdParty.AppGuard.Editor
{
    [CreateAssetMenu(fileName = "AppGuardBuildSetting", menuName = "AppGuard/BuildSettingObject", order = 99)]
    public class AppGuardBuildSetting : ScriptableObject
    {
        public enum AppGuardPlan
        {
            business,
            enterprise,
            game
        }

        [SerializeField] public Plan plan = Plan.Game;
        [SerializeField] public string appKey;
        [SerializeField] public string tmpApkFileSuffix = "_appguard";
        [SerializeField] public string protectionVersionAndroid = "1.10.0.5";
        [SerializeField] public string protectionVersionIOS = "1.3.2";
        [SerializeField] public List<string> androidSha256List = new List<string> { "7A:08:CF:F7:B9:CE:27:51:4F:B9:80:1E:57:5C:26:66:84:6A:F5:11:43:5D:DB:4B:84:0C:20:CE:E7:41:B8:12" };
    }
}