using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using CruFramework.Audio;
using CruFramework.ResourceManagement;
using CruFramework.Timeline;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using Pjfb.Voice;
using Spine.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.VFX;

namespace Pjfb.InGame
{
    public class BattleDigestObject : MonoBehaviour
    {
        [SerializeField] private PlayableDirector rootDirector;
        [SerializeField] private BattleDigestObjectSpine[] offenceChara;
        [SerializeField] private BattleDigestObjectSpine[] defenceChara;
        [SerializeField] private BattleDigestObjectH2MD h2mdObject;
        [SerializeField] private BattleDigestObjectScore scoreObject;
        [SerializeField] private BattleSecondBallDigest secondBallInfo;
        [SerializeField] private SpriteRenderer groundRenderer;
        [SerializeField] private Sprite[] groundSprites;
        [SerializeField] private GameObject tunnelEffect;
        [SerializeField] private ToggleSprite[] sideShieldingPlates;
        
        public Action<PlayableDirector> OnRootDirectorStoppedAction;
        private Action middleAction;
        private BattleConst.DigestType type;
        private float playSpeed = 1.0f;
        private int calledActivateAbilityChanceCount = 0;
        private double calledSwipeUITime = 0.0f;
        private long abilityId = -1;
        private BattleConst.TeamSide abilityUserSide = BattleConst.TeamSide.Left;

        private BattleDigestObjectSpine[] characterSpines;
        private VisualEffect[] visualEffects;

        private readonly int MaterialPropertyTintColorId = Shader.PropertyToID("_Color");
        private readonly float DefenceCharacterCoveredColorValue = 0.725f;
        private readonly float WarmUpPlaySpeed = 100f;

        private const float AbilityCutInTime = 3.0f;

        private bool isInitialized = false;
        public void Init(Animator dialogueAnimator, Action _middleAction)
        {
            if (isInitialized) return;
            
            // TimeLineTrackに必要オブジェクトのアタッチ
            var directors = gameObject.GetComponentsInChildren<PlayableDirector>();
            foreach (var director in directors)
            {
                foreach (PlayableBinding binding in director.playableAsset.outputs)
                {
                    if (binding.sourceObject == null) continue;
                    if (binding.sourceObject.GetType() == typeof(ShakePlayableTrack))
                    {
                        director.SetGenericBinding(binding.sourceObject, AppManager.Instance.WorldManager.WorldCamera.gameObject);
                    }
                    else if (dialogueAnimator != null && binding.streamName == "DialogTrack")
                    {
                        director.SetGenericBinding(binding.sourceObject, dialogueAnimator);
                    }
                }
            }

            middleAction = _middleAction;
            rootDirector.stopped += OnRootDirectorStoppedCallback;

            characterSpines = gameObject.GetComponentsInChildren<BattleDigestObjectSpine>();
            visualEffects = gameObject.GetComponentsInChildren<VisualEffect>();
        }
        
        public void Play()
        {
            gameObject.SetActive(true);
            
            if (!rootDirector.playableGraph.IsValid())
            {
                rootDirector.RebuildGraph();
            }
            

            var speed = GetPlaySpeed();

            // Spineの再生速度も変動させる
            foreach (var chara in characterSpines)
            {
                // プロパティインジェクションされて1.0以外がセットされてたらドンマイ.
                chara.SetSpineTimeScale(speed);
            }
            
            rootDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
            rootDirector.Play();
        }

        /// <summary>
        /// シェーダー等のウォームアップ用の再生
        /// </summary>
        public async UniTask WarmUpPlayAsync()
        {
            gameObject.SetActive(true);
            if (!rootDirector.playableGraph.IsValid())
            {
                rootDirector.RebuildGraph();
            }

            // レシーバーがあればOFF
            var receiver = GetComponent<SignalReceiver>();
            if (receiver != null)
            {
                while (receiver.Count() > 0)
                {
                    receiver.RemoveAtIndex(0);
                }
            }
            
            // 音鳴らさない
            var directors = gameObject.GetComponentsInChildren<PlayableDirector>();
            foreach (var director in directors)
            {
                IEnumerable<TrackAsset> tracks = ((TimelineAsset)director.playableAsset).GetOutputTracks();
                foreach (TrackAsset track in tracks)
                {
                    if (track.GetType() != typeof(AudioTrack)) continue;

                    track.muted = true;
                }
            }
     
            // 適当に高速再生
            rootDirector.playableGraph.GetRootPlayable(0).SetSpeed(WarmUpPlaySpeed);
            
            bool isDone = false;
            rootDirector.stopped += _=> isDone = true;
            rootDirector.Play();
            await UniTask.WaitUntil(()=>isDone);
        }

        public void Stop()
        {
            rootDirector.Stop();
            gameObject.SetActive(false);
        }
        
        public void Pause()
        {
            rootDirector.Pause();
            foreach (var chara in characterSpines)
            {
                chara.SetSpineTimeScale(0.0f);
            }
            
            foreach (var vfx in visualEffects)
            {
                vfx.transform.localScale = Vector3.zero;
            }
        }

        public void Resume()
        {
            rootDirector.Resume();
            var playSpeed = GetPlaySpeed();
            foreach (var chara in characterSpines)
            {
                // プロパティインジェクションされて1.0以外がセットされてたらドンマイ.
                chara.SetSpineTimeScale(playSpeed);
            }
            
            foreach (var vfx in visualEffects)
            {
                vfx.transform.localScale = Vector3.one;
            }
        }

        public async UniTask SetObjectData(ResourcesLoader resourcesLoader, BattleConst.DigestType _type,
            BattleConst.TeamSide offenceSide, BattleDigestCharacterData mainCharacterData,
            List<BattleDigestCharacterData> otherCharacterDataList, List<int> score, int distanceToGoal, float _playSpeed)
        {
            type = _type;

            playSpeed = _playSpeed;
            abilityId = mainCharacterData?.AbilityId ?? -1;
            abilityUserSide = mainCharacterData?.Side ?? BattleConst.TeamSide.Left;
            if (type == BattleConst.DigestType.MatchUp)
            {
                SetGroundSprite(distanceToGoal);
                tunnelEffect.SetActive(true);
                SetDefenceCharacterMaterialColor(1.0f, 0.0f).Forget();
            }
            
            await SetSkeletonDataAssetAsync(resourcesLoader, type, offenceSide, mainCharacterData, otherCharacterDataList.Count > 0 ? otherCharacterDataList[0] : null);
            await SetSpriteAsync(resourcesLoader, type, mainCharacterData, otherCharacterDataList);
            await SetH2MDAssetAsync(resourcesLoader, type, mainCharacterData);
            await SetScoreAsync(resourcesLoader, offenceSide, score);
            
            // 遮蔽版の色変え
            foreach (var plate in sideShieldingPlates)
            {
                int side;
                if (type == BattleConst.DigestType.ThrowInKeeper)
                {
                    // キーパーのスローインの場合はmainCharacterがいないので現在のoffenceSideから判定
                    side = offenceSide == BattleDataMediator.Instance.PlayerSide ? 1 : 0;
                }
                else
                {
                    // 現在の演出のメインサイドの色に
                    side = mainCharacterData.Side == BattleDataMediator.Instance.PlayerSide ? 0 : 1;
                }

                plate.Value = side;
            }
        }

        private async UniTask SetDefenceCharacterMaterialColor(float colorValue, float duration)
        {
            // 一応事後処理してないから同じマテリアルを他所で使われたら「なんで黒くなってるんだ？」って言われる原因にはなりえる.
            var target = defenceChara.FirstOrDefault();
            if (target == null)
            {
                return;
            }

            var renderer = target.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                return;
            }

            var materials = renderer.materials.Where(m => m.HasProperty(MaterialPropertyTintColorId)).ToList();

            if (materials.Count <= 0)
            {
                return;
            }

            var dt = 0.0f;
            var diff = 1.0f - colorValue;
            while (dt < duration)
            {
                dt += Time.deltaTime;
                var ratio = dt / duration;
                // 上方向のfadeは対応してないけどまあ使わないので.
                var c = 1.0f - (diff * ratio);
                Debug.Log(c);
                foreach (var material in materials)
                {
                    material.SetColor(MaterialPropertyTintColorId, new Color(c, c, c, 1.0f));
                }
                await UniTask.Yield();
            }
            
            foreach (var material in materials)
            {
                material.SetColor(MaterialPropertyTintColorId, new Color(colorValue, colorValue, colorValue, 1.0f));
            }
        }

        private void SetGroundSprite(int distanceToGoal)
        {
            var index = BattleGameLogic.GetGroundSpriteIndex(distanceToGoal);
            if (groundSprites.Length > index)
            {
                groundRenderer.sprite = groundSprites[index];
            }
        }
        
        private void SetGroundSpriteColorAsBlack()
        {
            if (groundRenderer == null)
            {
                return;
            }

            groundRenderer.color = new Color32(75, 75, 75, 255);
        }

        private async UniTask SetSkeletonDataAssetAsync(ResourcesLoader resourcesLoader, BattleConst.DigestType type, BattleConst.TeamSide offenceSide, 
            BattleDigestCharacterData firstCharaData, BattleDigestCharacterData secondCharaData)
        {
            if (firstCharaData == null)
            {
                return;
            }

            var playerSide = BattleDataMediator.Instance.PlayerSide;
            
            foreach (var chara in offenceChara)
            {
                await chara.SetSkeletonDataAssetAsync(resourcesLoader, type, firstCharaData.MCharaId, offenceSide == firstCharaData.Side, firstCharaData.Side == playerSide);
            }

            if (secondCharaData == null)
            {
                return;
            }
            foreach (var chara in defenceChara)
            {
                await chara.SetSkeletonDataAssetAsync(resourcesLoader, type, secondCharaData.MCharaId, offenceSide == secondCharaData.Side, secondCharaData.Side == playerSide);
            }
        }
        
        private async UniTask SetSpriteAsync(ResourcesLoader resourcesLoader, BattleConst.DigestType type, BattleDigestCharacterData mainCharaData, List<BattleDigestCharacterData> otherCharaDataList)
        {
            if (secondBallInfo == null)
            {
                return;
            }
            
            await secondBallInfo.SetSpriteAsync(resourcesLoader, type, mainCharaData, otherCharaDataList);
        }
        
        private async UniTask SetH2MDAssetAsync(ResourcesLoader resourcesLoader, BattleConst.DigestType type, BattleDigestCharacterData characterData)
        {
            if (h2mdObject == null || characterData == null)
            {
                return;
            }

            await h2mdObject.SetH2MDAssetAsync(resourcesLoader, type, characterData, characterData.Side == BattleDataMediator.Instance.PlayerSide);
        }

        private async UniTask SetScoreAsync(ResourcesLoader resourcesLoader, BattleConst.TeamSide offenceSide, List<int> score)
        {
            if (scoreObject == null)
            {
                return;
            }
            await scoreObject.SetScoreAsync(resourcesLoader, offenceSide, score);
        }

        private void OnRootDirectorStoppedCallback(PlayableDirector director)
        {
            // w
            if (type == BattleConst.DigestType.MatchUp)
            {
                SetDefenceCharacterMaterialColor(DefenceCharacterCoveredColorValue, 0.1f).Forget();
                AudioManager.Instance.SetVolume(AudioGroup.BGM, BattleDataMediator.Instance.DefaultBgmVolume / 10.0f);
            }
            
            rootDirector.stopped -= OnRootDirectorStoppedCallback;
            OnRootDirectorStoppedAction?.Invoke(director);
        }
        
        public void OnRootDirectorMiddleSignalCallback()
        {
            // w
            if (type == BattleConst.DigestType.MatchUp)
            {
                SetGroundSpriteColorAsBlack();
                SetDefenceCharacterMaterialColor(DefenceCharacterCoveredColorValue, 0.1f).Forget();
                AudioManager.Instance.SetVolume(AudioGroup.BGM, BattleDataMediator.Instance.DefaultBgmVolume / 20.0f);
            }

            middleAction?.Invoke();
        }

        /// <summary>
        /// called by timeline event
        /// </summary>
        public void OnStartOpenDialog()
        {
            BattleUIMediator.Instance.DialogueUi.PlayDialog();
        }
        
        /// <summary>
        /// called by timeline event
        /// </summary>
        public void OnStartCloseDialog()
        {
            if (!BattleUIMediator.Instance.DialogueUi.IsEndAllVoice)
            {
                rootDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
                BattleUIMediator.Instance.DialogueUi.OnEndCallback = () =>
                {
                    rootDirector.playableGraph.GetRootPlayable(0).SetSpeed(GetPlaySpeed());
                };
            }
        }

        public void OnActivateAbility()
        {
            calledActivateAbilityChanceCount++;
            
            if (BattleDataMediator.Instance.NextMatchUpResult == null)
            {
                return;
            }
            
            if (BattleDataMediator.Instance.NextMatchUpResult.OffenceAbilityId <= 0)
            {
                return;
            }

            var abilityTiming = BattleDataMediator.Instance.NextMatchUpResult.ActivateAbilityTimingType;
            // シュートのみクロス -> シュート の導線で来る可能性があり, クロスのタイミングで発火してしまうことがあるためはじく
            if (abilityTiming == BattleConst.AbilityEvaluateTimingType.ActiveSelectShootOF &&
                type != BattleConst.DigestType.Shoot)
            {
                return;
            }

            // パス/クロス演出は演出直後の発火タイミング(=パス/クロス打ち上げ必殺技)と演出終盤の発火タイミング(=パス/クロス受け取り時専用シュート)があるためindexで出し分け
            if (abilityTiming is BattleConst.AbilityEvaluateTimingType.ActiveReceivePass or BattleConst.AbilityEvaluateTimingType.ActiveReceiveCross &&
                calledActivateAbilityChanceCount <= 1)
            {
                return;
            }

            Pause();
            
            BattleEventDispatcher.Instance.OnActivateActiveAbilityCallback();
        }

        public void OnActivateAbilitySwipeUI()
        {
            calledSwipeUITime = rootDirector.time;
            BattleEventDispatcher.Instance?.OnActivateUseAbilityUICallback(abilityId, abilityUserSide == BattleConst.TeamSide.Right);
        }

        public void SetTimelineTimeAtStartAbilityDirection()
        {
            var startAbilityDirectionTime = calledSwipeUITime + AbilityCutInTime;
            
            var charaLibraryVoice = MasterManager.Instance.charaLibraryVoiceMaster.GetDataByAbilityId(abilityId);
            if (charaLibraryVoice != null)
            {
                VoiceManager.Instance.PlayVoiceForCharaLibraryVoiceAsync(charaLibraryVoice).Forget();
            }
            
            if (rootDirector.time >= startAbilityDirectionTime)
            {
                return;
            }
            
            rootDirector.time = startAbilityDirectionTime;
        }

        private float GetPlaySpeed()
        {
            // しぬほどバグるので必殺技は強制1倍
            if (type == BattleConst.DigestType.Special)
            {
                return playSpeed;
            }
            
            var baseSpeed = BattleDataMediator.Instance.PlaySpeed;
            return baseSpeed * playSpeed;
            /*
            if (BattleDataMediator.Instance.IsSpeedUpPlayerDigest)
            {
                return baseSpeed * BattleConst.AdjustableValueNonDigestPlaySpeed;
            }

            if (isNormalSpeedDigest)
            {
                return baseSpeed;
            }
            
            var coef = isNormalSpeedDigest ? 1.0f : BattleConst.AdjustableValueNonDigestPlaySpeed;
            return baseSpeed * coef;
            */
        }
    }
}