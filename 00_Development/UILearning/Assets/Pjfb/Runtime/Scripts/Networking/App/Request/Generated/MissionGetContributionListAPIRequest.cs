//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドミッションを含むカテゴリグループのミッション進捗および貢献度確認

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MissionGetContributionListAPIPost : AppAPIPostBase {
		public long categoryGroupId = 0; // カテゴリグループID

   }

   [Serializable]
   public class MissionGetContributionListAPIResponse : AppAPIResponseBase {
		public MissionCategoryContribution contributionSum = null; // ミッション貢献度合計
		public MissionCategoryContribution[] categoryContributionList = null; // ミッション貢献度リスト

   }
      
   public partial class MissionGetContributionListAPIRequest : AppAPIRequestBase<MissionGetContributionListAPIPost, MissionGetContributionListAPIResponse> {
      public override string apiName{get{ return "mission/getContributionList"; } }
   }
}