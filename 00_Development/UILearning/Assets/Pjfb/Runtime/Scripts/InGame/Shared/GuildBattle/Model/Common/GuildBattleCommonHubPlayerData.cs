using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonHubPlayerData
    {
        #region Properties and SerializeField
        [Key(0)]
        public long PlayerId;
        
        [IgnoreMember]
        public Guid ConnectionId;
        
        #endregion

        #region Fields
        #endregion

        #region Public Methods
        #endregion        
        
        #region Protected and Private Methods
        #endregion
    }
}