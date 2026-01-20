using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CombinationGetListOpenedAPIRequest : AppAPIRequestBase<CombinationGetListOpenedAPIPost, CombinationGetListOpenedAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CombinationGetListOpenedAPIResponse response ) {
        }
    }
}
