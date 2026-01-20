using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableGetRecommendCharaListAPIRequest : AppAPIRequestBase<CharaVariableGetRecommendCharaListAPIPost, CharaVariableGetRecommendCharaListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableGetRecommendCharaListAPIResponse response ) {
        }
    }
}
