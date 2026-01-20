using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Utility;
using Pjfb.Voice;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingEventResultPage : TrainingPageBase
    {
        private static readonly string LogSkillGetKey = "training.log.skill_get";
        private static readonly string LogStatusUpKey = "training.log.status_up";
        private static readonly string LogPerformanceUpKey = "training.log.performance_up";
        private static readonly string LogPerformanceLvMaxKey = "training.log.performance_lv_max";
        private static readonly string LogPracticeLvUpKey = "training.log.practice_lv_up";
        private static readonly string LogPracticeLvMaxKey = "training.log.practice_lv_max";
        private static readonly string LogTipGetKey = "training.log.tip_get";
        private static readonly string LogCardComboGetKey = "training.log.card_combo_get";
        
        private struct StatusUpData
        {
            public CharacterStatusType type;
            public BigValue value;
            
            public StatusUpData(CharacterStatusType type, BigValue value)
            {
                this.type = type;
                this.value = value;
            }
        }
        
        protected enum AnimationState
        {
            None,
            /// <summary> カードユニオン演出 </summary>
            CardUnion,
            //// <summary> カードコンボ演出 </summary>
            CardCombo,
            //// <summary> カードコンボ演出 </summary>
            CardComboClose,
            /// <summary>風景の表示</summary>
            SceneStart,
            /// <summary>風景の表示</summary>
            SceneEnd,
            /// <summary>ボーナス表示</summary>
            Bonus,
            /// <summary>スキル取得</summary>
            SkillGet,
            /// <summary>スキル取得を閉じる</summary>
            SkillClose,
            /// <summary>インスピレーション取得</summary>
            InspirationGet,
            /// <summary>インスピレーション取得</summary>
            InspirationClose,
            /// <summary>Flowインスピレーション取得</summary>
            FlowInspirationGet,
            /// <summary>Flowインスピレーション取得終了</summary>
            FlowInspirationClose,
            /// <summary>練習LvUp</summary>
            LvUpPractice,
            /// <summary>練習LvUp</summary>
            LvUpPracticeClose,
            /// <summary>パフォーマンスLvUp</summary>
            LvUpPerformance,
            /// <summary>パフォーマンスLvUp</summary>
            LvUpPerformanceClose,
            /// <summary>インスピレーションブーストLvUp</summary>
            LvUpInspirationBoost,
            /// <summary>インスピレーションブーストLvUp</summary>
            LvUpInspirationBoostClose,
            /// <summary> Flowポイント獲得 </summary>
            GetFlowPoint,
            /// <summary> Flowポイント獲得終了 </summary>
            GetFlowPointClose,
            /// <summary>ステータスアップ</summary>
            StatusUp,
            /// <summary>ステータスアップをテキスト表示</summary>
            StatusUpText,
            /// <summary>ターン数延長の権利を獲得</summary>
            GetExtraTurnRight,
            /// <summary>インスピレーションブースト</summary>
            InspirationBoost,
            /// <summary>スキル追加獲得</summary>
            ExtraSkillGet,
            /// <summary>スキル追加獲得を閉じる</summary>
            ExtraSkillGetClose,
            //// <summary> ターン延長ポイント変換 </summary>
            ConvertExtraTurn,
            //// <summary> ターン延長ポイント変換 </summary>
            ConvertExtraTurnClose,
            //// <summary> コンディションポイント変換 </summary>
            ConvertConditionPoint,
            //// <summary> コンディションポイント変換 </summary>
            ConvertConditionPointClose,
            //// <summary> トータルボーナス表示演出 </summary>
            TotalBonus,
            //// <summary> トータルボーナス表示演出 </summary>
            TotalBonusClose,
            End
        }

        [SerializeField]
        private TrainingStatusValuesView statusView = null;
        
        [SerializeField]
        private TrainingEventResultTrainingScene trainingScene = null;
        [SerializeField]
        private TrainingEventResultBonusAnimation bonusAnimation = null;
        [SerializeField]
        private TrainingEventResultAnimation resultAnimation = null;
        [SerializeField]
        private ExtraTurnRightView extraTurnRightView = null;
        [SerializeField]
        private TrainingEventResultBonusAnimation inspirationBoostAnimation = null;
        [SerializeField]
        private TrainingEventResultConvertExtraTurnPointAnimation convertExtraTurnAnimation = null;
        [SerializeField]
        private TrainingEventResultConvertConditionPointAnimation convertConditionPointAnimation;
        [SerializeField] 
        private TrainingCardComboBaseAnimation cardComboBaseAnimation;
        // カードユニオン演出
        [SerializeField]
        private TrainingEventResultCardUnionAnimation cardUnionAnimation = null;
        
        // アニメーションステート
        protected AnimationState animationState = AnimationState.CardCombo;
        private AnimationState currentAnimationState = AnimationState.None;
        
        // スキル表示
        private int skillIndex = 0;
        // ステータスアップリスト
        private List<StatusUpData> statusUpList = new List<StatusUpData>();
        // スキル取得用のキュー
        private Queue<TrainingUtility.AbilityCategoryData> getSkillDataQueue = null;
        // インスピレーション取得用のキュー
        private Queue<TrainingUtility.InspirationTypeData> getInspirationDataQueue = null;
        // インスピレーション取得用のキュー
        private Queue<TrainingUtility.InspirationTypeData> getFlowInspirationDataQueue = null;
        // 実行するカードコンボId(実行されると取り除かれる)
        private Queue<long> cardComboQueue = new Queue<long>();        

        private bool isEnded = false;
        

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // スキップモード変更不可
            Adv.Footer.EnableAutoButton(false);
            // ヘッダーの更新
            Header.UpdateCondition(MainArguments.Pending);
            
            // トレーニングシーンの読み込み
            if(MainArguments.TrainingCardId > 0)
            {
                // 必要なリソースの読み込み
                await trainingScene.LoadResource(MainArguments, IsSkipMode);
            }
            
            Camera3DUtility.Instance.ShowRenderTextureBackGround(MainArguments.TrainingCardId > 0 && IsSkipMode == false && TrainingUtility.IsPlay3DEffect );
            
            await base.OnPreOpen(args, token);
        }

        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            if (MainArguments.activeTimeline != null)
            {
                ObjectPoolUtility.ResetPool();
                MainArguments.activeTimeline.gameObject.SetActive(false);
                Destroy(MainArguments.activeTimeline.gameObject);
            }
            return base.OnPreClose(token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            // ステータス表示
            // 強化前のステータスを表示しておく
            statusView.SetStatus(MainArguments.Status - MainArguments.RewardStatus);
            
            // ステートを初期化
            animationState = AnimationState.None;
            // 現在のステート初期化
            currentAnimationState = AnimationState.None;
            
            isEnded = false;
            // ヘッダー
            Header.SetActiveAllPage();
            // ステータスを表示
            statusView.gameObject.SetActive(true);
            
            // キャラを表示
            MainPageManager.Character.gameObject.SetActive(true);
            
            resultAnimation.Hide();
            
            // Flowゾーン演出
            if (MainArguments.IsChangeConcentration())
            {
                if (MainArguments.IsFlow())
                {
                    // Flow演出
                    OpenPage(TrainingMainPageType.FlowEffect, MainArguments);
                    return;
                }
            }
            
            // インスピレーションブーストがある場合
            if(MainArguments.Reward.inspireEnhanceRate > 0)
            {
                animationState = AnimationState.InspirationBoost;
            }
            else
            {
                animationState = MainArguments.IsShowBonus() ? AnimationState.FlowInspirationGet : AnimationState.SkillGet;
            }
            
            // ステータス
            statusUpList.Clear();
            foreach(CharacterStatusType type in TrainingUtility.StatusUpTypes)
            {
                BigValue value = MainArguments.RewardStatus[type];
                if(value != 0)
                {
                    statusUpList.Add( new StatusUpData(type, value) );
                }
            }
            
            // 取得スキルのキューを作成
            getSkillDataQueue = TrainingUtility.GetAbilityCategoryQueue(MainArguments.Reward.getAbilityMapList);
            
            // 取得インスピレーションのキューを作成
            getInspirationDataQueue = TrainingUtility.GetInspirationQueue(MainArguments.Reward.inspireList);
            // Flowでの取得インスピレーションのキューを作成
            getFlowInspirationDataQueue = TrainingUtility.GetInspirationQueue(MainArguments.Reward.flowInspireList);
            
            cardComboQueue.Clear();

            if (MainArguments.Reward.mTrainingCardComboIdList != null)
            {
                // 発生したカードコンボId(ソート済み)
                long[] cardComboIdList = MainArguments.Reward.mTrainingCardComboIdList
                    // Priorityで昇順にソート(リザルトではpriorityが低いものから再生するので) 
                    .OrderBy(x => MasterManager.Instance.trainingCardComboMaster.FindData(x).priority)
                    // Idで降順でソート
                    .ThenByDescending(x => x).ToArray();

                // 実行するカードコンボのIdをキューに入れておく
                foreach (long id in cardComboIdList)
                {
                    cardComboQueue.Enqueue(id);
                }
            }


#if !PJFB_REL
            // Advのログに入れる
            Adv.AddMessageLog($"<color=#f00>[デバッグ]</color> {MainArguments.ActionName}", $"コンディション変動: {MainArguments.Reward.condition}<br>コンディション値: {MainArguments.Pending.condition}/{MainArguments.Pending.maxCondition}", 0);
#endif
            
            // スキル表示位置
            skillIndex = 0;
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            // 演出を再生する
            NextEffect();
        }

        protected virtual void NextEffect()
        {
            
            if(currentAnimationState == animationState)return;
            currentAnimationState = animationState;
            
            switch(animationState)
            {
                // Flowインスピレーション取得
                case AnimationState.FlowInspirationGet:
                {
                    PlayInspirationEffect(getFlowInspirationDataQueue, AnimationState.CardUnion, AnimationState.FlowInspirationClose);
                    break;
                }
                
                case AnimationState.FlowInspirationClose:
                {
                    // 閉じるアニメーション
                    resultAnimation.CloseInspirationGetAnimation(()=> 
                    {
                        // 全て表示するまで繰り返す
                        animationState = AnimationState.FlowInspirationGet;
                        NextEffect();
                    });
                    break;
                }
                // カードユニオン
                case AnimationState.CardUnion:
                {
                    // カードユニオンが発生
                    if (MainArguments.HasCardUnion())
                    {
                        // カードユニオン演出再生
                        cardUnionAnimation.PlayAnimation(MainArguments, Adv.AppAutoMode,
                            () =>
                            {
                                animationState = AnimationState.CardCombo;
                                NextEffect();
                            });
                        break;
                    }
                    
                    // カードユニオンが発生していないならカードコンボへ
                    animationState = AnimationState.CardCombo;
                    NextEffect();
                    break;
                }
                // カードコンボ演出
                case AnimationState.CardCombo:
                {
                    // 実行するカードコンボがないなら演出をしない
                    if (cardComboQueue.Count <= 0)
                    {
                        animationState = AnimationState.SceneStart;
                        NextEffect();
                        break;
                    }
                    
                    // キューからコンボIdを取り出す
                    long cardComboId = cardComboQueue.Dequeue();
                    TrainingCardComboMasterObject master = MasterManager.Instance.trainingCardComboMaster.FindData(cardComboId);
                    // 発生するコンボカードIdを取得(ソート済み)
                    long[] comboCardIdList = MasterManager.Instance.trainingCardComboElementMaster.GetComboCardSortIdList(cardComboId, MainArguments.TrainingCardId);
                    // コンボ数
                    long comboValue = comboCardIdList.Length;
                    // ステータス加算％
                    long statusPercent = master.baseStatusRate / 100;
                    // コンボ倍率
                    long comboBonusRate = master.comboBonusRate;

                    // コンボ発動をログに追加
                    Adv.AddMessageLog(MainArguments.ActionName, string.Format(StringValueAssetLoader.Instance[LogCardComboGetKey], comboValue), 0);
                    
                    TrainingCardComboBaseAnimation.EffectMode effectMode = TrainingCardComboBaseAnimation.EffectMode.None;
                    
                    // カードコンボ演出スキップ
                    if (IsCardComboEffectSkip)
                    {
                        effectMode = TrainingCardComboBaseAnimation.EffectMode.Skip;
                    }
                    // カードコンボ演出オート再生
                    else if (IsCardComboEffectAuto)
                    {
                        effectMode = TrainingCardComboBaseAnimation.EffectMode.Auto;
                    }
                    
                    // カードコンボ演出再生
                    cardComboBaseAnimation.PlayCardComboAnimation(comboValue, statusPercent, comboBonusRate, comboCardIdList, effectMode, () =>
                    {
                        animationState = AnimationState.CardComboClose;
                        NextEffect();
                    });
                    break;
                }
                // カードコンボ演出終了
                case AnimationState.CardComboClose:
                {
                    // 演出を閉じる
                    cardComboBaseAnimation.CloseAnimation(() =>
                    {
                        // コンボが連続する場合があるのでいったんカードコンボ演出に
                        animationState = AnimationState.CardCombo;
                        NextEffect();
                    });
                    break;
                }
                case AnimationState.SceneStart:
                {
                    
                    if(IsSkipMode)
                    {
                        // ボーナス表示へ
                        animationState = AnimationState.Bonus;
                        NextEffect();
                        break;
                    }
                    
                    // タッチガードOn
                    AppManager.Instance.UIManager.System.TouchGuard.Show();
                    // トレーニング風景再生
                    trainingScene.PlayOpenAnimation(MainArguments, ()=>
                    {
                        // タッチガードOff
                        AppManager.Instance.UIManager.System.TouchGuard.Hide();
                        // ボーナス表示へ
                        animationState = AnimationState.Bonus;
                        NextEffect();
                    });
                    break;
                }
                
                case AnimationState.SceneEnd:
                {
                    
                    if(IsSkipMode)
                    {
                        // ボーナス表示へ
                        animationState = AnimationState.SkillGet;
                        NextEffect();
                        break;
                    }

                    trainingScene.PlayCloseAnimation(()=>
                    {
                        // ボーナス表示へ
                        animationState = AnimationState.SkillGet;
                        NextEffect();
                        var data = MasterManager.Instance.trainingCardMaster.FindData(MainArguments.TrainingCardId);
                        TagHelperUtility.SetEnable(TagHelperUtility.GroupEnum.Model3D, $"training_{data.practiceType}", false);
                        Camera3DUtility.Instance.ShowRenderTextureBackGround(false);
                    });
                    break;
                }
            
                // 成功率を表示
                case AnimationState.Bonus:
                {
                    long value = MainArguments.Reward.conditionEffectRate / 100;
                    long baseValue = MainArguments.Reward.baseConditionEffectRate / 100;
                    bool isExtraBonus = (MainArguments.Reward.isGradeUp || MainArguments.Reward.isTrainingGradeUp);
                    // 該当する倍率(万分率なので10000で割る) 
                    long boostBonusRate = MainArguments.Reward.pointStatusEffectRate / 10000;
                    // ブーストボーナスが発生するか
                    bool isBoostBonus = MainArguments.Reward.pointStatusEffectRate > 0;
                    bonusAnimation.PlayAnimation(MainArguments.TrainingCharacter.MCharId, value, IsFastMode, MainArguments.Reward.getAbilityMapList.Length > 0, isExtraBonus, baseValue, isBoostBonus, boostBonusRate, MainArguments.Reward.pointStatusEffectRateLabelType, ()=>
                    {
                        // ブーストボーナスが発生するならトータルボーナス表示演出にそうでないなら取得スキル表示へ
                        animationState = isBoostBonus ? AnimationState.TotalBonus : AnimationState.SceneEnd;
                        NextEffect();
                    });
                    
                    break;
                }
                
                // トータルボーナス表示演出
                case AnimationState.TotalBonus:
                {
                    bonusAnimation.PlayTotalBonusAnimation(() =>
                    {
                        animationState = AnimationState.TotalBonusClose;
                        // 自動スキップなら次に進む
                        if (IsTotalBonusEffectSkip) NextEffect();
                    });
                    break;
                }

                // トータルボーナス表示演出
                case AnimationState.TotalBonusClose:
                {
                    bonusAnimation.CloseTotalBonusAnimation(() =>
                    {
                        animationState = AnimationState.SceneEnd;
                        NextEffect();
                    });
                    break;
                }
                
                // インスピレーションブースト
                case AnimationState.InspirationBoost:
                {
                    long value = MainArguments.Reward.inspireEnhanceRate / 100;
                    
                    inspirationBoostAnimation.PlayInspirationAnimation(MainArguments.TrainingCharacter.MCharId, value, IsInspirationBoostEffectSkip, MainArguments.Reward.getAbilityMapList.Length > 0, false, 0, false, 0, 0, ()=>
                    {
                        // 取得スキル表示へ
                        animationState = AnimationState.SkillGet;
                        NextEffect();
                    });
                    
                    break;
                }
                
                case AnimationState.SkillGet:
                {                    
                    // 表示なし
                    if(getSkillDataQueue.Count <= 0)
                    {
                        animationState = AnimationState.InspirationGet;
                        NextEffect();
                        break;
                    }

                    var skillData = getSkillDataQueue.Dequeue();
                    // 取得スキル
                    TrainingAbility ability = skillData.Skill;
                    // mAbility
                    AbilityMasterObject mAbility = MasterManager.Instance.abilityMaster.FindData(ability.id);
                    // Se
                    SEManager.PlaySE(SE.se_training_skill_get);
                    // ボイス
                    PlayTrainingCharacterVoice(VoiceResourceSettings.LocationType.SYSTEM_LV_UP);

                    // Advのログに入れる
                    Adv.AddMessageLog(MainArguments.ActionName, string.Format(StringValueAssetLoader.Instance[LogSkillGetKey], mAbility.name), 0);


                    resultAnimation.PlayGetSkillAnimation(ability, skillData.Category, ()=>
                    {
                        // 閉じるアニメーションへ
                        animationState = AnimationState.SkillClose;
                        // スキップ中は自動で次に進む
                        if(IsFastMode)NextEffect();
                    });
                    break;
                }
                
                case AnimationState.SkillClose:
                {
                    if(resultAnimation.IsPlayingCloseAnimation == false)
                    {
                        resultAnimation.CloseStatusUpAnimation(()=> 
                        {
                            // 取得スキルのキューが空になるまで繰り返す
                            animationState = AnimationState.SkillGet;    
                            NextEffect();
                        });
                    }
                    break;
                }

                case AnimationState.InspirationGet:
                {
                    PlayInspirationEffect(getInspirationDataQueue, AnimationState.LvUpInspirationBoost, AnimationState.InspirationClose);
                    break;
                }
                
                case AnimationState.InspirationClose:
                {
                    // 閉じるアニメーション
                    resultAnimation.CloseInspirationGetAnimation(()=> 
                    {
                        // 全て表示するまで繰り返す
                        animationState = AnimationState.InspirationGet;
                        NextEffect();
                    });
                    break;
                }
                
                case AnimationState.LvUpInspirationBoost:
                {
                    // レベルアップがあるか
                    if(MainArguments.Reward.isInspireLevelUp == false)
                    {
                        animationState = AnimationState.LvUpPractice;
                        NextEffect();
                        break;
                    }
                    

                    
                    // スキップ
                    if(IsInspirationBoostEffectSkip)
                    {
                        // パフォーマンスへ
                        animationState = AnimationState.LvUpPractice;
                        // メッセージ
                        ReservationMessage( StringValueAssetLoader.Instance["training.message.inspiration_boost_lvup"] );
                        NextEffect();
                    }
                    else
                    {
                        // 閉じるアニメーション
                        animationState = AnimationState.LvUpInspirationBoostClose;
                        // アニメーション再生
                        resultAnimation.PlayInspirationBoostLvUpAnimationAsync(()=>
                        {
                            // スキップ中は自動で次に進む
                            if(IsFastMode)NextEffect();
                        }).Forget();
                    }
                    
                    break;
                }
                
                case AnimationState.LvUpInspirationBoostClose:
                {
                    if(resultAnimation.IsPlayingCloseAnimation == false)
                    {
                        resultAnimation.CloseStatusUpAnimation(()=> 
                        {
                            animationState = AnimationState.LvUpPractice;
                            NextEffect();
                        });
                    }
                    break;
                }
                
                case AnimationState.LvUpPractice:
                {
                    // レベルアップがあるか
                    if(MainArguments.IsLvupPractice == false)
                    {
                        animationState = AnimationState.GetFlowPoint;
                        NextEffect();
                        break;
                    }
                
                    // ログを入れる
                    TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(MainArguments.TrainingCardId);
                    long lv = MainArguments.Pending.practiceProgressList[MainArguments.SelectedTrainingCardIndex].level;
                    bool isLvMax = MainArguments.IsLvMaxPracticeCard(MainArguments.SelectedTrainingCardIndex);
                    string name = StringValueAssetLoader.Instance[$"practice_name{mCard.practiceType}"];
                    Adv.AddMessageLog(MainArguments.ActionName, string.Format(StringValueAssetLoader.Instance[LogPracticeLvUpKey], name, isLvMax ? StringValueAssetLoader.Instance[LogPracticeLvMaxKey] : lv.ToString()), 0);
                
                    // Se
                    SEManager.PlaySE(SE.se_training_skill_get);
                    // 閉じるアニメーション
                    animationState = AnimationState.LvUpPracticeClose;
                    // アニメーション再生
                    resultAnimation.PlayPracticeLvUpAnimation(MainArguments.TrainingCardId, lv, isLvMax, ()=>
                    {
                        // スキップ中は自動で次に進む
                        if(IsFastMode)NextEffect();
                    });
                    
                    break;
                }
                
                case AnimationState.LvUpPracticeClose:
                {
                    if(resultAnimation.IsPlayingCloseAnimation == false)
                    {
                        resultAnimation.CloseStatusUpAnimation(()=> 
                        {
                            animationState = AnimationState.GetFlowPoint;
                            NextEffect();
                        });
                    }
                    break;
                }

                // Flowポイント獲得
                case AnimationState.GetFlowPoint:
                {
                    // コンセントレーションEXP獲得時(ステータス上昇に限らず表示)
                    if (MainArguments.Reward.concentrationExp > 0 || MainArguments.Reward.concentrationExpAddTurn > 0 || MainArguments.Reward.concentrationExpConditionTier > 0)
                    {
                        // 獲得Exp
                        long addExp = MainArguments.Reward.concentrationExp;
                        // ターン延長分Exp
                        long addExpAddTurn = MainArguments.Reward.concentrationExpAddTurn;
                        // 変換ターン数
                        long convertTurn = MainArguments.Reward.pointConvertAddedTurnValue;
                        // コンディションポイント変換分Exp
                        long addExpConditionTier = MainArguments.Reward.concentrationExpConditionTier;
                        
                        // 現在のExp(増加前のExpを求める)
                        long currentExp = MainArguments.Pending.concentrationExp - MainArguments.Reward.concentrationExp - MainArguments.Reward.concentrationExpAddTurn - MainArguments.Reward.concentrationExpConditionTier;
                        Header.FlowLevelView.PlayGetPointAnimation(Adv, currentExp, addExp, addExpAddTurn, convertTurn, addExpConditionTier, () =>
                        {
                            animationState = AnimationState.GetFlowPointClose;
                            NextEffect();
                        });
                    }
                    else
                    {
                        animationState = AnimationState.StatusUp;
                        NextEffect();
                    }
                    break;
                }
                
                case AnimationState.GetFlowPointClose:
                {
                    // 演出終了
                    Header.FlowLevelView.StopAnimation();
                    animationState = AnimationState.StatusUp;
                    NextEffect();
                    break;
                }
                    
                case AnimationState.LvUpPerformance:
                {
                    animationState = AnimationState.LvUpPerformanceClose;
                    // スキップ中は自動で次に進む
                    if(IsFastMode)NextEffect();
                    break;
                }
                
                case AnimationState.LvUpPerformanceClose:
                {
                    if(resultAnimation.IsPlayingCloseAnimation == false)
                    {
                        resultAnimation.CloseStatusUpAnimation(()=> 
                        {
                            animationState = AnimationState.StatusUpText;
                            NextEffect();
                        });
                    }
                    break;
                }
                
                case AnimationState.StatusUp:
                {
                    // 予約していたメッセージの表示
                    SetReservationMessage();
                    
                    // 表示なし
                    if(statusUpList.Count <= 0 && MainArguments.Reward.hp <= 0)
                    {
                        animationState = AnimationState.ExtraSkillGet;
                        NextEffect();
                        break;
                    }
                    
                    // インスピレーションブースト
                    Adv.SetInspirationBoost(MainArguments.Reward.inspireEnhanceRate);
                    // ブースト値の表示
                    string inspirationBoost = TrainingUtility.GetInspirationBoostMessage(MainArguments.Reward.inspireEnhanceRate);
                    // ランクアップ
                    foreach(StatusUpData data in statusUpList)
                    {
                        // Advのログに入れる
                        Adv.AddMessageLog(MainArguments.ActionName, string.Format(StringValueAssetLoader.Instance[LogStatusUpKey], StatusUtility.GetStatusName(data.type), data.value) + inspirationBoost, 0);
                    }
                    
                    // チップ枚数
                    Adv.SetRewardTipCount(MainArguments.Reward.hp);
                    
                    // チップ報酬
                    if(MainArguments.Reward.hp > 0)
                    {
                        
                        // Advのログに入れる
                        Adv.AddMessageLog(MainArguments.ActionName, string.Format(StringValueAssetLoader.Instance[LogTipGetKey], MainArguments.Reward.hp), 0);
                        
                        Header.PerformaceView.Add(MainArguments.Pending.overallProgress, MainArguments.Reward.hp, ()=>
                        {
                            // Advのログに入れる
                            Adv.AddMessageLog(MainArguments.ActionName, string.Format(StringValueAssetLoader.Instance[LogPerformanceUpKey], MainArguments.IsLvMaxPerformance() ? StringValueAssetLoader.Instance[LogPerformanceLvMaxKey] : MainArguments.Pending.overallProgress.currentLevel), 0);
                            // Lvアップ演出
                            resultAnimation.PlayParforceLvUpAnimation(MainArguments.Pending.overallProgress.currentLevel, MainArguments.IsLvMaxPerformance(), ()=>
                            {
                                // ステート
                                animationState = AnimationState.LvUpPerformance;
                                NextEffect();
                            });
                        });
                    }
                    
                    // Se
                    SEManager.PlaySE(SE.se_training_status_up);
                    // この時点でステート変更 (画面タップでステータス演出スキップできるように
                    animationState = AnimationState.StatusUpText;
                    // ステータスUpアニメーション再生
                    statusView.PlayStatusUpAnimation(MainArguments, Adv, MainPageManager.ConcentrationZoneEffectPlayer, MainPageManager.CharacterEffectRoot, ()=>
                    {
                        if(MainArguments.IsLvupParformace == false)
                        {
                            if(currentAnimationState == AnimationState.StatusUp)
                            {
                                NextEffect();
                            }
                        }
                    });
                    
                    break;
                }
                
                
                case AnimationState.StatusUpText:
                {
                    // ステータスを非表示に
                    statusView.Close();
                    // 終了コールバック
                    Adv.OnEnded -= OnEndStatusUpScenario;
                    Adv.OnEnded += OnEndStatusUpScenario;

                    // ステータス上昇量をセット
                    Adv.SetStatusUpData(MainArguments.RewardStatus);
                    // パフォーマンス
                    if (MainArguments.IsLvupParformace)
                    {
                        Adv.SetPerformanceLv(MainArguments.Pending.overallProgress.currentLevel, MainArguments.IsLvMaxPerformance());
                    }
                    else
                    {
                        Adv.SetPerformanceLv(0, false);
                    }

                    // 練習
                    if (MainArguments.IsLvupPractice)
                    {
                        TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(MainArguments.TrainingCardId);
                        long lv = MainArguments.Pending.practiceProgressList[MainArguments.SelectedTrainingCardIndex].level;
                        Adv.SetPracticeLv(mCard.practiceType, lv, MainArguments.IsLvMaxPracticeCard(MainArguments.SelectedTrainingCardIndex));
                    }
                    else
                    {
                        Adv.SetPracticeLv(-1, 0, false);
                    }

                    // Advを再生
                    Adv.LoadAdvFile(ResourcePathManager.GetPath(AppAdvManager.ResourcePathKey, TrainingUtility.Config.StatusUpScenarioId));
                    break;
                }

                // スキル追加獲得
                case AnimationState.ExtraSkillGet:
                {
                    if (MainArguments.Reward.mAbilityTrainingPointStatusList.Length > 0)
                    {
                        resultAnimation.PlayExtraSkillGetAnimationAsync(MainArguments.Reward.mAbilityTrainingPointStatusList, Adv, () =>
                        {
                            // 閉じるアニメーションへ
                            animationState = AnimationState.ExtraSkillGetClose;
                        }).Forget();
                    }
                    else
                    {
                        animationState = AnimationState.GetExtraTurnRight;
                        NextEffect();
                    }
                    break;
                }

                // スキル追加獲得を閉じる
                case AnimationState.ExtraSkillGetClose:
                {
                    if (resultAnimation.IsPlayingCloseAnimation == false)
                    {
                        resultAnimation.CloseStatusUpAnimation(() =>
                        {
                            animationState = AnimationState.GetExtraTurnRight;
                            NextEffect();
                        });
                    }
                    break;
                }

                case AnimationState.GetExtraTurnRight:
                {
                    if (MainArguments.Reward.turnAddValue <= 0)
                    {
                        // ブーストポイントが有効時
                        if (TrainingUtility.IsEnableBoostPoint(MainArguments.Pending.mTrainingScenarioId))
                        {
                            if (MainArguments.Reward.pointConvertAddedTurnValue > 0)
                            {
                                animationState = AnimationState.ConvertExtraTurn;
                                NextEffect();
                            }
                            else
                            {
                                animationState = AnimationState.ConvertConditionPoint;
                                NextEffect();
                            }
                        }
                        else
                        {
                            animationState = AnimationState.End;
                            NextEffect();
                        }
                    }
                    else
                    {
                        extraTurnRightView.PlayEffectAsync(
                            new ExtraTurnRightView.PlayArguments(Header, Adv, MainArguments, IsFastMode),
                            () =>
                            {
                                animationState = AnimationState.ConvertConditionPoint;
                                NextEffect();
                            }).Forget();
                    }
                    break;
                }
                // ターン延長ポイント変換処理
                case AnimationState.ConvertExtraTurn:
                {
                    // 変換された延長ターン
                    long extraTurnValue = MainArguments.Reward.pointConvertAddedTurnValue;
                    // 変換されたポイント数
                    long convertPointValue = MainArguments.Reward.addedTurnPointValue;
                    // ブーストポイント表示UIに必要なデータ
                    TrainingEventResultConvertBaseAnimation.Param param = new TrainingEventResultConvertBaseAnimation.Param()
                    {
                        // コンディションポイント変換分を引いた数値を渡す(ターン延長とポイント変換が同時に流れる際にこの段階で最終結果がでてしまうので)
                        TrainingPointValue = MainArguments.PointStatus.value - MainArguments.Reward.conditionTierPointValue,
                        TrainingScenarioId = MainArguments.Pending.mTrainingScenarioId,
                        TrainingPointLevel = MainArguments.PointStatus.level,
                        TrainingHandReloadCount = MainArguments.PointStatus.handResetCount,
                        AdvManager = Adv,
                        MainArguments = MainArguments,
                        isAuto = IsFastMode,
                    };
                    
                    convertExtraTurnAnimation.PlayConvertExtraTurnPointAnimation(param, extraTurnValue, convertPointValue, () =>
                    {
                        animationState = AnimationState.ConvertExtraTurnClose;
                        // オートモードなら自動で次に
                        if(IsFastMode)NextEffect();
                    });
                    break;
                }
                // ターン延長ポイント変換終了
                case AnimationState.ConvertExtraTurnClose:
                {
                    convertExtraTurnAnimation.CloseConvertAnimation(() =>
                    {
                        animationState = AnimationState.ConvertConditionPoint;
                        NextEffect();
                    });
                    break;
                }
                // コンディションポイント変換演出
                case AnimationState.ConvertConditionPoint:
                {
                    // 変換されたポイント数
                    long convertPointValue = MainArguments.Reward.conditionTierPointValue;
                    // 変換されたポイントがないならStateをEndに
                    if (convertPointValue <= 0)
                    {
                        animationState = AnimationState.End;
                        NextEffect();
                    }
                    // ポイントがあるなら変換演出を再生
                    else
                    {
                        // ブーストポイント表示UIに必要なデータ
                        TrainingEventResultConvertBaseAnimation.Param param = new TrainingEventResultConvertBaseAnimation.Param()
                        {
                            TrainingPointValue = MainArguments.PointStatus.value,
                            TrainingScenarioId = MainArguments.Pending.mTrainingScenarioId,
                            TrainingPointLevel = MainArguments.PointStatus.level,
                            TrainingHandReloadCount = MainArguments.PointStatus.handResetCount,
                            AdvManager = Adv,
                            MainArguments = MainArguments,
                            isAuto = IsFastMode,
                        };
                        
                        convertConditionPointAnimation.PlayConvertConditionPointAnimation(param, convertPointValue, () =>
                        {
                            animationState = AnimationState.ConvertConditionPointClose;
                            // オートモードなら自動で次に
                            if (IsFastMode) NextEffect();
                        });
                    }

                    break;
                }
                case AnimationState.ConvertConditionPointClose:
                {
                    convertConditionPointAnimation.CloseConvertAnimation(() =>
                    {
                        animationState = AnimationState.End;
                        NextEffect();
                    });
                    break;
                }
                case AnimationState.End:
                {
                    if(isEnded)break;
                    isEnded = true;
                    
                    // コンディションの変化をAdvのログに入れる
                    if(MainArguments.Reward.condition != 0)
                    {
                        Adv.AddMessageLog(MainArguments.ActionName, TrainingUtility.GetConditionChangeMessage(MainArguments.Reward.condition), 0);
                    }
                    
                    // スキップモード変更不可
                    Adv.Footer.EnableAutoButton(true);
                
                    // トレーニング終わり
                    if(MainArguments.IsEndTraining)
                    {
                        // トレーニング結果へ
                        OpenPage(TrainingMainPageType.TrainingResult, MainArguments);
                    }
                    else
                    {
                        OpenPage(TrainingMainPageType.Top, MainArguments);
                    }
                    break;
                }
            }
        }

        /// <summary>風景の速度</summary>
        public void SetTrainingSceneSpeed(long bonusDisplay)
        {
            trainingScene.SetSpeed(bonusDisplay);
            if (bonusDisplay >= TrainingUtility.Config.DisplayAuraFXThreshold)
            {
                trainingScene.DisplayAuraFX(MainArguments);
            }
        }

        /// <summary> インスピレーション獲得演出 </summary>
        private void PlayInspirationEffect(Queue<TrainingUtility.InspirationTypeData> inspireDataQueue, AnimationState nextState, AnimationState closeState)
        {
            // 表示なし
            if (inspireDataQueue.Count <= 0)
            {
                animationState = nextState;
                NextEffect();
                return;
            }

            // 演出スキップ
            if (IsGetInspirationEffectSkip)
            {
                // 獲得数
                int getCount = 0;

                // 演出スキップ時はキューに入ってるデータ全部の個数を合算して表示
                while (inspireDataQueue.Count > 0)
                {
                    TrainingUtility.InspirationTypeData inspirationData = inspireDataQueue.Dequeue();
                    // Advのログに入れる
                    foreach (TrainingInspire inspiration in inspirationData.InspireList)
                    {
                        TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(inspiration.id);
                        Adv.AddMessageLog(string.Empty, string.Format(StringValueAssetLoader.Instance[LogSkillGetKey], mCard.name), 0);
                    }

                    getCount += inspirationData.InspireList.Length;
                }

                // メッセージを表示
                ReservationMessage(string.Format(StringValueAssetLoader.Instance["training.message.get_inspiration"], getCount));
                // ブースト表示へ
                animationState = nextState;
                NextEffect();
            }
            // スキップしない時は１つずつキューのデータを処理
            else
            {
                TrainingUtility.InspirationTypeData data = inspireDataQueue.Dequeue();

                // Advのログに入れる
                foreach (TrainingInspire inspiration in data.InspireList)
                {
                    TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(inspiration.id);
                    Adv.AddMessageLog(string.Empty, string.Format(StringValueAssetLoader.Instance[LogSkillGetKey], mCard.name), 0);
                }

                // アニメーションの再生
                resultAnimation.PlayGetInspirationAnimation(data.InspireList, MainArguments.Pending.inspireList, data.Type,
                    (isSkiped) =>
                    {
                        // 閉じるアニメーションへ
                        animationState = closeState;
                        // スキップ中は自動で次に進む
                        if (IsFastMode || isSkiped) NextEffect();
                    });
            }
        }

        private void OnEndStatusUpScenario()
        {
            // コールバック削除
            Adv.OnEnded -= OnEndStatusUpScenario;
            // 終了ステートへ
            animationState = AnimationState.ExtraSkillGet;
            
            NextEffect();
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            // 終了している場合は処理しない
            if(isEnded)return;

            if (animationState == AnimationState.GetExtraTurnRight && extraTurnRightView.IsPlayingEffect)
            {
                extraTurnRightView.SkipEffectForget(new ExtraTurnRightView.PlayArguments(Header, Adv, MainArguments, IsFastMode));
            }
            else
            {
                NextEffect();
            }
        }
    }
}
