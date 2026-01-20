using System.Linq;
using Pjfb.Community;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildGetGuildAPIRequest : AppAPIRequestBase<GuildGetGuildAPIPost, GuildGetGuildAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildGetGuildAPIResponse response ) {
            if (UserData.UserDataManager.Instance.user.gMasterId == response.guild.gMasterId)
            {
                //メンバーIDリスト更新
                var memberIdList= response.guild.guildMemberList.Select(g => g.uMasterId).ToList();
                if (CommunityManager.guildMemberIdList != memberIdList)
                {
                    CommunityManager.guildMemberIdList = memberIdList;
                    //guildCanYellCount更新
                    CommunityManager.UpdateGuildCanYellCount();
                }
            }
        }
    }
}
