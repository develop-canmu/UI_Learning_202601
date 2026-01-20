using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugRequirementLevelNothingAPIRequest : AppAPIRequestBase<DebugRequirementLevelNothingAPIPost, DebugRequirementLevelNothingAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugRequirementLevelNothingAPIResponse response ) {
        }
    }
}
