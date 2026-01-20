using Pjfb.Community;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetYellListAPIRequest : AppAPIRequestBase<CommunityGetYellListAPIPost, CommunityGetYellListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetYellListAPIResponse response )
        {
            //未読エール数リセット
            CommunityManager.unViewedYellCount = 0;
            //メニューバッジ更新
            AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }
    }
}
