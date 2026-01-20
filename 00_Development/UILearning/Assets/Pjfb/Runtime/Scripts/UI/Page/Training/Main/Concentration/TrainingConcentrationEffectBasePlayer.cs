using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> コンセントレーション演出再生用Player </summary>
    public abstract class TrainingConcentrationEffectBasePlayer : MonoBehaviour
    {
        protected TrainingConcentrationEffectBase effect = null;
        public TrainingConcentrationEffectBase Effect => effect;
        
        [SerializeField]
        protected RectTransform root = null;

        protected CancellationTokenSource tokenSource = null;
        
        /// <summary> 演出アセット読み込みキーを取得 </summary>
        protected abstract string GetEffectPathKey(TrainingConcentrationEffectType effectType);

        /// <summary> 再生中か？ </summary>
        public bool IsPlaying
        {
            get
            {
                if (effect == null)
                {
                    return false;
                }

                return effect.IsPlaying;
            }
        }
        
        /// <summary> ラベルの表示 </summary>
        public void ShowEffect(bool isShow)
        {
            if (effect == null)
            {
                return;
            }
            effect.ShowEffect(isShow);
        }

        /// <summary> エフェクトをセット </summary>
        public void SetEffect(int effectId)
        {
            if (effect == null)
            {
                return;
            }
            
            effect.SetEffect(effectId);
        }

        /// <summary> ラベルの再生 </summary>
        public void PlayEffect(TrainingConcentrationEffectType effectType, int effectId, float timeScale)
        {
            PlayEffectAsync(effectType, effectId, timeScale).Forget();
        }
        
        public async UniTask PlayEffectAsync(TrainingConcentrationEffectType effectType, int effectId, float timeScale)
        {
            await LoadEffectAsync(effectType);
            if (effect != null)
            {
                await effect.PlayEffectAsync(effectId, timeScale, token: destroyCancellationToken);
            }
        }

        /// <summary> 演出読み込み </summary>
        protected async UniTask LoadEffectAsync(TrainingConcentrationEffectType effectType)
        {
            // 同一の読み込み済みエフェクトがある
            if (effect != null && effect.EffectType == effectType)
            {
                return;
            }
            
            // 読み込みが重複しないようにキャンセル
            Cancel();
            tokenSource = new CancellationTokenSource();

            // オブジェクトの破棄
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }
            
            string pathKey = ResourcePathManager.GetPath(GetEffectPathKey(effectType));
            await PageResourceLoadUtility.LoadAssetAsync<TrainingConcentrationEffectBase>(pathKey, labelEffect =>
                {
                    effect = Instantiate(labelEffect, root);
                },
                tokenSource.Token);
        }

        /// <summary> 演出を終了させる </summary>
        public void PlayCloseEffect()
        {
            if (effect == null)
            {
                return;
            }
            
            effect.PlayCloseEffect();
        }

        /// <summary> キャンセル </summary>
        protected void Cancel()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }
        }

        private void OnDestroy()
        {
            Cancel();
        }
    }
}