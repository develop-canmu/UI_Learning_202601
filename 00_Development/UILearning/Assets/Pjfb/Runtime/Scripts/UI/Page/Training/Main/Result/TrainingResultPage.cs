using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Event;
using Pjfb.Networking.App.Request;
using UnityEngine;
using UnityEngine.UI;

using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Voice;
using UnityEngine.AddressableAssets;

namespace Pjfb.Training
{
    
    
    public class TrainingResultPage : TrainingPageBase
    {
        public enum State
        {
            StatusAnimation,
            Status,
            RankAnimation,
            Rank,
            EndAnimation,
            Event,
            GotoHome
        }
        
        /// <summary>アニメーター</summary>
        private static readonly string OpenAnimation = "OpenStatus";
        /// <summary>アニメーター</summary>
        private static readonly string OpenRankAnimation = "OpenRank";
        /// <summary>アニメーター</summary>
        private static readonly string OpenRankEventAnimation = "OpenRankEvent";
        /// <summary>アニメーター</summary>
        private static readonly string OpenRankAgainAnimation = "OpenRankAgain";
        /// <summary>アニメーター</summary>
        private static readonly string BackStatusAnimation = "BackStatus";
        /// <summary>アニメーター</summary>
        private static readonly string OpenEventPointsAnimation = "OpenEventPoints";
        /// <summary>アニメーター</summary>
        private static readonly string OpenEventBonusAnimation = "OpenEventBonus";
        /// <summary>アニメーター</summary>
        private static readonly string OpenButtonsEventPointsAnimation = "OpenButtonsEventPoints";
        /// <summary>アニメーター</summary>
        private static readonly string OpenFinalPointsAgainAnimation = "OpenFinalPointsAgain";
        /// <summary>アニメーター</summary>
        private static readonly string BackRankAnimation = "BackRank";
        
        
        
        [SerializeField]
        private Animator animator = null;
  
        [SerializeField]
        private TrainingCharacterStatusResult statusResult = null;
        
        [SerializeField]
        private CharacterRankImage rankImage = null;
        [SerializeField]
        private CharacterRankImage currentRankImage = null;
        [SerializeField]
        private CharacterRankImage nextRankImage = null;
        [SerializeField]
        private Image rankUpSlider = null;
        [SerializeField]
        private TMPro.TMP_Text rankPointText = null;
        [SerializeField]
        private OmissionTextSetter rankPointOmissionTextSetter = null;
        
        [SerializeField]
        private CharacterSpine characterSpine = null;
        
        [SerializeField]
        private TrainingFestivalActivate festivalActivatePopup = null;
        
        [SerializeField]
        private TrainingResultFestivalPointsView festivalPointsView = null;

        [SerializeField]
        private TrainingResultMissionView missionView = null;
        
        [SerializeField]
        private StaminaView staminaView = null;

        [SerializeField]
        private UIButton backButton = null;
        [SerializeField]
        private UIButton eventBackButton = null;
        
        [SerializeField]
        private UIButton summaryButton = null;
        [SerializeField]
        private UIButton replayButton = null;

        [SerializeField, Header("イベントポイントの加算からミッション進捗画面の表示までの時間")]
        private float waitStartMissionProgressTime = 0.3f;
        
        // ランクアニメーションを表示
        private bool isPlayRankAnimation = true;
        // イベントアニメーションを表示
        private bool isPlayEventAnimation = true;
        
        private VoiceResourceSettings.LocationType voiceType = VoiceResourceSettings.LocationType.Unknown;
        
        // ステート
        private State state = State.Status;

        /// <summary>開くアニメーション終了時</summary>
        protected virtual void OnEndOpenAnimation(){ }
        /// <summary>開くアニメーション終了時</summary>
        protected virtual void OnEndOpenRankAnimation(){ }

        private async UniTask PlayOpenAnimation()
        {
            // ページ表示初期化
            await AnimatorUtility.WaitStateAsync(animator, OpenAnimation);
            // 開くアニメーション終了
            OnEndOpenAnimation();
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 自動トレーニング？
            bool isAutoTraining = (MainArguments.OptionFlags & TrainingMainArguments.Options.AutoTrainingFinished) != TrainingMainArguments.Options.None;
            
            // 自動トレーニングの場合
            if(isAutoTraining)
            {
                // スタミナ更新
                await staminaView.UpdateAsync(StaminaUtility.StaminaType.AutoTraining);
                staminaView.AutoUserStatus = MainArguments.AutoTrainingUserStatus;
                
                // 自動トレーニング時はcurrentTrainingDataがnullなのでここで更新する
                MainPageManager.UpdateData(MainArguments);
            }
            
            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            
            // キャラ表示
            characterSpine.SetSkeletonDataAsset( CharacterUtility.CharIdToStandingImageId(MainArguments.TrainingCharacter.MCharId) );
                        
            // ヘッダーを非表示
            Header.Hide();
            // フッターを非表示
            Footer.Hide();
            // ステータス表示
            statusResult.SetStatus(MainArguments.CharacterVariable.mCharaId, MainArguments.Status, MainArguments.CharacterVariable.abilityList);
            
            // 自動トレーニング？
            bool isAutoTraining = (MainArguments.OptionFlags & TrainingMainArguments.Options.AutoTrainingFinished) != TrainingMainArguments.Options.None;
            
            // サマリーボタン
            summaryButton.gameObject.SetActive(isAutoTraining);
            // リプレイボタン
            replayButton.gameObject.SetActive(isAutoTraining);
            // スタミナ
            staminaView.gameObject.SetActive(isAutoTraining);
            
            isPlayRankAnimation = true;
            
            // 評価点
            BigValue rankPoint = new BigValue(MainArguments.CharacterVariable.combatPower);
            
            long rankNumber = StatusUtility.GetCharacterRank(rankPoint);
            // 得点表示
            rankPointText.text = rankPoint.ToDisplayString(rankPointOmissionTextSetter.GetOmissionData());
            // ランク画像表示
            rankImage.SetTexture( rankNumber);
            // ランク画像表示
            currentRankImage.SetTexture( rankNumber );
            // ランク画像表示
            nextRankImage.SetTexture( StatusUtility.GetCharacterNextRank(rankPoint) );
            // ゲージを設定
            BigValue nextRankPoint = StatusUtility.GetCharacterNextRankValue(rankPoint);
            // ゲージを設定
            BigValue currentRankPoint = StatusUtility.GetCharacterRankValue(rankPoint);
            
            // イベントポイント
            festivalPointsView.Set(MainArguments);
            // ミッション進捗
            missionView.Set(MainArguments);
            
            // ボイス
            for(int i = 0; i < TrainingUtility.Config.RankVoiceDatas.Length; i++)
            {
                if(TrainingUtility.Config.RankVoiceDatas[i].RankNumber == rankNumber)
                {
                    voiceType = TrainingUtility.Config.RankVoiceDatas[i].VoiceType;
                    break;
                }
            }
            
            if(nextRankPoint == currentRankPoint)
            {
                rankUpSlider.fillAmount = 1.0f;
            }
            else
            {
                rankUpSlider.fillAmount = (float)BigValue.RatioCalculation((rankPoint - currentRankPoint), (nextRankPoint - currentRankPoint));
            }
            
            // ステータスを開くアニメーション
            PlayStatusOpenAnimationAsync().Forget();
        }
        
        private async UniTask PlayStatusOpenAnimationAsync()
        {
            // ステート
            state = State.StatusAnimation;
            // アニメーション再生
            await PlayOpenAnimation();
            // ステート変更
            state = State.Status;
        }
                
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnRankNextButton()
        {
            OnRankNextButtonAsync().Forget();
        }
        
        private async UniTask OnRankNextButtonAsync()
        {
            // ランクからのみ
            if(state != State.Rank)return;
            // ステート変更
            state = State.EndAnimation;
            
            // イベントがないかつ、ミッションの進捗がない
            if(MainArguments.FestivalPointProgress == null && MainArguments.MissionList == null)
            {
                await GotoHome();
                return;
            }
            // イベントがないがミッション進捗がある
            else if (MainArguments.FestivalPointProgress == null && MainArguments.MissionList != null)
            {
                bool isClosed = false;
                // 表示
                missionView.Open(() => { isClosed = true; });
                // 閉じるまで待つ
                await UniTask.WaitWhile(() => isClosed == false);
                await GotoHome();
                return;
            }

            // アニメーション再生済み
            if(isPlayEventAnimation == false)
            {
                // ポイント表示
                await AnimatorUtility.WaitStateAsync(animator, OpenFinalPointsAgainAnimation);
                // ステート変更
                state = State.Event;
                return;
            }

            // ポイント表示
            await AnimatorUtility.WaitStateAsync(animator, OpenEventPointsAnimation);
            // イベント発動
            if(MainArguments.FestivalEffectStatus != null && MainArguments.FestivalEffectStatus.isCausedNow)
            {
                bool isClosed = false;
                // ポップアップを開く
                festivalActivatePopup.Open(MainArguments, ()=>{isClosed = true;});
                EventManager.Instance.UpdateFestivalEffectStatus(MainArguments.FestivalEffectStatus);
                // 閉じるまで待つ
                await UniTask.WaitWhile(()=>isClosed == false);
            }
            
            // ポイント表示
            await AnimatorUtility.WaitStateAsync(animator, OpenEventBonusAnimation);
            
            // 合計ポイント表示
            await festivalPointsView.PlayPointAnimation();
            
            // ミッション進捗表示
            if (MainArguments.MissionList != null)
            {
                bool isClosed = false;
                // waitStartMissionProgressTime秒待つ
                await UniTask.Delay((int)(waitStartMissionProgressTime * 1000));
                // ミッション進捗を表示
                missionView.Open(() => { isClosed = true; });
                // 閉じるまで待つ
                await UniTask.WaitWhile(() => isClosed == false);
            }

            // ポイント表示
            await AnimatorUtility.WaitStateAsync(animator, OpenButtonsEventPointsAnimation);
            
            // アニメーション再生済み
            isPlayEventAnimation = false;
            
            // ステート変更
            state = State.Event;
        }
        
        
        /// <summary>イベント画面の戻る</summary>
        public void OnEventBackButton()
        {
            OnEventBackButtonAsync().Forget();
        }
        
        private async UniTask OnEventBackButtonAsync()
        {
            // イベントからのみ
            if(state != State.Event)return;
            // ステート変更
            state = State.RankAnimation;
            
            await AnimatorUtility.WaitStateAsync(animator, BackRankAnimation);
            // ステート変更
            state = State.Rank;
        }
        
        /// <summary>UGUI</summary>
        public void OnEventEndButton()
        {
            GotoHome().Forget();
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnRankBackButton()
        {
            OnRankBackButtonAsync().Forget();
        }
        
        private async UniTask OnRankBackButtonAsync()
        {
            // ランクからのみ
            if(state != State.Rank)return;
            // ステート変更
            state = State.StatusAnimation;
            
            await AnimatorUtility.WaitStateAsync(animator, BackStatusAnimation);
            
            // ステート変更
            state = State.Status;
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnRankButton()
        {
            OnRankButtonAsync().Forget();
        }
        
        private async UniTask OnRankButtonAsync()
        {
            // ステータスからのみ
            if(state != State.Status)return;
            // ステート移動
            state = State.RankAnimation;

            if(isPlayRankAnimation)
            {
                isPlayRankAnimation = false;
                
                // イベント 
                if(MainArguments.FestivalPointProgress != null)
                {
                    await AnimatorUtility.WaitStateAsync(animator, OpenRankEventAnimation);
                }
                else
                {
                    await AnimatorUtility.WaitStateAsync(animator, OpenRankAnimation);
                }
                // ボイス
                PlayTrainingCharacterVoice(voiceType);
                OnEndOpenRankAnimation();
            }
            else
            {
                await AnimatorUtility.WaitStateAsync(animator, OpenRankAgainAnimation);
            }
           
            // ステート移動
            state = State.Rank;
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnMemberInfoButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingMemberInfo, MainArguments);
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnTapToNext()
        {
            GotoHome().Forget();
        }
        
        
        /// <summary>UGUI</summary>
        public void OnReplayButton()
        {
            OnReplayButtonAsync().Forget();
        }
        
        /// <summary>UGUI</summary>
        public async UniTask OnReplayButtonAsync()
        {
            // モーダルに送る引数
            AutoTrainingConfirmModal.Arguments arguments = new AutoTrainingConfirmModal.Arguments(MainArguments.AutoTrainingPendingStatus, MainArguments.AutoTrainingUserStatus, null, AutoTrainingModalType.Replay);
            // モーダルを開く
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync( ModalType.AutoTrainingConfirm, arguments );
            // 閉じるまで待つ
            if(await modal.WaitCloseAsync() is AutoTrainingConfirmModal.CloseParamType r && r == AutoTrainingConfirmModal.CloseParamType.ExecuteReplay)
            {
                replayButton.interactable = false;
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnSummaryButton()
        {
            if(state == State.Status || state == State.Rank || state == State.Event)
            {
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoTraininglSummary, MainArguments);
            }
        }
        
        private async UniTask GotoHome()
        {
            // すでに遷移済み
            if(state == State.GotoHome)return;
            // ステート変更
            state = State.GotoHome;
            // フレンドフォロー確認
            // 自キャラの場合はモーダルを表示しない
            if (MainArguments.Friend.communityUserStatus.uMasterId != UserDataManager.Instance.user.uMasterId)
            {
                if(MainArguments.Friend.relationType != (int)TrainingUtility.FriendFollowType.Follow && MainArguments.Friend.relationType != (int)TrainingUtility.FriendFollowType.MutualFollow)
                {
                    TrainingFriendFollowModal.Arguments args = new TrainingFriendFollowModal.Arguments(MainArguments.Friend, MainArguments.FriendCharacterData.MCharId, MainArguments.FriendCharacterData.Lv, MainArguments.FriendCharacterData.LiberationId);
                    CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingFriendFollow, args);
                    await modalWindow.WaitCloseAsync();
                }
            }
            
            // イベントがない場合
            await AppManager.Instance.LoadingActionAsync(async ()=>
            {
                // 自動トレーニング？
                bool isAutoTraining = (MainArguments.OptionFlags & TrainingMainArguments.Options.AutoTrainingFinished) != TrainingMainArguments.Options.None;
                // 自動トレーニングの場合はトレーニング画面に戻る
                if(isAutoTraining)
                {
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.TrainingPreparation, true, null);
                }
                else
                {
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Home, true, null);
                }
                
            });
        }

        /// <summary>バックキー</summary>
        public override void OnBackkey()
        {
            switch(state)
            {
                case State.Rank:
                    SEManager.PlaySE(backButton.SoundType);
                    OnRankBackButton();
                    break;
                case State.Event:
                    SEManager.PlaySE(eventBackButton.SoundType);
                    OnEventBackButton();
                    break;
                case State.Status:
                    break;
            }
        }
    }
}