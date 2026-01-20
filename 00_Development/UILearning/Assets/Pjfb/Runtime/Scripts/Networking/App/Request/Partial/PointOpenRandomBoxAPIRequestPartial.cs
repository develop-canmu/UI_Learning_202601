using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PointOpenRandomBoxAPIRequest : AppAPIRequestBase<PointOpenRandomBoxAPIPost, PointOpenRandomBoxAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PointOpenRandomBoxAPIResponse response ) {
        }
    }
}
