using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PointGetPointDetailAPIRequest : AppAPIRequestBase<PointGetPointDetailAPIPost, PointGetPointDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PointGetPointDetailAPIResponse response ) {
        }
    }
}
