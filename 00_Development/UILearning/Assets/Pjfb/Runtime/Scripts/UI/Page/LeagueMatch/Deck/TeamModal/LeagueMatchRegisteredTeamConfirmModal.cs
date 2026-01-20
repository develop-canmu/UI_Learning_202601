using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.Training;
using TMPro;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRegisteredTeamConfirmModal : ModalWindow
    {
        public enum Type
        {
            // 登録解除
            Unregistration,
            // 登録解除確認
            UnregistrationConfirm,
            // 自クラブメンバー登録チーム確認
            MyTeamConfirm,
            // 他クラブメンバー登録チーム確認
            OpponentTeamConfirm,
        }

        public class Arguments
        {
            /// <summary>モーダルタイプ</summary>
            public Type Type;
            /// <summary>SBattleReserveFormationMatchingのID</summary>
            public long Id;
            /// <summary>1=>左、2=>右</summary>
            public long SideNumber;
            /// <summary>回線番号</summary>
            public long RoundNumber;
            private Action onCancelAction = null;
            public Action OnCancelAction{get{return onCancelAction;}}
            
            public Arguments(Type type, long id, long sideNumber, long roundNumber, Action onCancelAction = null)
            {
                this.Type = type;
                this.Id = id;
                this.SideNumber = sideNumber;
                this.RoundNumber = roundNumber;
                this.onCancelAction = onCancelAction;
            }
        }

        private Arguments arguments = null;
        private BattleReserveFormationPlayerInfo playerInfo = null;

        [SerializeField] private TMPro.TextMeshProUGUI titleText = null;
        [SerializeField] private TextMeshProUGUI confirmText = null;
        [SerializeField] private TextMeshProUGUI clubNameText = null;
        [SerializeField] private UserIcon playerUserIcon = null;
        [SerializeField] private TextMeshProUGUI playerNameText = null;
        [SerializeField] private RankPowerUI deckRankUI = null;
        [SerializeField] private List<CharacterVariableIcon> characterIconList = null;
        [SerializeField] private GameObject contentRoot = null;
        [SerializeField] private TextMeshProUGUI errorText = null;

        [SerializeField] private RectTransform buttonClose = null;
        [SerializeField] private RectTransform buttonCancel = null;
        [SerializeField] private RectTransform buttonReleaseRegister = null;
        [SerializeField] private RectTransform buttonOk = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;
            
            // エラーUI表示
            if(arguments.Id <= 0)
            {
                SetErrorUI();
                return;
            }

            // Request
            BattleReserveFormationGetDeckDetailAPIRequest request = new BattleReserveFormationGetDeckDetailAPIRequest();
            // Post
            BattleReserveFormationGetDeckDetailAPIPost post = new BattleReserveFormationGetDeckDetailAPIPost();
            post.id = arguments.Id;
            post.sideNumber = arguments.SideNumber;
            post.roundNumber = arguments.RoundNumber;
            request.SetPostData(post);
            
            // API
            await APIManager.Instance.Connect(request);
            // Response
            BattleReserveFormationGetDeckDetailAPIResponse response = request.GetResponseData();
            playerInfo = response.playerInfo;
            
            // エラーUI表示
            if (playerInfo == null || playerInfo.player.playerId <= 0)
            {
                SetErrorUI();
                return;
            }

            InitializeUI();
            await base.OnPreOpen(args, token);
        }
        
        /// <summary>
        /// UI設定
        /// </summary>
        private void InitializeUI()
        {
            contentRoot.SetActive(true);
            errorText.gameObject.SetActive(false);
            titleText.text = StringValueAssetLoader.Instance["league.match.team_confirm_modal.title"];
            clubNameText.text = playerInfo.groupName;
            playerNameText.text = playerInfo.player.name;
            playerUserIcon.SetIconId(playerInfo.player.mIconId);
            
            SetCharacterDeckImage(playerInfo.charaVariableList);
            SetButtonActive(arguments.Type);
        }
        
        /// <summary>
        /// UI設定
        /// </summary>
        private void SetErrorUI()
        {
            contentRoot.SetActive(false);
            errorText.gameObject.SetActive(true);   
            // ボタン制御
            buttonClose.gameObject.SetActive(true);
            buttonCancel.gameObject.SetActive(false);
            buttonReleaseRegister.gameObject.SetActive(false);
            buttonOk.gameObject.SetActive(false);
        }

        /// <summary>
        /// キャラアイコンの設定
        /// </summary>
        private void SetCharacterDeckImage(BattleV2Chara[] deckCharaList)
        {
            if (deckCharaList != null)
            {
                BigValue totalCombatPower = BigValue.Zero;
                
                for (int i = 0; i < characterIconList.Count; i++)
                {
                    BattleV2Chara charData = deckCharaList[i];
                    // アイコン
                    CharacterVariableIcon charIcon = characterIconList[i];
                    // アイコンの表示
                    charIcon.SetIconTextureWithEffectAsync(charData.mCharaId).Forget();
                    // 総合力
                    totalCombatPower += new BigValue(charData.combatPower);
                    
                    charIcon.OpenDetailModal = true;
                    CharacterVariableDetailData detailData = new CharacterVariableDetailData(charData);
                    charIcon.SetIcon(detailData, (RoleNumber)charData.roleNumber);
                    
                }
                deckRankUI.InitializePartyRankUI(totalCombatPower);
            }
            else 
            {
                return;
            }
        }

        /// <summary>
        /// ボタン表示設定
        /// </summary>
        /// <param name="type"></param>
        private void SetButtonActive(Type type)
        {
            buttonClose.gameObject.SetActive(false);
            buttonCancel.gameObject.SetActive(false);
            buttonReleaseRegister.gameObject.SetActive(false);
            buttonOk.gameObject.SetActive(false);
            confirmText.text = StringValueAssetLoader.Instance["league.match.team_confirm_modal.unregistration_confirm"];
            confirmText.gameObject.SetActive(false);

            switch (type)
            {
                case Type.Unregistration:
                {
                    buttonReleaseRegister.gameObject.SetActive(true);
                    buttonCancel.gameObject.SetActive(true);
                    BackKeyObject = buttonCancel;
                    return;
                }
                case Type.UnregistrationConfirm:
                {
                    buttonOk.gameObject.SetActive(true);
                    buttonCancel.gameObject.SetActive(true);
                    confirmText.gameObject.SetActive(true);
                    BackKeyObject = buttonCancel;
                    return;
                }
                case Type.MyTeamConfirm:
                {
                    buttonClose.gameObject.SetActive(true);
                    BackKeyObject = buttonClose;
                    return;
                }
                case Type.OpponentTeamConfirm:
                {
                    buttonClose.gameObject.SetActive(true);
                    BackKeyObject = buttonClose;
                    return;
                }
            }
        }

        /// <summary>
        /// 登録解除確認ボタン
        /// </summary>
        public void OnClickReleaseRegisterButton()
        {
            var param = new LeagueMatchRegisteredTeamConfirmModal.Arguments
            (
                Type.UnregistrationConfirm,
                arguments.Id,
                arguments.SideNumber,
                arguments.RoundNumber,
                arguments.OnCancelAction
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchRegisteredTeamConfirm, param);
        }

        /// <summary>
        /// 登録解除実行ボタン
        /// </summary>
        public async void OnClickOkButton()
        {
            BattleReserveFormationSetDeckAPIRequest request = new BattleReserveFormationSetDeckAPIRequest();
            BattleReserveFormationSetDeckAPIPost post = new()
            {
                id = arguments.Id,
                roundNumber = arguments.RoundNumber,
                partyNumber = 0,
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            InitializeUI();
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["league.match.entry_cancel_title"],
                StringValueAssetLoader.Instance["league.match.entry_cancel"], 
                "",
                    new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.close"], 
                        (
                            window =>
                            {
                                arguments.OnCancelAction?.Invoke();
                                window.Close();
                            }
                        )
                    )
                )
            );
        }
        
        /// <summary>
        /// ユーザープロフィール開く
        /// </summary>
        public void OnClickUserIconButton()
        {
            ColosseumManager.OpenUserProfileModal(playerInfo.player.playerId, (ColosseumPlayerType)playerInfo.player.playerType);
        }
    }
}
