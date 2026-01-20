using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.Voice;
using TMPro;

namespace Pjfb
{
    public class TrainingEventResultBonusAnimation : MonoBehaviour
    {
        
        public enum PracticeVoiceType
        {
            None,
            Success,
            Failed,
        }

        /// <summary> ボーナス倍率のしきい値タイプ </summary>
        public enum RollStateType
        {
            None = 0,
            
            Normal = 1,
            High = 2,
            SuperHigh = 3,
            HyperHigh = 4,
            UltraHigh = 5,
            LimitBreak = 6,
            LimitBreakHigh = 7,
        }
        
        /// <summary>アニメーション名</summary>
        private static readonly string OpenAnimation = "Open";
        /// <summary>アニメーション名</summary>
        private static readonly string CloseAnimation = "Close";
        // ルーレット回転前のボーナス表示アニメーション
        private static readonly string OpenLotteryAnimation = "OpenLottery";
        // ルーレット回転アニメーション
        private static readonly string StartLotteryAnimation = "StartLottery";
        
        // ブーストボーナスの表示アニメーション
        private static readonly string ActivateBoostBonusAnimationKey = "ActivateBoostBonus";
        // ブーストボーナスの倍率表示アニメーション
        private static readonly string ReleaseBoostBonusAnimationKey = "ReleaseBoostBonus";
        // ブーストボーナスの倍率表示後のアニメーション
        private static readonly string LeaveOnlyShadeAnimationKey = "LeaveOnlyShade";

        // ブーストボーナスのTypeIdに応じたアニメーションキーを返す
        private string ActivateBoostBonusTypeAnimation => ActivateBoostBonusAnimationKey + boostBonusType;
        private string ReleaseBoostBonusTypeAnimation => ReleaseBoostBonusAnimationKey + boostBonusType; 
        
        // デフォルトのボーナステキストキー
        private const string DefaultBonusTextKey = "training.bonus.text.default";
        
        /// <summary> ボーナス画像の抽象クラス </summary>
        [Serializable]
        private abstract class BonusSpriteData
        {
            [SerializeField]
            private Sprite baseImage = null;
            /// <summary> ベース画像 </summary>
            public Sprite BaseImage => baseImage;
            
            [SerializeField]
            private Sprite percentSprite = null;
            /// <summary> パーセント画像 </summary>
            public Sprite PercentSprite => percentSprite;
            
            [SerializeField]
            private Sprite[] numberSprites = null;
            /// <summary> 数字画像 </summary>
            public Sprite[] NumberSprites => numberSprites;
        }
        
        /// <summary> 通常ステートのしきい値データ </summary>
        [Serializable]
        private class ThresholdData : BonusSpriteData
        {
            [SerializeField]
            private int threshold = 0;
            /// <summary> しきい値 </summary>
            public int Threshold => threshold;
        }
        
        /// <summary> 特殊ステートのボーナス関連データ </summary>
        [Serializable]
        private class UniqueStateBonusData : BonusSpriteData
        {
            [SerializeField] 
            private RollStateType[] targetStateTypes = Array.Empty<RollStateType>();
            /// <summary> 特殊条件を適用するステートタイプ </summary>
            public RollStateType[] TargetStateTypes => targetStateTypes;
            
            [SerializeReference, SerializeReferenceDropdown]
            private UniqueRollCondition condition;
            /// <summary> 識別する条件 </summary>
            public UniqueRollCondition Condition => condition;
            
            [SerializeField, StringValue]
            private string viewText;
            /// <summary> 表示テキスト </summary>
            public string ViewText => viewText;
        }

        /// <summary> マスクデータ </summary>
        [System.Serializable]
        private class MaskData
        {
            [SerializeField]
            public Sprite[] numberMaskSprites = null;
        }
        
        /// <summary> 通常ステートのしきい値データ </summary>
        [System.Serializable]
        private class RollStateThresholdData
        {
            [SerializeField] 
            public RollStateType stateType = RollStateType.Normal;
            
            [SerializeField]
            public int threshold = 0;
            [SerializeField]
            public int bonusValue = 0;
            [SerializeField]
            public string stateName = string.Empty;
            [SerializeField]
            public float skipTime = 0;
        }
        
        /// <summary> 特殊ステートの条件基底クラス </summary>
        [Serializable]
        public abstract class UniqueRollCondition
        {
            /// <summary> 条件にマッチするか </summary>
            public abstract bool IsMatch(TrainingMainArguments mainArguments, RollStateType stateType, RollStateType[] targetStateTypes);

            /// <summary> 条件判定を行うステートタイプかチェック </summary>
            protected bool IsTargetStateType(RollStateType stateType, RollStateType[] targetStateTypes)
            {
                if (!Array.Exists(targetStateTypes, type => type == stateType))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary> FLOW条件 </summary>
        [Serializable]
        public sealed class FlowUniqueRollCondition : UniqueRollCondition
        {
            /// <summary> 条件にマッチするか </summary>
            public override bool IsMatch(TrainingMainArguments mainArguments, RollStateType stateType, RollStateType[] targetStateTypes)
            {
                // 条件判定を行うステートタイプかチェック
                if (!IsTargetStateType(stateType, targetStateTypes))
                {
                    return false;
                }
                
                // FLOW状態かチェック
                return mainArguments.IsFlow();
            }
        }
        
        /// <summary> 特殊ステートデータ本体 </summary>
        [Serializable]
        private class UniqueRollStateThresholdData
        {
            [SerializeField] 
            private RollStateType[] targetStateTypes = Array.Empty<RollStateType>();
            /// <summary> 特殊条件を適用するステートタイプ </summary>
            public RollStateType[] TargetStateTypes => targetStateTypes;
            
            [SerializeReference, SerializeReferenceDropdown]
            private UniqueRollCondition condition;
            /// <summary> 識別する条件 </summary>
            public UniqueRollCondition Condition => condition;
            
            [SerializeField]
            private string stateName = string.Empty;
            /// <summary> アニメーションステート名 </summary>
            public string StateName => stateName;
            
            [SerializeField]
            private float skipTime = 0;
            /// <summary> スキップ時間 </summary>
            public float SkipTime => skipTime;
        }

        /// <summary> ブーストボーナスの表示に必要なデータ </summary>
        [Serializable]
        private class BoostBonusLabelData
        {
            // 表示ラベルId
            [SerializeField] 
            private int labelId = 0;
            public int LabelId => labelId;

            // ブーストボーナス倍率画像のそれぞれの桁
            [SerializeField] 
            private BoostBonusRateImageData[] rateImageData;
            public BoostBonusRateImageData[] RateImageData => rateImageData;
            
            // ブーストボーナス倍率画像
            [SerializeField] 
            private Sprite[] numberSprites;
            public Sprite[] NumberSprites => numberSprites;
        }

        /// <summary> ブーストボーナスの倍率画像データ </summary>
        [Serializable]
        private class BoostBonusRateImageData
        {
            // ブーストボーナスの倍率画像
            [SerializeField]
            private Image boostBonusRateImage;
            public Image BoostBonusRateImage => boostBonusRateImage;

            // ブーストボーナスの残像演出用の倍率画像
            [SerializeField]
            private Image boostBonusAfterRateImage;
            public Image BoostBonusAfterRateImage => boostBonusAfterRateImage;
        }

        /// <summary> ブーストボーナスデータ本体 </summary>
        [Serializable]
        private class BoostBonusData
        {
            [SerializeField] private BoostBonusLabelData[] labelDatas;
            public BoostBonusLabelData[] LabelDatas => labelDatas;
        }
        
        /// <summary> グレードアップデータの抽象クラス </summary>
        [Serializable]
        private abstract class GradeUpDataBase
        {
            [SerializeField] 
            private GradeUpType gradeUpType;
            /// <summary> グレードタイプ </summary>
            public GradeUpType GradeUpType => gradeUpType;
            
            [SerializeField]
            private string animationStateName = string.Empty;
            /// <summary> 再生するアニメーションステートの名前 </summary>
            public string AnimationStateName => animationStateName;
        }

        /// <summary> グレードアップ時の切り替えデータ </summary>
        [Serializable]
        private class GradeUpThresholdData : GradeUpDataBase
        {
            // 対象となるグレードタイプの境界％
            [SerializeField] 
            private int thresholdPercent;
            public int ThresholdPercent => thresholdPercent;
        }

        /// <summary> 特殊ステートのグレードアップ切り替えデータ </summary>
        [Serializable]
        private class UniqueGradeUpData : GradeUpDataBase
        {
            [SerializeField] 
            private RollStateType[] targetStateTypes = Array.Empty<RollStateType>();
            /// <summary> 特殊条件を適用するステートタイプ </summary>
            public RollStateType[] TargetStateTypes => targetStateTypes;
            
            [SerializeReference, SerializeReferenceDropdown]
            private UniqueRollCondition condition;
            /// <summary> 識別する条件 </summary>
            public UniqueRollCondition Condition => condition;
        }
        
        // GradeUpが起こる時の演出タイプ
        public enum GradeUpType
        {
            // 何も発生しない
            None,
            // エクストラボーナス
            ExtraBonus,
            // リミットブレイク
            LimitBreak,
        }
        
#if UNITY_EDITOR
        [SerializeField]
#endif
        // 最大ボーナス値
        private long maxBonusValue = 0;
        
        [SerializeField]
        private Animator animator = null;
        // GradeUp(エクストラボーナス、リミットブレイク)発生時のラベルアニメーション
        [SerializeField] 
        private Animator gradeUpLabelAnimation = null;
        [SerializeField]
        private TMP_Text bonusText = null;
        [SerializeField]
        private Image baseImage = null;
        [SerializeField]
        private Image percentImage = null;
        [SerializeField]
        private Image[] valueImages = null;
        [SerializeField]
        private Image[] maskValueImages = null;
        [SerializeField]
        private Image[] effectValueImages = null;
        // 通常ステートのしきい値データ
        [SerializeField]
        private RollStateThresholdData[] rollStateThresholdDatas = null;
        // 特殊ステートのしきい値データ
        [SerializeField] 
        private UniqueRollStateThresholdData[] uniqueRollStateThresholdDatas = null;
        
        [SerializeField]
        private ThresholdData[] thresholdDatas = null;
        [SerializeField]
        private UniqueStateBonusData[] uniqueBonusData = null;
        [SerializeField]
        private MaskData maskData = null;
        // 通常ステートのグレードアップデータ
        [SerializeField] 
        private GradeUpThresholdData[] gradeUpThresholdDatas;
        // 特殊ステートのグレードアップデータ
        [SerializeField]
        private UniqueGradeUpData[] uniqueGradeUpDatas;
        [SerializeField] 
        private BoostBonusData boostBonusData;
        [SerializeField] 
        private TrainingEventResultTotalBonusAnimation totalBonusAnimation;
        // トータルボーナス用のスキップボタン
        [SerializeField] 
        private UIButton skipButton;
        
        // スキル獲得あり
        private bool isGetSkill = false;
        
        private bool isEnded = false;
        private  bool isSkiped = false;
        // base%(グレード上昇発生前の倍率)に到達したか
        private bool isBaseConditionEffectRate = false;
        private long baseConditionEffectRate = 0;
        private bool isBoostBonus = false;
        private bool isLockSkip = false;
        // グレード上昇データ
        private GradeUpDataBase gradeUpData = null;
        private Action onCompleted = null;
        // グレードアップアニメーションが再生済みか
        private bool isPlayedGradeUpAnim = false;
        
        private CharaMasterObject mCharacter = null;

        // ステート
        private int stateIndex = 0;
        private TrainingEventResultPage attachedTrainingResultPage;
        // 採用した特殊ステートデータ
        private RollStateThresholdData currentUniqueStateData = null;

        // ブーストボーナスの倍率
        private long boostBonusRate = 0;
        //　ブーストボーナスの表示タイプId
        private long boostBonusType = 0;
        // トータルボーナス値
        private long totalBonusValue = 0;
        // キャンセルトークン
        private CancellationTokenSource tokenSource = null;
        
        // グレードが上昇するか
        private  bool IsGradeUp => currentGradeUpType != GradeUpType.None;

        /// <summary> グレード上昇タイプを取得 </summary>
        private GradeUpType currentGradeUpType
        {
            get
            {
                // データがないならグレード上昇なし
                if (gradeUpData == null)
                {
                    return GradeUpType.None;
                }

                return gradeUpData.GradeUpType;
            }
        }

        private TrainingStatusUpVoiceThresholdData GetVoiceThresholdData(long value)
        {
            for(int i=0;i<TrainingUtility.Config.StatusUpVoiceThresholdDatas.Length;i++)
            {
                if(TrainingUtility.Config.StatusUpVoiceThresholdDatas[i].Value <= value)return TrainingUtility.Config.StatusUpVoiceThresholdDatas[i];
            }
            
            return TrainingUtility.Config.StatusUpVoiceThresholdDatas[TrainingUtility.Config.StatusUpVoiceThresholdDatas.Length-1];
        }
        
        /// <summary> 指定したボーナス値のステートタイプを返す </summary>
        private RollStateType GetStateTypeByBonusValue(long value)
        {
            // しきい値データから配列のインデックスを取得
            int index = GetRollStateThresholdData(value);
            // そのインデックスのステートタイプを返す
            return rollStateThresholdDatas[index].stateType;
        }
        
        /// <summary> ボーナスデータを取得 </summary>
        private BonusSpriteData GetBonusSpriteData(long value, RollStateType stateType = RollStateType.None)
        {
            // 引数でタイプの指定がない場合、採用した特殊ステートのタイプを参照する
            if (stateType == RollStateType.None && currentUniqueStateData != null)
            {
                stateType = currentUniqueStateData.stateType;
            }
            
            // 通常ステート
            if (stateType == RollStateType.None)
            {
                for(int i=0;i<thresholdDatas.Length;i++)
                {
                    if(thresholdDatas[i].Threshold <= value)return thresholdDatas[i];
                }
            }
            // 特殊ステート
            else
            {
                foreach (var uniqueData in uniqueBonusData)
                {
                    // 条件未設定はスキップ
                    if (uniqueData.Condition == null || uniqueData.TargetStateTypes.Length == 0)
                    {
                        continue;
                    }
                
                    // 条件判定
                    if (!uniqueData.Condition.IsMatch(attachedTrainingResultPage.MainArguments, stateType, uniqueData.TargetStateTypes))
                    {
                        continue;
                    }

                    // 条件にマッチしたら対応する画像データを返す
                    return uniqueData;
                }
            }
            
            return thresholdDatas[thresholdDatas.Length-1];
        }
        
        private int GetRollStateThresholdData(long value)
        {
            for(int i=rollStateThresholdDatas.Length-1;i>=0;i--)
            {
                if(rollStateThresholdDatas[i].threshold <= value)return i;
            }
            
            return 0;
        }

        /// <summary> 該当するグレードアップデータを返す </summary>
        private GradeUpDataBase GetGradeUpThresholdData(long rate = 0, RollStateType stateType = RollStateType.None)
        {
            // 引数でタイプが渡された場合のみ、特殊ステートかどうかを判定する
            if (stateType != RollStateType.None)
            {
                // 特殊ステートか判定
                foreach (var uniqueData in uniqueGradeUpDatas)
                {
                    // 条件未設定はスキップ
                    if (uniqueData.Condition == null || uniqueData.TargetStateTypes.Length == 0)
                    {
                        continue;
                    }
                
                    // 条件判定
                    if (!uniqueData.Condition.IsMatch(attachedTrainingResultPage.MainArguments, stateType, uniqueData.TargetStateTypes))
                    {
                        continue;
                    }
                    
                    // 条件にマッチしたら対応する画像データを返す
                    return uniqueData;
                }
            }
            
            // 通常ステート
            for (int i = 0; i < gradeUpThresholdDatas.Length; i++)
            {
                if (rate <= gradeUpThresholdDatas[i].ThresholdPercent)
                {
                    return gradeUpThresholdDatas[i];
                }
            }

            // 見つからない場合は最後のデータを返す
            return gradeUpThresholdDatas[gradeUpThresholdDatas.Length - 1];
        }
        
        private void SetBonusValue(long value, bool isPlayVoice)
        {
            // 段階を取得
            var data = GetBonusSpriteData(value);
            
            // 数字を書き換え
            long tempValue = value;
            // 桁を隠すかどうかのフラグ
            bool isValueHide = false;
            
            for(int i=0;i<valueImages.Length;i++)
            {
                // 桁が非表示ならオブジェクトを隠す
                if (isValueHide)
                {
                    valueImages[i].gameObject.SetActive(false);
                    effectValueImages[i].gameObject.SetActive(false);
                    continue;
                }
                
                valueImages[i].sprite = data.NumberSprites[ tempValue%10 ];
                maskValueImages[i].sprite = maskData.numberMaskSprites[ tempValue%10 ];
                effectValueImages[i].sprite = data.NumberSprites[ tempValue%10 ];
                // オブジェクトをアクティブに
                valueImages[i].gameObject.SetActive(true);
                effectValueImages[i].gameObject.SetActive(true);
                tempValue /= 10;
                if (tempValue == 0) isValueHide = true;
            }
            // %
            percentImage.sprite = data.PercentSprite;
            // 背景を書き換え
            baseImage.sprite = data.BaseImage;
            
            // ボイス
            // スキル獲得がある場合はスキル側を優先で流すのでこちらはOff
            if(isPlayVoice && isGetSkill == false)
            {
                PlayStatusUpVoice(value);
            }
        }

        /// <summary> キャラクターのボイスの再生 </summary>
        private void PlayStatusUpVoice(long value)
        {
            TrainingStatusUpVoiceThresholdData voiceData = GetVoiceThresholdData(value);
            VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(mCharacter, voiceData.VoiceType).Forget();
        }

        private void Init(long mCharId, long bonusValue, bool isGetSkill, bool isGradeUp, long baseBonusValue, bool isBoostBonus, long boostBonusRate, long boostBonusLabelType, Action onCompleted)
        {
            // 親からPageを取得
            if (attachedTrainingResultPage == null)
            {
                attachedTrainingResultPage = GetComponentInParent<TrainingEventResultPage>();
            }
            
            this.isGetSkill = isGetSkill;
            
            isEnded = false;
            isSkiped = false;
            isLockSkip = false;
            isPlayedGradeUpAnim = false;
            this.onCompleted = onCompleted;
            // マスタ
            mCharacter = MasterManager.Instance.charaMaster.FindData(mCharId);
            // ボーナス値
            maxBonusValue = bonusValue;
            // ステート
            stateIndex = 0;
            
            // ボーナステキストをリセット（InspirationBoostではオブジェクトが存在しないためnullスキップ）
            if (bonusText != null)
            {
                // テキストキー（初期値はデフォルトテキストで設定）
                string textKey = DefaultBonusTextKey;
                
                // 最終的に到達するタイプを取得
                RollStateType finalStateType = GetStateTypeByBonusValue(maxBonusValue);
                // 最終的に到達するデータを取得
                var data = GetBonusSpriteData(maxBonusValue, finalStateType);
                // 特殊ステートデータの場合、テキストキーを更新
                if (data is UniqueStateBonusData uniqueData)
                {
                    textKey = uniqueData.ViewText;
                }
                
                // テキスト設定
                bonusText.text = StringValueAssetLoader.Instance[textKey];
            }

            if (isGradeUp)
            {
                gradeUpData = GetGradeUpThresholdData(maxBonusValue);
            }
            else
            {
                gradeUpData = null;
            }
            
            baseConditionEffectRate = baseBonusValue;
            isBaseConditionEffectRate = false;
            tokenSource = new CancellationTokenSource();
            
            this.isBoostBonus = isBoostBonus;
            this.boostBonusRate = boostBonusRate;
            boostBonusType = boostBonusLabelType;    
        }
        
        public void PlayAnimation(long mCharId, long bonusValue, bool isSkip, bool isGetSkill, bool isGradeUp, long baseBonusValue, bool isBoostBonus, long boostBonusRate, long boostBonusLabelType, Action onCompleted)
        {
            PlayAnimationAsync(mCharId, bonusValue, isSkip, isGetSkill, isGradeUp, baseBonusValue, isBoostBonus, boostBonusRate, boostBonusLabelType, onCompleted).Forget();
        }

        /// <summary>アニメーションの再生</summary>
        public async UniTask PlayAnimationAsync(long mCharId, long bonusValue, bool isSkip, bool isGetSkill,bool isGradeUp, long baseBonusValue, bool isBoostBonus, long boostBonusRate, long boostBonusLabelType, Action onCompleted)
        {
            // パラメータの初期化
            Init(mCharId, bonusValue, isGetSkill, isGradeUp, baseBonusValue, isBoostBonus, boostBonusRate, boostBonusLabelType, onCompleted);
            
            // ブーストボーナス倍率の設定
            SetBoostBonusValue(boostBonusRate);
            
            // 0で初期化
            SetBonusValue(0, false);

            // ボタンのアクティブを切る
            skipButton.gameObject.SetActive(false);
            
            // アクティブOn
            gameObject.SetActive(true);
            // グレードアップ演出オブジェクトは最初はアクティブを切っておく
            if (gradeUpLabelAnimation != null)
            {
                gradeUpLabelAnimation.gameObject.SetActive(false);
            }
          
            // 開始アニメーションが終わるまで待つ
            await AnimatorUtility.WaitStateAsync(animator, OpenAnimation);
            
            if(isSkip)
            {
                OnSkipButton();
            }
            else
            {
                if(isSkiped)return;
                // ルーレット出現アニメーションが終わるまで待機
                await AnimatorUtility.WaitStateAsync(animator, OpenLotteryAnimation, tokenSource.Token);
                
                // ブーストボーナス演出があるなら流す
                if (isBoostBonus)
                {
                    // ブーストボーナスのラベル表示アニメーションを再生し待機
                    await AnimatorUtility.WaitStateAsync(animator, ActivateBoostBonusTypeAnimation, tokenSource.Token);
                }
                // ボーナスのルーレット回転をスタート
                animator.SetTrigger(StartLotteryAnimation);
            }
        }

        /// <summary> インスピレーションアニメーションの再生 </summary>
        public void PlayInspirationAnimation(long mCharId, long bonusValue, bool isSkip, bool isGetSkill,bool isGradeUp, long baseBonusValue, bool isBoostBonus, long boostBonusRate, long boostBonusLabelType, Action onCompleted)
        {
            // パラメータの初期化
            Init(mCharId, bonusValue, isGetSkill, isGradeUp, baseBonusValue, isBoostBonus, boostBonusRate, boostBonusLabelType, onCompleted);
            // 0で初期化
            SetBonusValue(0, false);
            // アクティブOn
            gameObject.SetActive(true);
            
            if(isSkip)
            {
                OnSkipButton();
            }
            else
            {
                // Open
                animator.SetTrigger(OpenAnimation);
            }
        }

        /// <summary> ブーストボーナスの倍率画像を設定する </summary>
        private void SetBoostBonusValue(long boostBonusRate)
        {
            // ブーストボーナスの倍率表示がないならリターン
            if (isBoostBonus == false)
            {
                return;
            }

            // 桁を隠すかどうかのフラグ
            bool isValueHide = false;
            
            // 一致するブーストボーナスのラベルの倍率画像を設定する
            foreach (BoostBonusLabelData boostBonusData in boostBonusData.LabelDatas)
            {
                // ラベルIdが一致するなら
                if (boostBonusData.LabelId == boostBonusType)
                {
                    // それぞれの桁数ごとに数値を設定する
                    for (int i = 0; i < boostBonusData.RateImageData.Length; i++)
                    {
                        // 桁の表示を隠す
                        if (isValueHide)
                        {
                            boostBonusData.RateImageData[i].BoostBonusRateImage.gameObject.SetActive(false);
                            boostBonusData.RateImageData[i].BoostBonusAfterRateImage.gameObject.SetActive(false);
                            continue;
                        } 
                        
                        // 倍率画像を設定
                        boostBonusData.RateImageData[i].BoostBonusRateImage.sprite = boostBonusData.NumberSprites[boostBonusRate % 10];
                        boostBonusData.RateImageData[i].BoostBonusRateImage.gameObject.SetActive(true);
                        // 残像演出用の画像も設定する
                        boostBonusData.RateImageData[i].BoostBonusAfterRateImage.sprite = boostBonusData.NumberSprites[boostBonusRate % 10];
                        boostBonusData.RateImageData[i].BoostBonusAfterRateImage.gameObject.SetActive(true);
                        
                        // 次の桁
                        boostBonusRate /= 10;

                        // 上の桁が０になるならそれ以降の桁は表示しない
                        if (boostBonusRate == 0) isValueHide = true;
                    }
                    // ループから抜ける
                    break;
                }
            }
        }
        
        private async UniTask PlayBoostBonusReleaseAnimationAsync()
        {
            // スキップされていないならブーストボーナスのアニメーションを再生する
            if (isSkiped == false)
            {
                // ブーストボーナスの倍率確定アニメーションを再生し、待つ
                await AnimatorUtility.WaitStateAsync(animator, ReleaseBoostBonusTypeAnimation);
            }

            // 背景のみを残すアニメーションを再生
            await AnimatorUtility.WaitStateAsync(animator, LeaveOnlyShadeAnimationKey);
            
            isEnded = true;
            // トータルボーナスを表示する際はCloseにはトータルボーナスが終わらないと遷移しないのでここで完了時のコールバックを実行する
            onCompleted();
        }
        
        /// <summary> ボーナス値の回転が終わった際の処理 </summary>
        private void CompleteRollState()
        {
            if (isBoostBonus)
            {
                // ボーナス値を表示 (ボイスはトータルボーナス表示の際に流すのでここでは流さない)
                SetBonusValue(maxBonusValue, false);
                // ブーストボーナスの倍率確定アニメーションを再生
                PlayBoostBonusReleaseAnimationAsync().Forget();
            }
            else
            {
                // ボーナス値を表示
                SetBonusValue(maxBonusValue, true);
                // アニメーション終了
                animator.SetTrigger(CloseAnimation);
            }
        }

        /// <summary> トータルボーナスの演出再生 </summary>
        public void PlayTotalBonusAnimation(Action onComplete)
        {
            // スキップボタンのアクティブをオンに
            skipButton.gameObject.SetActive(true);
            
            // トータルボーナス演出開始時はCloseステートに行かないのでラベル演出のアクティブを切る
            if (gradeUpLabelAnimation != null)
            {
                gradeUpLabelAnimation.gameObject.SetActive(false);
            }
            
            totalBonusValue = maxBonusValue * boostBonusRate;
            totalBonusAnimation.PlayAnimationAsync(baseConditionEffectRate, maxBonusValue, boostBonusRate, totalBonusValue, currentGradeUpType, onComplete).Forget();
        }

        public void CloseTotalBonusAnimation(Action onComplete)
        {
            CloseTotalBonusAnimationAsync(onComplete).Forget();
        }
        
        /// <summary> トータルボーナス表示演出終了 </summary>
        private async UniTask CloseTotalBonusAnimationAsync(Action onComplete)
        {
            // スキップボタンのアクティブをオフに
            skipButton.gameObject.SetActive(false);
            // トータルボーナス表示演出オブジェクトのアクティブを切る
            totalBonusAnimation.gameObject.SetActive(false);
            // StateがCloseになるまで待つ
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);

            // スキル取得がある場合はスキル獲得側でボイスが流れるのでここでは流さない
            if (isGetSkill == false)
            {
                PlayStatusUpVoice(totalBonusValue);
            }
            onComplete();
        }

        /// <summary> グレード上昇時の演出 </summary>
        private async UniTask PlayGradeUpEffect()
        {
            // インスピレーションブースト側ではエクストラボーナス演出はしないのでリターン
            if (IsGradeUp == false || isBaseConditionEffectRate == false || gradeUpLabelAnimation == null)
            {
                return;
            }
            
            // すでに再生済みならリターン
            if (isPlayedGradeUpAnim)
            {
                return;
            }
            // 再生済みにする
            isPlayedGradeUpAnim = true;
            
            // アニメーションオブジェクトのアクティブを切り替え
            gradeUpLabelAnimation.gameObject.SetActive(IsGradeUp);
            
            // オート段階により最速でスキップされた場合、アニメーションのルートがアクティブになっていないのでルートがアクティブになるまで待機
            await UniTask.WaitUntil(() => gradeUpLabelAnimation.gameObject.activeInHierarchy);
            
            // GradeUpTypeによってラベルアニメーションを変更
            gradeUpLabelAnimation.SetTrigger(gradeUpData.AnimationStateName);
        }
        
        /// <summary>
        /// Animator
        /// </summary>
        private void OnUpdateRollTextures()
        {
            if(rollStateThresholdDatas.Length <= stateIndex-1)
            {
                SetBonusValue(maxBonusValue, false);
                return;
            } 
            
            // データのIndex取得
            int index = GetRollStateThresholdData(maxBonusValue);
            bool isLast = index == stateIndex - 1;
            
            // 現在のステート情報
            RollStateThresholdData data = rollStateThresholdDatas[stateIndex-1];
            // ボーナス値を表示
            var bonusDisplay = Math.Min(maxBonusValue, data.bonusValue);
            
            // 一番最後の場合は最終値を表示
            if(isLast)
            {
                bonusDisplay = maxBonusValue;
            }
            
            // ボーナスアップ重複時の表示対応
            bool isRollData = IsRollData(baseConditionEffectRate);
            // グレードアップが発生した場合はbaseの％からグレード上昇発生％までとばす
            if (IsGradeUp && !isBaseConditionEffectRate && data.bonusValue >= baseConditionEffectRate)
            {
                stateIndex = GetRollStateThresholdData(maxBonusValue);
                bonusDisplay = baseConditionEffectRate;
                isBaseConditionEffectRate = true;
            }


            if (IsGradeUp && !isBaseConditionEffectRate && isLast)
            {
                if (!isRollData)
                {
                    isBaseConditionEffectRate = true;
                    stateIndex--;
                    bonusDisplay = baseConditionEffectRate;
                }
            }
            SetBonusValue( bonusDisplay, false);
            // 演出速度
            attachedTrainingResultPage.SetTrainingSceneSpeed(bonusDisplay);            
        }
        
        
        /// <summary>
        /// Animator
        /// </summary>
        private void OnNextRollState()
        {
            if(rollStateThresholdDatas.Length <= stateIndex)
            {
                // ボーナス値確定時の処理
                CompleteRollState();
                return;
            }

            // 現在のステート情報
            RollStateThresholdData data = rollStateThresholdDatas[stateIndex++];
            // しきい値を超えている場合は次のアニメーションへ
            if(data.threshold <= maxBonusValue)
            {
                // ステートデータを取得
                data = GetStateThresholdData(data);
                // アニメーターのステートを移動
                animator.SetTrigger(data.stateName);
            }
            // 終わり
            else
            { 
                // ボーナス値確定時の処理
                CompleteRollState();
            }
        }

        /// <summary>
        /// Animator
        /// </summary>
        private void OnUpdateGradeUp()
        {
            int index = GetRollStateThresholdData(maxBonusValue);
            // 最終％表示なら
            if(stateIndex == index+1)
            {
                PlayGradeUpEffect().Forget();
                // 最終％に入ったらスキップ出来ないようにする
                isLockSkip = true;
            }

            bool isRollData = IsRollData(baseConditionEffectRate);
            if (IsGradeUp && stateIndex == index && isRollData)
            {
                isBaseConditionEffectRate = true;
            }
        }

        private bool IsRollData(long value)
        {
            foreach(var data in rollStateThresholdDatas)
            {
                if(data.bonusValue == value)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSkipButton()
        {
            OnSkipButtonAsync().Forget();
        }

        private async UniTask OnSkipButtonAsync()
        {
            if(isEnded || isSkiped || isLockSkip)return;
            isSkiped = true;
            
            if (tokenSource != null)
            {
                // 処理をキャンセルする
                tokenSource.Cancel();
                // リソース解放
                DisposeToken();
            }
            // 全てのトリガーをオフにする
            animator.ResetAllTriggers();
            
            // スキップされた時、すべての移動にかかわるアニメーションを最後まで再生するようにする
            // 今回、アニメータが２か所で使われており片方に存在しないステートをスキップするため、エディタ警告が出ないように
            await animator.SkipToEnd(OpenLotteryAnimation, 0, destroyCancellationToken, true);
            
            // ブーストボーナスの表示があるなら倍率確定演出の終わりまで進める
            if (isBoostBonus)
            {
                await animator.SkipToEnd(ReleaseBoostBonusTypeAnimation, 0, destroyCancellationToken);
            }
            
            // ロールステートを取得
            stateIndex = GetRollStateThresholdData(maxBonusValue);
            // ステートデータを取得
            RollStateThresholdData data = rollStateThresholdDatas[stateIndex++];
            data = GetStateThresholdData(data);
            // アニメーターのステートを移動
            animator.Play(data.stateName, 0, data.skipTime);
            SetBonusValue(maxBonusValue, false);
            // スキップ時は最大％までとばすのでフラグをオンに
            isBaseConditionEffectRate = true;
            // グレード上昇があるならラベル表示演出
            PlayGradeUpEffect().Forget();
        }
        
        public void OnEndAnimation()
        {
            // キャンセルトークンのリソース解放
            if (tokenSource != null)
            {
                DisposeToken();
            }
            
            if(isEnded)return;
            isEnded = true;
            gameObject.SetActive(false);
           
            // グレードアップ時のラベル演出のアクティブを切る
            if (gradeUpLabelAnimation != null)
            {
                gradeUpLabelAnimation.gameObject.SetActive(false);
            }
            
            if(onCompleted != null)
            {
                onCompleted();
            }
        }

        /// <summary> キャンセルトークンのリソース解放 </summary>
        private void DisposeToken()
        {
            // リソースを解放
            tokenSource.Dispose();
            // 明示的にnullを入れとく
            tokenSource = null;
        }

        /// <summary> ステートのしきい値データを取得する </summary>
        private RollStateThresholdData GetStateThresholdData(RollStateThresholdData data)
        {
            // 採用した特殊ステートデータをリセット
            currentUniqueStateData = null;
            
            // 特殊ステートを採用するかの判定
            foreach (var uniqueData in uniqueRollStateThresholdDatas)
            {
                // 条件未設定はスキップ
                if (uniqueData.Condition == null || uniqueData.TargetStateTypes.Length == 0)
                {
                    continue;
                }
                
                // 条件判定
                if (!uniqueData.Condition.IsMatch(attachedTrainingResultPage.MainArguments, data.stateType, uniqueData.TargetStateTypes))
                {
                    continue;
                }

                // 条件を満たした特殊ステートの、アニメーション関連データを採用
                RollStateThresholdData returnData = new RollStateThresholdData
                {
                    stateType = data.stateType,
                    threshold = data.threshold,
                    bonusValue = data.bonusValue,
                    stateName = uniqueData.StateName,
                    skipTime = uniqueData.SkipTime
                };

                // 採用した特殊ステートデータを保持しておく
                currentUniqueStateData = returnData;
                // ステートデータからグレードアップデータを再取得
                gradeUpData = GetGradeUpThresholdData(stateType:currentUniqueStateData.stateType);
                
                return returnData;
            }

            return data;
        }
    }
}