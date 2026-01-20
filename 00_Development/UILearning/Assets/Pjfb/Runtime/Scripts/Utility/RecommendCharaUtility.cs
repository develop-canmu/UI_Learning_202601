using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Club;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb
{
    public class RecommendCharaData
    {
        public SwipeableParams<CharacterVariableDetailData> SwipeableParams { get; }
        public CharacterVariableDetailData CharacterVariableDetailData { get; }
        public long UMasterId { get; }
        public string UserName { get; }
        public string ClubName { get; }
        public ClubAccessLevel ClubAccessLevel { get; }
        public bool WantShowClubName { get; }
        public bool WantShowClubAccessLevel { get; }

        public RecommendCharaData(
            CharaVariableRecommendStatus apiData, SwipeableParams<CharacterVariableDetailData> swipeableParams,
            bool wantShowClubName, bool wantShowClubAccessLevel
            )
        {
            UMasterId = apiData.uMasterId;
            UserName = apiData.userName;
            ClubName = apiData.guildName;
            ClubAccessLevel = (ClubAccessLevel)apiData.guildRole;

            CharacterVariableDetailData = new CharacterVariableDetailData(apiData);
            SwipeableParams = swipeableParams;
            WantShowClubName = wantShowClubName;
            WantShowClubAccessLevel = wantShowClubAccessLevel;
        }

        public bool IsMe() => UMasterId == UserDataManager.Instance.user.uMasterId;
    }

    public static class RecommendCharaUtility
    {
        public static async UniTask<CharaVariableGetRecommendCharaListAPIResponse> GetRecommendCharaList()
        {
            var request = new CharaVariableGetRecommendCharaListAPIRequest();
            await APIManager.Instance.Connect(request);

            return request.GetResponseData();
        }
        
        public static IReadOnlyList<RecommendCharaData> CreateScrollDataList(CharaVariableRecommendStatus[] apiDataList, bool wantShowClubName, bool wantShowClubAccessLevel)
        {
            var characterVariableDetailDataList = apiDataList.Select(item => new CharacterVariableDetailData(item)).ToList();
            var scrollDataList = apiDataList.Select((value, index) =>
            {
                var swipeablePrams = new SwipeableParams<CharacterVariableDetailData>(characterVariableDetailDataList, index);
                return new RecommendCharaData(value, swipeablePrams, wantShowClubName, wantShowClubAccessLevel);
            });

            return scrollDataList.ToList();
        }
    }
}
