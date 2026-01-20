using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UI;
using Pjfb.UserData;
using Pjfb.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = CruFramework.Logger;

namespace Pjfb.Encyclopedia
{
    public class EncyclopediaDetailPage : Page
    {
        #region SerializeFields
        [SerializeField] [StringValue] private string PossessionString;
        [SerializeField] private CharacterCardBackgroundImage characterCardBackgroundImage;
        [SerializeField] private CharacterProfileView characterProfileView;
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private Slider gaugeSlider;
        [SerializeField] private PrizeJsonView nextRewardPrizeJsonView;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI possessionCountText;

        [SerializeField] private ScrollDynamic gameVoiceScroll;
        [SerializeField] private ScrollDynamic othersVoiceScroll;

        [SerializeField] private CharacterEncyclopediaTabSheetManager encyclopediaTabSheetManager;
        [SerializeField] private CharacterVoiceTabSheetManager voiceTabSheetManager;
        
        [SerializeField] private MCharacterScroll mCharacterScroll;
        [SerializeField] private GameObject topScrollArrow;
        [SerializeField] private TrustLevelUpEffect trustLevelUpEffect;
 
        [Header("Voice badge")]
        [SerializeField] private UIBadgeNotification voiceBadge;
        [SerializeField] private UIBadgeNotification othersVoiceBadge;
        
        #endregion

        private CharaParentData  charaParentData;
        private long CurrentParentMCharaId => charaParentData.CharaParentBase.parentMCharaId;
        private int currentIndex = 0;
        private bool isPageOpened;

        private MCharacterScrollData selectingScrollData;
        private long selectingMCharaId;
        
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            AddDebugCommand();
            isPageOpened = false;
            currentIndex = (int)args;
            
            topScrollArrow.SetActive(EncyclopediaPage.AvailableIdList.Count > 1);
            await SetCharaParentViewByIndexAsync();
            mCharacterScroll.OnSelectedItem -= OnSelectMCharacter;
            mCharacterScroll.OnSelectedItem += OnSelectMCharacter;
            
            await encyclopediaTabSheetManager.OpenSheetAsync(CharacterEncyclopediaTabSheetType.Profile, null);
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            
            isPageOpened = true;
            AppManager.Instance.TutorialManager.OpenTrustValueTutorialAsync().Forget();
        }

        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            RemoveDebugCommand();
            VoiceManager.Instance.ReleaseAllAsset();
            return base.OnPreClose(token);
        }
        
        private void OnSelectMCharacter(MCharacterScrollData value)
        {
            OnSelectMCharacterAsync(value).Forget();
        }
        
        private async UniTask OnSelectMCharacterAsync(MCharacterScrollData scrollData)
        {
            if(scrollData is null)  return;
            if(!scrollData.HasCharacter) return;
            if (selectingScrollData is not null)
            {
                selectingScrollData.IsSelected = false;
            }
            selectingScrollData = scrollData;
            selectingScrollData.IsSelected = true;
            mCharacterScroll.RefreshItemView();
            await SetMCharaView(selectingScrollData.MCharaId);
            
            
        }

        public void OpenRewardList()
        {
            TrustRewardListWindow.Open(new TrustRewardListWindow.WindowParams
            {
                levelRewardId = charaParentData.MCharaParent.mLevelRewardIdTrust,
                receivedLevel = charaParentData.CharaParentBase.trustLevel,
                onClosed = null
            });
            
        }

      
        private async UniTask OpenTrustLevelUpAsync(long fromLevel, long toLevel)
        {
            
            EncyclopediaPage parentPage = (EncyclopediaPage)Manager;
            await UniTask.WaitUntil(() => parentPage.IsPageOpened && isPageOpened);

            long levelRewardId = charaParentData.MCharaParent.mLevelRewardIdTrust;
            await trustLevelUpEffect.InitializeAndOpen(charaParentData.MCharaParent.parentMCharaId, levelRewardId, fromLevel, toLevel);
        }

        public void OpenCharacterIllustrator()
        {
            CharacterIllustratorModalWindow.Open(new CharacterIllustratorModalWindow.WindowParams(selectingScrollData.MCharaId, CardType.Character));
        }
        
        public void NextParentCharacter()
        {
            currentIndex = (currentIndex + 1) % EncyclopediaPage.AvailableIdList.Count;
            SetCharaParentViewByIndexAsync().Forget();
        }
        
        public void PrevParentCharacter()
        {
            currentIndex = (currentIndex - 1 + EncyclopediaPage.AvailableIdList.Count) % EncyclopediaPage.AvailableIdList.Count;
            SetCharaParentViewByIndexAsync().Forget();
        }
        
        
        

        private async UniTask SetCharaParentViewByIndexAsync()
        {
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            charaParentData = EncyclopediaPage.GetCharaParentDataByIndex(currentIndex);
            mCharacterScroll.InitializeScroll(CurrentParentMCharaId, EncyclopediaPage.MCharaPossessionHashSet);
            SetTrustUI();
            await UniTask.WhenAll(SetBackgroundAsync(), OnSelectMCharacterAsync(mCharacterScroll.GetFirstPossessionMCharacterScrollData()), ReceivePrizeIfNeeded());
            
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        } 
        
        private async UniTask SetBackgroundAsync()
        {
            await characterCardBackgroundImage.SetTextureAsync(charaParentData.CharaParentBase.parentMCharaId);
        }

        
        private async UniTask ReceivePrizeIfNeeded()
        {
            if (charaParentData.CharaParentBase.trustLevelRead >= charaParentData.CharaParentBase.trustLevel) return;
            OpenTrustLevelUpAsync(charaParentData.CharaParentBase.trustLevelRead, charaParentData.CharaParentBase.trustLevel).Forget();
            
            CharaLibraryReceivePrizeAPIRequest request = new();
            CharaLibraryReceivePrizeAPIPost post = new()
            {
                parentMCharaIdList = new[] { charaParentData.CharaParentBase.parentMCharaId }
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            charaParentData.CharaParentBase.trustLevelRead = charaParentData.CharaParentBase.trustLevel;
            charaParentData.CharaParentBase.hasTrustPrize = false;
            var response = request.GetResponseData();
            UserDataManager.Instance.UpdateHasTrustPrize(response.hasTrustPrize);
            AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
        }
        private void SetTrustUI()
        {
            characterProfileView.InitializeUI(charaParentData.MCharaParent);
            long trustLevel = charaParentData.CharaParentBase.trustLevel;

            var trustLevelDictionary = charaParentData.MCharaParent.MEnhanceLevelDictionary;
            bool isMaxLevel = !trustLevelDictionary.ContainsKey(trustLevel + 1);

            levelText.text = string.Format(
                StringValueAssetLoader.Instance[(isMaxLevel) ? "character.max_level" : "character.status.lv_value"],
                charaParentData.CharaParentBase.trustLevel);

            
            if (isMaxLevel)
            {
                nextRewardPrizeJsonView.gameObject.SetActive(false);
                gaugeSlider.value = 1;
            }
            else
            {
                long prevLevelTotalExp = trustLevelDictionary.ContainsKey(trustLevel)? trustLevelDictionary[trustLevel].totalExp : 0;
                long currentLevelTotalExp = trustLevelDictionary.ContainsKey(trustLevel + 1) ? trustLevelDictionary[trustLevel + 1].totalExp : 0;

                nextRewardPrizeJsonView.gameObject.SetActive(true);
                nextRewardPrizeJsonView.SetView(charaParentData.MCharaParent.GetMLevelRewardPrizeByLevel(charaParentData.CharaParentBase.trustLevel + 1)?.prizeJson[0] ?? null);
                gaugeSlider.value = (float)(charaParentData.CharaParentBase.trustExp - prevLevelTotalExp) / (currentLevelTotalExp - prevLevelTotalExp);

            }

            possessionCountText.text = possessionCountText.text = string.Format(StringValueAssetLoader.Instance[PossessionString],  charaParentData.PossessionCount,  charaParentData.MaxCount);
        }
        
        
        private async UniTask SetMCharaView(long mCharaId)
        {
            var mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            await characterCardImage.SetTextureAsync(mCharaId);
            await SetMCharaVoice(mChara);
        }

        private async UniTask SetMCharaVoice(CharaMasterObject mChara)
        {
            List<CharaLibraryVoiceMasterObject> voiceList = await VoiceManager.Instance.GetCharaLibraryVoiceListAsync(mChara);

            List<VoiceScrollData> gameVoiceScrollData = new();
            List<VoiceScrollData> othersVoiceScrollData = new();

            bool hasUnreadGameVoice = false;
            bool hasUnreadOthersVoice = false;

            var voiceBadgeData = EncyclopediaPage.VoiceBadgeReadDictionary.GetValueOrDefault(CurrentParentMCharaId, null);
            
            long trustLevel = charaParentData.CharaParentBase.trustLevel;
            long gameTrustLevelRead = voiceBadgeData?.gameVoiceReadLevel ?? 0;
            long othersTrustLevelRead = voiceBadgeData?.othersVoiceReadLevel ?? 0;
         
            foreach (var mCharaLibraryVoice in voiceList)
            {
                switch ((VoiceResourceSettings.VoiceType)mCharaLibraryVoice.voiceType)
                {
                    case VoiceResourceSettings.VoiceType.System:
                        othersVoiceScrollData.Add(new VoiceScrollData(mCharaLibraryVoice, mCharaLibraryVoice.releaseTrustLevel > trustLevel));
                        hasUnreadOthersVoice |= mCharaLibraryVoice.releaseTrustLevel <= trustLevel &&
                                                mCharaLibraryVoice.releaseTrustLevel > othersTrustLevelRead;
                        break;
                    case VoiceResourceSettings.VoiceType.In:
                    case VoiceResourceSettings.VoiceType.InSp:
                    case VoiceResourceSettings.VoiceType.Skill:
                        gameVoiceScrollData.Add(new VoiceScrollData(mCharaLibraryVoice, mCharaLibraryVoice.releaseTrustLevel > trustLevel));
                        hasUnreadGameVoice |= mCharaLibraryVoice.releaseTrustLevel <= trustLevel &&
                                             mCharaLibraryVoice.releaseTrustLevel > gameTrustLevelRead;
                        break;
                    default:
                        break;
                }
            }
            voiceBadge.SetActive(hasUnreadGameVoice || hasUnreadOthersVoice);
            othersVoiceBadge.SetActive(hasUnreadOthersVoice);
            
            gameVoiceScroll.SetItems(gameVoiceScrollData.OrderBy(x => x.VoiceData.releaseTrustLevel).ToList());
            othersVoiceScroll.SetItems(othersVoiceScrollData.OrderBy(x => x.VoiceData.releaseTrustLevel).ToList());
        }
        
        public void OnOpenGameVoice()
        {
            long trustLevel = charaParentData.CharaParentBase.trustLevel;
            if (trustLevel == EncyclopediaPage.VoiceBadgeReadDictionary[CurrentParentMCharaId].gameVoiceReadLevel) return;
            EncyclopediaPage.VoiceBadgeReadDictionary[CurrentParentMCharaId].gameVoiceReadLevel = trustLevel;
            voiceBadge.SetActive(othersVoiceBadge.gameObject.activeSelf);
            LocalSaveManager.Instance.SaveData();
        }
        
        public void OnOpenOthersVoice()
        {
            long trustLevel = charaParentData.CharaParentBase.trustLevel;
            if (trustLevel == EncyclopediaPage.VoiceBadgeReadDictionary[CurrentParentMCharaId].othersVoiceReadLevel) return;
            EncyclopediaPage.VoiceBadgeReadDictionary[CurrentParentMCharaId].othersVoiceReadLevel = trustLevel;
            othersVoiceBadge.SetActive(false);
            voiceBadge.SetActive(false);
            LocalSaveManager.Instance.SaveData();
        }

        public void OpenVoiceTab()
        {
            encyclopediaTabSheetManager.OpenSheet(CharacterEncyclopediaTabSheetType.Voice, null);
            voiceTabSheetManager.OpenSheet(CharacterVoiceTabSheetType.Game, null);
            OnOpenGameVoice();
        }
        
        #region Debug
        private int debugCharaLibraryVoiceId;
        private void AddDebugCommand()
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.AddOption<int>(name: "確認したい m_chara_library_voice.id",
                getter: () =>
                {
                    return debugCharaLibraryVoiceId;
                },
                setter: (value) =>
                {
                    debugCharaLibraryVoiceId = value;
                    
                },
                category: PageType.Encyclopedia.ToString());
            
            CruFramework.DebugMenu.AddOption(name: "ボイス再生",
                callback: () =>
                {
                    VoiceManager.Instance.DebugPlayVoice(debugCharaLibraryVoiceId).Forget();
                },
                category: PageType.Encyclopedia.ToString());
#endif
        }

        private void RemoveDebugCommand()
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.RemoveOption(PageType.Encyclopedia.ToString());
#endif
        }
        #endregion
    }

}
