using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public enum PlayerEnhanceType
    {
        // トレーニング編成強化
        TrainingDeckEnhance = 1001,
    }

    public static class PlayerEnhanceManager
    {
        private static Dictionary<long, PlayerEnhanceData> playerEnhanceList = new Dictionary<long, PlayerEnhanceData>();

        //// <summary> プレイヤー強化状況を取得する </summary>
        public static async UniTask GetEnhanceDataAPI()
        {
            // すでに取得しているならリターン
            if (playerEnhanceList != null)
            {
                return;
            }
            
            var request = new PlayerGetEnhanceListAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();

            // プレイヤー強化状況を更新
            UpdateEnhanceData(response.playerEnhanceList);
        }

        //// <summary> 指定のプレイヤー強化データを返す  </summary>
        public static PlayerEnhanceData GetPlayerEnhance(PlayerEnhanceType type)
        {
            return playerEnhanceList[(long)type];
        }

        //// <summary> キャッシュの削除 </summary>
        public static void ClearCache()
        {
            playerEnhanceList.Clear();
        }

        //// <summary> 強化情報の更新 </summary>
        public static void UpdateEnhanceData(PlayerEnhanceData enhanceData)
        {
            PlayerEnhanceMasterObject master = MasterManager.Instance.playerEnhanceMaster.FindData(enhanceData.mPlayerEnhanceId);

            // データがあれば更新
            if (playerEnhanceList.ContainsKey(master.contentType))
            {
                playerEnhanceList[master.contentType] = enhanceData;
            }
            // なければ追加
            else
            {
                playerEnhanceList.Add(master.contentType, enhanceData);
            }
        }

        //// <summary> プレイヤー強化状況更新 </summary>
        public static void UpdateEnhanceData(PlayerEnhanceData[] enhanceDataList)
        {
            foreach (var enhanceData in enhanceDataList)
            {
                UpdateEnhanceData(enhanceData);
            }
        }
    }
}