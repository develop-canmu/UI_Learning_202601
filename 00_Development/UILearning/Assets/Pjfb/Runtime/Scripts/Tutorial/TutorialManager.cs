
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using CruFramework.ResourceManagement;
using Pjfb.InGame;
using Pjfb.Runtime.Scripts.Utility;
using CodeStage.AntiCheat.Storage;
using CruFramework.Addressables;
using CruFramework.Page;
using Pjfb.Deck;
using Pjfb.Home;
using Pjfb.Storage;
using Pjfb.Menu;
using Pjfb.SystemUnlock;
using Pjfb.Utility;
using UnityEngine.UI;

namespace Pjfb
{
    public class TutorialManager : MonoBehaviour
    {
        #region const
        private const string TUTORIAL_SETTING_KEY = "Tutorial/TutorialSetting.asset";
        private const string TUTORIAL_IMAGE_SETTING_KEY = "Tutorial/TutorialImageSetting.asset";
        private const string TUTORIAL_TAP_EFFECT_PATH = "Prefabs/UI/Tutorial/TutorialTapEffect.prefab";
        private const string TUTORIAL_BANNER_KEY = "TutorialBanner";
        private const string TUTORIAL_POPUP_KEY = "TutorialPopUp";
        private const string TUTORIAL_GACHA_KEY = "Gacha";
        private const string TUTORIAL_STORY_KEY = "Story";
        private const string TUTORIAL_SUPPORT_CARD_KEY = "SupportCard";
        private const string TUTORIAL_EXTRA_SUPPORT_CARD_KEY = "ExtraSupportCard";
        private const string TUTORIAL_LIBRALY_KEY = "Libraly";
        private const string TUTORIAL_MISSION_KEY = "Mission";
        private const string TUTORIAL_CLUB_KEY = "Club";
        private const string TUTORIAL_PUSH_KEY = "Push";
        private const string TUTORIAL_RANK_MATCH_KEY = "RankMatch";
        private const string TUTORIAL_SKILLCONNECT_KEY = "SkillConnect";
        private const string TUTORIAL_CLUBMATCH_KEY = "ClubMatch";
        private const string TUTORIAL_CLUBMATCHDECK_KEY = "ClubMatchDeck";
        private const string TUTORIAL_TRAINING_SUPPORT_EQUIPMENT_KEY = "TrainingSupportEquipment";
        private const string TUTORIAL_CHARA_LEVEL_UP_BASE_MAX_KEY = "CharaLevelUpBaseMax";
        private const string TUTORIAL_LEAGUE_MATCH_HOW_TO_PLAY = "LeagueMatchHowToPlay";
        private const string TUTORIAL_AUTO_TRAINING_KEY = "AutoTraining";
        private const string TUTORIAL_TRAINING_DeckEnhance_KEY = "TrainingEnhance";
        
        private const int POPUP_STORY_VALUE = 0x01;
        private const int POPUP_GACHA_VALUE = 0x02;
        private const int POPUP_TRAINING_VALUE = 0x04;
        private const int POPUP_ENCYCLOPEDIA_VALUE = 0x08;
        private const int SCENARIO_ENCYCLOPEDIA_VALUE = 0x10;
        private const int POPUP_CLUB_VALUE = 0x20;
        private const int SCENARIO_CLUB_VALUE = 0x40;
        private const int POPUP_COLOSSEUM_VALUE = 0x80;
        private const int SCENARIO_COLOSSEUM_VALUE = 0x100;
        private const int POPUP_SKILLCONNECT_VALUE = 0x200;
        private const int SCENARIO_CLUB_MATCH_VALUE = 0x400;
        private const int POPUP_CLUB_MATCH_VALUE = 0x800;
        private const int POPUP_CLUB_DECK_VALUE = 0x1000;
        private const int SCENARIO_TRAINING_SUPPORT_EQUIPMENT_VALUE = 0x2000;
        private const int POPUP_TRAINING_SUPPORT_EQUIPMENT_VALUE = 0x4000;
        private const int POPUP_CHARA_LEVEL_UP_BASE_MAX_VALUE = 0x8000;
        private const int POPUP_LEAGUE_MATCH_HOW_TO_PLAY = 0x10000;
        private const int POPUP_TRAINING_EXTRA_SUPPORT_VALUE = 0x20000;
        private const int SCENARIO_LEAGUE_MATCH_VALUE = 0x40000;
        private const int SCENARIO_AUTO_TRAINING_VALUE = 0x80000;
        private const int POPUP_AUTO_TRAINING_VALUE = 0x100000;
        private const int POPUP_TRAINING_DeckEnhance_VALUE = 0x200000;
        
        private const int ADV_COLOSSEUM_ID = 99010;
        private const int ADV_CLUB_ID = 99011;
        private const int ADV_ENCYCLOPEDIA_ID = 99014;
        private const int ADV_CLUB_MATCH_ID = 99012;
        private const int ADV_TRAINING_SUPPORT_EQUIPMENT_ID = 99015;
        private const int ADV_LEAGUE_MATCH_ID = 99033;
        private const int ADV_AUTO_TRAINING_ID = 99047;
        
        #endregion
        
        [SerializeField] private GameObject backGroundObject;
        [SerializeField] private GameObject touchGuardObject;
        [SerializeField] private Transform focusObjectRoot;
        [SerializeField] private Transform tapEffectRoot;

        public Action OnExecuteTutorialAction;
        public Action OnCompletedExecuteTutorialAction;
        private readonly ResourcesLoader resourcesLoader = new ResourcesLoader();
        private TutorialSettings.Detail currentTutorialData;
        private TutorialSettings tutorialSettings;
        private TutorialImageSetting tutorialImageSetting;
        private GameObject tapEffect;
        private string userName;
        private long gender;
        private long mIconId;
        private GameObject copyObject;
        private GameObject focusObject;
        private int nextStepIndex;
        
        private int actionDataIndex = 0;
        private int progressIndex = 0;

        private void Awake()
        {
            backGroundObject.SetActive(false);
            touchGuardObject.SetActive(false);
        }

        private void ExitTutorial()
        {
            if (tapEffect != null)
            {
                Destroy(tapEffect);
                tapEffect = null;   
            }
            resourcesLoader.Release();
            tutorialSettings = null;
        }

        public bool IsSkippedTutorial()
        {
            // 初回ログイン時の確認モーダルでスキップしたか
            return UserDataManager.Instance.user.finishedTutorialNumberList.Contains((int)TutorialSettings.Step.SkippedTutorial);
        }

        public bool IsCompleteTutorial()
        {
            if (tutorialSettings == null)
            {
                return true;
            }
            var stepIds = UserDataManager.Instance.user.finishedTutorialNumberList;
            for (var i = 0; i < tutorialSettings.detailList.Count; i++)
            {
                var stepId = (int)tutorialSettings.detailList[i].stepId;
                if (!stepIds.Contains(stepId))
                {
                    return false;
                }
            }
            ExitTutorial();
            return true;
        }

        private TutorialSettings.Detail GetNextTutorialDetail()
        {
            if (tutorialSettings == null) return null;
            var detailList = tutorialSettings.detailList;
            var stepIds = UserDataManager.Instance.user.finishedTutorialNumberList;
            for (var i = nextStepIndex; i < detailList.Count; i++)
            {
                var stepId = (int)tutorialSettings.detailList[i].stepId;
                if (!stepIds.Contains(stepId))
                {
                    nextStepIndex = i + 1;
                    return tutorialSettings.detailList[i];
                }
            }
            return null;
        }

        private async UniTask UpdateTutorialStep(TutorialSettings.Detail detail)
        {
            var request = new UserSaveFinishedTutorialAPIRequest();
            var post = new UserSaveFinishedTutorialAPIPost();
            post.tutorialNumberList = new [] { (long)detail.stepId };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            UserDataManager.Instance.user.UpdateFinishedTutorialStep(response.finishedTutorialNumberList);
        }
        
        private async UniTask UpdateTutorialStep(long[] stepIds)
        {
            var request = new UserSaveFinishedTutorialAPIRequest();
            var post = new UserSaveFinishedTutorialAPIPost();
            post.tutorialNumberList = stepIds;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            UserDataManager.Instance.user.UpdateFinishedTutorialStep(response.finishedTutorialNumberList);
        }

        public async UniTask FinishTutorial()
        {
            actionDataIndex = 0;
            progressIndex = 0;
            OnExecuteTutorialAction = null;
            
            OnCompletedExecuteTutorialAction = null;

            var skipUpdateStep = currentTutorialData?.skipUpdateStep ?? true;

            if (!skipUpdateStep) await UpdateTutorialStep(currentTutorialData);

            touchGuardObject.SetActive(false);
            backGroundObject.SetActive(false);
            if (IsCompleteTutorial())
            {
                // トークン
                CancellationToken token = AppManager.Instance.UIManager.PageManager.CurrentPageObject.GetCancellationTokenOnDestroy();
                // アセットラベル
                string downloadKey = AddressablesUtility.RemotePreLoadBundleKey;
                
                // ダウンロード情報取得
                AddressablesManager.DownloadInfo downloadInfo = await AddressablesUtility.GetDownloadInfoAsync(downloadKey, token);

                if (downloadInfo.DownloadSize > 0)
                {
                    // ダウンロード完了後にホームに遷移
                    TipsPageArgs tipsArgs = new TipsPageArgs();
                    tipsArgs.PageType = PageType.Home;
                    tipsArgs.Stack = true;
                    tipsArgs.PreTask = async () =>
                    {
                        // Tipsページのトークンを取得
                        CancellationToken taskToken = AppManager.Instance.UIManager.PageManager.CurrentPageObject.GetCancellationTokenOnDestroy();
                        // アセットダウンロード
                        await AddressablesUtility.EssentialAssetDownloadAsync(downloadInfo, taskToken);
                    };
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Tips, true, tipsArgs);
                }
                else
                {
                    // そのままホームに遷移する
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Home, true, null);
                }
            }
            else
            {
                StartTutorial();
            }
        }

        private const int SummonCostMax = ushort.MaxValue;

        private async UniTask OnCompleteReviewActionAsync()
        {
            HideTouchGuard();
            await OpenTutorialPopupAsync(TUTORIAL_PUSH_KEY);
            
            // FCMプッシュ初期化
            await FCMPushNotificationUtility.InitializePushStateAsync(this.GetCancellationTokenOnDestroy());
            await OpenTutorialPopupAsync(TUTORIAL_MISSION_KEY);
            await ExecuteTutorialAction();
        }

        public TutorialSettings.Detail InitializeTutorialParams()
        {
            currentTutorialData = GetNextTutorialDetail();
            progressIndex = currentTutorialData.startProgressIndex;
            var touchGuard = currentTutorialData.TutorialPageType != PageType.TutorialAdv &&
                             currentTutorialData.TutorialPageType != PageType.TutorialTraining &&
                             currentTutorialData.TutorialPageType != PageType.TutorialNewInGame;
            touchGuardObject.SetActive(touchGuard);
            return currentTutorialData;
        }

        public void StartTutorial()
        {
            InitializeTutorialParams();
            var pageType = currentTutorialData.TutorialPageType; 
            AppManager.Instance.UIManager.PageManager.OpenPage(pageType, true, currentTutorialData);
        }
        
        public async UniTask InitializeManagerAsync()
        {
            nextStepIndex = 0;
            var asset = await resourcesLoader.LoadAssetAsync<TutorialSettings>(TUTORIAL_SETTING_KEY);
            if (asset != null)
            {
                tutorialSettings = Instantiate(asset);
            }

            var tapEffectPrefab = await resourcesLoader.LoadAssetAsync<GameObject>(TUTORIAL_TAP_EFFECT_PATH);
            if (tapEffectPrefab != null)
            {
                tapEffect = Instantiate(tapEffectPrefab, tapEffectRoot);
                tapEffect.SetActive(false);
            }
        }

        private async UniTask ExitAction(Vector3 scale, Action callback)
        {
            if (focusObject != null)
            {
                focusObject.transform.localScale = scale;
                focusObject = null;
            }
            DeleteFocusObject();
            var actionData = currentTutorialData.actionDataList[actionDataIndex];
            actionDataIndex++;
            OnCompletedExecuteTutorialAction?.Invoke();
            if (actionData.autoNext)
            {
                await ExecuteTutorialAction(callback);
            }
            else
            {
                callback?.Invoke();
                backGroundObject.SetActive(false);
            }
        }
        
        private async UniTask ExitAction(Action callback)
        {
            await ExitAction(Vector3.zero, callback);
        }

        private void DeleteFocusObject()
        {
            if (copyObject != null)
            {
                Destroy(copyObject);
                copyObject = null;
            }

            if (tapEffect != null)
            {
                tapEffect.SetActive(false);
            }
        }

        private async UniTask OpenTutorialModal(TutorialSettings.ActionData data, Action callback)
        {
            var modalParamIndex = data.intParams[0];
            var modalDetail = tutorialSettings.GetModalDetail(modalParamIndex);
            var titleText = "";
            var bodyText = "";
            
            if (modalDetail != null)
            {
                titleText = StringValueAssetLoader.Instance[modalDetail.titleKey];
                bodyText = StringValueAssetLoader.Instance[modalDetail.bodyKey];
            }
            
            var window = await ConfirmModalWindow.OpenAsync(new ConfirmModalData(
               titleText, bodyText,"",
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    window =>
                    {
                        window.Close();
                    })),this.GetCancellationTokenOnDestroy());
            var prev = touchGuardObject.activeSelf;
            touchGuardObject.SetActive(false);
            await window.WaitCloseAsync();
            touchGuardObject.SetActive(prev);
            await ExitAction(callback);
        }

        private async UniTask OpenImageModal(TutorialSettings.ActionData data, Action callback, ModalOptions options = ModalOptions.None)
        {
            var assetPath = data.stringParams[0];
            assetPath = ResourcePathManager.GetPath(TUTORIAL_BANNER_KEY, assetPath);
            var window = await ImageModal.OpenModalAsync(assetPath, options);
            var prev = touchGuardObject.activeSelf;
            touchGuardObject.SetActive(false);
            await window.WaitCloseAsync();
            touchGuardObject.SetActive(prev);
            await ExitAction(callback);
        }

        private async UniTask<bool> OpenImageModal(string assetPath, ModalOptions options = ModalOptions.None)
        {
            assetPath = ResourcePathManager.GetPath(TUTORIAL_BANNER_KEY, assetPath);
            var window = await ImageModal.OpenModalAsync(assetPath, options);
            await window.WaitCloseAsync();
            return true;
        }
        
        private void FocusUiButton(TutorialSettings.ActionData actionData, Action callback)
        {
            DeleteFocusObject();
            var findName = actionData.stringParams[0];
            focusObject = GameObject.Find(findName);
            CopyFocusObject(actionData,callback,focusObject);
        }

        private async UniTask FocusUiRoot(TutorialSettings.ActionData actionData, Action callback)
        {
            DeleteFocusObject();
            var findName = actionData.stringParams[0];
            var waitTime = actionData.intParams[0];
            focusObject = GameObject.Find(findName);
            copyObject = Instantiate(focusObject, backGroundObject.transform);
            copyObject.transform.position = focusObject.transform.position;
            await UniTask.Delay(waitTime);
            await ExitAction(Vector3.one, callback);
        }

        private void CopyFocusObject(TutorialSettings.ActionData actionData, Action callback, GameObject focusObject)
        {
            
            copyObject = Instantiate(focusObject, focusObjectRoot); 
            var scale = focusObject.transform.localScale;
            focusObject.transform.localScale = Vector3.zero;
            var animator = copyObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
            var copyButton = copyObject.GetComponentInChildren<UIButton>();
            copyButton.interactable = true;
            copyObject.transform.position = focusObject.transform.position;
            UpdateTapEffectPosition(copyObject);
            copyButton.ResetLongTapEvents();

            if (actionData.deleteButtonEvent)
            {
                copyButton.ResetClickEvents();
            }

            
            // レイヤー調整のためキャンバスを削除
            Canvas canvas = copyObject.GetComponent<Canvas>();
            if(canvas != null)
            {
                // GraphicRaycasterの削除
                GraphicRaycaster graphicRaycaster = copyObject.GetComponent<GraphicRaycaster>();
                if(graphicRaycaster != null)
                {
                    GameObject.Destroy( graphicRaycaster );
                }
                // Canvas削除
                GameObject.Destroy(canvas);
            }
            
            UIButton focusButton = null;
            if (actionData.invokeBaseButtonEvent)
            {
                focusButton = focusObject.GetComponentInChildren<UIButton>();
            }
            
            copyButton.onClick.AddListener(()=>
            {
                // 非永続的な入力イベントのみだとSEならないのでこちらで再生
                if (copyButton.OnLongTap.GetPersistentEventCount() <= 0)
                {
                    SEManager.PlaySE(copyButton.SoundType);
                }
                focusButton?.onClick?.Invoke();
                ExitAction(scale,callback).Forget();
            });
        }
        
        private void FocusCharacterIcon(TutorialSettings.ActionData actionData,Action callback)
        {
            DeleteFocusObject();
            var findName = actionData.stringParams[0];
            var targetId = actionData.intParams[0];
            var rootObject = GameObject.Find(findName);
            var characterObjects = rootObject.GetComponentsInChildren<CharacterScrollItem>();
            focusObject = characterObjects.FirstOrDefault(icon => targetId == icon.characterData.CharacterId)?.gameObject;
            CopyFocusObject(actionData,callback,focusObject);
        }
        
        public bool ValidateDigestTypeCondition(TutorialSettings.DigestTriggerType digestTriggerType, BattleConst.DigestType digestType)
        {
            var actionDataList = currentTutorialData.actionDataList;
            if (actionDataIndex >= actionDataList.Count)
            {
                return false;
            }
            var actionData = actionDataList[actionDataIndex];
            if (actionData.digestTriggerType != digestTriggerType)
            {
                return false;
            }
            if (!ComparisonDigestType(actionData.digestTypeCondition,digestType) || actionData.digestTypeCondition == BattleConst.DigestType.None)
            {
                return false;
            }
            return true;
        }

        private bool ComparisonDigestType(BattleConst.DigestType targetDigestType,  BattleConst.DigestType digestType)
        {
            if (targetDigestType == BattleConst.DigestType.ShootBlockL ||
                targetDigestType == BattleConst.DigestType.ShootBlockR)
            {
                return digestType == BattleConst.DigestType.ShootBlockL ||
                       digestType == BattleConst.DigestType.ShootBlockR;
            }
            
            if (targetDigestType == BattleConst.DigestType.ShootBlockTouchL ||
                targetDigestType == BattleConst.DigestType.ShootBlockTouchR)
            {
                return digestType == BattleConst.DigestType.ShootBlockTouchL ||
                       digestType == BattleConst.DigestType.ShootBlockTouchR;
            }
            
            if (targetDigestType == BattleConst.DigestType.ShootBlockNotReachL ||
                targetDigestType == BattleConst.DigestType.ShootBlockNotReachR)
            {
                return digestType == BattleConst.DigestType.ShootBlockNotReachL ||
                       digestType == BattleConst.DigestType.ShootBlockNotReachR;
            }
            return targetDigestType == digestType;
        }

        public async UniTask ExecuteTutorialAction(Action callback = null)
        {
            var actionDataList = currentTutorialData.actionDataList;
            if (actionDataIndex >= actionDataList.Count)
            {
                await FinishTutorial();
                return;
            }
            
            var actionData = actionDataList[actionDataIndex];
            var isShadeBg = ValidateShadeBg(actionData.actionType);
            // 次が暗転不要 or delay挟むならdelay開始前に開け
            if (!isShadeBg || actionData.delay > 0) backGroundObject.SetActive(false);
            
            if (actionData.actionType == TutorialSettings.ActionType.Message)
            {
                return;
            }
            
            await UniTask.Delay(actionData.delay);
            
            if (actionDataIndex >= actionDataList.Count)
            {
                // delayしているタイミングのスキップ実行でindex変わる場合もあるのでfaileSafe
                return;
            }

            if (currentTutorialData.TutorialPageType == PageType.TutorialNewInGame)
            {
                // インゲームのモーダルが開かれていた場合はそれを閉じてチュートリアル進行
                var modal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
                if (modal != null)
                {
                    AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                    await modal.WaitCloseAsync();
                }   
            }

            // 暗転必要ならdelay後に表示
            if (isShadeBg) backGroundObject.SetActive(true);
            OnExecuteTutorialAction?.Invoke();
            
            var type = actionData.actionType;

            if (currentTutorialData.TutorialPageType != PageType.TutorialAdv)
            {
                touchGuardObject.SetActive(!actionData.disableTouchGuard);   
            }

            switch (type)
            {
                case TutorialSettings.ActionType.PlayAdv:
                    // TODO シナリオ進行はとりあえすTutorialAdvページ限定で進行
                    break;
                case TutorialSettings.ActionType.Focus:
                    FocusUiButton(actionData,callback);
                    break;
                case TutorialSettings.ActionType.Modal:
                    await OpenTutorialModal(actionData,callback);
                    break;
                case TutorialSettings.ActionType.Image:
                    await OpenImageModal(actionData,callback);
                    break;
                case TutorialSettings.ActionType.FocusCharacterIcon:
                    FocusCharacterIcon(actionData,callback);
                    break;
                case TutorialSettings.ActionType.FocusRoot:
                    await FocusUiRoot(actionData, callback);
                    break;
                case TutorialSettings.ActionType.Skip:
                    await ExitAction(callback);
                    break;
                case TutorialSettings.ActionType.ExitHome:
                    ExitTutorialAction();
                    break;
            }
        }

        public void SkipTutorialAction()
        {
            actionDataIndex = int.MaxValue;
        }

        private void ExitTutorialAction()
        {
            actionDataIndex++;
            AdjustManager.TrackEvent(AdjustManager.TrackEventType.ClearTutorialPatternA);
            var token = new CancellationToken();
            StoreAppReviewUtility.OpenReviewWindow(ret =>
            {
                OnCompleteReviewActionAsync().Forget();
            }, token);
        }

        public int GetNextScenarioId()
        {
            var actionDataList = currentTutorialData.actionDataList;
            if (actionDataIndex >= actionDataList.Count)
            {
                FinishTutorial().Forget();
                return 0;
            }
            var actionData = actionDataList[actionDataIndex];
            if (actionData.actionType != TutorialSettings.ActionType.PlayAdv)
            {
                ExecuteTutorialAction().Forget();
                return 0;
            }            
            var scenarioId = actionData.intParams[0];
            actionDataIndex++;
            return scenarioId; 
        }
        
        public CharaV2FriendLend GetTutorialFriendData()
        {
            if (tutorialSettings == null)
            {
                return null;
            }
            return tutorialSettings.tutorialFriendDate;
        }

        public int GetTutorialTrainingScenarioId()
        {
            if (tutorialSettings == null)
            {
                return 0;
            }

            return tutorialSettings.tutorialTrainingScenarioId;
        }
        
        public int GetTutorialTrainingSupportDeckNumber()
        {
            if (tutorialSettings == null)
            {
                return 0;
            }

            return tutorialSettings.tutorialTrainingSupportDeckNumber;
        }

        public BattleV2ClientData GetBattleV2ClientData(int progressIndex)
        {
            if (tutorialSettings == null ||
                progressIndex >= tutorialSettings.battleV2ClientDataList.Count)
            {
                return new BattleV2ClientData();
            }
            return tutorialSettings.battleV2ClientDataList[progressIndex];
        }

        public void HideTouchGuard()
        {
            touchGuardObject.SetActive(false);
        }
        
        public void ShowTouchGuard()
        {
            touchGuardObject.SetActive(true);
        }

        private bool ValidateShadeBg(TutorialSettings.ActionType actionType)
        {
            switch (actionType)
            {
                case TutorialSettings.ActionType.Focus:
                case TutorialSettings.ActionType.FocusRoot:
                case TutorialSettings.ActionType.FocusCharacterIcon:
                    return true;
                default:
                    return false;
            }
        }

        private void UpdateTapEffectPosition(GameObject target)
        {
            if (tapEffect is null)
            {
                return;
            }
            
            var rectTransform = target.transform as RectTransform;
            if (rectTransform is null)
            {
                return;
            }
            
            // anchorが異なる場合があるので補正
            var position = rectTransform.position;
            var diff = new Vector3(
                Mathf.Lerp(-rectTransform.rect.size.x / 2f, rectTransform.rect.size.x / 2f, rectTransform.pivot.x) * rectTransform.transform.lossyScale.x,
                Mathf.Lerp(-rectTransform.rect.size.y / 2f, rectTransform.rect.size.y / 2f, rectTransform.pivot.y) * rectTransform.transform.lossyScale.y
            );
            tapEffect.transform.position = position - diff;
            tapEffect.SetActive(true);
        }

        private async UniTask LowSpecDeviceSetting()
        {
            if (!DeviceUtility.IsLowSpecDevice()) return;
            
            LocalSaveManager.saveData.appConfig.fieldDisplayType = (int)AppConfig.DisplayType.Light;
            LocalSaveManager.saveData.appConfig.trainingDisplayType = (int)AppConfig.DisplayType.Light;
            LocalSaveManager.Instance.SaveData();

            ConfirmModalData modalData = new ConfirmModalData
            {
                Title = StringValueAssetLoader.Instance["tutorial.device.title"],
                Message = StringValueAssetLoader.Instance["tutorial.device.body"],
                PositiveButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.ok"],
                    window =>
                    {
                        window.Close();
                    }
                )
            };
            
            var window = await ConfirmModalWindow.OpenAsync(modalData,this.GetCancellationTokenOnDestroy());
            await window.WaitCloseAsync();
        }

        public async UniTask ConfirmSkipTutorial()
        {
            var completeStepIds = UserDataManager.Instance.user.finishedTutorialNumberList;
            if (completeStepIds.Any(step => step == (int)TutorialSettings.Step.SkipTutorial))
            {
                // スキップモーダルを通過済み
                return;
            }
            
            bool isSkip = false;
            HideTouchGuard();
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            
            // 低スペック端末チェック
            await LowSpecDeviceSetting();
            
            var window = await ConfirmModalWindow.OpenAsync(new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                StringValueAssetLoader.Instance["tutorial.skip.body"],
                "",
                new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["tutorial.skip.positive"],
                    window =>
                    {
                        isSkip = false;
                        window.Close();
                    }),
                new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["tutorial.skip.negative"],
                    window =>
                    {
                        isSkip = true;
                        window.Close();
                    })),
                this.GetCancellationTokenOnDestroy());
            
            await window.WaitCloseAsync();

            if (isSkip)
            {
                await InitializeUserDataAsync(false);
                // 通信エラー等で正常な入力が行えていない場合はスキップしない
                if (!ValidateUserData()) return;
                var skippedId = new[] { (long)TutorialSettings.Step.SkippedTutorial };
                var stepIds = tutorialSettings.detailList
                    .Where(detail => detail.stepId != TutorialSettings.Step.ExitTutorial)
                    .Select(detail => (long)detail.stepId).Concat(skippedId).ToArray();
                // 吉良くんの解放を行ってチュートリアル全件終了
                await CharaPieceToCharaAsync(stepIds);
            }
            else
            {
                await UpdateTutorialStep(new[] { (long)TutorialSettings.Step.SkipTutorial });
            }
        }
        
        #region 初回遷移ポップアップ

        public async UniTask<string[]> GetTutorialImagePath(string key)
        {
            if (tutorialImageSetting == null)
            {
                var asset = await resourcesLoader.LoadAssetAsync<TutorialImageSetting>(TUTORIAL_IMAGE_SETTING_KEY);
                if (asset != null)
                {
                    tutorialImageSetting = Instantiate(asset);
                }
            }
            return tutorialImageSetting.GetImagePath(key);
        }
        
        public async UniTask OnClosedTutorialScenario(PageType pageType, bool stack, object args)
        {
            await AppManager.Instance.UIManager.PageManager.OpenPageAsync(pageType, stack, args);
        }
        
        public async UniTask OpenSkillConnectTutorialAsync()
        {
            // 演出と結びつくCollectionSkillの解放状況を参照
            await OpenTutorialPopUpSystemUnlockAsync(SystemUnlockDataManager.SystemUnlockNumber.CollectionSkill, POPUP_SKILLCONNECT_VALUE, TUTORIAL_SKILLCONNECT_KEY);
            
        }
        
        public async UniTask OpenTrainingDeckEnhanceTutorialAsync()
        {
            await OpenTutorialPopUpSystemUnlockAsync(SystemUnlockDataManager.SystemUnlockNumber.TrainingDeckEnhance, POPUP_TRAINING_DeckEnhance_VALUE, TUTORIAL_TRAINING_DeckEnhance_KEY);
        }
        
        public async UniTask OpenClubDeckTutorialAsync()
        {
            await OpenTutorialPopupAsync(POPUP_CLUB_DECK_VALUE,TUTORIAL_CLUBMATCHDECK_KEY);
        }


        public async UniTask OpenClubMatchHelpAsync()
        {
            await OpenTutorialPopupAsync(TUTORIAL_CLUBMATCH_KEY);
        }
        
        public async UniTask OpenTrainingSupportEquipmentTutorialAsync()
        {
            await OpenTutorialPopupAsync(POPUP_TRAINING_SUPPORT_EQUIPMENT_VALUE, TUTORIAL_TRAINING_SUPPORT_EQUIPMENT_KEY);
        }

        public async UniTask OpenClubMatchTutorialAsync()
        {

            var key = POPUP_CLUB_MATCH_VALUE;

            if (ValidatePopup(key))
            {
                 return;   
            }

            await OpenClubMatchHelpAsync();
            
            var title = StringValueAssetLoader.Instance["clubmatch.tutorial.deck.title"];
            var body = StringValueAssetLoader.Instance["clubmatch.tutorial.deck.body"];
            
            ConfirmModalData data = new ConfirmModalData(
                title,
                body,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.yes"], async (window) =>
                {
                    var deck = await DeckUtility.GetClubBattleDeck();
                    var partyNumber = deck.DeckDataList.FirstOrDefault()?.Deck.partyNumber ?? 0;
                    window.Close();
                    if (partyNumber == 0)
                    {
                        return;
                    }
                    var param = new DeckPageParameters{InitialPartyNumber = partyNumber, OpenFrom = PageType.ClubMatch};
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubDeck, true, param);
                } ),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.no"], window => window.Close())
            );
            
            var window = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
            await window.WaitCloseAsync();
            CheckedPopup(key);
            
        }
        
        public async UniTask OpenLeagueMatchTutorialAsync()
        {
            var key = POPUP_LEAGUE_MATCH_HOW_TO_PLAY;

            if (ValidatePopup(key))
            {
                return;   
            }

            await OpenLeagueMatchHowToPlayTutorialAsync();
        }

        /// <summary>
        /// 自動トレーニングの画像ポップアップの表示
        /// </summary>
        public async UniTask OpenAutoTrainingTutorialAsync()
        {
            await OpenTutorialPopUpSystemUnlockAsync(SystemUnlockDataManager.SystemUnlockNumber.AutoTraining, POPUP_AUTO_TRAINING_VALUE, TUTORIAL_AUTO_TRAINING_KEY);
        }
        
        public async UniTask OpenMemoryBollTutorialAsync()
        {
            await OpenTutorialPopupAsync(POPUP_GACHA_VALUE,TUTORIAL_GACHA_KEY);
        }
        
        public async UniTask OpenClubTutorialAsync()
        {
            await OpenTutorialPopUpSystemUnlockAsync(SystemUnlockDataManager.SystemUnlockNumber.Club, POPUP_CLUB_VALUE, TUTORIAL_CLUB_KEY);
        }
        
        public async UniTask OpenColosseumTutorialAsync()
        {
            await OpenTutorialPopUpSystemUnlockAsync(SystemUnlockDataManager.SystemUnlockNumber.Pvp, POPUP_COLOSSEUM_VALUE, TUTORIAL_RANK_MATCH_KEY);
        }
        
        public async UniTask OpenStoryTutorialAsync()
        {
            await OpenTutorialPopupAsync(POPUP_STORY_VALUE,TUTORIAL_STORY_KEY);
        }
        
        public async UniTask OpenCharaLevelUpBaseMaxTutorialAsync()
        {
            await OpenTutorialPopupForceAsync(POPUP_CHARA_LEVEL_UP_BASE_MAX_VALUE, TUTORIAL_CHARA_LEVEL_UP_BASE_MAX_KEY);
        }
        
        public async UniTask OpenLeagueMatchHowToPlayTutorialAsync()
        {
            await OpenTutorialPopupForceAsync(POPUP_LEAGUE_MATCH_HOW_TO_PLAY, TUTORIAL_LEAGUE_MATCH_HOW_TO_PLAY, ModalOptions.KeepFrontModal);
        }

        public async UniTask OpenTrustValueTutorialAsync()
        {
            await OpenTutorialPopupAsync(POPUP_ENCYCLOPEDIA_VALUE,TUTORIAL_LIBRALY_KEY);
        }
        
        /// <summary>
        /// 自動トレーニングのADVの再生
        /// </summary>
        public bool OpenAutoTrainingScenarioTutorial(bool stack, object args)
        {
            return OpenScenarioTutorialWithUnlockSystem(PageType.TrainingPreparation, stack, args, SystemUnlockDataManager.SystemUnlockNumber.AutoTraining, SCENARIO_AUTO_TRAINING_VALUE, ADV_AUTO_TRAINING_ID);
        }
        
        /// <summary>解放演出がある機能のチュートリアルが再生済みかどうか</summary>
        public bool IsAlreadyPlayedTutorial(SystemUnlockDataManager.SystemUnlockNumber systemUnlockNumber)
        {
            switch(systemUnlockNumber)
            {
                case SystemUnlockDataManager.SystemUnlockNumber.AutoTraining: return ValidatePopup(SCENARIO_AUTO_TRAINING_VALUE) && ValidatePopup(POPUP_AUTO_TRAINING_VALUE);
                default: throw new NotImplementedException("解放演出番号が定義されてません");
            }
        }
        
        public bool OpenScenarioTutorial(PageType type, bool stack, object args)
        {
            var saveValue = GetTutorialAdvSaveValue(type);
            if (ValidatePopup(saveValue))
            {
                return false;
            }
            CheckedPopup(saveValue);
            var param = CreateTutorialAdvParam(type, stack, args);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TutorialAdv, false, param);
            AppManager.Instance.UIManager.Header.Show();
            AppManager.Instance.UIManager.Footer.Show();
            return true;
        }
        
        /// <summary>
        /// 解放演出を伴うADVの再生に使用します
        /// </summary>
        public bool OpenScenarioTutorialWithUnlockSystem(PageType type, bool stack, object args, SystemUnlockDataManager.SystemUnlockNumber systemUnlockNumber)
        {
            var saveValue = GetTutorialAdvSaveValue(type);
            var scenarioId = GetScenarioIdFromPageType(type);
            return OpenScenarioTutorialWithUnlockSystem(type, stack, args, systemUnlockNumber, saveValue, scenarioId);
        }
        
        /// <summary>
        /// 解放演出を伴うADVの再生に使用します
        /// </summary>
        private bool OpenScenarioTutorialWithUnlockSystem(PageType type, bool stack, object args, SystemUnlockDataManager.SystemUnlockNumber systemUnlockNumber, int saveValue, int scenarioId)
        {
            // ADV再生後、一時既読フラグが立っていた場合、ローカル再生済みにする
            // 解放済みかつ、ローカル再生済みの場合スキップ
            if (SystemUnlockDataManager.Instance.IsReadScenario(saveValue) ||
                UserDataManager.Instance.IsUnlockSystem((long)systemUnlockNumber) && ValidatePopup(saveValue))
            {
                return false;
            }
            
            // 一時既読フラグを立てる
            SystemUnlockDataManager.Instance.UpdateTempReadScenarioValue(saveValue);
            CheckedPopup(saveValue);
            var param = CreateTutorialAdvParam(type, stack, args, scenarioId);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TutorialAdv, false, param);
            AppManager.Instance.UIManager.Header.Show();
            AppManager.Instance.UIManager.Footer.Show();
            return true;
        }
        
        /// <summary>ページタイプからシナリオIDを取得</summary>
        private int GetScenarioIdFromPageType(PageType pageType)
        {
            switch (pageType)
            {
                case PageType.Encyclopedia: return ADV_ENCYCLOPEDIA_ID;
                case PageType.Club: return ADV_CLUB_ID;
                case PageType.Colosseum: return ADV_COLOSSEUM_ID;
                case PageType.ClubMatch: return ADV_CLUB_MATCH_ID;
                case PageType.TrainingPreparation: return ADV_TRAINING_SUPPORT_EQUIPMENT_ID;
                case PageType.LeagueMatch: return ADV_LEAGUE_MATCH_ID;
                default: throw new NotImplementedException("PageTypeが定義されてません");
            }
        }

        /// <summary>
        /// ADVパラメータの設定
        /// </summary>
        private TutorialAdvPage.SceneParam CreateTutorialAdvParam(PageType pageType, bool stack, object args)
        {
            int scenarioId = GetScenarioIdFromPageType(pageType);
            return CreateTutorialAdvParam(pageType, stack, args, scenarioId);
        }

        /// <summary>
        /// ADVパラメータの設定　(ScenarioIdをPageTypeに依存せずに設定)
        /// </summary>
        private TutorialAdvPage.SceneParam CreateTutorialAdvParam(PageType pageType, bool stack, object args, int scenarioId)
        {
            var ret = new TutorialAdvPage.SceneParam();
            ret.nextPageType = pageType;
            ret.args = args;
            ret.stack = stack;
            ret.scenarioId = scenarioId;
            return ret;
        }

        private int GetTutorialAdvSaveValue(PageType pageType)
        {
            switch (pageType)
            {
                case PageType.Encyclopedia:
                    return SCENARIO_ENCYCLOPEDIA_VALUE;
                case PageType.Club:
                    return SCENARIO_CLUB_VALUE;
                case PageType.Colosseum:
                    return SCENARIO_COLOSSEUM_VALUE;
                case PageType.ClubMatch:
                    return SCENARIO_CLUB_MATCH_VALUE;
                case PageType.TrainingPreparation:
                    return SCENARIO_TRAINING_SUPPORT_EQUIPMENT_VALUE;
                case PageType.LeagueMatch:
                    return SCENARIO_LEAGUE_MATCH_VALUE;
            }
            return 0;
        }

        public async UniTask OpenSupportCardTutorialAsync()
        {
            if (DeckUtility.GetSpecialSupportCharacterIndex(DeckType.Training,false).Length <= 0)
            {
                return;
            }
            await OpenTutorialPopupAsync(POPUP_TRAINING_VALUE,TUTORIAL_SUPPORT_CARD_KEY);
        }
        
        public async UniTask OpenExtraSupportCardTutorialAsync()
        {
            if (DeckUtility.GetExtraSupportCharacterIndex(DeckType.Training,false).Length <= 0)
            {
                return;
            }
            
            await OpenTutorialPopupAsync(POPUP_TRAINING_EXTRA_SUPPORT_VALUE,TUTORIAL_EXTRA_SUPPORT_CARD_KEY);
        }

        private UniTask OpenTutorialPopupAsync(int key, string imageKey)
        {
            if (ValidatePopup(key))
            {
                return UniTask.CompletedTask;
            }

            return OpenTutorialPopupForceAsync(key, imageKey);
        }
        
        private async UniTask OpenTutorialPopupForceAsync(int key, string imageKey, ModalOptions options = ModalOptions.None)
        {
            var assetPath = await GetTutorialImagePath(imageKey);

            foreach (var path in assetPath)
            {
                await OpenImageModal(path, options);   
            }

            CheckedPopup(key);
        }
        
        private async UniTask OpenTutorialPopupAsync(string imageKey)
        {
            var assetPath = await GetTutorialImagePath(imageKey);
            foreach (var path in assetPath)
            {
                await OpenImageModal(path);   
            }
        }

        /// <summary>
        /// 汎用解放演出の対象となる機能のイメージモーダルのポップアップを開く
        /// </summary>
        /// <param name="systemNumber">機能番号</param>
        /// <param name="key">ポップアップキー</param>
        /// <param name="imageKey">イメージモーダルキー</param>
        private async UniTask OpenTutorialPopUpSystemUnlockAsync(SystemUnlockDataManager.SystemUnlockNumber systemNumber, int key, string imageKey)
        {
            // 機能解放済みの場合return
            if (UserDataManager.Instance.IsUnlockSystem((long)systemNumber) && ValidatePopup(key))
            {
                return;
            }
            
            await OpenTutorialPopupAsync(imageKey);
            CheckedPopup(key);

            if (!UserDataManager.Instance.IsUnlockSystem((long)systemNumber))
            {
                await SystemUnlockDataManager.Instance.RequestReadUnlockEffectAsync();   
            }
        }
        
        private bool ValidatePopup(int popupKey)
        {
            var saved = ObscuredPrefs.Get<int>(TUTORIAL_POPUP_KEY, 0);
            return (saved & popupKey) != 0;
        }
        
        private void CheckedPopup(int popupKey)
        {
            var saveValue = ObscuredPrefs.Get<int>(TUTORIAL_POPUP_KEY, 0);
            saveValue |= popupKey;
            ObscuredPrefs.Set<int>(TUTORIAL_POPUP_KEY,saveValue);
        }
        
        #endregion
 
        #region ホーム
        public TutorialSettings.HomeGetDataContainer GetHomeGetDataContainer()
        {
            if (tutorialSettings == null)
            {
                return new TutorialSettings.HomeGetDataContainer();
            }

            return tutorialSettings.homeData;
        }
        #endregion

        #region インゲーム
        public TutorialSettings.InGameSettingData GetBattleScenarioData()
        {
            return tutorialSettings.inGameSettingDataList[progressIndex];
        }

        public TutorialSettings.InGameMessageContainer ExecuteTutorialMessageAction(BattleConst.DigestType digestType, bool skipAddCount = false)
        {
            var actionDataList = currentTutorialData.actionDataList;
            if (actionDataIndex >= actionDataList.Count)
            {
                return null;
            }

            var actionData = actionDataList[actionDataIndex];
            var type = actionData.actionType;
            if (type != TutorialSettings.ActionType.Message)
            {
                return null;
            }

            if (actionData.digestTriggerType != TutorialSettings.DigestTriggerType.Message)
            {
                return null;
            }
            
            if (!ComparisonDigestType(actionData.digestTypeCondition,digestType))
            {
                return null;
            }

            var index = actionData.intParams[0];

            if (!skipAddCount)
            {
                ExitAction(null).Forget();
            }

            return tutorialSettings.inGameMessageContainerList[index];
        }
    
        #endregion

        #region トレーニング

        public TrainingProgressAPIResponse GetTrainingProgressData()
        {
            var res = new TrainingProgressAPIResponse();
            if (tutorialSettings == null || progressIndex >= tutorialSettings.trainingProgressDataList.Count)
            {
                return res;
            }
            
            var progressData = tutorialSettings.trainingProgressDataList[progressIndex];
            res.code = progressData.code;
            res.eventReward = progressData.eventReward;
            res.trainingEvent = progressData.trainingEvent;
            res.pending = progressData.pending;
            res.charaVariable = progressData.charaVariable;
            res.battlePending = progressData.battlePending;
            
            progressIndex++;

            return res;
        }

        public List<int> GetPreloadAdvIdList()
        {
            return tutorialSettings != null ? tutorialSettings.preloadAdvIdList : new List<int>();
        }

        #endregion

        #region ライバルリーバトル

        public TutorialSettings.HuntFinishContainer GetHuntFinishContainer()
        {
            if (tutorialSettings == null)
            {
                return new TutorialSettings.HuntFinishContainer();
            }
            var currentData = currentTutorialData;
            return tutorialSettings.huntFinishContainerList[currentData.startProgressIndex];
        }
        
        public TutorialSettings.HuntChoicePrizeContainer GetHuntChoicePrizeContainer()
        {
            if (tutorialSettings == null)
            {
                return new TutorialSettings.HuntChoicePrizeContainer();
            }
            var currentData = currentTutorialData;
            return tutorialSettings.huntChoicePrizeContainerList[currentData.startProgressIndex];
        }

        public TutorialSettings.HuntGetTimetableContainer GetHuntGetTimetableContainer()
        {
            if (tutorialSettings == null)
            {
                return new TutorialSettings.HuntGetTimetableContainer();
            }
            return tutorialSettings.huntGetTimetable;
        }
        
        #endregion
        
        #region ユーザー情報入力
        public async UniTask InitializeUserDataAsync(bool updateStep = true)
        {
            while (true)
            {
                ResetUserData();
                var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TutorialInputUserData,null);
                await modal.WaitCloseAsync();
                modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TutorialSelectUserIcon, null);
                await modal.WaitCloseAsync();
                if (!ValidateUserData()) continue;
                try
                {
                    var request = new UserInitializeAPIRequest();
                    var post = new UserInitializeAPIPost();
                    post.name = userName;
                    post.gender = gender;
                    post.mIconId = mIconId;
                    if (updateStep)
                    {
                        var step = (long)(currentTutorialData?.stepId ?? 0);
                        post.tutorialNumberList = new[] { step };
                    }
                    request.SetPostData(post);
                    await APIManager.Instance.Connect(request);
                    var response = request.GetResponseData();
                    RegisterUserPersonal(response);
                    UserDataManager.Instance.user.UpdateUserPersonalData(response);
                }
                catch (APIException e)
                {
                    ResetUserData();
                    if (APIUtility.CalcErrorType(e.errorParam) != ErrorModalType.MessageOnly)
                    {
                        // 2000番以外のエラーが返ってきた場合はリトライせずに終了
                        return;
                    }
                }
                if (ValidateUserData()) break;
            }
        }

        public void RegisterUserDetail(string userName, long gender)
        {
            this.userName = userName;
            this.gender = gender;
        }

        public void RegisterUserIcon(long mIconId)
        {
            this.mIconId = mIconId;
        }

        private void RegisterUserPersonal(UserInitializeAPIResponse response)
        {
            RegisterUserDetail(response.name,response.gender);
            RegisterUserIcon(response.mIconId);
        }

        private bool ValidateUserData()
        {
            return !string.IsNullOrEmpty(userName) && gender != 0 && mIconId != 0;
        }

        private void ResetUserData()
        {
            userName = null;
            gender = 0;
            mIconId = 0;
        }

        #endregion

        #region キャラ強化
        
        public async UniTask CharaPieceToCharaAsync(long[] stepIds)
        {
            var request = new CharaPieceToCharaForTutorialAPIRequest();
            var post = new CharaPieceToCharaForTutorialAPIPost();
            post.mCharaId = tutorialSettings.tutorialReleaseCharaId;
            post.tutorialNumberList = stepIds;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }

        public int GetReleaseCharaId()
        {
            if (tutorialSettings == null)
            {
                return 0;
            }
            return tutorialSettings.tutorialReleaseCharaId;
        }
        #endregion
        
        #region ウォームアップ
        /// <summary>
        /// ウォームアップ処理(シェーダ用)
        /// シェーダーコンパイルが再生時に走るので事前に1回演出を再生させてコンパイルを行っておく
        /// </summary>
        public async UniTask WarmUpShaderAsync()
        {
            if (IsCompleteTutorial())
            {
                return;
            }
            
            foreach (var address in tutorialSettings.warmUpDigestObjectAddressList)
            {
                var prefab = await resourcesLoader.LoadAssetAsync<GameObject>(address);
                if (prefab != null)
                {
                    var obj = Instantiate(prefab, AppManager.Instance.WorldManager.RootTransform, false);
                    var digestObject = obj.GetComponent<BattleDigestObject>();
                    // 演出をウォームアップ用再生する
                    await digestObject.WarmUpPlayAsync();
                    Destroy(obj);
                }
            }
        }
        #endregion

        #region Debug
        private async UniTask AllUpdateTutorialStep()
        {
            if (tutorialSettings == null) return;
            
            var stepIds = tutorialSettings.detailList.Select(detail => (long)detail.stepId).ToArray();
            await UpdateTutorialStep(stepIds);
        }

        public void AddDebugCommand(PageType pageType)
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.AddOption(name: "初回チュートリアルを完了",
                callback: () =>
                {
                    AppManager.Instance.TutorialManager.AllUpdateTutorialStep().Forget();
                },
                category: pageType.ToString());

            TutorialSettings.Step skipStep = TutorialSettings.Step.None;
            SRDebugger.SRDebugDropDownEnumData dropDownEnumData = new (skipStep);
            CruFramework.DebugMenu.AddOption(name: "手順スキップ",
                getter: () => { return dropDownEnumData; },
                setter:(enumData) =>
                {
                    TutorialSettings.Step skipStep = (TutorialSettings.Step)enumData.Value;
                    if (skipStep == TutorialSettings.Step.None)
                    {
                        return;
                    }
                    
                    // enumDataの手順までの数値を配列に追加
                    List<long> skipIds = new (); 
                    foreach (TutorialSettings.Step step in Enum.GetValues(typeof(TutorialSettings.Step)))
                    {
                        skipIds.Add((long)step);
                        if (step == skipStep)
                        {
                            // スキップする手順まで追加したらbreak
                            break;
                        }
                    }
                    
                    CruFramework.Logger.Log($"スキップする手順: {string.Join(",", skipIds)}");
                    
                    AppManager.Instance.TutorialManager.UpdateTutorialStep(skipIds.ToArray()).Forget();
                },
                category: pageType.ToString());
#endif
        }

        public void RemoveDebugCommand(PageType pageType)
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.RemoveOption(pageType.ToString());
#endif
        }
        #endregion
    }
}
