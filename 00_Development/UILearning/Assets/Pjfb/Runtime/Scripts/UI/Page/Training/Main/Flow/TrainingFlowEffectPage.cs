using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> Flow演出ページ </summary>
    public class TrainingFlowEffectPage : TrainingConcentrationEffectBasePage
    {
        // Flowゾーン突入演出
        [SerializeField]
        private TrainingFlowZoneEnterEffect enterEffect = null;

        // Flowカットインアニメーション
        [SerializeField]
        private TrainingEventFlowCutInAnimation flowCutIn = null;
        
        [SerializeField]
        private float skipModeSpeed = 2.0f;

        
        protected override async UniTask OnOpen(object args)
        {
            // イベント名非表示
            Header.ShowEventView(false);
            
            await PlayAnimation();
            await base.OnOpen(args);
        }

        /// <summary> アニメーション再生 </summary>
        private async UniTask PlayAnimation()
        {
               // タッチガード
            SetTouchGurad(true);
            // 再生速度
            float timeScale = Adv.IsSkipOrFastMode ? skipModeSpeed : 1.0f;
            // エフェクトId
            int effectId = MainArguments.GetConcentrationEffectId();
            
            if(MainArguments.Reward != null)
            {
                // 開始
                if(MainArguments.Reward.mTrainingConcentrationId != 0)
                {
                    // ログを追加
                    Adv.AddMessageLog(string.Empty, StringValueAssetLoader.Instance["training.flow_zone.begin"], 0);
                    
                    // Flowカットイン再生
                    await flowCutIn.PlayAnimationAsync(MainArguments.TrainingCharacter.MCharId, Adv.AppAutoMode);
                    
                    // コンセントレーションのエフェクトを出す
                    await MainPageManager.ConcentrationZoneEffectPlayer.PlayEffectAsync(TrainingConcentrationEffectType.Flow, effectId, timeScale);
                    
                    // スキップ
                    if(IsConcentrationEffectSkip)
                    {
                        SetMessage( StringValueAssetLoader.Instance["training.message.flow_zone.entry"] );
                    }
                    else
                    {
                        // 表示
                        await enterEffect.PlayEffectAsync(effectId, timeScale, true);
                    }

                    // Flowラベル表示
                    await Header.ConcentrationLabelEffectPlayer.PlayEffectAsync(TrainingConcentrationEffectType.Flow, MainArguments.GetConcentrationEffectId(), 1.0f);
                }
            }

            // 終了演出はリリース時点ではないらしい(最終ターンでのみ発生する前提なので)
            
            
            // 発生コンセントレーションをチェック済み
            MainArguments.OptionFlags |= TrainingMainArguments.Options.ConcentrationEffectEnd;

            // タッチガード
            SetTouchGurad(false);
            // Topへ戻る
            OpenPage(TrainingMainPageType.EventResult, MainArguments);
        }

        /// <summary> デバック用の演出リセット関数 </summary>
#if UNITY_EDITOR
        public override void ResetEffectId()
        {
            enterEffect.ShowEffect(false);
        }
#endif
    }
}