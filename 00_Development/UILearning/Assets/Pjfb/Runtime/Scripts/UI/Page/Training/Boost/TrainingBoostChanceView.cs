using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using System;
using System.Threading;
using Pjfb.Adv;
using Pjfb.Extensions;
using Pjfb.UI;

namespace Pjfb.Training
{
    public class TrainingBoostChanceView : MonoBehaviour
    {
        /// <summary>スキップ時の待機時間</summary>
        private const float SkipWaitDuration = 1.0f;
        /// <summary>自動モード時の待機時間</summary>
        private const float AutoModeWaitDuration = 3.0f;
        
        /// <summary>開くアニメーション</summary>
        private static readonly string OpenModalAnimation = "OpenModal";
        /// <summary>モーダルを閉じるアニメーション</summary>
        private static readonly string CloseModalAnimation = "CloseModal";
        
        /// <summary>ブースト開始アニメーション</summary>
        private static readonly string StartBoostAnimation = "StartBoost";
        
        /// <summary>結果アニメーション</summary>
        private static readonly string ReleaseResultAnimation = "ReleaseBoostEffect";
        
        /// <summary>ブーストエフェクトアニメーション</summary>
        private static readonly string BoostEffectAnimation = "ActivateBoostEffect";
        
        /// <summary>Exブーストエフェクトアニメーション</summary>
        private static readonly string ExtraBoostAnimation = "ActivateExtraBoost";

        /// <summary> Spブーストエフェクトアニメーション </summary>
        private static readonly string SpecialBoostAnimation = "ActivateSpecialBoost";
        
        /// <summary>閉じるアニメーション</summary>
        private static readonly string CloseAnimation = "CloseAll";
        
        [SerializeField]
        private Animator animator = null;
        
        
        [SerializeField]
        private TMP_Text modalTitleText = null;
        [SerializeField]
        private TMP_Text modalMessageText = null;
        
        [SerializeField]
        private TMP_Text boostPointBeforeText = null;
        [SerializeField]
        private TMP_Text boostPointAfterText = null;
        
        [SerializeField]
        private TMP_Text boostLevelBeforeText = null;
        [SerializeField]
        private TMP_Text boostLevelAfterText = null;
        
        [SerializeField]
        private TMP_Text boostEffectText = null;
        
        [SerializeField]
        private OmissionTextSetter boostEffectOmissionSetter = null;

        // スペシャルブースト発動キャラアイコン
        [SerializeField]
        private CharacterIcon specialBoostCharaIcon = null;
        
        [SerializeField]
        private UIToggle skipToggle = null;
        
        [SerializeField]
        private TrainingBoostEffectView boostEffectView = null;
        
        [SerializeField]
        private PoolListContainer boostEffectScroll = null;

        // スペシャルブースト表示スクロール
        [SerializeField]
        private PoolListContainer specialBoostScroll = null;
        
        [SerializeField]
        private GameObject extraBoostRoot = null;

        // スペシャルブーストルートオブジェクト
        [SerializeField]
        private GameObject specialBoostRoot = null;
        
        // 次へ進む
        private bool isNext = false;
        // オートモード
        private AppAdvAutoMode autoMode = AppAdvAutoMode.None;
        // AdvManager
        private AppAdvManager appAdvManager = null;
        // ポイント更新コールバック
        private Action<TrainingPending, TrainingPointStatus> onUpdatePoint = null;
        // 演出完了時のコールバック
        private Action onComplete = null;
        
        // アニメーションキャンセルよう
        private CancellationTokenSource animationCancellationToken = null;

        // トレーニングデータ
        private TrainingMainArguments args = null;

        // 発動するポイントステータスレベル
        private long activePointStatusLevel = 0;
        
        /// <summary>開く</summary>
        public void Open(TrainingMainArguments args, AppAdvManager appAdvManager, Action<TrainingPending, TrainingPointStatus> onUpdatePoint, Action onComplete)
        {
            this.args = args;
            // コールバック
            this.onUpdatePoint = onUpdatePoint;
            this.onComplete = onComplete;
            // 自動モード
            this.autoMode = appAdvManager.AppAutoMode;
            // AdvManager
            this.appAdvManager = appAdvManager;
            // 発動レベルを保持しておく
            activePointStatusLevel = args.PointStatus.level;
            
            // 所持数を表示
            boostPointBeforeText.text = args.PointStatus.value.GetStringNumberWithComma();
            // 使用後の数を設定
            boostPointAfterText.text = (args.PointStatus.value - args.PointStatus.levelUpCostValue).GetStringNumberWithComma();
            
            // レベル
            boostLevelBeforeText.text = string.Format(StringValueAssetLoader.Instance["common.lv"], activePointStatusLevel);
            
            // ブーストレベル
            modalTitleText.text = string.Format(StringValueAssetLoader.Instance["training.boost_chance.modal_title"], activePointStatusLevel);
            // メッセージ
            modalMessageText.text = string.Format(StringValueAssetLoader.Instance["training.boost_chance.modal_msg"], args.PointStatus.levelUpCostValue.GetStringNumberWithComma());

            // スキップトグル
            skipToggle.SetIsOnWithoutNotify(TrainingUtility.IsSkilBoostChanceEffect);
            
            animator.Play(OpenModalAnimation);
        }
        

        
        public void OnChangeSkipToggle(bool isSkip)
        {
            TrainingUtility.IsSkilBoostChanceEffect = isSkip;
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnCancelButton()
        {
            animator.Play(CloseAnimation);
        }
        
        /// <summary>ブースト開始</summary>
        public void OnStartButton()
        {
            StartAPI().Forget();
        }
        
        /// <summary>
        /// アニメーター側のイベントを何か登録しないとエラー出てるので回避用の関数
        /// </summary>
        public void OnAnimationEventDummy()
        {
            
        }
        
        private async UniTask StartAPI()
        {
            // API
            TrainingUpdatePointLevelAPIPost post = new TrainingUpdatePointLevelAPIPost();
            TrainingUpdatePointLevelAPIRequest request = new TrainingUpdatePointLevelAPIRequest();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // 結果
            TrainingUpdatePointLevelAPIResponse response = request.GetResponseData();
            
            // 更新コールバック
            if(onUpdatePoint != null)
            {
                onUpdatePoint(response.pending, response.pointStatus);
            }
            
            await StartBoostAsync(response);
        }
        
        
        
        /// <summary>画面タップ</summary>
        public void OnNextButton()
        {
            // アニメーション再生中は停止
            if(animationCancellationToken != null)
            {
                DisposeAnimationToken();
                return;
            }
            
            isNext = true;
        }
        
        private UniTask WaitNext(bool enableAuto)
        {
            isNext = false;

            // 自動で進まない場合は画面タップ待ち
            if(enableAuto == false)
            {
                return UniTask.WaitWhile(()=>isNext == false);
            }
            
            // スキップモード
            switch(autoMode)
            {
                case AppAdvAutoMode.None:
                    return UniTask.WaitWhile(()=>isNext == false);
                
                case AppAdvAutoMode.Skip4:
                {
                    float time = SkipWaitDuration;
                    return UniTask.WaitWhile(()=>
                    {
                        time -= Time.deltaTime; 
                        return time > 0 && isNext == false;
                    });
                }
                
                default:
                {
                    float time = AutoModeWaitDuration;
                    return UniTask.WaitWhile(()=>
                    {
                        time -= Time.deltaTime; 
                        return time > 0 && isNext == false;
                    });
                }
            }
        }
        
        private void DisposeAnimationToken()
        {
            if(animationCancellationToken != null)
            {
                animationCancellationToken.Cancel();
                animationCancellationToken.Dispose();
                animationCancellationToken = null;
            }
        }

        private async UniTask PlayBoostAnimation(long effectId)
        {
            try
            {
                TrainingPointStatusEffectMasterObject mStatusEffect = MasterManager.Instance.trainingPointStatusEffectMaster.FindData(effectId);
                // テキスト
                boostEffectText.text = mStatusEffect.GetDisplayDescription(boostEffectOmissionSetter.GetOmissionData());

                long effectRank = mStatusEffect.imageId;

                // トークンの破棄
                DisposeAnimationToken();
                // キャンセルトークン
                animationCancellationToken = new CancellationTokenSource();

                // ブースト開始
                await AnimatorUtility.WaitStateAsync(animator, StartBoostAnimation, animationCancellationToken.Token);
                // 結果表示
                await AnimatorUtility.WaitStateAsync(animator, ReleaseResultAnimation + effectRank, animationCancellationToken.Token);

                // トークンの破棄
                DisposeAnimationToken();

                // 画面が押されるまで待つ
                await WaitNext(false);
            }
            // キャンセル時はスキップするので何もしない
            catch (OperationCanceledException)
            {

            }
        }

        private async UniTask PlayExtraBoostAnimation()
        {
            try
            {
                // トークンの破棄
                DisposeAnimationToken();
                // キャンセルトークン
                animationCancellationToken = new CancellationTokenSource();

                // Exブースト開始
                await AnimatorUtility.WaitStateAsync(animator, ExtraBoostAnimation, animationCancellationToken.Token);

                // トークンの破棄
                DisposeAnimationToken();
            }
            // キャンセル時はスキップするので何もしない
            catch (OperationCanceledException)
            {
              
            }
        }

        /// <summary> スペシャルブーストアニメーション再生 </summary>
        private async UniTask PlaySpecialBoostBonusAnimation(long mCharaId)
        {
            try
            {
                // スペシャルブースト発動キャラ
                TrainingCharacterData boostActivationChara = args.GetTrainingCharacterData(mCharaId);

                // 発動キャラのアイコンセット
                await specialBoostCharaIcon.SetIconAsync(boostActivationChara.MCharId, boostActivationChara.Lv, boostActivationChara.LiberationId);

                // トークンの破棄
                DisposeAnimationToken();
                // キャンセルトークン
                animationCancellationToken = new CancellationTokenSource();

                // Spブースト開始
                await AnimatorUtility.WaitStateAsync(animator, SpecialBoostAnimation, animationCancellationToken.Token);

                // トークンの破棄
                DisposeAnimationToken();
            }
            // キャンセル時はスキップするので何もしない
            catch (OperationCanceledException)
            {
              
            }
        }

        public async UniTask StartBoostAsync(TrainingUpdatePointLevelAPIResponse response)
        {
            // モーダルを閉じる
            await AnimatorUtility.WaitStateAsync(animator, CloseModalAnimation);
            
            // 獲得したブースト
            long effectId = -1;
            // Exブーストがあるか
            bool hasExtraBoost = response.additionMTrainingPointStatusEffectIdList != null && response.additionMTrainingPointStatusEffectIdList.Length > 0;
            // Spブーストがあるか
            bool hasSpecialBoost = response.charaMTrainingPointStatusEffectIdList != null && response.charaMTrainingPointStatusEffectIdList.Length > 0;
            
            TrainingPointStatus pointStatus = null;
            
            // ブーストチャンス実行時のログを追加
            appAdvManager.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance"], string.Format(StringValueAssetLoader.Instance["training.log.start_boost_chance"], activePointStatusLevel), 0);

            
            // 獲得したブースト
            // 一個しか獲得しない仕様らしい
            if(response.mTrainingPointStatusEffectIdList != null && response.mTrainingPointStatusEffectIdList.Length > 0)
            {
                effectId = response.mTrainingPointStatusEffectIdList[0];
                TrainingPointStatusEffectMasterObject mStatusEffect = MasterManager.Instance.trainingPointStatusEffectMaster.FindData(effectId);
                // ブーストチャンス効果抽選結果のログを追加
                appAdvManager.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance.effect"], mStatusEffect.GetDisplayDescription(), 0);
            }
            
            // 獲得したExブースト
            // エクストラブーストは複数取得する可能性がある
            if(hasExtraBoost)
            {
                // エクストラブースト発生ログを追加
                appAdvManager.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance"], StringValueAssetLoader.Instance["training.log.extra_boost"], 0);
                // エクストラブーストで獲得したブースト分ログを追加する
                foreach (long exBoostId in response.additionMTrainingPointStatusEffectIdList)
                {
                    TrainingPointStatusEffectMasterObject mStatusEffect = MasterManager.Instance.trainingPointStatusEffectMaster.FindData(exBoostId);
                    // エクストラブースト効果抽選結果のログを追加
                    appAdvManager.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance.effect"], mStatusEffect.GetDisplayDescription(), 0);
                }
            }

            // Spブースト発生時のログ追加
            if (hasSpecialBoost)
            {
                // Spブースト効果名
                string spBoostEffectName = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(response.mTrainingPointStatusEffectCharaId).name;
                // スペシャルブースト発生ログを追加
                appAdvManager.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance"], string.Format(StringValueAssetLoader.Instance["training.log.special_boost"], spBoostEffectName), 0);
                // スペシャルブーストで獲得したブースト分ログを追加する
                foreach (long spBoostId in response.charaMTrainingPointStatusEffectIdList)
                {
                    TrainingPointStatusEffectMasterObject mStatusEffect = MasterManager.Instance.trainingPointStatusEffectMaster.FindData(spBoostId);
                    // スペシャルブースト効果抽選結果のログを追加
                    appAdvManager.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance.effect"], mStatusEffect.GetDisplayDescription(), 0);
                }
            }
            
            pointStatus = response.pointStatus;
            
            // 実行後のレベル
            boostLevelAfterText.text = string.Format(StringValueAssetLoader.Instance["common.lv"], pointStatus.level);

            
            // スキップ
            if(skipToggle.isOn == false && autoMode != AppAdvAutoMode.Skip4)
            {
                // 獲得したブーストがある
                if (effectId >= 0)
                {
                    await PlayBoostAnimation(effectId);
                }

                // 獲得したExブーストがある
                if(hasExtraBoost)
                {
                    foreach(long extraEffectId in response.additionMTrainingPointStatusEffectIdList)
                    {
                        await PlayExtraBoostAnimation();
                        await PlayBoostAnimation(extraEffectId);
                    }
                    
                }

                // 獲得したSpブースト
                if (hasSpecialBoost)
                {
                    foreach(long spEffectId in response.charaMTrainingPointStatusEffectIdList)
                    {
                        TrainingPointStatusEffectCharaMasterObject pointStatusEffectCharaId = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(response.mTrainingPointStatusEffectCharaId);
                        long charaId = pointStatusEffectCharaId.mCharaId;
                            
                        await PlaySpecialBoostBonusAnimation(charaId);
                        await PlayBoostAnimation(spEffectId);
                    } 
                }
            }
            
            // ビューのセット
            boostEffectView.Set(effectId);
            // Exブースト表示
            extraBoostRoot.SetActive(hasExtraBoost);

            // Spブースト表示
            specialBoostRoot.SetActive(hasSpecialBoost);
            
            // 結果表示
            await AnimatorUtility.WaitStateAsync(animator, BoostEffectAnimation);
            
            // 一覧
            List<TrainingBoostEffectScrollItem.BoostData> boostList = new List<TrainingBoostEffectScrollItem.BoostData>();
            
            foreach(long id in response.additionMTrainingPointStatusEffectIdList)
            {
                boostList.Add( new TrainingBoostEffectScrollItem.BoostData(id) );
            }
            
            // スクロールにセット
            await boostEffectScroll.SetDataList(boostList);

            // 発動したスペシャルブーストがある
            if (hasSpecialBoost)
            {
                // スペシャルブーストの発動キャラ
                TrainingPointStatusEffectCharaMasterObject mTrainingPointStatusEffectChara = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(response.mTrainingPointStatusEffectCharaId);
                TrainingCharacterData characterData = args.GetTrainingCharacterData(mTrainingPointStatusEffectChara.mCharaId);
            
                // スペシャルブースト効果
                List<TrainingBoostEffectScrollItem.BoostData> specialBoostList = new List<TrainingBoostEffectScrollItem.BoostData>();
                foreach (long id in response.charaMTrainingPointStatusEffectIdList)
                {
                    specialBoostList.Add(new TrainingBoostEffectScrollItem.BoostData(id, characterData));
                }
            
                // スペシャルブーストをセット
                await specialBoostScroll.SetDataList(specialBoostList);
            }

            // 画面が押されるまで待つ
            await WaitNext(true);
            // 閉じる
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            // トークンの破棄
            DisposeAnimationToken();
            
            // リスト初期化
            boostEffectScroll.Clear();
            specialBoostScroll.Clear();
            
            // 演出完了処理実行
            onComplete();
        }
    }
}