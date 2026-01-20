using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;

namespace Pjfb.RecommendChara
{
    public enum RecommendCharaPageType
    {
        List
    }
    
    public class RecommendCharaPage : PageManager<RecommendCharaPageType>
    {
        public static IReadOnlyList<RecommendCharaData> AllDataList { get; private set; }
        public static IReadOnlyList<RecommendCharaData> ClubDataList { get; private set; }
        
        protected override string GetAddress(RecommendCharaPageType page)
        {
            return $"Prefabs/UI/Page/RecommendChara/RecommendChara{page}Page.prefab";
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 一括でデータを取得し、それぞれ作成
            CharaVariableGetRecommendCharaListAPIResponse recommendCharaList = await RecommendCharaUtility.GetRecommendCharaList();
            
            AllDataList = RecommendCharaUtility.CreateScrollDataList(recommendCharaList.allList, true, false);
            ClubDataList = RecommendCharaUtility.CreateScrollDataList(recommendCharaList.guildList, false, true);
            
            // バッチの処理
            LocalSaveManager.saveData.RecommendCharaCheckNotificationData.UpdateViewDate();
            LocalSaveManager.Instance.SaveData();
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
            
            await OpenPageAsync(RecommendCharaPageType.List, true, args, token);
        }
    }
}
