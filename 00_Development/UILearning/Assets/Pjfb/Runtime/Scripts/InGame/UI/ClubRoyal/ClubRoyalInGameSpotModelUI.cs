using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSpotModelUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] goalPrefabs;
        [SerializeField] private GameObject[] blueRockManPrefabs;
        [SerializeField] private GameObject[] baseBlueRockManPrefabs;
        [SerializeField] private Transform[] goalParents;
        [SerializeField] private Transform[] blueRockManParents;
        [SerializeField] private Transform[] damageEffectRoots;
        [SerializeField] private Transform[] deadEffectRoots;
        [SerializeField] private Transform[] wordEffectRoots;
        [SerializeField] private RuntimeAnimatorController blueRockManAnimatorController;
        [SerializeField] private AnimationCurve blinkCharacterCurve;

        private const int DefaultBlueRockManScale = 200;
        private readonly Vector3 DefaultBlueRockManScaleVector = new Vector3(DefaultBlueRockManScale, DefaultBlueRockManScale, DefaultBlueRockManScale);
        private const int DefaultGoalScale = 180;
        private readonly Vector3 DefaultGoalScaleVector = new Vector3(DefaultGoalScale, DefaultGoalScale, DefaultGoalScale);
        private const int BaseBlueRockManScale = 220;
        private readonly Vector3 BaseBlueRockManScaleVector = new Vector3(BaseBlueRockManScale, BaseBlueRockManScale, BaseBlueRockManScale);
        private const int BaseGoalScale = 200;
        private readonly Vector3 BaseGoalScaleVector = new Vector3(BaseGoalScale, BaseGoalScale, BaseGoalScale);
        
        private GuildBattleCommonConst.GuildBattleTeamSide side;
        private bool isBase = false;
        private Vector3 brmScale;
        private Animator blueRockManAnimator;
        private ParticleSystem currentPlayingParticle;
        private ParticleSystem currentPlayingLowHPParticle;
        private ParticleSystem currentPlayingWordParticle = null;
        private const string IdleTrigger = "Idle";
        private const string JumpLeftTrigger = "JumpLeft";
        private const string JumpRightTrigger = "JumpRight";
        private const string DeadTrigger = "Dead";

        public enum BlueRockManAnimationType
        {
            Idle,
            JumpLeft,
            JumpRight,
            Dead,
        }

#if !PJFB_REL
        // Debug
        public void SetModelAnimator(Animator animator)
        {
            blueRockManAnimator = animator;
        }
#endif
        
        public void Initialize(bool isAlly, bool _isBase)
        {
            side = isAlly ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right;
            isBase = _isBase;
            var goalPrefab = goalPrefabs[(int)side];
            var blueRockManPrefab = isBase ? baseBlueRockManPrefabs[(int)side] : blueRockManPrefabs[(int)side];
            var goalParent = goalParents[(int)side];
            var blueRockManParent = blueRockManParents[(int)side];
            
            var goalScale = DefaultGoalScaleVector;
            var blueRockManScale = DefaultBlueRockManScaleVector;
            if (isBase)
            {
                goalScale = BaseGoalScaleVector;
                blueRockManScale = BaseBlueRockManScaleVector;
            }

            var goal = Instantiate(goalPrefab, goalParent);
            var blueRockMan = Instantiate(blueRockManPrefab, blueRockManParent);
            goal.transform.localScale = goalScale;
            blueRockMan.transform.localScale = blueRockManScale;
            brmScale = blueRockManScale;

            blueRockManAnimator = blueRockMan.GetComponentInChildren<Animator>();
            blueRockManAnimator.runtimeAnimatorController = blueRockManAnimatorController;
        }

        public void UpdateView(GuildBattleMapSpotModel spotModel)
        {
            if (spotModel.RemainHP <= 0)
            {
                goalParents[(int)side].gameObject.SetActive(false);
                blueRockManParents[(int)side].gameObject.SetActive(false);
                if (currentPlayingParticle != null)
                {
                    ClubRoyalInGameUIMediator.Instance.ReturnEffectParticle(currentPlayingParticle);
                }
                
                if (currentPlayingLowHPParticle != null)
                {
                    ClubRoyalInGameUIMediator.Instance.ReturnEffectParticle(currentPlayingLowHPParticle);
                }
            }

            if (spotModel.RemainHP > 0 && spotModel.GetHpRatio() <= BattleConst.HpColorThresholdUnder50 && currentPlayingLowHPParticle == null)
            {
                PlayEffect(BattleConst.ClubRoyalBattleEffectType.DamageTakenBLM).Forget();
            }
        }
        
        public void PlayBlueRockManAnimation(BlueRockManAnimationType animationType)
        {
            switch (animationType)
            {
                case BlueRockManAnimationType.Idle:
                    blueRockManAnimator.SetTrigger(IdleTrigger);
                    break;
                case BlueRockManAnimationType.JumpLeft:
                    blueRockManAnimator.SetTrigger(JumpLeftTrigger);
                    break;
                case BlueRockManAnimationType.JumpRight:
                    blueRockManAnimator.SetTrigger(JumpRightTrigger);
                    break;
                case BlueRockManAnimationType.Dead:
                    blueRockManAnimator.SetTrigger(DeadTrigger);
                    break;
            }
        }

        public void ResetBlueRockManPositionAndRotation()
        {
            blueRockManAnimator.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            blueRockManAnimator.transform.localScale = isBase ? BaseBlueRockManScaleVector : DefaultBlueRockManScaleVector;
            blueRockManAnimator.transform.parent.localScale = Vector3.one;
        }
        
        public async UniTask PlayBlueRockManBlinkAnimation()
        {
            var elapsed = 0.0f;
            var index = 0;
            while (elapsed < 1.0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
                elapsed += Time.deltaTime;
                if (blinkCharacterCurve.length <= index)
                {
                    break;
                }
                
                var keyframe = blinkCharacterCurve[index];
                if (keyframe.time <= elapsed)
                {
                    index++;
                }

                blueRockManAnimator.transform.parent.localScale = keyframe.value >= 1.0f ? Vector3.one : Vector3.zero;
            }

            PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadBLM).Forget();
        }
        
        public async UniTask PlayEffect(BattleConst.ClubRoyalBattleEffectType effectType)
        {
            // 一部エフェクトはBLMのみなのでこちらでは再生なし.
            switch (effectType)
            {
                case BattleConst.ClubRoyalBattleEffectType.DamageTakenBLM:
                    // HP低下エフェクトは連続で再生がくるので.
                    if (currentPlayingLowHPParticle != null)
                    {
                        return;
                    }
                    currentPlayingLowHPParticle = await ClubRoyalInGameUIMediator.Instance.GetOrCreateEffectParticle(effectType, destroyCancellationToken);
                    currentPlayingLowHPParticle.gameObject.transform.SetParent(damageEffectRoots[(int)side], false);
                    currentPlayingLowHPParticle.Play();
                    break;
                case BattleConst.ClubRoyalBattleEffectType.DeadBLM:
                    currentPlayingParticle = await ClubRoyalInGameUIMediator.Instance.GetOrCreateEffectParticle(effectType, destroyCancellationToken);
                    currentPlayingParticle.gameObject.transform.SetParent(deadEffectRoots[(int)side], false);
                    currentPlayingParticle.Play();
                    break;
                default:
                    return;
            }
        }

        public async UniTask PlayWordEffect(BattleConst.ClubRoyalWordEffectType wordType)
        {
            var root = wordEffectRoots[(int)side];
            ParticleSystem particleSystem = null;

            particleSystem = await ClubRoyalInGameUIMediator.Instance.GetOrCreateWordEffectParticle(wordType, side, destroyCancellationToken);
            particleSystem.transform.SetParent(root, false);
            particleSystem.transform.localPosition = new Vector3(0, Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            if (currentPlayingWordParticle != null)
            {
                ClubRoyalInGameUIMediator.Instance.ReturnWordParticle(currentPlayingWordParticle);
            }

            currentPlayingWordParticle = particleSystem;
        }
        
        public Vector3 GetGoalPosition()
        {
            return new Vector3(goalParents[(int)side].transform.position.x, goalParents[(int)side].transform.position.y, 0);
        }
    }
}