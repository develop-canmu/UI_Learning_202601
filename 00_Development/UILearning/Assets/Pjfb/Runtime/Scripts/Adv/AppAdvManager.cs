using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using Spine.Unity;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Pjfb.Adv
{

    public enum AppAdvAutoMode
    {
        None = 0,
        Auto = 1,
        Fast = 2,
        Skip3 = 3,
        Skip4 = 4,
    }
    
    [Flags]
    public enum AppAdvOptions
    {
        None = 0,
        /// <summary>４段目のスキップ有効</summary>
        EnableSkip4 = 1 << 0,
        /// <summary>ログを強制的に開く</summary>
        ForceOpenLog = 1 << 1,
    }
    
    public class AppAdvManager : AdvManager
    {
        public enum GenderType
        {
        	TypeA = 1,
        	TypeB = 2
        }
        

    
        private static  readonly string AnimatorKeyOpen = "Open";
        private static  readonly string AnimatorKeyClose = "Close";
        
        public static readonly string ChoiceKey = "Choice";
        public static readonly string ResourcePathKey = "AdvCommand";
        
        
        
        [SerializeField]
        private GameObject uiRoot = null;
        [SerializeField]
        private Animator animator = null;
        
        [SerializeField]
        private AdvFooter footer = null;
        /// <summary>フッター</summary>
        public AdvFooter Footer{get{return footer;}}
        
        [SerializeField]
        private AdvTransition transition = null;
        /// <summary>場面転換用</summary>
        public AdvTransition Transition{get{return transition;}}
        
        [SerializeField]
        private GameObject awakeningAnimationRoot = null;
        
        [SerializeField]
        private AppAdvOptions options = AppAdvOptions.None;
                
        public event Action<AppAdvAutoMode> OnChangeAutoMode = null;
        
        private long[] choiceList = null;
        /// <summary>選択肢データ</summary>
        public long[] ChoiceList{get{return choiceList;}}
        
        private AppAdvAutoMode appAutoMode = AppAdvAutoMode.None;
        /// <summary>AutoMode</summary>
        public AppAdvAutoMode AppAutoMode{get{return appAutoMode;}}
        

        private AdvTrainingEventResult eventResult = new AdvTrainingEventResult();
        /// <summary>トレーニング結果よう</summary>
        public AdvTrainingEventResult EventResult{get{return eventResult;}}
        

        
        private long trainingCharacterId = 0;
        /// <summary>トレーニングのキャラId</summary>
        public long TrainingCharacterId{get{return trainingCharacterId;}}
        
        // 覚醒演出
        private PlayableDirector awakeningAnimation = null;

        public ModalManager ModalManager
        {
            get
            {
#if UNITY_EDITOR
                ModalManager m = AppManager.Instance?.UIManager?.ModalManager;
                if(m == null)
                {
                    return GameObject.FindAnyObjectByType<ModalManager>(FindObjectsInactive.Include);
                }
                
                return m;
#else
                return AppManager.Instance.UIManager.ModalManager;
#endif
            }
        }

        protected override bool CanExecuteCommand()
        {
#if UNITY_EDITOR
            if(AppManager.Instance == null)return true;
#endif
            // モーダルが開いているときは停止
            return AppManager.Instance.UIManager.ModalManager.ModalStack.Count == 0;
        }

        protected override void OnSkipButtonEnable(bool isEnable)
        {
            Footer.EnableSkipButton(isEnable);
        }

        public async UniTask PlayOpenAnimationAsync()
        {
            if(animator == null)return;
            animator.SetTrigger(AnimatorKeyOpen);
            
            await AnimatorUtility.WaitStateAsync(animator, AnimatorKeyOpen);

        }
        
        private async UniTask PlayCloseAnimationAsync()
        {
            if(animator == null)return;
            animator.SetTrigger(AnimatorKeyClose);
            
            await AnimatorUtility.WaitStateAsync(animator, AnimatorKeyClose);
        }

        protected override void OnAwake()
        {
            // 置換テキスト更新
            UpdateReplaceText();
            base.OnAwake();
        }
        
        public void UpdateReplaceText()
        {
            // ユーザー名
            string userName = UserDataManager.Instance.user.name;
            // 一人称
            GenderType genderType = (GenderType)UserDataManager.Instance.user.gender;
            
            // 一応初期化
            SetTrainingTipCount(0);
            
            // テスト表示用
#if UNITY_EDITOR            
            // カラだった場合適当に入れる
            if(string.IsNullOrEmpty(userName))
            {
                userName = "ユーザー名";
            }
            
            if( (int)genderType == 0)
            {
                genderType = GenderType.TypeA;
            }
            
            SetTrainingTipCount(1234);
            SetTrainingCharacterId(10001001);
#endif
            // ユーザー名の置換登録
            AddMessageReplaceString(AppAdvConstants.ReplaceUserName, userName);
            // 一人称の置換登録
            switch(genderType)
            {
                case GenderType.TypeA:
                    AddMessageReplaceString(AppAdvConstants.ReplaceFirstPersonString, StringValueAssetLoader.Instance["tutorial.name.firstperson.a"]);
                    break;
                case GenderType.TypeB:
                    AddMessageReplaceString(AppAdvConstants.ReplaceFirstPersonString, StringValueAssetLoader.Instance["tutorial.name.firstperson.b"]);
                    break;
            }
        }
        
        /// <summary>ステータスアップ値</summary>
        public void SetStatusUpData(CharacterStatus status)
        {
            eventResult.StatusUpValue = status;
        }
        
        /// <summary>パフォーマンスレベル</summary>
        public void SetPerformanceLv(long lv, bool isLvMax)
        {
            eventResult.PerformanceLv = lv;
            eventResult.IsLvMaxPerformance = isLvMax;
        }
        
        /// <summary>練習レベル</summary>
        public void SetPracticeLv(long id, long lv, bool isLvMax)
        {
            eventResult.PracticeId = id;
            eventResult.PracticeLv = lv;
            eventResult.IsLxMaxPractice = isLvMax;
        }
        
        /// <summary>インスピレーションブースト</summary>
        public void SetInspirationBoost(long value)
        {
            eventResult.InspirationBoost = value;
        }

        /// <summary>チップ獲得枚数</summary>
        public void SetRewardTipCount(long count)
        {
            eventResult.TipCount = count;
        }

        /// <summary>選択肢データ</summary>
        public void SetChoice(long[] choiceList)
        {
            this.choiceList = choiceList;
        }
        
        /// <summary>トレーニングチップ枚数設定</summary>
        public void SetTrainingTipCount(long count)
        {
            AddMessageReplaceString(AppAdvConstants.ReplaceTrainingTips, count.ToString());
        }

        /// <summary>トレーニングキャラIdを登録</summary>
        public void SetTrainingCharacterId(long id)
        {
            trainingCharacterId = id;
        }
        
        public async UniTask PreloadAwakeningAnimation()
        {
            if(awakeningAnimation != null)return;
            string path = "Prefabs/UI/Traning/Condition_SkillCutInAnim.prefab";
            PlayableDirector playableDirector = await LoadResourceAsync<PlayableDirector>(path);
            awakeningAnimation = GameObject.Instantiate<PlayableDirector>(playableDirector, awakeningAnimationRoot.transform);
            awakeningAnimation.gameObject.SetActive(false);
        }
        
        public PlayableDirector GetAwakeningAnimation()
        {
            return awakeningAnimation;
        }

        protected override string GetConfigPath()
        {
            return "Adv/AdvConfig.asset";
        }
        
        protected override async UniTask OnRestartAsync()
        {
            await PlayOpenAnimationAsync();
        }

        protected override async UniTask OnEndAsync()
        {
            // エフェクトの処理
            if(awakeningAnimation != null)
            {
                GameObject.Destroy(awakeningAnimation.gameObject);
                awakeningAnimation = null;
            }
            
            await PlayCloseAnimationAsync();

            
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.RemoveOption("Adv");
            CruFramework.DebugMenu.RemoveOption("AdvCommand");
#endif
            
        }

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
        private void OnExecuteCommandDebug(IAdvCommandObject command)
        {
            CruFramework.DebugMenu.RemoveOption("AdvCommand");
            CruFramework.DebugMenu.AddOption("AdvCommand", command.GetType().ToString(), ()=>
            {
                GUIUtility.systemCopyBuffer = command.GetType().ToString();
            });
        }
#endif

        protected override void OnLoadFile(string path)
        {
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            
            OnExecuteCommand -= OnExecuteCommandDebug;
            OnExecuteCommand += OnExecuteCommandDebug;
            
            
            
            CruFramework.DebugMenu.AddOption("Adv", "AdvFile : " + path,
                () =>
                {
                    GUIUtility.systemCopyBuffer = path;
                });
#endif
        }

        public void OnUIOffButton()
        {
            uiRoot.SetActive(false);
        }
        
        public void OnSkipButton()
        {
            OnSkipButtonAsync().Forget();
        }
        
        public async UniTask OnSkipButtonAsync()
        {
            if(IsEnded)return;
            
            // コマンドを停止
            IsPause = true;
            
            bool executeSkip = false;
            
            // 確認モーダル
            
            // Yes
            ConfirmModalButtonParams button1 = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.yes"], async (m)=>
            {
                // スキップ処理中に
                executeSkip = true;
                // モーダルを閉じる
                await m.CloseAsync();
#if UNITY_EDITOR
                if(AppManager.Instance == null)
                {
                    await SkipAsync(AdvSkipMode.Skip);
                }
                else
                {
#endif
                    await AppManager.Instance.LoadingActionAsync( async ()=>
                    {
                        await SkipAsync(AdvSkipMode.Skip);
                    });
                    
#if UNITY_EDITOR
                }
#endif
                // スキップ処理終了
                executeSkip = false;
                
            });
            
            // No
            ConfirmModalButtonParams button2 = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (m)=>{m.Close();});
            // Open
            ConfirmModalData modalData = new ConfirmModalData( StringValueAssetLoader.Instance["adv.skip_modal.title"], StringValueAssetLoader.Instance["adv.skip_modal.message"], string.Empty, button1, button2);
            // 閉じるまで待機
            CruFramework.Page.ModalWindow modal = await ModalManager.OpenModalAsync(ModalType.Confirm, modalData);
            await modal.WaitCloseAsync();
            // スキップ処理が終わるまで待つ
            await UniTask.WaitWhile(()=>executeSkip);
            
            // コマンドを再開
            IsPause = false;
        }
        
        public void SetAutoMode(AppAdvAutoMode mode)
        {
            appAutoMode = mode;
            footer.SetAutoModeButton(mode);
           
            switch(mode)
            {
                case AppAdvAutoMode.None:
                    AutoMode = AdvAutoMode.None;
                    AutoMessage = 0;
                    break;
                case AppAdvAutoMode.Auto:
                    AutoMode = AdvAutoMode.Auto;
                    AutoMessage = 0.5f;
                    break;
                
                case AppAdvAutoMode.Skip4:
                case AppAdvAutoMode.Skip3:
                case AppAdvAutoMode.Fast:
                    AutoMode = AdvAutoMode.Fast;
                    AutoMessage = 0;
                    break;
            }
            
            if(OnChangeAutoMode != null)
            {
                OnChangeAutoMode(mode);
            }
        }

        public void OnAutoModeButton()
        {
            switch(AppAutoMode)
            {
                case AppAdvAutoMode.None:
                    SetAutoMode(AppAdvAutoMode.Auto);
                    break;
                case AppAdvAutoMode.Auto:
                    SetAutoMode(AppAdvAutoMode.Fast);
                    break;
                case AppAdvAutoMode.Fast:
                    SetAutoMode(AppAdvAutoMode.Skip3);
                    break;
                case AppAdvAutoMode.Skip3:
                {
                    // スキップ４が有効の場合
                    if( (options & AppAdvOptions.EnableSkip4) != AppAdvOptions.None )
                    {
                        SetAutoMode(AppAdvAutoMode.Skip4);
                    }
                    else
                    {
                        SetAutoMode(AppAdvAutoMode.None);
                    }
                    break;
                }
                case AppAdvAutoMode.Skip4:
                    SetAutoMode(AppAdvAutoMode.None);
                    break;
            }
        }
        
        public void OnLogButton()
        {
            OnLogButtonAsync().Forget();
        }
        
        private async UniTask OnLogButtonAsync()
        {
            // すでに終了している
            if(IsEnded && ( (options & AppAdvOptions.ForceOpenLog) == AppAdvOptions.None) )return;
            // 進行を停止
            IsPause = true;
            // ログ表示モーダル
            CruFramework.Page.ModalWindow modal = await ModalManager.OpenModalAsync(ModalType.AdvMessageLog, MessageLogs );
            // 閉じるのを待つ
            await modal.WaitCloseAsync();
            // 進行再開
            IsPause = false;
        }

        public void OnUIOnButton()
        {
            uiRoot.SetActive(true);
        }


        public override void OpenDebugModal(IAdvCommandObject command, string message)
        {
            OpenDebugModalAsync(command, message).Forget();

        }
        
        public async UniTask OpenDebugModalAsync(IAdvCommandObject command, string message)
        {
            // コマンドを停止
            IsStopCommand = true;
            
            string commandName = command.GetType().Name;
            if(commandName.StartsWith("AdvCommand"))commandName = commandName.Substring(10);
            ConfirmModalData data = new ConfirmModalData(
                $"{commandName}（テストラン）",
                message + "<br><br>テストラン用のモーダルです。<br>実際の挙動はゲーム再生で確認してください。",
                string.Empty,
                new ConfirmModalButtonParams("OK", (v)=>{v.Close();})
            );
                
            CruFramework.Page.ModalWindow modal = await ModalManager.OpenModalAsync(ModalType.Confirm, data);
            await modal.WaitCloseAsync();
            IsStopCommand = false;
        }
    }
}