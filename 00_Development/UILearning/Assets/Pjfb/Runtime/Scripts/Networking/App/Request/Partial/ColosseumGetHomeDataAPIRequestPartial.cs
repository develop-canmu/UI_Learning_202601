using Pjfb.Networking.API;
using Pjfb.UserData;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetHomeDataAPIRequest : AppAPIRequestBase<ColosseumGetHomeDataAPIPost, ColosseumGetHomeDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetHomeDataAPIResponse response ) {
            UserDataManager.Instance.UpdateColosseumSeasonDataList(response.colosseum);
        }
    }
}
