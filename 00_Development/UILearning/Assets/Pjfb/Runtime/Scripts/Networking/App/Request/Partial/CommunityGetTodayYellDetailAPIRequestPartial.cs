using Pjfb.Community;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetTodayYellDetailAPIRequest : AppAPIRequestBase<CommunityGetTodayYellDetailAPIPost, CommunityGetTodayYellDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetTodayYellDetailAPIResponse response )
        {
            //エール情報の更新
            CommunityManager.yellDetail = response;
            CommunityManager.yellCount = response.yellCount;
            //メニューバッジ更新
            AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }
    }
}
