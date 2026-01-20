using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Random = UnityEngine.Random;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameFieldGroupUI : MonoBehaviour
    {
        [SerializeField] private Transform uiRoot;
        [SerializeField] private Transform modelRoot;
        [SerializeField] private RuntimeAnimatorController modelAnimatorController;
        [SerializeField] private ClubRoyalInGamePartyLeaderUI[] partyLeaderUIs;
        [SerializeField] private Transform[] characterModelRoot;
        [SerializeField] private Transform[] dashEffectRoot;
        [SerializeField] private Transform[] spawnEffectRoot;
        [SerializeField] private Transform[] damageEffectRoot;
        [SerializeField] private Transform[] deadEffectRoot;
        [SerializeField] private Transform[] allyWordEffectRoot;
        [SerializeField] private Transform[] enemyWordEffectRoot;
        [SerializeField] private Transform[] ballModelRoot;
        [SerializeField] private Transform[] ballPosition;
        [SerializeField] private AnimationCurve blinkCharacterCurve;
        [SerializeField] private ClubRoyalInGameMapWinStreakUI winStreakUI;
        [SerializeField] private Transform resultUIRoot;
        
        public bool DoUpdate { get; private set; } = false;
        public int PositionId { get; private set; } = -1;
        public List<int> PartyIds { get; private set; } = new List<int>();

        private GameObject currentViewing3DModel = null;
        private Animator currentViewingBallAnimator;
        private Animator modelAnimator;
        private long currentViewing3DModelMCharaId = -1;
        private GuildBattleCommonConst.GuildBattleTeamSide viewSide = GuildBattleCommonConst.GuildBattleTeamSide.All;
        private List<GuildBattlePartyModel> currentParties = new List<GuildBattlePartyModel>();

        private AnimationType currentCharacterAnimationType = AnimationType.Idle;
        private BattleConst.ClubRoyalBallAnimationType currentBallAnimationType = BattleConst.ClubRoyalBallAnimationType.Idle;
        private ParticleSystem currentPlayingDribbleParticle = null;
        private ParticleSystem currentPlayingOneTimeParticle = null;
        private ParticleSystem currentPlayingWordParticle = null;
        private Vector3 targetUIPosition = Vector3.zero;
        private Vector3 targetModelPosition = Vector3.zero;
        private Vector3 uiPositionMoveValue = Vector3.zero;
        private Vector3 modelPositionMoveValue = Vector3.zero;
        private bool willDisappearAtHalf = false;
        private float movePhraseDelay = -1.0f;

        private const float MoveDuration = 5.1f;
        private const float MoveFinishDistance = 0.02f;
        private const int MaxVisiblePartyCount = 3;
        private const int ModelScale = 200;
        private readonly Vector3 ModelScaleVector = new Vector3(ModelScale, ModelScale, ModelScale);
        private readonly Vector3 LeaderUIPositionFirst = Vector3.zero;
        private readonly Vector3 LeaderUIPositionSecond = new Vector3(-45, 0, 0);
        private readonly Vector3 LeaderUIPositionThird = new Vector3(-75, -0, 0);
        private const float NormalMovePhraseDelayMax = 4.0f; 
        private const float HalfMovePhraseDelayMax = 2.0f;

        private const int ModelRotationAngle = 45;
        private int NearSatelliteSpotPositionX = 300;
        private int FarSatelliteSpotPositionX = 700;

        private Vector3 GroupUIPositionDiff = new Vector3(0, -1920.0f / 2, 0);

        public enum AnimationType
        {
            None,
            Idle,
            Dribble,
            Shoot,
            SideStep,
            Dead,
        }

        private const string IdleTrigger = "Idle";
        private const string DribbleTrigger = "Dribble";
        private const string ShootTrigger = "Shoot";
        private const string SideStepTrigger = "SideStep";
        private const string DeadTrigger = "Dead";
        
#if !PJFB_REL
        // Debug
        public void SetModelAnimator(Animator animator)
        {
            modelAnimator = animator;
        }

        public void SetBallAnimator(Animator animator)
        {
            currentViewingBallAnimator = animator;
        }

        public void SetCurrentViewingModel(GameObject go)
        {
            currentViewing3DModel = go;
        }
#endif

        private void Update()
        {
            if (currentPlayingOneTimeParticle != null && currentPlayingOneTimeParticle.isStopped)
            {
                ReturnEffectParticle();
            }
            
            if (targetModelPosition == Vector3.zero)
            {
                return;
            }

            var dt = Time.deltaTime;
            // 毎回計算したほうが動き気持ちよさげ.
            var modelMoveValue = modelPositionMoveValue * dt;
            var uiMoveValue = uiPositionMoveValue * dt;

            modelRoot.transform.position += modelMoveValue;
            uiRoot.transform.localPosition += uiMoveValue;

            if (currentCharacterAnimationType == AnimationType.Dribble && movePhraseDelay >= 0.0f)
            {
                movePhraseDelay -= dt;
                if (movePhraseDelay < 0.0f)
                {
                    PlayWordEffect(BattleConst.ClubRoyalWordEffectType.Dash).Forget();
                    PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnStartMove);
                }
            }

            if (!ShouldPlayMoveAnimation())
            {
                ResetMoveTargetPosition();
            }

            if (ShouldDisappearInHalf())
            {
                ResetMoveTargetPosition();
                DisappearInHalf().Forget();
            }
        }

        public void Initialize(bool isAlly, Transform characterUIRoot)
        {
            viewSide = isAlly ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right;
            foreach (var leaderUI in partyLeaderUIs)
            {
                leaderUI.gameObject.SetActive(false);
            }
            
            uiRoot.SetParent(characterUIRoot, false);
        }
        
        public void SetPositionId(int positionId)
        {
            PositionId = positionId;
        }
        
        public void AddParty(GuildBattlePartyModel party)
        {
            if (!currentParties.Contains(party))
            {
                currentParties.Add(party);
            }
        }
        
        public void RemoveParty(GuildBattlePartyModel party)
        {
            currentParties.Remove(party);
        }

        public void ClearParty()
        {
            currentParties.Clear();
        }
        
        public bool HaveParty()
        {
            return currentParties.Count > 0;
        }

        public void SetWillUpdateUI(bool flag)
        {
            DoUpdate = flag;
        }
        
        public void SetActiveIncludeUnits(bool isActive)
        {
            gameObject.SetActive(isActive);
            uiRoot.gameObject.SetActive(isActive);
            if (!isActive)
            {
                ResetMoveTargetPosition();
                ReturnCharacterModel(true);
                ReturnEffectParticle();
                ReturnBallModel();
                winStreakUI.Close(true);
            }
        }

        public void SetInitialPosition(Vector3 uiPosition, Vector3 modelPosition)
        {
            // 何言ってるか分からない自分でもわけわかってないが, 同じParent, 同じRectの設定, 同じLocalPositionの指定をしたときに
            // 何故かこいつが親の描画サイズ(縦1920)に引っ張られて位置がずれる. 自分でも何を言ってるのか分からない.
            uiRoot.transform.localPosition = uiPosition + GroupUIPositionDiff;
            modelRoot.transform.position = modelPosition;
        }
        
        public void SetMoveTargetPosition(Vector3 uiPosition, Vector3 modelPosition, bool disappearAtHalf)
        {
            targetUIPosition = uiPosition + GroupUIPositionDiff;
            targetModelPosition = modelPosition;
            willDisappearAtHalf = disappearAtHalf;
            
            modelPositionMoveValue = ((targetModelPosition - modelRoot.transform.position) / MoveDuration);
            uiPositionMoveValue = ((targetUIPosition - uiRoot.transform.localPosition) / MoveDuration);
            PlayCharacterAnimation(AnimationType.Dribble);
            PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LeftRotate);
            PlayEffect(BattleConst.ClubRoyalBattleEffectType.Dribble).Forget();
            movePhraseDelay = Random.Range(0.0f, willDisappearAtHalf ? HalfMovePhraseDelayMax : NormalMovePhraseDelayMax);
            SetModelRotation();
        }

        private void SetModelRotation()
        {
            var firstParty = currentParties.FirstOrDefault();
            if (firstParty == null)
            {
                return;
            }

            var positionX = firstParty.XPosition;
            var lastPositionX = firstParty.LastXPosition;
            var positionY = firstParty.LaneNumber;
            
            characterModelRoot[(int)viewSide].transform.localRotation = Quaternion.identity;
            ballModelRoot[(int)viewSide].transform.localRotation = Quaternion.identity;
            // 中央レーンは回転なし
            if (positionY == 1)
            {
                return;
            }
            
            // 上下レーンで左右の斜め地帯にいない場合は回転なし
            if ((firstParty.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left && lastPositionX >= NearSatelliteSpotPositionX && lastPositionX < FarSatelliteSpotPositionX) ||
                (firstParty.Side == GuildBattleCommonConst.GuildBattleTeamSide.Right && lastPositionX > NearSatelliteSpotPositionX && lastPositionX <= FarSatelliteSpotPositionX))
            {
                return;
            }

            // 所属チームによってPositionXの値が反転するため.
            var plusMinus = 1;//side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? 1 : -1;
            if (viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Right)
            {
                plusMinus *= -1;
            }

            // 上部レーンは反転
            if (positionY == 2)
            {
                plusMinus *= -1;
            }

            // 表示上, データ上の左右表示で反転
            if ((firstParty.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left && positionX > FarSatelliteSpotPositionX) ||
                (firstParty.Side == GuildBattleCommonConst.GuildBattleTeamSide.Right && positionX < NearSatelliteSpotPositionX))
            {
                plusMinus *= -1;
            }

            characterModelRoot[(int)viewSide].transform.localRotation = Quaternion.Euler(new Vector3(0, ModelRotationAngle * plusMinus, 0));
            ballModelRoot[(int)viewSide].transform.localRotation = Quaternion.Euler(new Vector3(0, ModelRotationAngle * plusMinus, 0));;
        }
        
        private void ResetMoveTargetPosition()
        {
            targetUIPosition = Vector3.zero;
            targetModelPosition = Vector3.zero;
            willDisappearAtHalf = false;
            //PlayCharacterAnimation(AnimationType.Idle);
            //PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Idle);
        }

        private bool ShouldPlayMoveAnimation()
        {
            if(targetModelPosition == Vector3.zero)
            {
                return false;
            }

            return Vector3.Distance(modelRoot.transform.position, targetModelPosition) > MoveFinishDistance;
        }
        
        private bool ShouldDisappearInHalf()
        {
            if(!willDisappearAtHalf)
            {
                return false;
            }

            return Vector3.Distance(modelRoot.transform.position, targetModelPosition) < 0.4f;
        }

        public void FixUI(bool showLastMilitaryStrength)
        {
            if (currentParties.Count == 0)
            {
                SetActiveIncludeUnits(false);
                return;
            }
            
            SetActiveIncludeUnits(true);
            PartyIds.Clear();
            foreach (var party in currentParties)
            {
                PartyIds.Add(party.PartyId);
            }

            var firstParty = currentParties.FirstOrDefault();
            var leaderCharacterData = firstParty.GetLeaderCharacterData();
            var isAlly = firstParty.Side == PjfbGuildBattleDataMediator.Instance.PlayerSide;
            //myUnitCountTextRoot.SetActive(false);
            foreach (var partyUI in partyLeaderUIs)
            {
                partyUI.gameObject.SetActive(false);
            }
            
            // 3Dモデルのロード
            if (currentViewing3DModel == null || !currentViewing3DModel.activeSelf ||
                currentViewing3DModelMCharaId != leaderCharacterData.mCharaId)
            {
                LoadCharacterModel(leaderCharacterData.mCharaId, isAlly).Forget();
            }

            if (!firstParty.IsDefendingAtAnySpot() && currentViewingBallAnimator == null)
            {
                LoadBallModel().Forget();
            }
            
            var haveHiddenParty = currentParties.Count > MaxVisiblePartyCount;
            //var hiddenPartyRoot = isAlly ? allyHiddenPartyCountRoot : enemyHiddenPartyCountRoot;
            //hiddenPartyRoot.SetActive(haveHiddenParty);
            if (haveHiddenParty)
            {
                var hiddenPartyCount = currentParties.Count - MaxVisiblePartyCount;
                //var countText = isAlly ? allyHiddenPartyCountText : enemyHiddenPartyCountText;
                //countText.text = hiddenPartyCount.ToString();

                if (isAlly)
                {
                    var hiddenMyPartyCount = 0;
                    for (var i = MaxVisiblePartyCount; i < currentParties.Count; i++)
                    {
                        if (currentParties[i].PlayerIndex == PjfbGuildBattleDataMediator.Instance.PlayerIndex)
                        {
                            hiddenMyPartyCount++;
                        }
                    }
                }
            }

            for (var i = 0; i < MaxVisiblePartyCount && i < currentParties.Count; i++)
            {
                var leaderUI = partyLeaderUIs[i];
                leaderUI.gameObject.SetActive(true);
                leaderUI.InitializeOnMap(currentParties[i], showLastMilitaryStrength);
                leaderUI.transform.localPosition = isAlly ? GetPartyUIPosition(i) : -GetPartyUIPosition(i);
            }

            // 先頭の軍隊が解散されたケースもありうるので, 連撃表示のチェックをする
            if (winStreakUI.IsActive &&
                firstParty.WinStreakCount < PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.CommendWinStreakCountForLog)
            {
                winStreakUI.Close();
            }
            
            SetWillUpdateUI(false);
        }

        private Vector3 GetPartyUIPosition(int index)
        {
            switch (index)
            {
                case 0:
                    return LeaderUIPositionFirst;
                case 1:
                    return LeaderUIPositionSecond;
                case 2:
                    return LeaderUIPositionThird;
            }
            
            return Vector3.zero;
        }

        public async UniTask LoadCharacterModel(long mCharaId, bool isAlly)
        {
            if (currentViewing3DModel != null)
            {
                ReturnCharacterModel(false);
            }
            
            StopEffect();
            currentViewing3DModelMCharaId = mCharaId;

            var model = await ClubRoyalInGameUIMediator.Instance.GetOrCreateCharacterModel(mCharaId, isAlly, destroyCancellationToken);
            
            // 順番前後して読み込み完了した場合のfailsafe. 呼び出し順は保証されているはず.
            if (currentViewing3DModelMCharaId != mCharaId)
            {
                ReturnCharacterModel(false);
                return;
            }

            currentViewing3DModelMCharaId = mCharaId;
            characterModelRoot[(int)viewSide].gameObject.SetActive(true);
            characterModelRoot[(int)viewSide].transform.localScale = ModelScaleVector;
            model.SetActive(true);
            model.transform.SetParent(characterModelRoot[(int)viewSide], false);
            
            ResetModelPositionAndRotation();
            currentViewing3DModel = model;
            currentViewing3DModelMCharaId = mCharaId;
            modelAnimator = model.GetComponentInChildren<Animator>();
            modelAnimator.runtimeAnimatorController = modelAnimatorController;
            PlayCharacterAnimation(currentCharacterAnimationType);
            PlayEffect(BattleConst.ClubRoyalBattleEffectType.Spawn).Forget();
            PlayWordEffect(BattleConst.ClubRoyalWordEffectType.Spawn).Forget();
        }

        private void ReturnCharacterModel(bool playEffect)
        {
            if (currentViewing3DModel != null)
            {
                currentCharacterAnimationType = AnimationType.None;
                ClubRoyalInGameUIMediator.Instance.ReturnCharacterModel(currentViewing3DModel);
                currentViewing3DModel = null;
                if (playEffect)
                {
                    PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadCharacter).Forget();
                }
            }

            currentViewing3DModelMCharaId = -1;
        }
        
        private void ReturnEffectParticle()
        {
            if (currentPlayingOneTimeParticle != null)
            {
                ClubRoyalInGameUIMediator.Instance.ReturnEffectParticle(currentPlayingOneTimeParticle);
                currentPlayingOneTimeParticle = null;
            }
        }

        private async UniTask LoadBallModel()
        {
            PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LeftRotate);
            currentViewingBallAnimator = await ClubRoyalInGameUIMediator.Instance.GetOrCreateBallObject(destroyCancellationToken);
            var root = ballModelRoot[(int)viewSide];
            currentViewingBallAnimator.gameObject.transform.SetParent(root, false);
            currentViewingBallAnimator.gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            PlayBallAnimation(currentBallAnimationType);
        }
        
        public void ReturnBallModel()
        {
            if (currentViewingBallAnimator != null)
            {
                ClubRoyalInGameUIMediator.Instance.ReturnBallModel(currentViewingBallAnimator.gameObject);
                currentViewingBallAnimator = null;
            }
        }
        
        public bool NeedSeparateUnits()
        {
            var defendOnSpot = false;
            var moveToFront = false;
            foreach (var party in currentParties)
            {
                var spot = PjfbGuildBattleDataMediator.Instance.BattleField.GetMapSpot(party.LastJoinedSpotId);
                // 拠点から動かないパターン.
                if (spot.Id == party.TargetSpotId)
                {
                    defendOnSpot = true;
                }

                // 前進するパターン.
                if (party.XPosition != party.LastXPosition)
                {
                    moveToFront = true;
                }
            }

            // 表示している軍隊が移動しないパターンと移動するパターンの両方を含んでいる場合は移動演出でそれぞれを分離.
            return defendOnSpot && moveToFront;
        }
        
        public List<GuildBattlePartyModel> GetWillMoveParties()
        {
            var ret = new List<GuildBattlePartyModel>();
            foreach (var party in currentParties)
            {
                // 前進するパターン.
                if (party.XPosition != party.LastXPosition)
                {
                    ret.Add(party);
                }
            }

            return ret;
        }
        
        public bool NeedMovePosition()
        {
            foreach (var party in currentParties)
            {
                // 前進するパターン.
                if (party.XPosition != party.LastXPosition)
                {
                    return true;
                }
            }

            return false;
        }
        
        public GuildBattlePartyModel GetFirstParty()
        {
            return currentParties.FirstOrDefault();
        }
        
        public bool ShouldPlayBattleAnimation()
        {
            var firstParty = currentParties.FirstOrDefault();
            return firstParty.IsFighting;
        }

        public void PlayCharacterAnimation(AnimationType animationType)
        {
            currentCharacterAnimationType = animationType;
            if (currentCharacterAnimationType == AnimationType.None || modelAnimator == null)
            {
                return;
            }

            // Dribble -> Dribbleは実行させない.
            if (!(animationType == AnimationType.Dribble &&
                modelAnimator.GetCurrentAnimatorStateInfo(0).IsName(DribbleTrigger)))
            {
                modelAnimator.SetTrigger(animationType.ToString());                
            }
            
            if (animationType != AnimationType.Dribble && currentPlayingDribbleParticle != null)
            {
                StopEffect();
            }

            ResetModelPositionAndRotation();
        }

        public async UniTask PlayEffect(BattleConst.ClubRoyalBattleEffectType effectType)
        {
            Transform root = null;
            ParticleSystem particleSystem = null;
            // 一部エフェクトはBLMのみなのでこちらでは再生なし.
            switch (effectType)
            {
                case BattleConst.ClubRoyalBattleEffectType.Dribble:
                    // ドリブルだけ連続で再生が飛んでくる可能性があるのでそれはブロック.
                    if (currentPlayingDribbleParticle != null)
                    {
                        return;
                    }
                    root = dashEffectRoot[(int)viewSide];
                    break;
                case BattleConst.ClubRoyalBattleEffectType.Spawn:
                    root = spawnEffectRoot[(int)viewSide];
                    break;
                case BattleConst.ClubRoyalBattleEffectType.DamageTaken:
                    root = damageEffectRoot[(int)viewSide];
                    break;
                case BattleConst.ClubRoyalBattleEffectType.DeadCharacter:
                    root = deadEffectRoot[(int)viewSide];
                    break;
                default:
                    return;
            }
            
            particleSystem = await ClubRoyalInGameUIMediator.Instance.GetOrCreateEffectParticle(effectType, destroyCancellationToken);
            particleSystem.gameObject.transform.SetParent(root, false);
            particleSystem.Play();

            if (effectType == BattleConst.ClubRoyalBattleEffectType.Dribble)
            {
                currentPlayingDribbleParticle = particleSystem;
            }
            else
            {
                currentPlayingOneTimeParticle = particleSystem;
            }
        }

        public void StopEffect()
        {
            if (currentPlayingDribbleParticle != null)
            {
                currentPlayingDribbleParticle.Stop();
                ClubRoyalInGameUIMediator.Instance.ReturnEffectParticle(currentPlayingDribbleParticle);
                currentPlayingDribbleParticle = null;
            }

            if (currentPlayingOneTimeParticle != null)
            {
                currentPlayingOneTimeParticle.Stop();
                ClubRoyalInGameUIMediator.Instance.ReturnEffectParticle(currentPlayingOneTimeParticle);
                currentPlayingOneTimeParticle = null;
            }
        }

        public async UniTask PlayWordEffect(BattleConst.ClubRoyalWordEffectType wordType)
        {
            var root = viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left
                ? allyWordEffectRoot[(int)wordType] : enemyWordEffectRoot[(int)wordType];
            ParticleSystem particleSystem = null;
            
            particleSystem = await ClubRoyalInGameUIMediator.Instance.GetOrCreateWordEffectParticle(wordType, viewSide, destroyCancellationToken);
            particleSystem.transform.SetParent(root, false);
            particleSystem.transform.localPosition = new Vector3(0, Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            if (currentPlayingWordParticle != null)
            {
                ClubRoyalInGameUIMediator.Instance.ReturnWordParticle(currentPlayingWordParticle);
            }

            currentPlayingWordParticle = particleSystem;
        }

        public void PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType animationType)
        {
            currentBallAnimationType = animationType;
            if (currentViewingBallAnimator == null)
            {
                return;
            }
            
            currentViewingBallAnimator.SetTrigger(animationType.ToString());
        }

        public void PlayWinStreakUI(int winStreakCount, bool immediately)
        {
            winStreakUI.PlayAnimation(winStreakCount, viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left, immediately);
        }

        public Vector3 GetBallRootPosition()
        {
            return ballPosition[(int)viewSide].position;
            //return ballModelRoot[(int)side].position;
        }

        public void ResetBallPosition()
        {
            ballModelRoot[(int)viewSide].localPosition = Vector3.zero;
        }
        
        public async UniTask SetBallPosition(Vector3 position)
        {
            if (currentViewingBallAnimator == null)
            {
                await LoadBallModel();
            }
            
            ballModelRoot[(int)viewSide].position = position;
        }
        
        public void SetBallRotation(bool isReverse)
        {
            ballModelRoot[(int)viewSide].localRotation = isReverse ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        public void ResetModelPositionAndRotation()
        {
            if (currentViewing3DModel == null)
            {
                return;
            }
            
            currentViewing3DModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            characterModelRoot[(int)viewSide].localScale = ModelScaleVector;
            
            characterModelRoot[(int)viewSide].transform.localRotation = Quaternion.identity;
            ballModelRoot[(int)viewSide].transform.localRotation = Quaternion.identity;
            
            if (currentCharacterAnimationType == AnimationType.Dribble)
            {
                SetModelRotation();
            }
        }

        public async UniTask PlayCharacterBlinkAnimation()
        {
            var modelRootTransform = characterModelRoot[(int)viewSide];
            var elapsed = 0.0f;
            var index = 0;
            while (elapsed < 1.0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
                elapsed += Time.deltaTime * 5;
                if (blinkCharacterCurve.length <= index)
                {
                    break;
                }
                
                var keyframe = blinkCharacterCurve[index];
                if (keyframe.time <= elapsed)
                {
                    index++;
                }

                modelRootTransform.localScale = keyframe.value >= 1.0f ? ModelScaleVector : Vector3.zero;
            }

            modelRootTransform.localScale = Vector3.zero;
            
            PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadCharacter).Forget();
            ReturnCharacterModel(false);
        }

        public async UniTask DisappearInHalf()
        {
            await PlayCharacterBlinkAnimation();
            gameObject.SetActive(false);
            uiRoot.gameObject.SetActive(false);
            winStreakUI.Close(true);
        }

        public void PlayHeadLeaderUIAnimation(ClubRoyalInGamePartyLeaderUI.AnimationType animationType)
        {
            var headLeaderUI = partyLeaderUIs[0];
            if (!headLeaderUI.gameObject.activeSelf)
            {
                return;
            }

            if (animationType is ClubRoyalInGamePartyLeaderUI.AnimationType.AttackL
                or ClubRoyalInGamePartyLeaderUI.AnimationType.AttackR)
            {
                if (viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left)
                {
                    animationType = ClubRoyalInGamePartyLeaderUI.AnimationType.AttackL;
                }
                else
                {
                    animationType = ClubRoyalInGamePartyLeaderUI.AnimationType.AttackR;
                }
            }
            
            headLeaderUI.PlayAnimation(animationType);
        }

        public void SetHeadLeaderBallCountText(int ballCount)
        {
            var headLeaderUI = partyLeaderUIs[0];
            if (!headLeaderUI.gameObject.activeSelf)
            {
                return;
            }

            headLeaderUI.SetBallCountText(ballCount);
        }

        public void PlayHeadLeaderBallCountUpAnimation(bool isRecovered, int finalCount)
        {
            var headLeaderUI = partyLeaderUIs[0];
            if (!headLeaderUI.gameObject.activeSelf)
            {
                return;
            }

            // スコア的に回復しないこともあるため.
            if (!isRecovered)
            {
                return;
            }

            headLeaderUI.PlayCountUpBallCountAnimation(finalCount);
        }

        public void PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType type)
        {
            var headLeaderUI = partyLeaderUIs[0];
            if (!headLeaderUI.gameObject.activeSelf)
            {
                return;
            }

            headLeaderUI.PlayWordPhraseBalloon(type);
        }

        public Vector3 GetResultUIPosition()
        {
            return resultUIRoot.position;
        }

        public Vector3 GetHeadLeaderUIPosition()
        {
            return partyLeaderUIs[0].transform.position;
        }
    }
}