using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using CruFramework;
using CruFramework.Audio;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.Voice;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.Training;
using Pjfb.UserData;

namespace Pjfb.Menu
{
    public class ConfigurationModalWindow : ModalWindow
    {
        #region Params

        public enum SettingType
        {
            Default,
            Training
        }
        
        [Serializable]
        public class DisplayToggle
        {
            public AppConfig.DisplayType Type;
            public Toggle toggle;
        }
        
        public class WindowParams
        {
            public SettingType Type = SettingType.Default;
            public AppConfig DefaultSetting;
            public Action onClosed;
            public long TrainingScenarioId;
        }

        [SerializeField] private ConfigSlider bgmSlider;
        [SerializeField] private ConfigSlider seSlider;
        [SerializeField] private ConfigSlider voiceSlider;
        [SerializeField] private List<DisplayToggle> fieldViewToggleList;
        [SerializeField] private List<DisplayToggle> trainingPracticeToggleList;
        [SerializeField] private List<DisplayToggle> characterCardEffectToggleList;
        [SerializeField] private List<PushSettingToggle> pushSettingToggles;
        [SerializeField] private UIButton applyButton;
        [SerializeField] private UIToggle trainingConfirmRest;
        [SerializeField] private UIToggle trainingConfirmPracticeGameCard;
        [SerializeField] private UIToggle trainingConfirmPracticeCardRedraw;
        [SerializeField] private GameObject trainingConfirmPracticeCardRedrawRoot;
        [SerializeField] private GameObject trainingRoot;
        [SerializeField] private GameObject matchRoot;
        [SerializeField] private GameObject pushSettingRoot;
        [SerializeField] private Transform pushSettingParent;
        [SerializeField] private PushSettingToggle pushSettingObj;

        private const string ConfigChangeVoice = "sys_0001_0001";
        private WindowParams _windowParams;
        private WrapperIntList[] pushSettingList;

        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Configuration, data);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            await Init();
            await base.OnPreOpen(args, token);
        }
        private async UniTask Init()
        {
            switch (_windowParams.Type)
            {
                case SettingType.Training:
                    trainingRoot.SetActive(true);
                    matchRoot.SetActive(true);
                    pushSettingRoot.SetActive(false);
                    // 引き直しの機能が有効か
                    trainingConfirmPracticeCardRedrawRoot.SetActive(TrainingUtility.IsEnableTrainingPracticeCardRedraw(_windowParams.TrainingScenarioId));
                    break;
                case SettingType.Default:
                    trainingRoot.SetActive(false);
                    matchRoot.SetActive(true);　// TODO: マッチ設定復帰後の表示
                    pushSettingRoot.SetActive(true);
                    break;
            }
            _windowParams.DefaultSetting = LocalSaveManager.saveData.appConfig;
            SetByConfigSetting(_windowParams.DefaultSetting);
            bgmSlider.OnEditValue = OnEditValue;
            seSlider.OnEditValue = OnEditValue;
            voiceSlider.OnEditValue = OnEditValue;
            voiceSlider.OnEditValue += OnEditVoicePlayVoice;
            trainingConfirmRest.onValueChanged.AddListener((v)=>OnEditValue());
            trainingConfirmPracticeGameCard.onValueChanged.AddListener((v)=>OnEditValue());
            trainingConfirmPracticeCardRedraw.onValueChanged.AddListener((v)=>OnEditValue());
            OnEditValue();

            if (_windowParams.Type == SettingType.Default)
            {
                var pushSetting = await PushGetSettingAPI();
                pushSettingList = pushSetting.settingList.OrderBy(s => s.l[0]).ToArray();
                SetPushNotificationSetting();
            }
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            UpdatePushNotificationSetting();
            Close(onCompleted: _windowParams.onClosed);
        }

        public void OnClickReset()
        {
            string title = StringValueAssetLoader.Instance["common.confirm"];
            string message = StringValueAssetLoader.Instance["menu.config.reset_message"];
            string positiveButtonText = StringValueAssetLoader.Instance["common.ok"];
            string negativeButtonText = StringValueAssetLoader.Instance["common.cancel"];
            ConfirmModalWindow.Open(new ConfirmModalData
            (
                title,
                message,
                string.Empty, 
                new ConfirmModalButtonParams(positiveButtonText, confirmWindow =>
                {
                    //”OK”ボタンの押す処理
                    SetByConfigSetting(GetResetConfig());
                    var pushMaster = Master.MasterManager.Instance.pushMaster;
                    if (_windowParams.Type == SettingType.Default)  pushSettingToggles.ForEach(t => t.toggle.isOn = pushMaster.FindData(t.pushSettingType).sentDefault);
                    SaveConfigSettingData();
                    confirmWindow.Close();
                }),new ConfirmModalButtonParams(negativeButtonText, window =>
                {
                    //”キャンセル”ボタンの押す処理
                    window.Close();
                })
            ));
        }

        public void OnClickApply()
        {
            SaveConfigSettingData();
            OnClickClose();
        }

        public void OnEditValue()
        {
            if(_windowParams == null) return;
            // 現在の設定とモーダルを開いた時の比較
            var equal = GetConfigSetting().IsEqual(_windowParams.DefaultSetting);
            //変更があれば保存する
            if (!equal) SaveConfigSettingData();
        }

        private void OnEditVoicePlayVoice()
        {
            VoiceManager.Instance.StopVoice();
            VoiceManager.Instance.PlayVoiceAsync(ConfigChangeVoice).Forget();
        }

        #endregion

        #region Other

        private void SetByConfigSetting(AppConfig setting)
        {
            bgmSlider.Init(setting.bgmVolume,setting.LastBgmVolume);
            seSlider.Init(setting.seVolume,setting.LastSeVolume);
            voiceSlider.Init(setting.voiceVolume,setting.LastVoiceVolume);
            fieldViewToggleList.ForEach(t => t.toggle.SetIsOnWithoutNotify((int)t.Type == setting.fieldDisplayType));
            trainingPracticeToggleList.ForEach(t => t.toggle.SetIsOnWithoutNotify((int)t.Type == setting.trainingDisplayType));
            characterCardEffectToggleList.ForEach(t => t.toggle.SetIsOnWithoutNotify((int)t.Type == setting.CharacterCardEffectType));
            trainingConfirmRest.SetIsOnWithoutNotify(setting.IsTrainingConfirmRest);
            trainingConfirmPracticeGameCard.SetIsOnWithoutNotify(setting.IsTrainingConfirmPracticeGame);
            trainingConfirmPracticeCardRedraw.SetIsOnWithoutNotify(setting.IsTrainingConfirmPracticeCardRedraw);
        }

        private AppConfig GetConfigSetting()
        {
            // コンフィグ画面にないappConfigのメンバは流用する
            AppConfig appConfigAll = LocalSaveManager.saveData.appConfig;
            return new AppConfig()
            {
                bgmVolume = bgmSlider.GetSliderValue(),
                seVolume = seSlider.GetSliderValue(),
                voiceVolume = voiceSlider.GetSliderValue(),
                fieldDisplayType = (int)fieldViewToggleList.First(t => t.toggle.isOn).Type,
                trainingDisplayType = (int)trainingPracticeToggleList.First(t => t.toggle.isOn).Type,
                CharacterCardEffectType = (int)characterCardEffectToggleList.First(t => t.toggle.isOn).Type,
                IsTrainingConfirmRest = trainingConfirmRest.isOn,
                IsTrainingConfirmPracticeGame = trainingConfirmPracticeGameCard.isOn,
                IsTrainingConfirmPracticeCardRedraw = trainingConfirmPracticeCardRedraw.isOn,
                LastBgmVolume = bgmSlider.GetLastSliderValue(),
                LastSeVolume = seSlider.GetLastSliderValue(),
                LastVoiceVolume = voiceSlider.GetLastSliderValue(),
                IsTrainingSkipBoostChanceEffect = appConfigAll.IsTrainingSkipBoostChanceEffect,
            };
        }
        
        private void SaveConfigSettingData()
        {
            LocalSaveManager.saveData.appConfig = GetConfigSetting();
            LocalSaveManager.Instance.SaveData();
            _windowParams.DefaultSetting = LocalSaveManager.saveData.appConfig;
        }

        private AppConfig GetResetConfig()
        {
            AppConfig resetConfig = new AppConfig();
            
            var defaultGraphicSetting = DeviceUtility.IsLowSpecDevice()
                ? AppConfig.DisplayType.Light
                : AppConfig.DisplayType.Standard;
            
            switch (_windowParams.Type)
            {
                case SettingType.Training:
                    resetConfig = LocalSaveManager.saveData.appConfig;
                    resetConfig.bgmVolume = 5;
                    resetConfig.seVolume = 5;
                    resetConfig.voiceVolume = 5;
                    resetConfig.IsTrainingConfirmRest = true;
                    resetConfig.IsTrainingConfirmPracticeGame = true;
                    resetConfig.IsTrainingConfirmPracticeCardRedraw = true;
                    resetConfig.fieldDisplayType = (int)defaultGraphicSetting;
                    resetConfig.trainingDisplayType = (int)defaultGraphicSetting;
                    resetConfig.CharacterCardEffectType = (int)defaultGraphicSetting;
                    break;
                case SettingType.Default:
                    resetConfig.IsTrainingConfirmRest = LocalSaveManager.saveData.appConfig.IsTrainingConfirmRest;
                    resetConfig.IsTrainingConfirmPracticeGame = LocalSaveManager.saveData.appConfig.IsTrainingConfirmPracticeGame;
                    resetConfig.IsTrainingConfirmPracticeCardRedraw = LocalSaveManager.saveData.appConfig.IsTrainingConfirmPracticeCardRedraw;
                    resetConfig.fieldDisplayType = (int)defaultGraphicSetting;
                    resetConfig.trainingDisplayType = (int)defaultGraphicSetting;
                    break;
            }

            return resetConfig;
        }
        
        private void SetPushNotificationSetting()
        {
            if (!pushSettingToggles.Any())
            {
                //マスタデータでプッシュオプション追加
                var pushOptions = Master.MasterManager.Instance.pushMaster.values.Where(v => v.priority > 0).OrderBy(v=> v.priority);
                foreach (var option in pushOptions)
                {
                    var obj = Instantiate(pushSettingObj, pushSettingParent);
                    obj.Init(option);
                    pushSettingToggles.Add(obj);
                }
                pushSettingToggles.Last()?.borderObj.SetActive(false);
            }
            
            if (!pushSettingList.Any()) return;
            foreach (var item in pushSettingList)
            {
                var setting =pushSettingToggles.FirstOrDefault(t => t.pushSettingType == item.l[0]);
                if(setting != null) setting.toggle.SetIsOnWithoutNotify(item.l[1] == 1);
            }
        }

        private void UpdatePushNotificationSetting()
        {
            if (!pushSettingToggles.Any() || _windowParams.Type != SettingType.Default) return;

            var settingList = pushSettingToggles.Select(t => new WrapperIntList
                {
                    l = new long[] { t.pushSettingType, t.toggle.isOn ? 1 : 2 }
                }).OrderBy(o => o.l[0]).ToArray();
            
            if (pushSettingList != settingList)
            {
                PushEditSettingAPI(settingList).Forget();
                // ローカルPUSHの設定更新
                UserDataManager.Instance.UpdatePushSettingList(settingList);
                UserDataManager.Instance.UpdateRegisteredLocalPushNotification(false);
                LocalPushNotificationUtility.SetLocalNotification();
            }
        }

        #endregion

        #region API
        private async UniTask<PushGetSettingAPIResponse> PushGetSettingAPI()
        {
            PushGetSettingAPIRequest request = new PushGetSettingAPIRequest();
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        private async UniTask PushEditSettingAPI(WrapperIntList[] list)
        {
            PushEditSettingAPIRequest request = new PushEditSettingAPIRequest();
            PushEditSettingAPIPost post = new PushEditSettingAPIPost{settingList = list};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }

        #endregion
    }
}