using Pjfb.Gacha;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaGetListAPIRequest : AppAPIRequestBase<GachaGetListAPIPost, GachaGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaGetListAPIResponse response ) {
        }
    }
}
