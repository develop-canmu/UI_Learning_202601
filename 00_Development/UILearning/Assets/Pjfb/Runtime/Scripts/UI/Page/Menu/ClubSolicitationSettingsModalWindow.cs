using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class ClubSolicitationSettingsModalWindow : ModalWindow
    {
        #region Params

        [Serializable]
        public class ToggleObject
        {
            public long type;
            public Toggle toggle;
        }

        public class WindowParams
        {
            public Action<bool> onClosed;
        }
        
        [SerializeField] private TMP_InputField appealInputField;
        [SerializeField] private TextMeshProUGUI showText;
        [SerializeField] private TextMeshProUGUI limitCountText;
        [SerializeField] private List<ToggleObject> invitationToggleList;  //受け付けない=0,受け付ける=1
        [SerializeField] private List<ToggleObject> clubRankToggleList;  //指定なし=0,C=3は最小値
        [SerializeField] private List<ToggleObject> playStyleToggleList;  //指定なし=0,まったり=1,ガチンコ=3,わいわい=5
        [SerializeField] private List<ToggleObject> battleTypeToggleList;  //指定なし=0,積極参加=1,１日１戦以上 =2,リアル優先=3
        [SerializeField] private List<ToggleObject> participationPriorityToggleList;
        [SerializeField] private int minimumClubRank = 3; //3 => Cランクは最小値
        [SerializeField] private ClubToggle clubRankObj;
        [SerializeField] private Transform clubRankOptionParent;
        [SerializeField] private ClubToggle participationPriorityObj;
        [SerializeField] private Transform participationPriorityOptionParent;
        [SerializeField] private UIButton applyButton;
        
        private WindowParams _windowParams;
        private bool _isUpdate = false;

        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubSolicitationSettings, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            _isUpdate = false;
            Init();
            return base.OnPreOpen(args, token);
        }
        private void Init()
        {
            var userData = UserDataManager.Instance.user;
            OnInputFieldValueChanged(userData.guildInvitationMessage);
            SetClubRankToggleList();
            SetParticipationPriorityToggleList();
            invitationToggleList.ForEach(t=> t.toggle.SetIsOnWithoutNotify(t.type == Convert.ToInt32(userData.allowsGuildInvitation)));
            clubRankToggleList.ForEach(t=> t.toggle.SetIsOnWithoutNotify(t.type == userData.guildInvitationGuildRank));
            playStyleToggleList.ForEach(t=> t.toggle.SetIsOnWithoutNotify(t.type == userData.guildInvitationPlayStyleType));
            battleTypeToggleList.ForEach(t=> t.toggle.SetIsOnWithoutNotify(t.type == userData.guildInvitationGuildBattleType));
            participationPriorityToggleList.ForEach(t=> t.toggle.SetIsOnWithoutNotify(t.type == userData.guildParticipationPriority));
            OnEditValue();
            appealInputField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, appealInputField.characterLimit,appealInputField.fontAsset);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: ()=>{
                _windowParams.onClosed?.Invoke(_isUpdate);
            });
        }
        
        public void OnInputFieldValueChanged(string input)
        {
            appealInputField.SetTextWithoutNotify(input);
            showText.text = input;
            limitCountText.text = $"{input?.Length ?? 0}/{appealInputField.characterLimit}";
        }

        public void OnInputFieldEndEdit(string input)
        {
            string message = StringUtility.GetLimitNumCharacter(input,appealInputField.characterLimit); 
            appealInputField.SetTextWithoutNotify(message);
            showText.text = message;
            limitCountText.text = $"{message.Length}/{appealInputField.characterLimit}";
            OnEditValue();
        }
        public void OnEditValue()
        {
            applyButton.interactable = IsSettingChanged();
        }

        public async void OnClickApply()
        {
            await SaveOptions();
            _isUpdate = true;
            OnClickClose();
        }
        
        public void OnClickInputField()
        {
            appealInputField.ActivateInputField();
        }

        #endregion

        #region API
        private async UniTask UpdateUserInvitationAPI(bool allowsGuildInvitation,long guildInvitationGuildRank,long guildInvitationPlayStyleType,long guildInvitationGuildBattleType,string guildInvitationMessage, long participationPriorityType)
        {
            GuildUpdateUserInvitationAPIRequest request = new GuildUpdateUserInvitationAPIRequest();
            GuildUpdateUserInvitationAPIPost post = new GuildUpdateUserInvitationAPIPost
            {
                allowsGuildInvitation = allowsGuildInvitation,
                guildInvitationGuildRank = guildInvitationGuildRank,
                guildInvitationPlayStyleType = guildInvitationPlayStyleType,
                guildInvitationGuildBattleType = guildInvitationGuildBattleType,
                guildInvitationMessage = guildInvitationMessage,
                participationPriorityType = participationPriorityType
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }


        #endregion
        
        #region Other

        private bool IsSettingChanged()
        {
            var userData = UserDataManager.Instance.user;
            bool equal = appealInputField.text == userData.guildInvitationMessage &&
                         invitationToggleList.First(t=>t.toggle.isOn).type == Convert.ToInt32(userData.allowsGuildInvitation) &&
                         clubRankToggleList.First(t=> t.toggle.isOn).type == userData.guildInvitationGuildRank &&
                         playStyleToggleList.First(t=> t.toggle.isOn).type == userData.guildInvitationPlayStyleType &&
                         battleTypeToggleList.First(t=> t.toggle.isOn).type == userData.guildInvitationGuildBattleType &&
                         participationPriorityToggleList.First(t=> t.toggle.isOn).type == userData.guildParticipationPriority;
            return !equal;
        }

        private async UniTask SaveOptions()
        {
            bool allowsGuildInvitation = invitationToggleList.First(t => t.toggle.isOn).type == 1;
            long guildInvitationGuildRank = clubRankToggleList.First(t => t.toggle.isOn).type;
            long guildInvitationPlayStyleType= playStyleToggleList.First(t => t.toggle.isOn).type;
            long guildInvitationGuildBattleType= battleTypeToggleList.First(t => t.toggle.isOn).type;
            long participationPriorityType = participationPriorityToggleList.First(t => t.toggle.isOn).type;
            await UpdateUserInvitationAPI(allowsGuildInvitation,guildInvitationGuildRank,guildInvitationPlayStyleType,guildInvitationGuildBattleType,appealInputField.text, participationPriorityType);
            UserDataManager.Instance.user.UpdateUserInvitationSetting(allowsGuildInvitation,guildInvitationGuildRank,guildInvitationPlayStyleType,guildInvitationGuildBattleType,appealInputField.text, participationPriorityType);
        }
        
        private void SetClubRankToggleList()
        {
            if (!clubRankToggleList.Any())
            {
                string rankFormat = StringValueAssetLoader.Instance["menu.club.upper_rank"];
                //マスタデータでオプション追加
                var rankOptions = Master.MasterManager.Instance.guildRankMaster.values.Where(v => v.id >= minimumClubRank).OrderByDescending(v=>v.id);
                foreach (var option in rankOptions)
                {
                    var obj = Instantiate(clubRankObj, clubRankOptionParent);
                    obj.toggleObj.type = option.id;
                    obj.label.text = string.Format(rankFormat,option.name);
                    clubRankToggleList.Add(obj.toggleObj);
                }
                var lastObj = Instantiate(clubRankObj, clubRankOptionParent);
                lastObj.label.text = StringValueAssetLoader.Instance["club.noneConditions"];
                clubRankToggleList.Add(lastObj.toggleObj);
                clubRankToggleList.ForEach(obj => obj.toggle.gameObject.SetActive(true));
            }
        }
        
        private void SetParticipationPriorityToggleList()
        {
            if (!participationPriorityToggleList.Any())
            {
                foreach (ConfGuildSearchParticipationPriorityData option in ConfigManager.Instance.guildSearchParticipationPriorityTypeList.OrderByDescending(t => t.priority))
                {
                    ClubToggle obj = Instantiate(participationPriorityObj, participationPriorityOptionParent);
                    obj.toggleObj.type = option.id;
                    obj.label.text = option.name;
                    obj.toggleObj.toggle.gameObject.SetActive(true);
                    participationPriorityToggleList.Add(obj.toggleObj);
                }
            }
        }

        #endregion
    }
}