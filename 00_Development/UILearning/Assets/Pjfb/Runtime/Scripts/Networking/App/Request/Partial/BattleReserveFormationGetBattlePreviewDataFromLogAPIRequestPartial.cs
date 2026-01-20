using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationGetBattlePreviewDataFromLogAPIRequest : AppAPIRequestBase<BattleReserveFormationGetBattlePreviewDataFromLogAPIPost, BattleReserveFormationGetBattlePreviewDataFromLogAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationGetBattlePreviewDataFromLogAPIResponse response ) {
        }
    }
}
