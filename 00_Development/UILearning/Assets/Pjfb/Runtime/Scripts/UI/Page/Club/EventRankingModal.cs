using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.EventRanking
{
    public class EventRankingModal : ModalWindow
    {
        public class Param {
            public long pointId;

        }

        [SerializeField]
        EventRankingModalSheetManager _sheetManager = null;

        Param _param = null;
        Pjfb.Club.ClubData _clubData = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _param = (Param)args;
            
            
            //クラブ情報取得
            var request = new GuildGetGuildAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            _clubData = new Pjfb.Club.ClubData(response.guild);

            _sheetManager.SetParam(_clubData, _param.pointId);
            await _sheetManager.OpenSheetAsync(EventRankingModalSheetType.Club, null);
            await base.OnPreOpen(args,token);
        }
        protected async override UniTask OnOpen(CancellationToken token){
            await base.OnOpen(token);
        }
        

        public void OnClickCloseButton()
        {
            Close();
        }

    }
}
