using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class StaminaGetListAPIRequest : AppAPIRequestBase<StaminaGetListAPIPost, StaminaGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( StaminaGetListAPIResponse response ) {
        }
    }
}
