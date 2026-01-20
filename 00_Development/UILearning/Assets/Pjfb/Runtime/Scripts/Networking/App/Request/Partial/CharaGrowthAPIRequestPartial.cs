using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaGrowthAPIRequest : AppAPIRequestBase<CharaGrowthAPIPost, CharaGrowthAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaGrowthAPIResponse response ) {
        }
    }
}
