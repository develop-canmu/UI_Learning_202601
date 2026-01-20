using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Pjfb.InGame.ClubRoyal
{
    public class DebugClubRoyalInGameCharacterAnimation : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGameFieldGroupUI charaA;
        [SerializeField] private Animator charaAAnimator;
        [SerializeField] private GameObject charaAModel;
        [SerializeField] private ClubRoyalInGameFieldGroupUI charaB;
        [SerializeField] private Animator charaBAnimator;
        [SerializeField] private GameObject charaBModel;
        [SerializeField] private Animator ballAnimator;
        [SerializeField] private ClubRoyalInGameSpotModelUI brmModel;
        [SerializeField] private Animator brmAnimator;
        [SerializeField] private ParticleSystem hitParticle;
        [SerializeField] private ParticleSystem characterDeadParticle;
        [SerializeField] private ParticleSystem brmDeadParticle;

#if !PJFB_REL
        private void Awake()
        {
            charaA.SetModelAnimator(charaAAnimator);
            charaA.Initialize(true, null);
            charaA.SetCurrentViewingModel(charaAModel);
            charaA.SetBallAnimator(ballAnimator);

            if (charaB != null)
            {
                charaB.SetModelAnimator(charaBAnimator);
                charaB.Initialize(false, null);
                charaB.SetCurrentViewingModel(charaBModel);
            }

            if (brmModel != null)
            {
                brmModel.SetModelAnimator(brmAnimator);
            }
        }
#endif

        public void ResetPosition()
        {
            if (charaA != null)
            {
                charaA.ResetModelPositionAndRotation();
                charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Idle);
            }
            
            if (charaB != null)
            {
                charaB.ResetModelPositionAndRotation();
                charaB.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            }

            if (brmModel != null)
            {
                brmModel.ResetBlueRockManPositionAndRotation();
                brmModel.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.Idle);
            }
        }

        public async UniTask PlayMatchUp()
        {
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            charaB.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            if (Random.Range(0, 2) == 0)
            {
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            }
            else
            {
                charaA.SetBallPosition(charaB.GetBallRootPosition()).Forget();
                charaA.SetBallRotation(true);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
            }

            charaA.ResetBallPosition();
            charaA.SetBallRotation(false);
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.ShootHitOpponent);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            hitParticle.Play();
            charaB.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Dead);
            charaB.PlayCharacterBlinkAnimation().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: destroyCancellationToken);
            characterDeadParticle.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f), cancellationToken: destroyCancellationToken);
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            charaA.ResetModelPositionAndRotation();
        }
        
        public async UniTask PlayLongMatchUp()
        {
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            charaB.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            if (Random.Range(0, 2) == 0)
            {
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            }
            else
            {
                charaA.SetBallPosition(charaB.GetBallRootPosition()).Forget();
                charaA.SetBallRotation(true);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
            }

            charaA.ResetBallPosition();
            charaA.SetBallRotation(false);

            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongShootHitOpponent);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            hitParticle.Play();
            charaB.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Dead);
            charaB.PlayCharacterBlinkAnimation().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            characterDeadParticle.Play();
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            charaA.ResetModelPositionAndRotation();
        }

        public async UniTask PlayGoalMatchUp()
        {
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.ShootGoal);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            if (BattleGameLogic.GetNonStateRandomValue(0, 2) == 0)
            {
                brmModel.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.JumpLeft);
            }
            else
            {
                brmModel.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.JumpRight);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken: destroyCancellationToken);
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            charaA.ResetModelPositionAndRotation();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            charaA.PlayCharacterBlinkAnimation().Forget();
            characterDeadParticle.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            brmModel.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.Idle);
            brmModel.ResetBlueRockManPositionAndRotation();
        }
        
        public async UniTask PlayLastGoalMatchUp()
        {
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            charaA.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.ShootHitBLM);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: destroyCancellationToken);
            hitParticle.Play();
            brmModel.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.Dead);
            brmModel.PlayBlueRockManBlinkAnimation().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            brmDeadParticle.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            charaA.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            charaA.ResetModelPositionAndRotation();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            charaA.PlayCharacterBlinkAnimation().Forget();
            characterDeadParticle.Play();
        }

    }
}