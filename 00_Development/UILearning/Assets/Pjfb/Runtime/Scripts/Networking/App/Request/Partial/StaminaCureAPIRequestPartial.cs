using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class StaminaCureAPIRequest : AppAPIRequestBase<StaminaCureAPIPost, StaminaCureAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( StaminaCureAPIResponse response ) {
        }
    }
}
