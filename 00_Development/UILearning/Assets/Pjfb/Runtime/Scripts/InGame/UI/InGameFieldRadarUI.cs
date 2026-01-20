using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cinemachine;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pjfb.Master;
using SRF;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    public class InGameFieldRadarUI : MonoBehaviour
    {
        [SerializeField] private RawImage ballImage;
        [SerializeField] private RectTransform rootTransform;
        [SerializeField] private Image chanceImage;
        [SerializeField] private Image pinchImage;
        [SerializeField] private Image[] leftSidePlayerDots;
        [SerializeField] private Image[] leftSidePlayerDotBackImages;
        [SerializeField] private Image[] rightSidePlayerDots;
        [SerializeField] private Image[] rightSidePlayerDotBackImages;

        [SerializeField] private GameObject characterModelRoot;
        [SerializeField] private GameObject _3dLayerObject;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private GameObject[] leftSidePlayerModelRoots;
        [SerializeField] private GameObject[] rightSidePlayerModelRoots;
        [SerializeField] private GameObject ballModel;
        [SerializeField] private Transform[] characterOriginalTransforms;
        [SerializeField] private Transform[] characterTransformsLookToFront;

        private Animator ballAnimator;
        private Animator[,] playerAnimators;
        private Vector3[,] kickOffPositions;
        private Dictionary<long, Rigidbody> rigidbodies;
        private List<Dictionary<long, GameObject>> characterModelDictionaryList;
        private List<Dictionary<long, ModelMaterialSetter>> characterModelMaterialSetterList;

        private Tween moveTween;
        private Vector3 nextPosition;
        private Tweener alphaFadeTween = null;
        private List<Vector3> targetPositions = new List<Vector3>();
        private Vector3 defaultCameraPos;

        private readonly int CharacterIdleHash = Animator.StringToHash("Base Layer.Idle");
        private readonly int CharacterBallKeepIdleHash = Animator.StringToHash("Base Layer.BallKeepIdle");
        private readonly int CharacterDefenceIdleHash = Animator.StringToHash("Base Layer.DefenceIdle");
        private readonly int CharacterDribbleHash = Animator.StringToHash("Base Layer.Dribble");
        private readonly int CharacterShootHash = Animator.StringToHash("Base Layer.Shoot");
        
        private readonly int BallLeftRotateHash = Animator.StringToHash("Base Layer.LeftRotate");
        private readonly int BallRightRotateHash = Animator.StringToHash("Base Layer.RightRotate");

        /// <summary>
        /// 3DView params.
        /// </summary>
        private const float BallPositionWidth_3DView = 3.6f;
        private const float BallPositionHalfHeight_3DView = 2.4f;
        private const float PenaltyAreaWidth_3DView = 3.0f;
        private const float GoalPositionX_3DView = 4.0f;
        private const float GoalPositionY_3DView = 0.0f;
        private const float FieldHalfWidth_3DView = BattleConst.FieldSize / 2.0f;
        private int[] eachLinePlayerCountOn3D;

        /// <summary>
        /// 2DView params.
        /// </summary>
        private const int BallPositionWidth = 320;
        private const int BallPositionHalfHeight = 200;
        private const int PenaltyAreaWidth = 220;
        private const int GoalPositionX = 420;
        private const int GoalPositionY = 30;
        private const float FieldHalfWidth = BattleConst.FieldSize / 2.0f;
        private int[] eachLinePlayerCountOn2D = new int[] { 1, 2, 2 };

        private BattleConst.TeamSide lastOffenceSide = BattleConst.TeamSide.TeamSizeMax;
        private long lastBallOwnerId = -1;
        private List<List<long>> eachSideCharacterIds;

        private bool is2DDisplayMode = false;
        
        private void Awake()
        {
            playerAnimators = new Animator[(int)BattleConst.TeamSide.TeamSizeMax, TeamSize];
            kickOffPositions = new Vector3[(int)BattleConst.TeamSide.TeamSizeMax, TeamSize];
            
            defaultCameraPos = virtualCamera.transform.localPosition;

            ballAnimator = ballModel.GetComponentInChildren<Animator>();
            
            SetAllModelActive(false);
            ShowChance(false);
            ShowPinch(false);

            is2DDisplayMode = BattleDataMediator.Instance.Is2DFieldViewMode;
            if (is2DDisplayMode)
            {
                PrepareFor2DMode();
            }
            else
            {
                PrepareFor3DMode();
            }
        }

        private void PrepareFor3DMode()
        {
            rootTransform.gameObject.SetActive(false);
            _3dLayerObject.SetActive(true);
            characterModelRoot.SetActive(true);
            virtualCamera.gameObject.SetActive(true);
        }

        private void PrepareFor2DMode()
        {
            rootTransform.gameObject.SetActive(true);
            _3dLayerObject.SetActive(false);
            characterModelRoot.SetActive(false);
            virtualCamera.gameObject.SetActive(false);
        }

        public void ResetCharacterData(List<BattleCharacterModel> leftSideCharacters, List<BattleCharacterModel> rightSideCharacters)
        {
            if (eachSideCharacterIds == null)
            {
                eachSideCharacterIds = new List<List<long>>();
                eachSideCharacterIds.Add(new List<long>());
                eachSideCharacterIds.Add(new List<long>());
            }

            if (rigidbodies == null)
            {
                rigidbodies = new Dictionary<long, Rigidbody>();
                for (var i = 0; i < TeamSize; i++)
                {
                    rigidbodies.Add(leftSideCharacters[i].id, leftSidePlayerModelRoots[i].GetComponent<Rigidbody>());
                    rigidbodies.Add(rightSideCharacters[i].id, rightSidePlayerModelRoots[i].GetComponent<Rigidbody>());
                }
            }

            eachSideCharacterIds[(int)BattleDataMediator.Instance.PlayerSide] = leftSideCharacters.OrderBy(character => character.Position).Select(character => character.id).ToList();
            eachSideCharacterIds[(int)BattleDataMediator.Instance.EnemySide] = rightSideCharacters.OrderBy(character => character.Position).Select(character => character.id).ToList();
        }

        public async UniTask LoadCharacterModel(ResourcesLoader resourcesLoader, List<BattleCharacterModel> leftSideCharacters, List<BattleCharacterModel> rightSideCharacters)
        {
            // 生成済みなら特に何もいらない(リプレイなどの場合. InGameを抜けてないのでシーンから生きているはず.)
            if (characterModelDictionaryList != null || is2DDisplayMode)
            {
                return;
            }
            
            characterModelDictionaryList = new List<Dictionary<long, GameObject>> { new(), new() };
            characterModelMaterialSetterList = new List<Dictionary<long, ModelMaterialSetter>> { new(), new() };
            var source = new CancellationTokenSource();
            
            // TODO BattleCharacterModelにsameCharaIdがないのでこれは治す.
            var taskList = new List<UniTask>();

            for (var i = 0; i < leftSideCharacters.Count; i++)
            {
                var index = i;
                var address = PageResourceLoadUtility.GetCharacter3DLowModelGameObjectPath(Convert.ToInt32(leftSideCharacters[i].MCharaId.ToString().Substring(0, 5)), BattleConst.TeamSide.Left);
                taskList.Add(resourcesLoader.LoadAssetAsync<GameObject>(address,
                    go =>
                    {
                        var model = Instantiate(go, leftSidePlayerModelRoots[index].transform);
                        model.transform.localRotation = characterOriginalTransforms[(int)BattleConst.TeamSide.Left].localRotation;
                        characterModelMaterialSetterList[(int)BattleDataMediator.Instance.PlayerSide].Add(leftSideCharacters[index].id, model.GetComponentInChildren<ModelMaterialSetter>());
                        kickOffPositions[(int)BattleConst.TeamSide.Left, index] = leftSidePlayerModelRoots[index].transform.localPosition;
                        playerAnimators[(int)BattleConst.TeamSide.Left, index] = model.GetComponentInChildren<Animator>(true); 
                        playerAnimators[(int)BattleConst.TeamSide.Left, index].Play(CharacterDefenceIdleHash); 
                    }, source.Token));
                characterModelDictionaryList[(int)BattleDataMediator.Instance.PlayerSide].Add(leftSideCharacters[index].id, leftSidePlayerModelRoots[index]);
            }
            
            for (var i = 0; i < rightSideCharacters.Count; i++)
            {
                var index = i;
                var address = PageResourceLoadUtility.GetCharacter3DLowModelGameObjectPath(Convert.ToInt32(rightSideCharacters[i].MCharaId.ToString().Substring(0, 5)), BattleConst.TeamSide.Right);
                taskList.Add(resourcesLoader.LoadAssetAsync<GameObject>(address,
                    go =>
                    {
                        var model = Instantiate(go, rightSidePlayerModelRoots[index].transform);
                        model.transform.localRotation = characterOriginalTransforms[(int)BattleConst.TeamSide.Right].localRotation;
                        characterModelMaterialSetterList[(int) BattleDataMediator.Instance.EnemySide].Add(rightSideCharacters[index].id, model.GetComponentInChildren<ModelMaterialSetter>());
                        kickOffPositions[(int)BattleConst.TeamSide.Right, index] = rightSidePlayerModelRoots[index].transform.localPosition;
                        playerAnimators[(int)BattleConst.TeamSide.Right, index] = model.GetComponentInChildren<Animator>(true); 
                        playerAnimators[(int)BattleConst.TeamSide.Right, index].Play(CharacterDefenceIdleHash); 
                    }, source.Token));
                characterModelDictionaryList[(int) BattleDataMediator.Instance.EnemySide].Add(rightSideCharacters[index].id, rightSidePlayerModelRoots[index]);
            }
            
            await UniTask.WhenAll(taskList);
            
            SetAllModelActive(true);
        }

        private void SetAllModelActive(bool isActive)
        {
            foreach (var root in leftSidePlayerModelRoots)
            {
                root.SetActive(isActive);
            }
            foreach (var root in rightSidePlayerModelRoots)
            {
                root.SetActive(isActive);
            }
        }

        public void SetActive(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void SetVisible(bool isVisible)
        {
            _3dLayerObject.SetActive(isVisible);
            characterModelRoot.SetActive(isVisible);
        }

        public void ShowFieldText(BattleConst.TeamSide side, bool isInShootRange)
        {
            ShowChance(false);
            ShowPinch(false);
            if (side == BattleDataMediator.Instance.PlayerSide)
            {
                ShowChance(isInShootRange);
            }
            else
            {
                ShowPinch(isInShootRange);
            }
        }

        private void ShowChance(bool isShow)
        {
            chanceImage.gameObject.SetActive(isShow);
            if (!isShow)
            {
                return;
            }

            if (alphaFadeTween != null)
            {
                alphaFadeTween.Kill();
                alphaFadeTween = null;
            }

            chanceImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            alphaFadeTween = chanceImage.DOFade(1.0f, 1.0f).SetLoops(-1, LoopType.Yoyo);
        }

        private void ShowPinch(bool isShow)
        {
            pinchImage.gameObject.SetActive(isShow);
            if (!isShow)
            {
                return;
            }
            
            if (alphaFadeTween != null)
            {
                alphaFadeTween.Kill();
                alphaFadeTween = null;
            }

            pinchImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            alphaFadeTween = pinchImage.DOFade(1.0f, 1.0f).SetLoops(-1, LoopType.Yoyo);
        }
        
        public void ResetPosition()
        {
            if (moveTween != null)
            {
                moveTween.Kill();
                moveTween = null;
            }

            ballImage.transform.localPosition = new Vector3(0, GoalPositionY_3DView, 0);
            ballModel.transform.localPosition = new Vector3(0, GoalPositionY_3DView, 0);
        }

        public void MoveBall(int nextBallPosition, BattleDigestLog log)
        {
            // Left基準のプレイヤー視点で値が入っているので, Rightからのリプレイだったら反転
            if (BattleDataMediator.Instance.PlayerSide == BattleConst.TeamSide.Right)
            {
                nextBallPosition = BattleConst.FieldSize - nextBallPosition;
            }
            
            if (is2DDisplayMode)
            {
                MoveBall2D(nextBallPosition, log);
            }
            else
            {
                MoveBall3D(nextBallPosition, log);
            }
        }

        public void MoveBall3D(int nextBallPosition, BattleDigestLog log)
        {
            var digestType = log.Type;
            if (digestType == BattleConst.DigestType.None)
            {
                return;
            }

            if (!IsShowRadar(digestType))
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);
            if (log.OffenceSide != lastOffenceSide)
            {
                OnSideReset(log.OffenceSide);
                lastOffenceSide = log.OffenceSide;
            }
            
            var isShowBall = IsShowBall(digestType);
            var ballOwnerId = log.MainCharacterData?.CharaId ?? -1;
            var xPosition = (nextBallPosition - FieldHalfWidth_3DView) / FieldHalfWidth_3DView * BallPositionWidth_3DView;
            var yPosition = ballModel.transform.localPosition.y;
            if (IsFocusOnGoal(digestType))
            {
                xPosition = log.OffenceSide == BattleDataMediator.Instance.PlayerSide ? GoalPositionX_3DView : -GoalPositionX_3DView;
                yPosition = GoalPositionY_3DView;
            }else if (isShowBall)
            {
                var max = MoveXPositionCoefficient(digestType);
                var min = max * -1;
                var diff = BattleGameLogic.GetNonStateRandomValue(min, max);
                if (IsMoveOverYCenterLine(digestType))
                {
                    diff += yPosition > 0 ? -BallPositionHalfHeight_3DView : BallPositionHalfHeight_3DView;
                }
                yPosition = Mathf.Clamp(yPosition + diff, -BallPositionHalfHeight_3DView, BallPositionHalfHeight_3DView);

                if (IsUpdateRadar(digestType))
                {
                    var againstCharacterId = log.MarkCharacterData?.CharaId ?? -1;
                    SetPlayerModelPosition(xPosition, yPosition, log.MainCharacterData.Side, ballOwnerId, againstCharacterId, digestType);
                }
                
                if (digestType is BattleConst.DigestType.DribbleL)
                {
                    ballAnimator.Play(BallLeftRotateHash);
                }else if (digestType is BattleConst.DigestType.DribbleR)
                {
                    ballAnimator.Play(BallRightRotateHash);
                }
            }
            
            ResetModelRotation();
            PlayModelAnimation(log.MainCharacterData.Side, digestType, ballOwnerId);
            
            nextPosition = new Vector3(xPosition, yPosition, 0);
        }
        
        public void MoveBall2D(int nextBallPosition, BattleDigestLog log)
        {
            var digestType = log.Type;
            if (digestType == BattleConst.DigestType.None)
            {
                return;
            }

            if (!IsShowRadar(digestType))
            {
                return;
            }
            
            if (moveTween != null)
            {
                moveTween.Kill();
                moveTween = null;
                ballImage.transform.localPosition = nextPosition;
            }

            var isPlayerAce = log.MainCharacterData != null && log.MainCharacterData.IsAce && log.MainCharacterData.Side == BattleDataMediator.Instance.PlayerSide;
            var duration = isPlayerAce ? 2.0f / BattleDataMediator.Instance.PlaySpeed : 0.5f;
            if (log.Type == BattleConst.DigestType.KickOffL || log.Type == BattleConst.DigestType.KickOffR)
            {
                duration = 0;
            }
            
            var isShowBall = IsShowBall(digestType);
            ballImage.transform.localScale = isShowBall ? Vector3.one : Vector3.zero;

            var xPosition = (nextBallPosition - FieldHalfWidth) / FieldHalfWidth * BallPositionWidth;
            var yPosition = ballImage.transform.localPosition.y;
            if (IsFocusOnGoal(digestType))
            {
                xPosition = log.OffenceSide == BattleDataMediator.Instance.PlayerSide ? GoalPositionX : -GoalPositionX;
                yPosition = GoalPositionY;
                SetPlayerDotsVisible(true);
            }else if (isShowBall)
            {
                var isShowDots = IsShowDots(digestType);
                SetPlayerDotsVisible(isShowDots);
                
                var max = Mathf.CeilToInt(MoveXPositionCoefficient(digestType) * 50);
                var min = max * -1;
                var diff = BattleGameLogic.GetNonStateRandomValue(min, max);
                if (IsMoveOverYCenterLine(digestType))
                {
                    diff += yPosition > 0 ? -BallPositionHalfHeight : BallPositionHalfHeight;
                }
                yPosition = Mathf.Clamp(yPosition + diff, -BallPositionHalfHeight, BallPositionHalfHeight);

                if (isShowDots && IsUpdateRadar(digestType))
                {
                    SetPlayerDotsPosition((int)xPosition, (int)yPosition, log.MainCharacterData.Side, duration);
                }
            }
            
            nextPosition = new Vector3(xPosition, yPosition, 0);
            moveTween = ballImage.transform.DOLocalMove(nextPosition, duration).OnComplete(() =>
            {
                moveTween.Kill();
                moveTween = null;
            });
            
            ballImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            ballImage.DOFade(1.0f, 1.0f);
        }
        
        private float MoveXPositionCoefficient(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                    return 0.5f;
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    return 0.5f;
                case BattleConst.DigestType.MatchUp:
                    return 0.0f;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                    return 0.3f;
                case BattleConst.DigestType.Cross:
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                    return 1.0f;
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
                    return 0.0f;
                case BattleConst.DigestType.Shoot:
                case BattleConst.DigestType.ShootBlockL:
                case BattleConst.DigestType.ShootBlockR:
                case BattleConst.DigestType.ShootBlockTouchL:
                case BattleConst.DigestType.ShootBlockTouchR:
                case BattleConst.DigestType.ShootBlockNotReachL:
                case BattleConst.DigestType.ShootBlockNotReachR:
                case BattleConst.DigestType.ShootResultSuccessL:
                case BattleConst.DigestType.ShootResultSuccessR:
                case BattleConst.DigestType.ShootResultPunchL:
                case BattleConst.DigestType.ShootResultPunchR:
                case BattleConst.DigestType.ShootResultPostL:
                case BattleConst.DigestType.ShootResultPostR:
                case BattleConst.DigestType.ShootResultCatch:
                    return 0.0f; // シュート系はゴールに合わせる.
                case BattleConst.DigestType.Goal:
                case BattleConst.DigestType.Goal_GameSet:
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                case BattleConst.DigestType.OutBall:
                case BattleConst.DigestType.ThrowIn:
                case BattleConst.DigestType.ThrowInKeeper:
                case BattleConst.DigestType.TimeUp:
                    return 0.0f; // 映さない

                case BattleConst.DigestType.Special:
                    return 0.0f;
            }

            return 0.0f;
        }

        public bool IsShowRadar(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                    /*
                case BattleConst.DigestType.Cross:
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                case BattleConst.DigestType.Shoot:
                case BattleConst.DigestType.ShootBlockL:
                case BattleConst.DigestType.ShootBlockR:
                case BattleConst.DigestType.ShootBlockTouchL:
                case BattleConst.DigestType.ShootBlockTouchR:
                case BattleConst.DigestType.ShootBlockNotReachL:
                case BattleConst.DigestType.ShootBlockNotReachR:
                case BattleConst.DigestType.ShootResultSuccessL:
                case BattleConst.DigestType.ShootResultSuccessR:
                case BattleConst.DigestType.ShootResultPunchL:
                case BattleConst.DigestType.ShootResultPunchR:
                case BattleConst.DigestType.ShootResultPostL:
                case BattleConst.DigestType.ShootResultPostR:
                case BattleConst.DigestType.ShootResultCatch:
                */
                    return true;
            }

            return false;
        }

        private bool IsUpdateRadar(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    /*
                case BattleConst.DigestType.Shoot:
                case BattleConst.DigestType.ShootBlockL:
                case BattleConst.DigestType.ShootBlockR:
                case BattleConst.DigestType.ShootBlockTouchL:
                case BattleConst.DigestType.ShootBlockTouchR:
                case BattleConst.DigestType.ShootBlockNotReachL:
                case BattleConst.DigestType.ShootBlockNotReachR:
                case BattleConst.DigestType.ShootResultSuccessL:
                case BattleConst.DigestType.ShootResultSuccessR:
                case BattleConst.DigestType.ShootResultPunchL:
                case BattleConst.DigestType.ShootResultPunchR:
                case BattleConst.DigestType.ShootResultPostL:
                case BattleConst.DigestType.ShootResultPostR:
                case BattleConst.DigestType.ShootResultCatch:
                */
                    return true;
            }

            return false;
        }
        
        private bool IsShowDots(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                    return false;
            }

            return true;
        }
        
        private bool IsShowBall(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                case BattleConst.DigestType.Goal:
                case BattleConst.DigestType.Goal_GameSet:
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                case BattleConst.DigestType.OutBall:
                case BattleConst.DigestType.ThrowIn:
                case BattleConst.DigestType.ThrowInKeeper:
                case BattleConst.DigestType.TimeUp:
                    return false;
            }

            return true;
        }

        private bool IsFocusOnGoal(BattleConst.DigestType digestType)
        {
            if (BattleConst.DigestType.Shoot <= digestType && digestType <= BattleConst.DigestType.Goal)
            {
                return true;
            }

            return false;
        }

        private bool IsMoveOverYCenterLine(BattleConst.DigestType digestType)
        {
            if (BattleConst.DigestType.Cross <= digestType && digestType <= BattleConst.DigestType.PassSuccess)
            {
                return true;
            }

            return false;
        }

        private const int TeamSize = 5;
        public void OnSideReset(BattleConst.TeamSide offenceSide)
        {
            lastBallOwnerId = -1;
            eachLinePlayerCountOn3D = new int[3];
            foreach (var character in BattleDataMediator.Instance.Decks[(int)offenceSide])
            {
                eachLinePlayerCountOn3D[(int)character.Position - 1]++;
            }
        }

        private void SetPlayerDotsVisible(bool isVisible)
        {
            foreach (var dot in leftSidePlayerDots)
            {
                dot.transform.parent.gameObject.SetActive(isVisible);
            }
            
            foreach (var dot in rightSidePlayerDots)
            {
                dot.transform.parent.gameObject.SetActive(isVisible);
            }
        }

        private void SetPlayerModelPosition(float ballXPosition, float ballYPosition, BattleConst.TeamSide offenceSide, long ballOwnerId, long againstCharaId, BattleConst.DigestType digestType)
        {
            targetPositions.Clear();

            if (ballOwnerId != lastBallOwnerId)
            {
                ReOrderCharacterData(offenceSide, ballOwnerId);
            }
            lastBallOwnerId = ballOwnerId;

            foreach (var kvp in rigidbodies)
            {
                kvp.Value.constraints = RigidbodyConstraints.FreezeRotation;
                if (kvp.Key == ballOwnerId || kvp.Key == againstCharaId)
                {
                    kvp.Value.constraints = RigidbodyConstraints.FreezeAll;
                }
                kvp.Value.linearVelocity = Vector3.zero;
            }

            var offenceModels = characterModelDictionaryList[(int)offenceSide];
            var defenceModels = characterModelDictionaryList[(int) BattleGameLogic.GetOtherSide(offenceSide)];

            SetBallPosition(ballXPosition, ballYPosition);
            SetCameraPosition(ballXPosition, digestType);
            if (digestType is BattleConst.DigestType.KickOffL or BattleConst.DigestType.KickOffR)
            {
                SetKickOffPosition(offenceSide, leftSidePlayerModelRoots, rightSidePlayerModelRoots);
                SetBallPosition(0, 0);
                foreach (var materialSetters in characterModelMaterialSetterList)
                {
                    foreach (var materialSetter in materialSetters.Values)
                    {
                        materialSetter.SetOpaqueMaterial();
                    }
                }
            }
            else
            {
                // autoSimulation=Offにして問題があるようならここ検討.
                // Physics.autoSimulation = false;
                SetOffenceModels(offenceSide, ballXPosition, ballYPosition, offenceModels, ballOwnerId);
                SetDefenceModels(offenceSide, ballXPosition, ballYPosition, defenceModels, ballOwnerId, againstCharaId);
                // 一気に進めようとしてもイマイチだったので.
                for (var i = 0; i < 60; i++)
                {
                    Physics.Simulate(1/60.0f);
                }

                // 物理演算で壁にめり込むたわけがいるので強制的に(壁、ボール保有者、BLMは不動になるのでしょうがない). このあとはPhysics.Simulateすることはないのでここでキャラクター同士重なっちゃうのは許容.
                foreach (var model in offenceModels.Values)
                {
                    var pos = model.transform.localPosition;
                    pos.x = Mathf.Clamp(pos.x, -BallPositionWidth_3DView, BallPositionWidth_3DView);
                    pos.y = Mathf.Clamp(pos.y, -BallPositionHalfHeight_3DView, BallPositionHalfHeight_3DView);
                    model.transform.localPosition = pos;
                }
                foreach (var model in defenceModels.Values)
                {
                    var pos = model.transform.localPosition;
                    pos.x = Mathf.Clamp(pos.x, -BallPositionWidth_3DView, BallPositionWidth_3DView);
                    pos.y = Mathf.Clamp(pos.y, -BallPositionHalfHeight_3DView, BallPositionHalfHeight_3DView);
                    model.transform.localPosition = pos;
                }
            }
            FlashBackImages();
        }
        
        private void SetPlayerDotsPosition(int ballXPosition, int ballYPosition, BattleConst.TeamSide offenceSide, float duration)
        {
            targetPositions.Clear();
            
            var offenceDots = offenceSide == BattleDataMediator.Instance.PlayerSide ? leftSidePlayerDots : rightSidePlayerDots;
            var defenceDots = offenceSide == BattleDataMediator.Instance.PlayerSide ? rightSidePlayerDots : leftSidePlayerDots;

            SetOffenceDots(offenceDots, offenceSide, ballXPosition, ballYPosition, 0);
            SetDefenceDots(defenceDots, ballXPosition, offenceSide, 0);
            ReOrderDots(offenceDots, defenceDots);
            FlashBackImages();
        }

        private const float PositionCoef = 0.95f;
        private const float XPositionDiff = 0.75f;
        private const float XPositionDiffForCoef = 0.8f;
        private const float XPositionRandValue = 0.25f;
        private const float YPositionRandValue = 0.5f;

        private void SetBallPosition(float ballXPosition, float ballYPosition)
        {
            ballModel.transform.localPosition = new Vector3(ballXPosition, ballYPosition, 0);
        }

        private const float CameraPosCoef = 50.0f;
        private const float DefaultFOV = 60.0f;
        private const float MinFOV = 35.0f;
        private const float MaxCameraPos = 100.0f;
        private void SetCameraPosition(float ballXPosition, BattleConst.DigestType digestType)
        {
            if (digestType is BattleConst.DigestType.KickOffL or BattleConst.DigestType.KickOffR)
            {
                virtualCamera.transform.localPosition = defaultCameraPos;
                virtualCamera.m_Lens.FieldOfView = DefaultFOV;
                return;
            }
            
            var cameraPos = ballXPosition * PositionCoef * CameraPosCoef;
            cameraPos = Mathf.Clamp(cameraPos, -MaxCameraPos, MaxCameraPos);
            virtualCamera.transform.localPosition = new Vector3(cameraPos, defaultCameraPos.y, defaultCameraPos.z);
            virtualCamera.m_Lens.FieldOfView = MinFOV;
        }

        private void SetKickOffPosition(BattleConst.TeamSide side, GameObject[] leftSideRoots, GameObject[] rightSideRoots)
        {
            if (side == BattleDataMediator.Instance.PlayerSide)
            {
                for (var i = 0; i < TeamSize; i++)
                {
                    leftSideRoots[i].transform.localPosition = kickOffPositions[(int)BattleConst.TeamSide.Left, i];
                    rightSideRoots[i].transform.localPosition = kickOffPositions[(int)BattleConst.TeamSide.Right, i];
                }
            }
            else
            {
                var scale = new Vector3(-1, 1, 1);
                for (var i = 0; i < TeamSize; i++)
                {
                    var leftPos = kickOffPositions[(int)BattleConst.TeamSide.Right, i];
                    leftPos.Scale(scale);
                    leftSideRoots[i].transform.localPosition = leftPos;
                    var rightPos = kickOffPositions[(int)BattleConst.TeamSide.Left, i];
                    rightPos.Scale(scale);
                    rightSideRoots[i].transform.localPosition = rightPos;
                }
            }
        }

        private void ReOrderCharacterData(BattleConst.TeamSide side, long characterId)
        {
            var targetList = eachSideCharacterIds[(int)side];
            var index = targetList.IndexOf(characterId);
            // 最前列なので移動なし.
            if (index < eachLinePlayerCountOn2D[0])
            {
                return;
            }

            var moveNum = 0;
            var lineNum = 0;
            var inLineCount = 0;
            for (var i = 0; i < targetList.Count; i++)
            {
                inLineCount++;
                moveNum++;
                
                if (characterId == targetList[i])
                {
                    break;
                }

                if (inLineCount >= eachLinePlayerCountOn2D[lineNum])
                {
                    inLineCount = 0;
                    moveNum = 0;
                    lineNum++;
                }
            }

            var moveIndex = Mathf.Max(index - moveNum, 0);
            targetList.RemoveAt(index);
            targetList.Insert(moveIndex, characterId);
        }
        
        private void SetOffenceModels(BattleConst.TeamSide side, float ballXPosition, float ballYPosition, Dictionary<long, GameObject> models, long ballOwnerId)
        {
            var isInPenaltyArea = Mathf.Abs(ballXPosition) >= PenaltyAreaWidth_3DView;
            // ボールが端に寄り過ぎてるとプレイヤーが場外に出るので見た目からちょいずらす
            ballXPosition = (ballXPosition * PositionCoef);
            ballYPosition = (ballYPosition * PositionCoef);
            var xPositionDiff = XPositionDiff;
            var xPositionDiffForCoef = XPositionDiffForCoef;
            var xCoef = 1.0f - Mathf.Abs(ballXPosition / BallPositionWidth_3DView);
            if (side == BattleDataMediator.Instance.EnemySide)
            {
                xPositionDiff *= -1;
                xPositionDiffForCoef *= -1;
                xCoef *= -1;
            }

            var indexInLine = 0;
            var lineIndex = 0;
            
            // 今どこのラインにボールがあるかの判定.
            var ballLineIndex = 0;
            var offenceCharacters = eachSideCharacterIds[(int)side];
            foreach (var characterId in offenceCharacters)
            {
                if (characterId == ballOwnerId)
                {
                    break;
                }
                
                if (indexInLine >= eachLinePlayerCountOn3D[lineIndex])
                {
                    indexInLine = 0;
                    ballLineIndex++;
                }

                indexInLine++;
            }

            indexInLine = 0;
            for (var i = 0; i < offenceCharacters.Count; i++)
            {
                if (indexInLine >= eachLinePlayerCountOn3D[lineIndex])
                {
                    indexInLine = 0;
                    lineIndex++;
                }

                var xPosition = ballXPosition;
                var yPosition = ballYPosition;
                var model = models[offenceCharacters[i]];
                // X位置. 前中後でそれぞれボール位置基準に補正.
                var lineIndexDiff = lineIndex - ballLineIndex;
                xPosition += BattleGameLogic.GetNonStateRandomValue(-XPositionRandValue, XPositionRandValue) * xPositionDiffForCoef - ((xPositionDiff + XPositionDiffForCoef * xCoef) * lineIndexDiff);

                // Y位置. 基本はラインに配置されるプレイヤー数によって適当に.
                switch (eachLinePlayerCountOn3D[lineIndex])
                {
                    case 1:
                        yPosition = BattleGameLogic.GetNonStateRandomValue(-YPositionRandValue, YPositionRandValue);
                        break;
                    case 2:
                        if (indexInLine == 0)
                        {
                            yPosition = BattleGameLogic.GetNonStateRandomValue(-BallPositionHalfHeight_3DView, -YPositionRandValue);
                        }
                        else
                        {
                            yPosition = BattleGameLogic.GetNonStateRandomValue(YPositionRandValue, BallPositionHalfHeight_3DView);
                        }
                        break;
                }

                // ポジションによる補正. ボール保有している列でない場合, ポジションによって前に寄せたり後ろに寄せたりする.
                if(lineIndex != ballLineIndex)
                {
                    switch (lineIndex)
                    {
                        case 0:
                            xPosition += BattleGameLogic.GetNonStateRandomValue(0, XPositionRandValue) * xCoef;
                            break;
                        case 1:
                            break;
                        case 2:
                            xPosition += BattleGameLogic.GetNonStateRandomValue(-XPositionRandValue, 0) * xCoef;
                            break;
                    }                    
                }

                // 後衛ラインの例外系
                // Y軸はややボール位置に寄っているように見せる
                if (lineIndex == 2)
                {
                    yPosition -= (yPosition + ballYPosition) / 3;
                }

                // ボール保有者はボールにビタでつける.
                var isBallOwner = offenceCharacters[i] == ballOwnerId;
                if (isBallOwner)
                {
                    xPosition = ballXPosition - xPositionDiff;
                    yPosition = ballYPosition;
                }

                if (isBallOwner)
                {
                    characterModelMaterialSetterList[(int)side][offenceCharacters[i]].SetOpaqueMaterial();
                }else{
                    characterModelMaterialSetterList[(int)side][offenceCharacters[i]].SetTransparentMaterial();
                }

                xPosition = Mathf.Clamp(xPosition, -BallPositionWidth_3DView, BallPositionWidth_3DView);
                yPosition = Mathf.Clamp(yPosition, -BallPositionHalfHeight_3DView, BallPositionHalfHeight_3DView);

                var targetPosition = new Vector3(xPosition, yPosition, 0);
                model.transform.localPosition = targetPosition;
                targetPositions.Add(targetPosition);
                indexInLine++;
            }
        }
        
        private void SetDefenceModels(BattleConst.TeamSide offenceSide, float ballXPosition, float ballYPosition, Dictionary<long, GameObject> models, long ballOwnerId, long againstCharacterId)
        {
            var isInPenaltyArea = Mathf.Abs(ballXPosition) >= PenaltyAreaWidth_3DView;
            var basePositionDiff = XPositionDiff;
            var indexInLine = 0;
            var lineIndex = 0;
            for (var i = 0; i < TeamSize; i++)
            {
                var model = models.ElementAt(i).Value;
                var targetPosition = targetPositions[i];
                
                if (indexInLine >= eachLinePlayerCountOn3D[lineIndex])
                {
                    indexInLine = 0;
                    lineIndex++;
                }

                var radian = 0.0f;
                if (isInPenaltyArea)
                {
                    radian = BattleGameLogic.GetNonStateRandomValue(-45, 45) / 180.0f * Mathf.PI;
                }
                else
                {
                    radian = BattleGameLogic.GetNonStateRandomValue(-60, 60) / 180.0f * Mathf.PI;
                }

                // xPosは1ライン分ずらす
                var xDiff = (1.5f + Mathf.Cos(radian)) * basePositionDiff;
                var yDiff = Mathf.Sin(radian) * basePositionDiff;

                if (offenceSide == BattleDataMediator.Instance.EnemySide)
                {
                    xDiff *= -1;
                }

                var xPosition = targetPosition.x;
                var yPosition = targetPosition.y;
                
                // 後衛ラインの例外系
                // がっつりマークについてないようにやや離す
                if (lineIndex == 2)
                {
                    yDiff *= 2;
                }

                xPosition = Mathf.Clamp(xPosition + xDiff, -BallPositionWidth_3DView, BallPositionWidth_3DView);
                yPosition = Mathf.Clamp(yPosition + yDiff, -BallPositionHalfHeight_3DView, BallPositionHalfHeight_3DView);
                
                model.transform.localPosition = new Vector3(xPosition, yPosition, 0);
                indexInLine++;
            }
            
            foreach (var materialSetter in characterModelMaterialSetterList[(int)BattleGameLogic.GetOtherSide(offenceSide)])
            {
                materialSetter.Value.SetTransparentMaterial();
            }

            if (ballOwnerId >= 0 && againstCharacterId >= 0)
            {
                var xPositionDiff = XPositionDiff * (offenceSide == BattleDataMediator.Instance.PlayerSide ? -1 : 1);
                var offenceCharacters = eachSideCharacterIds[(int) offenceSide];
                var index = offenceCharacters.FindIndex(id => id == ballOwnerId);
                var defenceModel = models[againstCharacterId];
                var swapTarget = models.ElementAt(index).Value;
                (defenceModel.transform.localPosition, swapTarget.transform.localPosition) =
                    (
                        // ボール保有者に対するマークはボールにビタでつける.
                        new Vector3(ballXPosition - xPositionDiff, ballYPosition, 0),
                        new Vector3(defenceModel.transform.localPosition.x, defenceModel.transform.localPosition.y, defenceModel.transform.localPosition.z));
            }
            
            characterModelMaterialSetterList[(int)BattleGameLogic.GetOtherSide(offenceSide)][againstCharacterId].SetOpaqueMaterial();
        }

        private void PlayModelAnimation(BattleConst.TeamSide offenceSide, BattleConst.DigestType digestType, long ballOwnerId)
        {
            var isShoot = digestType is BattleConst.DigestType.Shoot or BattleConst.DigestType.Cross or BattleConst.DigestType.PassSuccess or BattleConst.DigestType.PassFailed;
            var isMatchUp = digestType == BattleConst.DigestType.MatchUp;
            var isKickOff = digestType is BattleConst.DigestType.KickOffL or BattleConst.DigestType.KickOffR;
            
            var indexInLine = 0;
            
            foreach (var animator in playerAnimators)
            {
                animator.transform.localPosition = Vector3.zero;
            }
            
            var defenceSide = BattleGameLogic.GetOtherSide(offenceSide);

            var ballOwnerIndex = 0;
            foreach (var characterId in eachSideCharacterIds[(int)offenceSide])
            {
                if (characterId == ballOwnerId)
                {
                    break;
                }

                ballOwnerIndex++;
            }

            for (var i = 0; i < TeamSize; i++)
            {
                var isBallOwner = i == ballOwnerIndex;

                var offenceAnimHash = CharacterDefenceIdleHash;
                var defenceAnimHash = CharacterDefenceIdleHash;
                if (isMatchUp && isBallOwner)
                {
                    offenceAnimHash = CharacterBallKeepIdleHash;
                    defenceAnimHash = CharacterDefenceIdleHash;
                }else if (isShoot && isBallOwner)
                {
                    offenceAnimHash = CharacterShootHash;
                    defenceAnimHash = CharacterDefenceIdleHash;
                    playerAnimators[(int) offenceSide, i].transform.localRotation = characterTransformsLookToFront[(int)offenceSide].transform.localRotation;
                }
                else if(!isKickOff)
                {
                    offenceAnimHash = CharacterDribbleHash;
                    defenceAnimHash = CharacterDribbleHash;
                }

                playerAnimators[(int) offenceSide, i].Play(offenceAnimHash, -1, BattleGameLogic.GetNonStateRandomValue(0.0f, 1.0f));
                playerAnimators[(int) defenceSide, i].Play(offenceAnimHash, -1, BattleGameLogic.GetNonStateRandomValue(0.0f, 1.0f));

                indexInLine++;
            }
        }

        private void ResetModelRotation()
        {
            for (var i = 0; i < playerAnimators.GetLength(1); i++)
            {
                playerAnimators[(int)BattleConst.TeamSide.Left, i].transform.localRotation = characterOriginalTransforms[(int)BattleConst.TeamSide.Left].localRotation;
                playerAnimators[(int)BattleConst.TeamSide.Right, i].transform.localRotation = characterOriginalTransforms[(int)BattleConst.TeamSide.Right].localRotation;
            }
        }
        
        private void ReOrderDots(Image[] offenceDots, Image[] defenceDots)
        {
            foreach (var image in defenceDots.OrderByDescending(dot => dot.transform.localPosition.y))
            {
                image.transform.parent.transform.SetAsLastSibling();
            }
            
            foreach (var image in offenceDots.OrderByDescending(dot => dot.transform.localPosition.y))
            {
                image.transform.parent.transform.SetAsLastSibling();
            }
        }

        private void FlashBackImages()
        {
            var endScale = new Vector3(1.6f, 1.6f, 1.6f);
            foreach (var back in leftSidePlayerDotBackImages)
            {
                back.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                back.DOFade(1.0f, 0.6f).SetLoops(2, LoopType.Yoyo);;
                back.transform.localScale = Vector3.one;
                back.transform.DOScale(endScale, 0.6f).SetLoops(2, LoopType.Yoyo);
            }
            
            foreach (var back in rightSidePlayerDotBackImages)
            {
                back.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                back.DOFade(1.0f, 0.6f).SetLoops(2, LoopType.Yoyo);;
                back.transform.localScale = Vector3.one;
                back.transform.DOScale(endScale, 0.6f).SetLoops(2, LoopType.Yoyo);
            }
        }
        
#region 2DView

        private void SetOffenceDots(Image[] dots, BattleConst.TeamSide side, int ballXPosition, int ballYPosition, float duration)
        {
            var isInPenaltyArea = Mathf.Abs(ballXPosition) >= PenaltyAreaWidth;
            // ボールが端に寄り過ぎてるとプレイヤーが場外に出るので見た目からちょいずらす
            ballXPosition = (int)(ballXPosition * 0.95f);
            ballYPosition = (int)(ballYPosition * 0.95f);
            var xPositionDiff = 150;
            var xPositionDiffForCoef = 100;
            var xCoef = ballYPosition / FieldHalfWidth;
            if (side == BattleConst.TeamSide.Right)
            {
                xPositionDiff *= -1;
                xPositionDiffForCoef *= -1;
                xCoef *= -1;
            }
            
            var indexInLine = 0;
            var lineIndex = 0;
            for (var i = 0; i < TeamSize; i++)
            {
                if (indexInLine >= eachLinePlayerCountOn2D[lineIndex])
                {
                    indexInLine = 0;
                    lineIndex++;
                }

                var xPosition = ballXPosition;
                var yPosition = ballYPosition;
                var dot = dots[i];
                dot.transform.parent.gameObject.SetActive(true);
                // Y位置. 前中後でそれぞれボール位置基準に補正.
                switch (lineIndex)
                {
                    case 0:
                        xPosition += xPositionDiff + BattleGameLogic.GetNonStateRandomValue(-20, 20) - (int)(xPositionDiffForCoef * xCoef);
                        break;
                    case 1:
                        break;
                    case 2:
                        xPosition -= xPositionDiff + BattleGameLogic.GetNonStateRandomValue(-20, 20) +  (int)(xPositionDiffForCoef * xCoef);
                        break;
                }

                // X位置. 基本はラインに配置されるプレイヤー数によって適当に.
                switch (eachLinePlayerCountOn2D[lineIndex])
                {
                    case 1:
                        yPosition = BattleGameLogic.GetNonStateRandomValue(-100, 100);
                        break;
                    case 2:
                        if (indexInLine == 0)
                        {
                            yPosition = BattleGameLogic.GetNonStateRandomValue(-200, -50);
                        }
                        else
                        {
                            yPosition = BattleGameLogic.GetNonStateRandomValue(50, 200);
                        }
                        break;
                }

                // ボール保持ラインの例外系
                // ボールに寄っている側はボール保有者っぽく見せるため.
                if (lineIndex == 1)
                {
                    if (eachLinePlayerCountOn2D[lineIndex] == 1 ||
                        (yPosition > 0 && ballYPosition > 0) || (yPosition < 0 && ballYPosition < 0))
                    {
                        xPosition = ballXPosition;
                        yPosition = ballYPosition;
                        dot.transform.parent.gameObject.SetActive(false);
                    }
                }

                // 後衛ラインの例外系
                // X軸はややボール位置に寄っているように見せる
                if (lineIndex == 2)
                {
                    yPosition -= (yPosition + ballYPosition) / 3;
                }

                if (lineIndex == 0 && isInPenaltyArea)
                {
                    yPosition = (int)((yPosition + GoalPositionY) * 0.5f);
                }
                
                // 横位置縦位置による補正
                var xDiff = (xPosition / BallPositionWidth) * 50.0f;
                var yDiff = -yPosition / BallPositionHalfHeight;
                xPosition += (int)(xDiff * yDiff);
                var scale = 1.0f + yDiff * 0.15f;
                dot.transform.parent.transform.localScale = new Vector3(scale, scale, scale);

                xPosition = Mathf.Clamp(xPosition, -BallPositionWidth, BallPositionWidth);
                yPosition = Mathf.Clamp(yPosition, -BallPositionHalfHeight, BallPositionHalfHeight);

                //dot.transform.localPosition = new Vector3(xPosition, yPosition, 0);
                var targetPosition = new Vector3(xPosition, yPosition, 0);
                dot.transform.parent.transform.DOLocalMove(targetPosition, duration);
                targetPositions.Add(targetPosition);
                indexInLine++;
                
                dot.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                dot.DOFade(1.0f, 1.0f); //.SetLoops(-1, LoopType.Yoyo);
            }
        }
        
        private void SetDefenceDots(Image[] dots, int ballXPosition, BattleConst.TeamSide offenceSide, float duration)
        {
            var isInPenaltyArea = Mathf.Abs(ballXPosition) >= PenaltyAreaWidth;
            var basePositionDiff = 50;
            var indexInLine = 0;
            var lineIndex = 0;
            for (var i = 0; i < TeamSize; i++)
            {
                var dot = dots[i];
                var targetPosition = targetPositions[i];
                
                dot.transform.parent.gameObject.SetActive(true);
                if (indexInLine >= eachLinePlayerCountOn2D[lineIndex])
                {
                    indexInLine = 0;
                    lineIndex++;
                }

                var xDiff = 0.0f;
                var yDiff = 0.0f;
                var radian = BattleGameLogic.GetNonStateRandomValue(-90, 90) / 180.0f * Mathf.PI;

                xDiff = Mathf.Cos(radian) * basePositionDiff;
                yDiff = Mathf.Sin(radian) * basePositionDiff;

                if (offenceSide == BattleConst.TeamSide.Right)
                {
                    xDiff *= -1;
                }

                var xPosition = targetPosition.x;
                var yPosition = targetPosition.y;
                
                // 横位置縦位置による補正
                var xPosCoef = (xPosition / BallPositionWidth) * 50.0f;
                var yPosCoef = -yPosition / BallPositionHalfHeight;
                xPosition += (int)(xPosCoef * yPosCoef);
                var scale = 1.0f + yPosCoef * 0.15f;
                dot.transform.parent.transform.localScale = new Vector3(scale, scale, scale);

                // 後衛ラインの例外系
                // がっつりマークについてないようにやや離す
                if (lineIndex == 2)
                {
                    yDiff *= 2;
                }

                xPosition = Mathf.Clamp(xPosition + xDiff, -BallPositionWidth, BallPositionWidth);
                yPosition = Mathf.Clamp(yPosition + yDiff, -BallPositionHalfHeight, BallPositionHalfHeight);
                
                if (isInPenaltyArea && lineIndex == 0)
                {
                    xPosition = (targetPosition.x + (offenceSide == BattleDataMediator.Instance.PlayerSide ? GoalPositionX : -GoalPositionX)) / 2;
                    yPosition = (targetPosition.y + GoalPositionY) / 2;
                }
                
                //dot.transform.localPosition = new Vector3(xPosition, yPosition, 0.0f);
                dot.transform.parent.transform.DOLocalMove(new Vector3(xPosition, yPosition, 0), duration);
                indexInLine++;
                
                dot.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                dot.DOFade(1.0f, 1.0f);//.SetLoops(-1, LoopType.Yoyo);
            }
        }

#endregion
    }
}