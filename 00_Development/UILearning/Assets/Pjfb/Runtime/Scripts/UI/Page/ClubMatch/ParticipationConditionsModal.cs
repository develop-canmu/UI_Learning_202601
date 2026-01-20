using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class ParticipationConditionsModal : ModalWindow
    {
        public class Params
        {
            public string title { get; set; }
            public string message { get; set; }
            public ColosseumEventMasterObject colosseumEventMaster { get; set; }
        }

        [SerializeField] 
        private TextMeshProUGUI titleText = null;
        [SerializeField]
        private TextMeshProUGUI messageText = null;
        
        [SerializeField]
        private GameObject searchPlayerButton = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Params param = (Params)args;
            titleText.text = param.title;
            messageText.text = param.message;
            searchPlayerButton.SetActive(param.colosseumEventMaster.clientHandlingType != ColosseumClientHandlingType.InstantTournament);
            return base.OnPreOpen(args, token);
        }

        public void OnClickFindMemberButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, true, new ClubPage.Param{firstPageType = ClubPageType.FindMember});
            Close();
        }
        
        public void OnClickCloseButton()
        {
            Close();
        }
    }
}