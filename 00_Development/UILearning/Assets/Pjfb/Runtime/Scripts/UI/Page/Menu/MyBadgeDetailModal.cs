using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class MyBadgeDetailModal : ModalWindow
    {
        public class MyBadgeDetailModalParam
        {
            public long BadgeId { get; }
            public MyBadgeDetailModalParam(long badgeId)
            {
                BadgeId = badgeId;
            }
        }
        
        [SerializeField]
        private MyBadgeImage badgeImage = null;
        [SerializeField]
        private TextMeshProUGUI badgeName = null;
        [SerializeField]
        private TextMeshProUGUI badgeDescription = null;
        
        private MyBadgeDetailModalParam param = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (MyBadgeDetailModalParam) args;

            EmblemMasterObject master = MasterManager.Instance.emblemMaster.FindData(param.BadgeId);
            
            // 画像設定
            await badgeImage.SetTextureAsync(param.BadgeId);
            
            // テキスト設定
            badgeName.text = master.name;
            badgeDescription.text = master.description;
            
            await base.OnPreOpen(args, token);
        }
    }
}