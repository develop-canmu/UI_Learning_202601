using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Club
{
    public class ClubFindMemberSettingModal : ModalWindow
    {
        public class EditData {
        
            public BigValue powerFrom{get;set;} = BigValue.Zero;
            public BigValue powerTo{get;set;} = BigValue.Zero;
            public long rank{get;set;} = 0;
            public int activityPolicy{get;set;} = 0;
            public int clubMatchPolicy{get;set;} = 0;
            public long participationPriority { get; set; } = 0;

            public bool Equals( EditData data ){
                if( this.powerFrom != data.powerFrom ) {
                    return false;
                }
                if( this.powerTo != data.powerTo ) {
                    return false;
                }
                if( this.rank != data.rank ) {
                    return false;
                }
                if( this.activityPolicy != data.activityPolicy ) {
                    return false;
                }
                if( this.clubMatchPolicy != data.clubMatchPolicy ) {
                    return false;
                }
                if( this.participationPriority != data.participationPriority ) {
                    return false;
                }
                return true;
            }
        }


        public class Param {
            public EditData editData{get;set;}  = null;
            public Action<EditData> onClickButton{get;set;} = null;
        }

        [SerializeField]
        ClubInputField _powerFrom = null;
        [SerializeField]
        ClubInputField _powerTo = null;
        [SerializeField]
        TextMeshProUGUI _powerFromDisplayText = null;
        [SerializeField]
        TextMeshProUGUI _powerToDisplayText = null;
        
        [SerializeField]
        ToggleGroup _activityPolicy = null;
        [SerializeField]
        ToggleGroup _clubMatchPolicy = null;
        [SerializeField]
        ToggleGroup _participationPriority = null;
        [SerializeField]
        UIButton _initButton = null;
        /// <summary>Cランクが最小値なので3をデフォルトに設定(ClubSolicitationSettingsModalWindow.minimumClubRankと同じ値)</summary>
        [SerializeField] 
        int minimumClubRank = 3; 
        [SerializeField]
        ClubToggle _rankToggle = null;
        [SerializeField]
        Transform _rankToggleParent = null;
        [SerializeField]
        ClubToggle _participationPriorityToggle = null;
        [SerializeField]
        Transform _participationPriorityToggleParent = null;
        
        private List<ClubToggle> _rankToggleList = new ();
        private List<ClubToggle> _participationPriorityToggleList = new ();

        private Param _param = null;

        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _param = (Param)args;
            CreateClubRankToggleList();
            CreateParticipationPriorityToggleList();
            UpdateView(_param.editData);
            await base.OnPreOpen(args, token);
            CheckButtonActive();
        }

        private void CreateClubRankToggleList()
        {
            if(_rankToggleList.Count > 0) return;
            string rankFormat = StringValueAssetLoader.Instance["menu.club.upper_rank"];
            // ランク用のトグルを追加
            foreach (GuildRankMasterObject guildRankMasterObject in MasterManager.Instance.guildRankMaster.values.Where(v => v.id >= minimumClubRank).OrderByDescending(v=>v.id))
            {
                CreateToggle(guildRankMasterObject.id, string.Format(rankFormat, guildRankMasterObject.name), _rankToggleParent, _rankToggle, _rankToggleList);
            }
            // 指定なし用のトグルを追加
            CreateToggle(0, StringValueAssetLoader.Instance["club.noneConditions"], _rankToggleParent, _rankToggle, _rankToggleList);
        }

        private void CreateParticipationPriorityToggleList()
        {
            // 参加優先度のトグルを追加
            if (_participationPriorityToggleList.Count > 0) return;
            // 優先度順で格納
            foreach (ConfGuildSearchParticipationPriorityData priorityData in ConfigManager.Instance.guildSearchParticipationPriorityTypeList.OrderByDescending(data => data.priority))
            {
                CreateToggle(priorityData.id, priorityData.name, _participationPriorityToggleParent, _participationPriorityToggle, _participationPriorityToggleList);
            }
        }

        private void CreateToggle(long rankId, string displayText, Transform parent, ClubToggle toggleObject, List<ClubToggle> toggleList)
        {
            ClubToggle toggle = Instantiate(toggleObject, parent);
            toggle.label.text = displayText;
            toggle.toggleObj.type = rankId;
#if UNITY_EDITOR
            toggle.gameObject.name = displayText;
#endif
            toggle.gameObject.SetActive(true);
            toggleList.Add(toggle);
        }

        public void OnClickInitSettingData() {
            var data = new EditData();
            UpdateView(data);
            CheckButtonActive();
        }

        public void OnClickSettingButton() {
            _param.onClickButton?.Invoke(CreateEditData());
            Close();
        }

        public void OnClickCloseButton() {
            Close();
        }

        public EditData CreateEditData(){
            var data = new EditData();
            BigValue powerFrom = BigValue.Zero;
            if( !BigValue.TryParse(_powerFrom.text, out powerFrom) ) {
                powerFrom = BigValue.Zero;
            }
            data.powerFrom = powerFrom;

            BigValue powerTo = BigValue.Zero;
            if( !BigValue.TryParse(_powerTo.text, out powerTo) ) {
                powerTo = BigValue.Zero;
            }
            data.powerTo = powerTo;

            data.rank = GetSelectedToggle(_rankToggleList);
            data.activityPolicy = ClubUtility.GetToggleParam(_activityPolicy);
            data.clubMatchPolicy = ClubUtility.GetToggleParam(_clubMatchPolicy);
            data.participationPriority = GetSelectedToggle(_participationPriorityToggleList);
            return data;
        }

        public void UpdateView( EditData editData ){
            SetPowerEditText(_powerFrom, editData.powerFrom, _powerFromDisplayText);
            SetPowerEditText(_powerTo, editData.powerTo, _powerToDisplayText);
            
            SetToggleStatus(_rankToggleList, editData.rank);
            SetToggleStatus(_participationPriorityToggleList, editData.participationPriority);
            ClubUtility.SetActiveToggle(_activityPolicy, editData.activityPolicy);
            ClubUtility.SetActiveToggle(_clubMatchPolicy, editData.clubMatchPolicy);
        }

        
        public void OnEndPowerFromEdit( string str ){
            BigValue powerFrom = BigValue.Zero;
            //不正な入力 or 入力されていない
            if( !BigValue.TryParse(str, out powerFrom) ) {
                SetPowerEditText(_powerFrom, BigValue.Zero, _powerFromDisplayText);
                return;
            }
            //0以下だったら下限なし
            if( powerFrom <= 0 ) {
                SetPowerEditText(_powerFrom, BigValue.Zero, _powerFromDisplayText);
                return;
            }

        
            //上限チェック
            BigValue powerTo = BigValue.Zero;
            if( !BigValue.TryParse(_powerTo.text, out powerTo) ) {
                SetPowerEditText(_powerFrom, powerFrom, _powerFromDisplayText);
                return;
            }

            //上限以上だったら上限-1
            if( powerFrom >= powerTo ) {
                powerFrom = powerTo - 1;
            }
            SetPowerEditText(_powerFrom, powerFrom, _powerFromDisplayText);
        }

        public void OnEndPowerToEdit( string str ){
            BigValue powerTo = BigValue.Zero;
            //不正な入力 or 入力されていない
            if( !BigValue.TryParse(str, out powerTo) ) {
                SetPowerEditText(_powerTo, BigValue.Zero, _powerToDisplayText);
                return;
            }
            //0以下だったら上限なし
            if( powerTo <= 0 ) {
                SetPowerEditText(_powerTo, BigValue.Zero, _powerToDisplayText);
                return;
            }

            //下限チェック
            BigValue powerFrom = BigValue.Zero;
            if( !BigValue.TryParse(_powerFrom.text, out powerFrom) ) {
                SetPowerEditText(_powerTo, powerTo, _powerToDisplayText);
                return;
            }
            //下限以下だったら下限+1
            if( powerFrom >= powerTo ) {
                powerTo = powerFrom + 1;
            }
            
            SetPowerEditText(_powerTo, powerTo, _powerToDisplayText);
        }


        public void SetPowerEditText( ClubInputField inputField ,BigValue power ,TextMeshProUGUI displayText){
             //0以下
            if( power < 0 ) {
                power = BigValue.Zero;
            }

            if( power <= 0 ) {
                inputField.text = string.Empty; 
            } else {
                inputField.text = power.ToString();
                inputField.inputField.textComponent.gameObject.SetActive(false);

                displayText.text = power.ToDisplayCommaString();
            }
        }
        
        public void OnSelectPowerFrom(){
            _powerFrom.inputField.textComponent.gameObject.SetActive(true);
            _powerFromDisplayText.text = string.Empty;
        }
        
        public void OnSelectPowerTo(){
            _powerTo.inputField.textComponent.gameObject.SetActive(true);
            _powerToDisplayText.text = string.Empty;
        }

        public void CheckButtonActive() {
            var initData = new EditData();   
            var data = CreateEditData();
            _initButton.interactable = !initData.Equals(data);
        }

        private long GetSelectedToggle(List<ClubToggle> toggleList)
        {
            foreach (ClubToggle clubRankToggle in toggleList)
            {
                if (clubRankToggle.toggleObj.toggle.isOn)
                {
                    return clubRankToggle.toggleObj.type;
                }
            }
            return 0;
        }

        private void SetToggleStatus(List<ClubToggle> toggleList, long selectedToggleType)
        {
            foreach (ClubToggle clubToggle in toggleList)
            {
                clubToggle.toggleObj.toggle.SetIsOnWithoutNotify(clubToggle.toggleObj.type == selectedToggleType);
                if (clubToggle.toggleObj.type == selectedToggleType) return;
            }
        }
    }
}
