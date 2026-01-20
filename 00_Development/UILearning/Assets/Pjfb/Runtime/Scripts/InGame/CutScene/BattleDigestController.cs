using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using CruFramework.ResourceManagement;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Voice;
using UnityEngine.Playables;

namespace Pjfb.InGame
{
    public class BattleDigestController : MonoBehaviour
    {
        public static BattleDigestController Instance
        {
            get;
            private set;
        }
        
        private BattleDigestObject _currentDigest;
        private bool isDoneAnimDirector;
        private bool forceQuitAnimDirector;
        protected Action middleAction;
        
        private CancellationTokenSource source = null;
        private ResourcesLoader resourcesLoader = new ResourcesLoader();
        public ResourcesLoader BattleResourcesLoader => resourcesLoader;

        private bool forceFlatPlaySpeed = false;
        private Dictionary<BattleConst.DigestType, bool> playedDigestDictionary;
        
        private static readonly float[] dribbleLocationTypeByStaminaRatio= { 0.3f, 0.6f };
        private static readonly CharaVoiceLocationType[] dribbleLocationTypes = { CharaVoiceLocationType.DribbleOwnerTired, CharaVoiceLocationType.DribbleOwnerImpatience, CharaVoiceLocationType.DribbleOwnerGood};
        
        private void Awake()
        {
            Instance = this;

            playedDigestDictionary = new Dictionary<BattleConst.DigestType, bool>();
            for (var i = BattleConst.DigestType.None; i < BattleConst.DigestType.Max; i++)
            {
                playedDigestDictionary.Add(i, false);
            }
        }

        protected virtual float GetDigestPlaySpeed(BattleConst.DigestType digestType, BattleDigestCharacterData mainCharacter, List<BattleDigestCharacterData> otherCharacters, BattleConst.TeamSide offenceSide)
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.ForceFlatPlaySpeed)
            {
                return 1.0f;
            }
#endif
            if (BattleDataMediator.Instance.IsReplayDigestMode)
            {
                return 1.0f;
            }
            
            // バグりまくるので一旦1倍強制
            if (digestType == BattleConst.DigestType.Special)
            {
                return 1.0f;
            }
            
            // プレイヤーのも早くする設定だったら常に8倍
            if (BattleDataMediator.Instance.IsSpeedUpPlayerDigest)
            {
                return 8.0f;
            }
            
            if (forceFlatPlaySpeed)
            {
                return 1.0f;
            }
            
            var isPlayerCaptain = mainCharacter != null && mainCharacter.IsAce && mainCharacter.Side == BattleDataMediator.Instance.PlayerSide;
            var isAlly = offenceSide == BattleDataMediator.Instance.PlayerSide;
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                    playedDigestDictionary[BattleConst.DigestType.DribbleL] = false;
                    return 1.0f;
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    if (!playedDigestDictionary[BattleConst.DigestType.DribbleL])
                    {
                        return 1.0f;
                    }
                    return 2.0f;
                case BattleConst.DigestType.MatchUp:
                    // 初回キャプテン等倍
                    if (isPlayerCaptain/* && !PlayedDigestDictionary[BattleConst.DigestType.MatchUp]*/)
                    {
                        // DO SOME THING
                        return 1.0f;
                    }
                    return isAlly ? 2.0f : 8.0f;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                    if (isPlayerCaptain && !playedDigestDictionary[BattleConst.DigestType.TechnicMatchUpWinL])
                    {
                        return 1.0f;
                    }
                    return isPlayerCaptain ? 2.0f : 8.0f;
                case BattleConst.DigestType.Cross:
                    // 参加者にプレイヤーキャプテンがいたら, のため
                    isPlayerCaptain |= otherCharacters.Any(chara => chara.IsAce && chara.Side == BattleDataMediator.Instance.PlayerSide);
                    if (isPlayerCaptain && !playedDigestDictionary[BattleConst.DigestType.Cross])
                    {
                        return 1.0f;
                    }
                    return isPlayerCaptain ? 2.0f : 8.0f;
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                    if (isPlayerCaptain && !playedDigestDictionary[BattleConst.DigestType.PassFailed])
                    {
                        return 1.0f;
                    }
                    return isPlayerCaptain ? 2.0f : 8.0f;
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
                    return isPlayerCaptain ? 1.0f : 8.0f;
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
                    return isPlayerCaptain ? 1.0f : 2.0f;
                case BattleConst.DigestType.Goal:
                case BattleConst.DigestType.Goal_GameSet:
                    return 1.0f;
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                    return 1.0f;
                    /*
                    // 参加者にプレイヤーキャプテンがいたら, のため
                    isPlayerCaptain |= otherCharacters.Any(chara => chara.IsAce && chara.Side == BattleConst.TeamSide.Left);
                    if (isPlayerCaptain && !playedDigestDictionary[BattleConst.DigestType.SecondBall2])
                    {
                        playedDigestDictionary[BattleConst.DigestType.SecondBall2] = true;
                        return 1.0f;
                    }
                    return isPlayerCaptain ? 2.0f : 8.0f;
                    */
                case BattleConst.DigestType.OutBall:
//                    return 8.0f;
                case BattleConst.DigestType.ThrowIn:
                    /*
                    // 参加者にプレイヤーキャプテンがいたら, のため
                    isPlayerCaptain |= otherCharacters.Any(chara => chara.IsAce && chara.Side == BattleConst.TeamSide.Left);
                    if (isPlayerCaptain && !playedDigestDictionary[BattleConst.DigestType.ThrowIn])
                    {
                        playedDigestDictionary[BattleConst.DigestType.ThrowIn] = true;
                        return 1.0f;
                    }
                    return isPlayerCaptain ? 2.0f : 8.0f;
                    */
                case BattleConst.DigestType.ThrowInKeeper:
                    return 1.0f;
                case BattleConst.DigestType.TimeUp:
                    return 1.0f;
                case BattleConst.DigestType.Special:
                    return isAlly ? 1.0f : 8.0f;
            }
            
            return 1.0f;
        }

        private bool IsShowBlackLayer(BattleConst.DigestType digestType, bool isPlayerCaptain)
        {
            if (forceFlatPlaySpeed)
            {
                return false;
            }
            
            if (BattleDataMediator.Instance.IsReplayDigestMode)
            {
                return false;
            }

            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                    return false;
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    if (!playedDigestDictionary[BattleConst.DigestType.DribbleL])
                    {
                        playedDigestDictionary[BattleConst.DigestType.DribbleL] = true;
                        return false;
                    }
                    return true;
                case BattleConst.DigestType.MatchUp:
                    if (isPlayerCaptain)
                    {
                        return false;
                    }
                    return true;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                    playedDigestDictionary[BattleConst.DigestType.TechnicMatchUpWinL] = true;
                    return false;
                case BattleConst.DigestType.Cross:
                    playedDigestDictionary[BattleConst.DigestType.Cross] = true;
                    return false;
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                    playedDigestDictionary[BattleConst.DigestType.PassFailed] = true;
                    if (isPlayerCaptain)
                    {
                        return false;
                    }
                    return true;
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
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
                case BattleConst.DigestType.Goal:
                case BattleConst.DigestType.Goal_GameSet:
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                case BattleConst.DigestType.OutBall:
                case BattleConst.DigestType.ThrowIn:
                case BattleConst.DigestType.ThrowInKeeper:
                case BattleConst.DigestType.TimeUp:
                case BattleConst.DigestType.Special:
                    return false;
            }
            
            return false;
        }
        
        private bool IsShowRadar(BattleConst.DigestType digestType)
        {
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                    return false;
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    return true;
                case BattleConst.DigestType.MatchUp:
                    return false;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                case BattleConst.DigestType.Cross:
                    return true;
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
                    return false;
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
                    return true;
                case BattleConst.DigestType.Goal:
                case BattleConst.DigestType.Goal_GameSet:
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                    return false;
                case BattleConst.DigestType.OutBall:
                case BattleConst.DigestType.ThrowIn:
                case BattleConst.DigestType.ThrowInKeeper:
                    return false;
                case BattleConst.DigestType.TimeUp:
                case BattleConst.DigestType.Special:
                    return false;
            }
            
            return false;
        }
        
        public virtual async UniTask PlayAsync(BattleDigestLog digestLog, CancellationToken token, Action finishAction, Action _middleAction = null)
        {
            // 敵側AceCharacterのどうでもいい系MatchUp演出はスキップ
            if (digestLog.Type == BattleConst.DigestType.MatchUp &&
                digestLog.OffenceAbilityId <= 0 &&
                digestLog.MainCharacterData.Side == BattleDataMediator.Instance.EnemySide)
            {
                finishAction?.Invoke();
                return;
            }

            token.ThrowIfCancellationRequested();
            middleAction = _middleAction;
            if (digestLog.Type != BattleConst.DigestType.None)
            {
                await PlayAsync(digestLog.Type, digestLog.OffenceSide, digestLog.MainCharacterData, digestLog.OtherCharacterDataList, digestLog.Score, digestLog.DistanceToGoal, digestLog.IsLastScoreToEnd);
            }
            // TODO オブジェクト破棄で演出停止されてfinishActionが呼ばれるとコールバック先でエラー吐くので必要に応じてキャンセル

            if (digestLog.Type is  BattleConst.DigestType.Goal or BattleConst.DigestType.ThrowIn or BattleConst.DigestType.ThrowInKeeper &&
                BattleDataMediator.Instance.IsReleaseAsset)
            {
                Release();
                await Resources.UnloadUnusedAssets().ToUniTask();
                GC.Collect();
            }
            
            finishAction?.Invoke();
        }
        
        public async UniTask PlayAsync(BattleConst.DigestType type, BattleConst.TeamSide offenceSide, 
            BattleDigestCharacterData mainCharacterData, List<BattleDigestCharacterData> otherCharacterDataList, List<int> score, int distanceToGoal, bool isLastScoreToEnd = false, long abilityId = -1)
        {
            type = GetSideRelatedDigestType(BattleDataMediator.Instance.PlayerSide != BattleConst.TeamSide.Left, type);
            
            // 今再生してるやつを止める
            CruFramework.Logger.Log("PlayCutScene :" + type);
            Stop();

            if (type == BattleConst.DigestType.None || type == BattleConst.DigestType.Max || type == BattleConst.DigestType.Stop) return;

            // ドリブルが入るタイミングで一連の流れは終了したものとみなす
            if (type is BattleConst.DigestType.DribbleL or BattleConst.DigestType.DribbleR)
            {
                forceFlatPlaySpeed = false;
            }
            
            // キャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            // 読み込み
            string key = GetAddress(type, mainCharacterData);
            await resourcesLoader.LoadAssetAsync<GameObject>(key,
                obj =>
                {
                    if (obj == null)
                    {
                        return;
                    }
                    var assetObj = Instantiate(obj, AppManager.Instance.WorldManager.RootTransform, false);
                    _currentDigest = assetObj.GetComponent<BattleDigestObject>();
                },
                source.Token
            );
            if (_currentDigest == null)
            {
                CruFramework.Logger.LogWarning("演出を読み込めませんでした");
                return;
            }
            // 初期化
            _currentDigest.gameObject.SetActive(false);
            _currentDigest.Init(BattleUIMediator.Instance?.DialogueUi?.Animator, OnDigestMiddleSignalCallback);

            // ダイジェスト演出はメインのキャラとそれ以外(0人以上）で構成され
            // メインキャラからセリフ等が開始される
            // 誰がメインキャラになるかは各演出による
            
            // SpineやH2MD等のデータを設定
            var isPlayerAce = mainCharacterData != null && mainCharacterData.Side == BattleDataMediator.Instance.PlayerSide && mainCharacterData.IsAce;
            var speed = GetDigestPlaySpeed(type, mainCharacterData, otherCharacterDataList, offenceSide);
            var isShowBlackLayer = IsShowBlackLayer(type, isPlayerAce);
            var isShowDialogPhrase = !isShowBlackLayer && speed <= 2.0f;
            var isShowRadar = IsShowRadar(type);
            var isPlayDialogVoice = speed == 1.0f;
            await _currentDigest.SetObjectData(resourcesLoader, type, offenceSide, mainCharacterData, otherCharacterDataList, score, distanceToGoal, speed);

            // セリフ設定
            var data = CreateBattleDialogueDataList(type, offenceSide, mainCharacterData, otherCharacterDataList, isShowDialogPhrase);
            if (type == BattleConst.DigestType.KickOffL || type == BattleConst.DigestType.KickOffR)
            {
                BattleUIMediator.Instance.RadarUI.ResetPosition();
            }

            BattleUIMediator.Instance?.DialogueUi?.SetDialogueData(data, isPlayDialogVoice);
            BattleUIMediator.Instance?.BlackOverLay?.SetActive(isShowBlackLayer && !isPlayDialogVoice);
            BattleUIMediator.Instance?.RadarUI?.SetActive(isShowRadar);

            forceQuitAnimDirector = false;
            isDoneAnimDirector = false;
            _currentDigest.OnRootDirectorStoppedAction += OnDigestStopped;
            _currentDigest.gameObject.SetActive(true);
            _currentDigest.Play();
            
            OnLoadedAction(type);
            
            BattleUIMediator.Instance?.SetVisibleLowerElements(type != BattleConst.DigestType.Special);
            
#if !PJFB_REL
            await UniTask.WaitUntil(() => !BattleDataMediator.Instance.StopLogPlay &&
                                          (forceQuitAnimDirector || isDoneAnimDirector && BattleUIMediator.Instance.DialogueUi.IsEndAllVoice));
#endif
            
            await UniTask.WaitUntil(() => forceQuitAnimDirector ||
                                          isDoneAnimDirector && BattleUIMediator.Instance.DialogueUi.IsEndAllVoice);
            
            if (type == BattleConst.DigestType.Special)
            {
                BattleUIMediator.Instance?.SetVisibleLowerElements(true);
            }
        }

#if !PJFB_REL
        public async UniTask DebugPlayAsync(BattleConst.DigestType type, BattleConst.TeamSide offenceSide,
            BattleDigestCharacterData mainCharacterData, List<BattleDigestCharacterData> otherCharacterDataList,
            List<int> score, int distanceToGoal, bool isLastScoreToEnd = false, long abilityId = -1, Action middleAction = null)
        {
            mainCharacterData.Side = offenceSide;
            mainCharacterData.IsAce = true;
            this.middleAction = middleAction;
            await PlayAsync(type, offenceSide, mainCharacterData, otherCharacterDataList, score, distanceToGoal, isLastScoreToEnd, abilityId);
        }
#endif

        protected virtual List<BattleDialogueData> CreateBattleDialogueDataList(BattleConst.DigestType type, BattleConst.TeamSide offenceSide, BattleDigestCharacterData mainCharacterData, List<BattleDigestCharacterData> otherCharacterDataList, bool isShowDialogPhrase)
        {
            var dialogueDataList = new List<BattleDialogueData>();
            if (mainCharacterData == null)
            {
                return dialogueDataList;
            }
            
            if (!isShowDialogPhrase || BattleDataMediator.Instance.IsSpeedUpPlayerDigest)
            {
                return dialogueDataList;
            }

            CharaMasterObject firstChara = MasterManager.Instance.charaMaster.FindData(mainCharacterData.MCharaId);
            var otherCharacterData = otherCharacterDataList.Count > 0 ? otherCharacterDataList[0] : null;
            var firstAbilityId = mainCharacterData.AbilityId;
            CharaMasterObject secondChara = otherCharacterData != null ? MasterManager.Instance.charaMaster.FindData(otherCharacterData.MCharaId) : null;
            var locationTypeList = GetCharaVoiceLocationTypeList(type, mainCharacterData, otherCharacterDataList);
            var secondAbilityId = otherCharacterData?.AbilityId ?? -1;
            var side = BattleDataMediator.Instance.PlayerSide;
            
            // セリフデータは２つでセットになっている
            for(int i = 0; i < locationTypeList.Count ;i += 2)
            {
                var firstType = locationTypeList[i];
                var secondType = locationTypeList.Count > i+1 ? locationTypeList[i + 1] : CharaVoiceLocationType.None;
                var pairData = MasterManager.Instance.charaLibraryVoiceMaster.GetPairData(firstType,
                    firstChara, firstAbilityId, secondType, secondChara, secondAbilityId);
                if (pairData == null)
                {
                    continue;
                }

                for(int j = 0; j < pairData.Length ;j++)
                {
                    var dialogueData = new BattleDialogueData();
                    dialogueData.master = pairData[j];
                    if (pairData[j] == null)
                    {
                        dialogueDataList.Add(dialogueData);
                        continue;
                    }
                    
                    if (j % 2 == 0)
                    {
                        // １人目のキャラ
                        dialogueData.charaId = mainCharacterData.MCharaId;
                        dialogueData.isPlayer = mainCharacterData.Side == side;
                    }
                    else
                    {
                        // ２人目のキャラ
                        if (otherCharacterData != null)
                        {
                            dialogueData.charaId = otherCharacterData.MCharaId;
                            dialogueData.isPlayer = otherCharacterData.Side == side;
                        }
                    }
                    
                    dialogueDataList.Add(dialogueData);
                }
            }

            return dialogueDataList;
        }

        private List<CharaVoiceLocationType> GetCharaVoiceLocationTypeList(BattleConst.DigestType type, BattleDigestCharacterData mainCharacterData, List<BattleDigestCharacterData> otherCharacterDataList)
        {
            List<CharaVoiceLocationType> locationTypeList = new List<CharaVoiceLocationType>();

            switch (type)
            {
                case BattleConst.DigestType.KickOffL:
                case BattleConst.DigestType.KickOffR:
                    locationTypeList.Add(CharaVoiceLocationType.KickOffOut);
                    locationTypeList.Add(CharaVoiceLocationType.KickOffIn);
                    break;
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    // スタミナ判定
                    var locationType = CharaVoiceLocationType.DribbleOwnerGood;
                    int index = 0;
                    for(; index < dribbleLocationTypeByStaminaRatio.Length ; index++)
                    {
                        if (mainCharacterData.StaminaRatio < dribbleLocationTypeByStaminaRatio[index])
                        {
                            break;
                        }
                    }
                    if (dribbleLocationTypes.Length > index)
                    {
                        locationType = dribbleLocationTypes[index];
                    }
                    
                    locationTypeList.Add(locationType);
                    if (otherCharacterDataList.Count > 0)
                    {
                        locationTypeList.Add( mainCharacterData.Side == otherCharacterDataList[0].Side ? CharaVoiceLocationType.DribbleSupporter : CharaVoiceLocationType.DribbleMaker);
                    }
                    break;
                case BattleConst.DigestType.MatchUp:
                    locationTypeList.Add(CharaVoiceLocationType.OnMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.OnMatchUpDefence);
                    break;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                    locationTypeList.Add(CharaVoiceLocationType.TechnicMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.TechnicMatchUpDefence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpWinOffence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpLoseDefence);
                    break;
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                    locationTypeList.Add(CharaVoiceLocationType.TechnicMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.TechnicMatchUpDefence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpLoseOffence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpWinDefence);
                    break;
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                    locationTypeList.Add(CharaVoiceLocationType.PhysicalMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.PhysicalMatchUpDefence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpWinOffence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpLoseDefence);
                    break;
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                    locationTypeList.Add(CharaVoiceLocationType.PhysicalMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.PhysicalMatchUpDefence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpLoseOffence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpWinDefence);
                    break;
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                    locationTypeList.Add(CharaVoiceLocationType.SpeedMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.SpeedMatchUpDefence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpWinOffence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpLoseDefence);
                    break;
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                    locationTypeList.Add(CharaVoiceLocationType.SpeedMatchUpOffence);
                    locationTypeList.Add(CharaVoiceLocationType.SpeedMatchUpDefence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpLoseOffence);
                    locationTypeList.Add(CharaVoiceLocationType.MatchUpWinDefence);
                    break;
                case BattleConst.DigestType.Cross:
                    locationTypeList.Add(CharaVoiceLocationType.Cross);
                    break;
                case BattleConst.DigestType.PassFailed:
                    locationTypeList.Add(CharaVoiceLocationType.PassOut);
                    break;
                case BattleConst.DigestType.PassSuccess:
                    locationTypeList.Add(CharaVoiceLocationType.PassOut);
                    locationTypeList.Add(CharaVoiceLocationType.PassIn);
                    break;
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
                    locationTypeList.Add(CharaVoiceLocationType.PassCut);
                    break;
                case BattleConst.DigestType.Shoot:
                    locationTypeList.Add(CharaVoiceLocationType.Shoot);
                    break;
                case BattleConst.DigestType.ShootBlockL:
                case BattleConst.DigestType.ShootBlockR:
                    locationTypeList.Add(CharaVoiceLocationType.ShootBlockReady);
                    locationTypeList.Add(CharaVoiceLocationType.None);
                    locationTypeList.Add(CharaVoiceLocationType.ShootBlockCatched);
                    break;
                case BattleConst.DigestType.ShootBlockTouchL:
                case BattleConst.DigestType.ShootBlockTouchR:
                    locationTypeList.Add(CharaVoiceLocationType.ShootBlockReady);
                    locationTypeList.Add(CharaVoiceLocationType.None);
                    locationTypeList.Add(CharaVoiceLocationType.ShootBlockTouched);
                    break;
                case BattleConst.DigestType.ShootBlockNotReachL:
                case BattleConst.DigestType.ShootBlockNotReachR:
                    locationTypeList.Add(CharaVoiceLocationType.ShootBlockReady);
                    locationTypeList.Add(CharaVoiceLocationType.None);
                    locationTypeList.Add(CharaVoiceLocationType.ShootBlockNotReach);
                    break;
                case BattleConst.DigestType.ShootResultSuccessL:
                case BattleConst.DigestType.ShootResultSuccessR:
                case BattleConst.DigestType.ShootResultPunchL:
                case BattleConst.DigestType.ShootResultPunchR:
                case BattleConst.DigestType.ShootResultPostL:
                case BattleConst.DigestType.ShootResultPostR:
                case BattleConst.DigestType.ShootResultCatch:
                    // nothing
                    break;
                case BattleConst.DigestType.Goal:
                    locationTypeList.Add(CharaVoiceLocationType.GoalOwner);
                    break;
                case BattleConst.DigestType.Goal_GameSet:
                    // nothing
                    break;
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                    locationTypeList.Add(CharaVoiceLocationType.SecondBallGet);
                    locationTypeList.Add(CharaVoiceLocationType.SecondBallFail);
                    break;
                case BattleConst.DigestType.OutBall:
                    // nothing
                    break;
                case BattleConst.DigestType.ThrowIn:
                    locationTypeList.Add(CharaVoiceLocationType.ThrowIn);
                    break;
                case BattleConst.DigestType.ThrowInKeeper:
                    // nothing
                    break;
                case BattleConst.DigestType.TimeUp:
                    // nothing
                    break;
                case BattleConst.DigestType.Special:
                    locationTypeList.Add(CharaVoiceLocationType.SpecialAbility);
                    locationTypeList.Add(CharaVoiceLocationType.None);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            return locationTypeList;
        }

        void OnDigestStopped(PlayableDirector aDirector)
        {
            CruFramework.Logger.Log("OnDigestStopped");
            isDoneAnimDirector = true;
            _currentDigest.OnRootDirectorStoppedAction -= OnDigestStopped;
        }
        
        void OnDigestMiddleSignalCallback()
        {
            CruFramework.Logger.Log("OnDigestMiddleSignalCallback");
            middleAction?.Invoke();
        }

        private void Stop(bool isDestroy = true)
        {
            if (_currentDigest == null)
            {
                return;
            }
            
            _currentDigest.Stop();
            if (isDestroy)
            {
                Destroy(_currentDigest.gameObject);
            }
            else
            {
                _currentDigest.gameObject.SetActive(false);
            }
            _currentDigest = null;
        }
        
        protected virtual string GetAddress(BattleConst.DigestType type, BattleDigestCharacterData mainCharacterData)
        {
            string file = "";
            switch (type)
            {
                case BattleConst.DigestType.KickOffL:
                    file = "KickOff_L";
                    break;
                case BattleConst.DigestType.KickOffR:
                    file = "KickOff_R";
                    break;
                case BattleConst.DigestType.DribbleL:
                    file = "Dribble_L";
                    break;
                case BattleConst.DigestType.DribbleR:
                    file = "Dribble_R";
                    break;
                case BattleConst.DigestType.MatchUp:
                    file = "MatchupSelect";
                    break;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                    file = "TechniqueMatchup_win_L";
                    break;
                case BattleConst.DigestType.TechnicMatchUpWinR:
                    file = "TechniqueMatchup_win_R";
                    break;
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                    file = "TechniqueMatchup_lose_L";
                    break;
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                    file = "TechniqueMatchup_lose_R";
                    break;
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                    file = "PhysicalMatchup_win_L";
                    break;
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                    file = "PhysicalMatchup_win_R";
                    break;
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                    file = "PhysicalMatchup_lose_L";
                    break;
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                    file = "PhysicalMatchup_lose_R";
                    break;
                case BattleConst.DigestType.Cross:
                    file = "Cross";
                    break;
                case BattleConst.DigestType.PassFailed:
                    file = "Short-pass";
                    break;
                case BattleConst.DigestType.PassSuccess:
                    file = "Short-pass";
                    break;
                case BattleConst.DigestType.PassCutBlock:
                    file = "Pass-cut";
                    break;
                case BattleConst.DigestType.PassCutCatch:
                    file = "Pass-cut_keep";
                    break;
                case BattleConst.DigestType.Shoot:
                    file = "Shoot";
                    break;
                case BattleConst.DigestType.ShootBlockL:
                    file = "Shoot-block_L_b";
                    break;
                case BattleConst.DigestType.ShootBlockR:
                    file = "Shoot-block_R_b";
                    break;
                case BattleConst.DigestType.ShootBlockTouchL:
                    file = "Shoot-block_L_b_scratch";
                    break;
                case BattleConst.DigestType.ShootBlockTouchR:
                    file = "Shoot-block_R_b_scratch";
                    break;
                case BattleConst.DigestType.ShootBlockNotReachL:
                    file = "Shoot-block_L_b_Not_reach";
                    break;
                case BattleConst.DigestType.ShootBlockNotReachR:
                    file = "Shoot-block_R_b_Not_reach";
                    break;
                case BattleConst.DigestType.ShootResultSuccessL:
                    file = "G-Shoot_L";
                    break;
                case BattleConst.DigestType.ShootResultSuccessR:
                    file = "G-Shoot_R";
                    break;
                case BattleConst.DigestType.ShootResultPunchL:
                    file = "G-Shoot_L_punching";
                    break;
                case BattleConst.DigestType.ShootResultPunchR:
                    file = "G-Shoot_R_punching";
                    break;
                case BattleConst.DigestType.ShootResultPostL:
                    file = "G-Shoot_Goal_post_L";
                    break;
                case BattleConst.DigestType.ShootResultPostR:
                    file = "G-Shoot_Goal_post_R";
                    break;
                case BattleConst.DigestType.ShootResultCatch:
                    file = "G-Shoot_BLM_catch";
                    break;
                case BattleConst.DigestType.Goal:
                    file = "Goal";
                    break;
                case BattleConst.DigestType.Goal_GameSet:
                    file = "Goal_GameSet";
                    break;
                case BattleConst.DigestType.SecondBall2:
                    file = "Second-ball_Cutin2";
                    break;
                case BattleConst.DigestType.SecondBall3:
                    file = "Second-ball_Cutin3";
                    break;
                case BattleConst.DigestType.SecondBall4:
                    file = "Second-ball_Cutin4";
                    break;
                case BattleConst.DigestType.OutBall:
                    file = "Ball-out";
                    break;
                case BattleConst.DigestType.ThrowIn:
                    file = "Throw-in";
                    break;
                case BattleConst.DigestType.ThrowInKeeper:
                    file = "Throw-in_BLM";
                    break;
                case BattleConst.DigestType.TimeUp:
                    file = "Goal_TimeUP";
                    break;
                case BattleConst.DigestType.Special:
                    file = $"{mainCharacterData.AbilityId}_Skill";
                    break;
            }

            // Prefabs/InGame/KickOff_L.prefab
            var address = $"Prefabs/InGame/{file}.prefab";
            
            CruFramework.Logger.Log(address);
            return address;
        }

        public void SetCurrentDigestTimeAtStartAbilityDirectionTime()
        {
            _currentDigest.SetTimelineTimeAtStartAbilityDirection();
        }

        public void ForceQuitCurrentDigest()
        {
            forceQuitAnimDirector = true;
        }
        
        public void Pause()
        {
            _currentDigest?.Pause();
        }

        public void Resume()
        {
            _currentDigest?.Resume();
        }

        protected void Release()
        {
            resourcesLoader.Release();
        }
        
        /// <summary>キャンセル</summary>
        private void Cancel()
        {
            // キャンセル
            if(source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }

        private void OnDestroy()
        {
            // ゲームオブジェクト削除時にキャンセル
            Cancel();
            Stop();
            Release();
        }

        protected virtual void OnLoadedAction(BattleConst.DigestType type)
        {
            
        }

        private BattleConst.DigestType GetSideRelatedDigestType(bool doInverse, BattleConst.DigestType digestType)
        {
            if (!doInverse)
            {
                return digestType;
            }
            
            switch (digestType)
            {
                case BattleConst.DigestType.KickOffL:
                    return BattleConst.DigestType.KickOffR;
                case BattleConst.DigestType.KickOffR:
                    return BattleConst.DigestType.KickOffL;
                case BattleConst.DigestType.DribbleL:
                    return BattleConst.DigestType.DribbleR;
                case BattleConst.DigestType.DribbleR:
                    return BattleConst.DigestType.DribbleL;
                case BattleConst.DigestType.TechnicMatchUpWinL:
                    return BattleConst.DigestType.TechnicMatchUpWinR;
                case BattleConst.DigestType.TechnicMatchUpWinR:
                    return BattleConst.DigestType.TechnicMatchUpWinL;
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                    return BattleConst.DigestType.TechnicMatchUpLoseR;
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                    return BattleConst.DigestType.TechnicMatchUpLoseR;
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                    return BattleConst.DigestType.PhysicalMatchUpWinR;
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                    return BattleConst.DigestType.PhysicalMatchUpWinL;
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                    return BattleConst.DigestType.PhysicalMatchUpLoseR;
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                    return BattleConst.DigestType.PhysicalMatchUpLoseL;
                case BattleConst.DigestType.SpeedMatchUpWinL:
                    return BattleConst.DigestType.SpeedMatchUpWinR;
                case BattleConst.DigestType.SpeedMatchUpWinR:
                    return BattleConst.DigestType.SpeedMatchUpWinL;
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                    return BattleConst.DigestType.SpeedMatchUpLoseR;
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                    return BattleConst.DigestType.SpeedMatchUpLoseL;
            }

            return digestType;
        }

    }

}