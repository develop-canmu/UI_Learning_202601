using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pjfb
{
    public static class AppEnvironment
    {
        public enum EnvironmentEnum
        {
            DEV = 1,
            PLN = 2,
            EXA = 3,
            COP = 4,
            STG = 5,
            PROD = 6,
            MAN = 7,
            POL = 8,
        }
        
#if (CRUFRAMEWORK_DEBUG && !PJFB_REL) || UNITY_EDITOR
        
        // Editorではプロジェクトフォルダ下に保存(Editorだと保存先が同プロジェクトだと共通になるので)
#if UNITY_EDITOR
        private static readonly string EnvSavePath = "PjfbTools/EnvSaveData";
#else 
        private static readonly string EnvSavePath = $"{Application.persistentDataPath}/EnvSaveData"; 
#endif
        
        static AppEnvironment()
        {
            // ファイルがないなら無視
            if (File.Exists(EnvSavePath) == false)
            {
                return;
            }

            CurrentEnvironment = JsonUtility.FromJson<EnvironmentEnum>(File.ReadAllText(EnvSavePath));
        }

        /// <summary> 現在の環境を保存 </summary>
        public static void SaveCurrentEnv(EnvironmentEnum saveEnv)
        {
            string directory = Path.GetDirectoryName(EnvSavePath);
            // まだディレクトリがないなら作成
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(EnvSavePath, JsonUtility.ToJson(saveEnv));
        }
        
#endif
        
        public static bool IsReview { get; set; } = false;
        
        /// <summary>BaseURL</summary>
        private static string AssetURL
        {
            get
            {
#if PJFB_REL
                if (IsReview)
                {
                    return "https://preview.cfn.pjfb.gochipon.net";
                }
                else
                {
                    return "https://cfn.bluelock-pwc.jp";
                }
#else
                switch (CurrentEnvironment)
                {
                    case EnvironmentEnum.DEV:
                    default:
                    {
                        return "https://a.pjfb.gochipon.net";
                    }
                    case EnvironmentEnum.EXA:
                    {
                        return "https://exam-a.check.pjfb.gochipon.net";
                    }
                    case EnvironmentEnum.PLN:
                    {
                        return "https://plan-a.check.pjfb.gochipon.net";
                    }
                    case EnvironmentEnum.COP:
                    {
                        return "https://compati-a.check.pjfb.gochipon.net";
                    }
                    case EnvironmentEnum.STG:
                    {
                        return "https://staging-a.check.pjfb.gochipon.net";
                    }
                    case EnvironmentEnum.MAN:
                    {
                        return "https://manual-a.check.pjfb.gochipon.net";  
                    }
                    case EnvironmentEnum.POL:
                    {
                        return "https://poly-a.check.pjfb.gochipon.net";
                    }
                }
#endif
            }
        }

        // 現在の環境
        private static EnvironmentEnum currentEnvironment = BaseEnvironment;
        
        public static EnvironmentEnum CurrentEnvironment
        {
            get
            {
                return currentEnvironment;          
            }
#if (CRUFRAMEWORK_DEBUG && !PJFB_REL) || UNITY_EDITOR
            set
            {
                // 本番環境にはセット出来ないようにする
                if (value == EnvironmentEnum.PROD)
                {
                    return;
                }

                currentEnvironment = value;
            }
#endif
        }

        //// <summary> シンボルで定義されている環境 </summary>
        public static EnvironmentEnum BaseEnvironment
        {
            get
            {
#if PJFB_MAN
                return EnvironmentEnum.MAN;  
#elif PJFB_STG
                return EnvironmentEnum.STG;
#elif PJFB_COP
                return EnvironmentEnum.COP;
#elif PJFB_PLN
                return EnvironmentEnum.PLN;
#elif PJFB_EXA
                return EnvironmentEnum.EXA;
#elif PJFB_DEV
                return EnvironmentEnum.DEV;
#elif PJFB_REL
                return EnvironmentEnum.PROD;
#elif PJFB_POL
                return EnvironmentEnum.POL;
#else
                return EnvironmentEnum.DEV;
#endif
            }
        }
        
        public static string VersionDownloadURL
        {
            get
            {
#if PJFB_REL
                return "https://bluelock-pwc.jp/native-api/";
#else
                switch (CurrentEnvironment)
                {
                    case EnvironmentEnum.DEV:
                    default:
                    {
                        return "https://check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.EXA:
                    {
                        return "https://exam.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.PLN:
                    {
                        return "https://plan.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.COP:
                    {
                        return "https://compati.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.STG:
                    {
                        return "https://staging.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.MAN:
                    {
                        return "https://manual.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.POL:
                    {
                        return "https://poly.check.pjfb.gochipon.net/native-api/";
                    }
                }
#endif
            }
        }

        public static string APIURL
        {
            get
            {
#if PJFB_REL
                return "https://bluelock-pwc.jp/native-api/";
#else
                switch (CurrentEnvironment)
                {
                    case EnvironmentEnum.DEV:
                    default:
                    {
                        return "https://check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.EXA:
                    {
                        return "https://exam.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.PLN:
                    {
                        return "https://plan.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.COP:
                    {
                        return "https://compati.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.STG:
                    {
                        return "https://staging.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.MAN:
                    {
                        return "https://manual.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.POL:
                    {
                        return "https://poly.check.pjfb.gochipon.net/native-api/";
                    }
                }
#endif
            }
        }
        
        public static string APIReviewURL
        {
            get
            {
#if PJFB_REL
                return "https://preview.check.pjfb.gochipon.net/native-api/";
#else
                switch (CurrentEnvironment)
                {
                    case EnvironmentEnum.DEV:
                    default:
                    {
                        return "https://check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.EXA:
                    {
                        return "https://exam.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.PLN:
                    {
                        return "https://plan.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.COP:
                    {
                        return "https://compati.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.STG:
                    {
                        return "https://staging.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.MAN:
                    {
                        return "https://manual.check.pjfb.gochipon.net/native-api/";
                    }
                    case EnvironmentEnum.POL:
                    {
                        return "https://poly.check.pjfb.gochipon.net/native-api/";
                    }
                }
#endif
            }
        }

        /// <summary>アセットバンドルURL</summary>
        public static string AssetBundleURL
        {
            get { return AssetURL + "/asset"; }
        }
        
        /// <summary>画像サーバーURL</summary>
        public static string AssetBrowserURL
        {
            get { return AssetURL + "/asset_browser"; }
        }
        
        /// <summary>ヘルプ</summary>
        public static string HelpJsonURL
        {
            get { return AssetURL + "/json/help/help_list.json"; }
        }
        
        /// <summary>Google Play Store Url</summary>
        public static string GooglePlayUrl 
        {
            get { return "market://launch?id=jp.pjfb"; }
        }
        
        public static string AddressableCacheDirectory
        {
            get{ return $"{Application.persistentDataPath}/AssetBundles"; }
        }
        
        public static string AddressableCatalogCacheDirectory
        {
            get{ return $"{Application.persistentDataPath}/com.unity.addressables"; }
        }
    }
}
