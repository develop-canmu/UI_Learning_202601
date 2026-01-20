using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.Community
{
    public class FollowSearchModalWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public Action onClosed;
            public bool showUserProfileOtherButtons = true;
        }
        
        [SerializeField] private ScrollGrid scroller;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_InputField friendCodeInput;
        [SerializeField] private TMP_InputField lowerPowerInput;
        [SerializeField] private TMP_InputField upperPowerInput;
        [SerializeField] private DropDownUI lowerRankDropdown;
        [SerializeField] private DropDownUI upperRankDropdown;
        [SerializeField] private UINotification notificationUI;
        [SerializeField] private GameObject textEmptyObject;
        [SerializeField] private TextMeshProUGUI lowerPowerDisplayText;
        [SerializeField] private TextMeshProUGUI upperPowerDisplayText;

        private WindowParams _windowParams;
        private List<FollowBlockScrollItemInfo> searchUserList = new List<FollowBlockScrollItemInfo>();
        private List<FollowBlockScrollItem> cacheScrollItems = new List<FollowBlockScrollItem>();
        private List<CharaRankMasterObject> deckRanks;
        private CancellationTokenSource cancellationTokenSource = null;
        
        #endregion
        
        #region Life Cycle
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.FollowSearch, data);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            nameInput.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, nameInput.characterLimit,nameInput.fontAsset);
            await Init();
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            if (!cacheScrollItems.Any())
            {
                UpdateScrollItems();
            }
            else
            {
                UpdateScrollItemButton();
            }
            //Scroll　Itemのtime Text更新
            UpdateScrollItemTimeText().Forget();
            base.OnOpened();
        }

        private async UniTask Init()
        {
            SetRankDropdownOptions();
            nameInput.text = "";
            lowerPowerInput.text = "";
            upperPowerInput.text = "";
            if(!cacheScrollItems.Any())searchUserList = await SearchUserAPI();
        }

        protected override void OnClosed()
        {
            if(cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            base.OnClosed();
        }

        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: () => _windowParams.onClosed?.Invoke());
            cacheScrollItems.Clear();
        }

        public async void OnClickSearch()
        {
            searchUserList = await SearchUserAPI();
            UpdateScrollItems();
        }
        
        private void OnClickConfirmYell(UserCommunityUserStatus userStatus)
        {
            string badgeText = string.Format(StringValueAssetLoader.Instance["community.yell.count_badge"],ConfigManager.Instance.yellLimit - CommunityManager.yellCount, ConfigManager.Instance.yellLimit);
            FollowConfirmModalWindow.Open(new FollowConfirmModalWindow.WindowParams
            {
                UMasterId = userStatus.uMasterId,
                UserName = userStatus.name,
                BadgeCountText = badgeText,
                onClosed = UpdateScrollItemButton
            });
        }
        
        private void OnClickChat(long targetUMasterId)
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close(onCompleted: () =>
            {
                var info = new CommunityPage.CommunityPageInfo{ Status = CommunityStatus.PersonalChat, TargetUMasterId = targetUMasterId};
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, info);
            });
        }

        private void OnClickBlock(UserCommunityUserStatus userStatus)
        {
            BlockAPI(userStatus).Forget();
        }

        public void OnInputFieldEndEdit(string input)
        {
            input = StringUtility.GetLimitNumCharacter(input,nameInput.characterLimit);
            nameInput.SetTextWithoutNotify(input);
        }

        public void OnPlayerIdInputFieldEndEdit(string input)
        {
            long friendCode = 0;
            if(!long.TryParse(input, out friendCode))
            {
                friendCodeInput.text = string.Empty;
                return;
            }

            if (friendCode <= 0) friendCodeInput.text = string.Empty;
        }

        public void OnLowerRankDropdownChanged(int value)
        {
            if (value > upperRankDropdown.value + 1) lowerRankDropdown.value = upperRankDropdown.value;
        }
        
        public void OnUpperRankDropdownChanged(int value)
        {
            if (value < lowerRankDropdown.value - 1) upperRankDropdown.value = lowerRankDropdown.value;
        }

        public void OnEndPowerFromEdit( string str )
        {
            BigValue powerFrom = BigValue.Zero;
            //不正な入力 or 入力されていない
            if( !BigValue.TryParse(str, out powerFrom) ) {
                SetPowerEditText(lowerPowerInput, BigValue.Zero, lowerPowerDisplayText);
                return;
            }
            //0以下だったら下限なし
            if( powerFrom <= 0 ) {
                SetPowerEditText(lowerPowerInput, BigValue.Zero, lowerPowerDisplayText);
                return;
            }
            
            //上限チェック
            BigValue powerTo = BigValue.Zero;
            if( !BigValue.TryParse(upperPowerInput.text, out powerTo) ) {
                SetPowerEditText(lowerPowerInput, powerFrom, lowerPowerDisplayText);
                return;
            }

            //上限以上だったら上限-1
            if( powerFrom > powerTo ) {
                powerFrom = powerTo - 1;
            }
            SetPowerEditText(lowerPowerInput, powerFrom, lowerPowerDisplayText);
        }

        public void OnEndPowerToEdit( string str )
        {
            BigValue powerTo = BigValue.Zero;
            //不正な入力 or 入力されていない
            if( !BigValue.TryParse(str, out powerTo) ) {
                SetPowerEditText(upperPowerInput, BigValue.Zero, upperPowerDisplayText);
                return;
            }
            //0以下だったら上限なし
            if( powerTo <= 0 ) {
                SetPowerEditText(upperPowerInput, BigValue.Zero, upperPowerDisplayText);
                return;
            }

            //下限チェック
            BigValue powerFrom = BigValue.Zero;
            if( !BigValue.TryParse(lowerPowerInput.text, out powerFrom) ) {
                SetPowerEditText(upperPowerInput, powerTo, upperPowerDisplayText);
                return;
            }
            //下限以下だったら下限+1
            if( powerFrom > powerTo ) {
                powerTo = powerFrom + 1;
            }
            
            SetPowerEditText(upperPowerInput, powerTo, upperPowerDisplayText);
        }
        
        public void SetPowerEditText( TMP_InputField inputField ,BigValue power ,TextMeshProUGUI displayText){
             //0以下
            if( power < 0 ) {
                power = BigValue.Zero;
            }

            if( power <= 0 ) {
                inputField.text = string.Empty; 
            } else {
                inputField.text = power.ToString();
                inputField.textComponent.gameObject.SetActive(false);
                displayText.text = power.ToDisplayCommaString();
            }
        }

        public void OnSelectPowerFromRemoveComma() {
            lowerPowerDisplayText.text = string.Empty;
            lowerPowerInput.textComponent.gameObject.SetActive(true);
        }
        
        public void OnSelectPowerToRemoveComma() {
            upperPowerDisplayText.text = string.Empty;
            upperPowerInput.textComponent.gameObject.SetActive(true);
        }

        #endregion
        
        #region API
        
        private async UniTask<List<FollowBlockScrollItemInfo>> SearchUserAPI()
        {
            // todo: インフレ対応サーバー対応時に修正
            CommunitySearchUserAPIRequest request = new CommunitySearchUserAPIRequest();
            CommunitySearchUserAPIPost post = new CommunitySearchUserAPIPost
            {
                name = nameInput.text,
                friendCode = friendCodeInput.text,
                combatPowerFrom = string.IsNullOrEmpty(lowerPowerInput.text) ? "0" : lowerPowerInput.text,
                combatPowerTo = string.IsNullOrEmpty(upperPowerInput.text) ? "0" : upperPowerInput.text,
                deckRankFrom = GetDeckRankNumber(lowerRankDropdown.captionText.text),
                deckRankTo = GetDeckRankNumber(upperRankDropdown.captionText.text),
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CommunitySearchUserAPIResponse response = request.GetResponseData();
            var infoList = response.communityUserStatusList.Select(status => new FollowBlockScrollItemInfo
            {
                userStatus = status,
                OnClickYell = () => OnClickConfirmYell(status),
                OnClickFollow = () => FollowAPI(status).Forget(),
                OnClickChat = () => OnClickChat(status.uMasterId),
                OnClickBlock = () => OnClickBlock(status),
                showUserProfileOtherButtons = _windowParams.showUserProfileOtherButtons
                
            }).ToList();
            return infoList;
        }
        private async UniTask FollowAPI(UserCommunityUserStatus userStatus)
        {
            CommunityFollowAPIRequest request = new CommunityFollowAPIRequest();
            CommunityFollowAPIPost post = new CommunityFollowAPIPost{targetUMasterId = userStatus.uMasterId};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            searchUserList.Remove(searchUserList.First(user => user.userStatus.uMasterId == userStatus.uMasterId));
            scroller.SetItems(searchUserList);
            textEmptyObject.SetActive(searchUserList.Count == 0);
            string message = string.Format(StringValueAssetLoader.Instance["community.followed"],userStatus.name);
            notificationUI.ShowNotification(message);
            CommunityManager.followUserList.Add(userStatus);
            CommunityManager.yellDetail.followedCount++;
            if (CommunityManager.yellDetail.todayYelledList.All(y => y.uMasterId != userStatus.uMasterId))
            {
                CommunityManager.yellDetail.followedCanYellCount++;
                //メニューバッジ更新
                AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            }
        }
        
        private async UniTask BlockAPI(UserCommunityUserStatus userStatus)
        {
            CommunityBlockAPIRequest request = new CommunityBlockAPIRequest();
            CommunityBlockAPIPost post = new CommunityBlockAPIPost{targetUMasterId = userStatus.uMasterId};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            string message = string.Format(StringValueAssetLoader.Instance["community.blocked"],userStatus.name);
            notificationUI.ShowNotification(message);
            CommunityManager.blockUserList.Add(userStatus);
            CommunityManager.followUserList.Remove(userStatus);
        }

        #endregion

        #region Other

        private void SetRankDropdownOptions()
        {
            deckRanks = MasterManager.Instance.charaRankMaster.FindDatas(CharaRankMasterStatusType.PartyTotal).ToList();

            lowerRankDropdown.options = new List<TMP_Dropdown.OptionData>();
            upperRankDropdown.options = new List<TMP_Dropdown.OptionData>();
            
            lowerRankDropdown.options.Add(new TMP_Dropdown.OptionData{text = StringValueAssetLoader.Instance["community.search.no_lower_limit"]});
            foreach (var rank in deckRanks)
            {
                var option = new TMP_Dropdown.OptionData { text = rank.name };
                lowerRankDropdown.options.Add(option);
                upperRankDropdown.options.Add(option);
            }
            upperRankDropdown.options.Add(new TMP_Dropdown.OptionData{text = StringValueAssetLoader.Instance["community.search.no_upper_limit"]});
            
            lowerRankDropdown.value = 0;
            upperRankDropdown.value = upperRankDropdown.options.Count - 1;
            lowerRankDropdown.RefreshShownValue();
            upperRankDropdown.RefreshShownValue();
        }

        private long GetDeckRankNumber(string rankName)
        {
            var rank = deckRanks.FirstOrDefault(rank => rank.name == rankName);
            // 対象ランクが見つからない場合は-1を返す
            return (rank != null) ? rank.rankNumber : -1;
        }
        
        private async void UpdateScrollItems()
        {
            scroller.SetItems(searchUserList);
            textEmptyObject.SetActive(searchUserList.Count == 0);
            //scroller item　active状態更新のため,1 frame待ち
            await UniTask.NextFrame();
            cacheScrollItems.Clear();
            foreach (Transform child in scroller.content)
            {
                if (!child.gameObject.activeSelf) continue;
                var item = child.GetComponent<FollowBlockScrollItem>();
                if(item != null) cacheScrollItems.Add(item);
            }
        }
        
        private async UniTask UpdateScrollItemTimeText()
        {
            if(cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            cancellationTokenSource = new CancellationTokenSource();

            await CommunityManager.UpdateActionInterval(5, () =>
            {
                cacheScrollItems.ForEach(item=>item.SetTimeText());
            }, cancellationTokenSource: cancellationTokenSource);
        }
        
        private void UpdateScrollItemButton()
        {
            cacheScrollItems.ForEach(item =>item.ActiveButtons());
        }

        #endregion

    }
}