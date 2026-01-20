using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using Pjfb.Deck;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;
using Pjfb.UI;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;
using static Pjfb.Voice.VoiceResourceSettings;
using Pjfb.Voice;

namespace Pjfb.Story
{
    public class StoryDeckSelectPage : StoryPageBase
    {
        #region PageParams
        public class PageParams
        {
            public long currentProgress;
            public HuntStageMasterObject storyData;
            public HuntEnemyMasterObject subStoryData;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public FestivalTimetableMasterObject festivalTimetableMasterObject;
            public FestivalUserStatus nullableFestivalUserStatus;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private GameObject[] playerObjects;
        [SerializeField] private GameObject[] enemyObjects;
        [SerializeField] private TextMeshProUGUI scenarioNameText;
        [SerializeField] private TextMeshProUGUI switchText;
        [SerializeField] private DeckPanelView enemyDeckPanelView;
        [SerializeField] private GameObject playerDeckPanel; // TODO: scrollgridとかに変える
        [SerializeField] private ScrollBanner playerDeckScrollGrid;
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private CanvasGroup characterCardImageParentCanvas;
        [SerializeField] private Button startButton;
        #endregion

        #region PrivateFields
        private DeckListData _deckListData;
        private PageParams _pageParams;
        private HuntEnemyMasterObject _mEnemy;
        
        private bool _isShowPlayerDeck;
        private int _selectedPlayerDeckIndex;
        private List<DeckPanelScrollGridItem.Parameters> _playerDeckList;
        [CanBeNull] private Tweener cardImageTween;
        
        private readonly Color EnemyColor = new(202f/255f, 55f/255f, 94f/255f);
        private readonly Color PlayerColor = new (38f/255f, 64f/255f, 218f/255f);
        #endregion

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            Init((PageParams)args);
            await base.OnPreOpen(args, token);
        }

        protected override UniTask OnMessage(object value)
        {
            // メモ：デッキ編成から戻るときに実行される
            if ((PageManager.MessageType)value == PageManager.MessageType.BeginFade && Page.TransitionType == PageTransitionType.Back) Init(_pageParams);
            return base.OnMessage(value);
        }

        private void Awake()
        {
            playerDeckScrollGrid.onChangedPage += OnChangePage;
            InitDisplay();
        }

        protected override async UniTask<bool> OnPreLeave(CancellationToken token)
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
            DebugMenu.DebugMenuStory.RemoveOptions();
#endif
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            var deckReady  = selectedBannerParameters?.viewParams.iconParams
                .All(aData => aData.nullableCharacterData != null && aData.nullableCharacterData.MCharaId != 0) ?? false;
            if (deckReady) await TryCallDeckSelectAPI(selectedPartyNumber);
            return await base.OnPreLeave(token);
        }
        #endregion

        #region PrivateMethods
        private void Init(PageParams pageParams)
        {
            _pageParams = pageParams;
            InitDisplay();
            SetEnemyDisplay();
            SetPlayerDeckDisplay();
            SwitchDisplay(_isShowPlayerDeck);
            UpdateDisplay();
            
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
            DebugMenu.DebugMenuStory.AddOptions(_pageParams);
#endif
        }
        
        private void InitDisplay()
        {
            enemyDeckPanelView.Init();
            characterCardImageParentCanvas.alpha = 0;
        }

        private async void SetPlayerDeckDisplay()
        {
            characterCardImageParentCanvas.alpha = 0;
            _deckListData = await DeckUtility.GetDeckList(DeckType.Battle);
            var deckDataList = _deckListData.DeckDataList.ToList();
            _playerDeckList = deckDataList.Select(aDeck =>
            {
                return new DeckPanelScrollGridItem.Parameters
                {
                    viewParams = new DeckPanelView.ViewParams
                    (
                        deckId: aDeck.Deck.partyNumber,
                        deckName: aDeck.Deck.name,
                        strategy: (BattleConst.DeckStrategy)aDeck.Deck.optionValue,
                        iconParams: aDeck.Deck.memberIdList
                            .Where(aMember => aMember.l?.Length >= 3) // データ不正の場合処理されない
                            .Select(aMember => new DeckPanelCharaIconView.ViewParams
                            {
                                type = aMember.l[0],
                                nullableCharacterData = aMember.l[1] > 0 ? new CharacterVariableDetailData(UserDataManager.Instance.charaVariable.Find(aMember.l[1])) : null,
                                position = (RoleNumber) aMember.l[2]
                            }).ToList(),
                        isPlayerDeck: true,
                        onClickDeckEditButton: OnClickDeckEditButton,
                        rankBgColor: PlayerColor,
                        onStrategyChanged: TrySaveDeck
                    )
                };
            }).ToList();
            playerDeckScrollGrid.SetBannerDatas(_playerDeckList);
            _selectedPlayerDeckIndex = _deckListData.SelectingIndex;
            playerDeckScrollGrid.SetIndex(_selectedPlayerDeckIndex);
        }

        private void SetEnemyDisplay()
        {
            _mEnemy = MasterManager.Instance.huntEnemyMaster.FindData(_pageParams.subStoryData.id);
            scenarioNameText.text = _mEnemy.subName;
            
            var enemyIconParams = new List<DeckPanelCharaIconView.ViewParams>();
            
            for (var i = 0; i < Mathf.Max(_mEnemy.mCharaNpcIdList.Length, _mEnemy.roleNumberList.Length); i++)
            {
                enemyIconParams.Add(new DeckPanelCharaIconView.ViewParams
                {
                    nullableCharacterData = _mEnemy.mCharaNpcIdList.Length > i ? new CharacterVariableDetailData(MasterManager.Instance.charaNpcMaster.FindData(_mEnemy.mCharaNpcIdList[i])) : null,
                    position = (RoleNumber)(_mEnemy.roleNumberList.Length > i ? _mEnemy.roleNumberList[i] : 0),
                    type = 1
                });   
            }
            
            enemyDeckPanelView.SetDisplay(new DeckPanelView.ViewParams (
                isPlayerDeck: false,
                deckName: _mEnemy.subName,
                strategy: BattleConst.DeckStrategy.None,
                rankBgColor: EnemyColor,
                iconParams: enemyIconParams,
                deckId: 0,
                onClickDeckEditButton: null,
                onStrategyChanged: null
            ));
        }

        private void SwitchDisplay(bool isShowPlayerDeck)
        {
            this._isShowPlayerDeck = isShowPlayerDeck;
            foreach (var obj in playerObjects) obj.SetActive(isShowPlayerDeck);
            foreach (var obj in enemyObjects) obj.SetActive(!isShowPlayerDeck);
            enemyDeckPanelView.gameObject.SetActive(!isShowPlayerDeck);
            playerDeckPanel.SetActive(isShowPlayerDeck);
            switchText.text = StringValueAssetLoader.Instance[isShowPlayerDeck ? "common.enemy_team" : "common.self_team"];
        }

        private void UpdateDisplay()
        {
            var selectedPlayerDeck = _playerDeckList[_selectedPlayerDeckIndex];
            startButton.interactable = selectedPlayerDeck.viewParams.iconParams.TrueForAll(aData => aData.nullableCharacterData != null);
            
            var charaId = 
                _isShowPlayerDeck && selectedPlayerDeck.viewParams.iconParams.Count > 0 ? selectedPlayerDeck.viewParams.iconParams[0].nullableCharacterData?.MCharaId ?? 0 :
                !_isShowPlayerDeck && enemyDeckPanelView.viewParams.iconParams.Count > 0 ? enemyDeckPanelView.viewParams.iconParams[0].nullableCharacterData.MCharaId :
                0;
            SetCharacterCardImage(charaId);
            
            enemyDeckPanelView.SetEnemyNoticeGameObject(
                isActive: selectedPlayerDeck.viewParams.NewTotalCombatPower < enemyDeckPanelView.viewParams.NewTotalCombatPower);
        }

        private async void SetCharacterCardImage(long charaId)
        {
            cardImageTween?.Kill();
            characterCardImageParentCanvas.alpha = 0;
            if (charaId <= 0) return;
            
            await characterCardImage.SetTextureAsync(charaId);
            cardImageTween = DOTween.To(val => characterCardImageParentCanvas.alpha = val, 0, 1, 0.2f);
        }

        private void TrySaveDeck(BattleConst.DeckStrategy strategy)
        {
            _deckListData.DeckDataList[_deckListData.SelectingIndex].Deck.optionValue = (int)strategy;
            _deckListData.DeckDataList[_deckListData.SelectingIndex].SaveDeckAsync().Forget();
        }

        private async Task TryCallDeckSelectAPI(long selectedPartyNumber)
        {
            if (_deckListData.DeckDataList[_deckListData.SelectingIndex].PartyNumber != selectedPartyNumber) await _deckListData.SelectDeckAsync(partyNumber: selectedPartyNumber);
        }

        private async void ProceedBattleFromStart()
        {
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            var teamCaptainMCharaId = selectedBannerParameters?.viewParams.iconParams[0].nullableCharacterData?.MCharaId;
            var teamCaptainMChara = MasterManager.Instance.charaMaster.FindData((long)teamCaptainMCharaId);
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            await TryCallDeckSelectAPI(selectedPartyNumber);
            await VoiceManager.Instance.PlayInVoiceForLocationTypeAsync(teamCaptainMChara, LocationType.IN_YELL);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            StoryManager.Instance.OnStoryBattleStart(_pageParams);
        }
        #endregion
        
        #region EventListener
        public void OnClickStartButton()
        {
            // TODO: TryResumeLastStoryBattleはしばらくダミー処理です。
            // レジューム機能が実装されたら、TryResumeLastStoryBattleの実行を外しても大丈夫です。残すも安全装置として問題ない想定
            StoryManager.Instance.TryResumeLastStoryBattle(onCanceled: null, onSkip: ProceedBattleFromStart);
        }

        public async void OnClickSwitchDeckButton()
        {
            SwitchDisplay(!_isShowPlayerDeck);
            await UniTask.NextFrame();
            UpdateDisplay();
        }
        
        public void OnBackButton()
        {
            Page.OpenPage(StoryPageType.StoryScenarioSelect, false, new StoryScenarioSelectPage.PageParams(_pageParams.storyData, _pageParams.huntTimetableMasterObject, _pageParams.festivalTimetableMasterObject, _pageParams.nullableFestivalUserStatus, progress: _pageParams.currentProgress));
        }

        private void OnChangePage(int index)
        {
            _selectedPlayerDeckIndex = index;
            UpdateDisplay();
        }

        public void OnClickExitConditions()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ExitConditions, null);
        }
        

        private async void OnClickDeckEditButton(long partyNumber)
        {
            await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Deck, true, new DeckPageParameters{InitialPartyNumber = partyNumber});
        }
        #endregion
    }
}