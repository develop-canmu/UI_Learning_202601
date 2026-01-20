using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using System;
using CruFramework;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

using Pjfb.Master;
using Pjfb.Training;
using Pjfb.Voice;
using Pjfb.Utility;
using System.Linq;

namespace Pjfb
{
    public class TrainingEventResultTrainingScene : MonoBehaviour
    {
        /// <summary>アニメーション名</summary>
        private static readonly string OpenAnimation = "OpenNormalTraining";
        /// <summary>アニメーション名</summary>
        private static readonly string OpenSpecialAnimation = "OpenSpecialTraining";
        /// <summary>アニメーション名</summary>
        private static readonly string CloseAnimation = "Close";
        
        
        [SerializeField]
        private Animator animator = null;
        
        [SerializeField]
        private Image backgroundImage = null;
        [SerializeField]
        private Image nameImage = null;
        
        // エフェクト表示ルートオブジェクト
        [SerializeField]
        private RectTransform effectRoot = null;
        
        private float currentPlaySpeed = 0;
        private long currentPracticeType = 0;

        // FlowZone用エフェクト
        private TrainingFlowZonePracticeEffect flowZonePracticeEffect = null;
        
        public async UniTask LoadResource(TrainingMainArguments args, bool isSkip)
        {
            currentPracticeType = -1;
            
            if(isSkip)return;
            
            // 3D演出あり
            if(TrainingUtility.IsPlay3DEffect)
            {
                await PreLoad3DCompanions(args);
                var root = TagHelperUtility.GetHelper(TagHelperUtility.GroupEnum.Default, "3DTrainingRoot");
                long practiceType = MasterManager.Instance.trainingCardMaster.FindData(args.TrainingCardId).practiceType;
                var directorPrefab = await ObjectPoolUtility.GetPlayableDirectorOnly((ObjectPoolUtility.TrainingTypeEnum)practiceType);
                args.activeTimeline = Instantiate(directorPrefab, root.transform, false);
                await Bind3DModels(args.JoinSupportCharacters, args.Pending.mCharaId, practiceType);

                // Flow用エフェクト
                if (args.IsFlow())
                {
                    if (flowZonePracticeEffect != null)
                    {
                        Destroy(flowZonePracticeEffect.gameObject);
                        flowZonePracticeEffect = null;
                    }

                    // 先に再生エフェクトを読み込んどく
                    await PageResourceLoadUtility.LoadAssetAsync<TrainingFlowZonePracticeEffect>(ResourcePathManager.GetPath("TrainingFlowZonePracticeEffect"),
                        effect =>
                        {
                            flowZonePracticeEffect = Instantiate(effect, effectRoot);
                            flowZonePracticeEffect.gameObject.SetActive(false);
                        }, gameObject.GetCancellationTokenOnDestroy());

                    // エフェクトセット
                    await flowZonePracticeEffect.SetEffectAsync(args.GetConcentrationEffectId());
                }
            }
            // 2D演出
            else
            {
                // マスタ
                TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(args.TrainingCardId);
                long nameImageId = mCard.nameImageId;
                long backgroundId = mCard.backgroundId;
                
                await PageResourceLoadUtility.LoadAssetAsync<Sprite>( ResourcePathManager.GetPath("TrainingNameImage", nameImageId), (v)=>
                    {
                        nameImage.sprite = v;
                    }, 
                    default);
            
                await PageResourceLoadUtility.LoadAssetAsync<Sprite>( ResourcePathManager.GetPath("TrainingSceneBackground", backgroundId), (v)=>
                    {
                        backgroundImage.sprite = v;
                    }, 
                    default);
            }
        }
        
        protected async UniTask PreLoad3DCompanions(TrainingMainArguments args)
        {
            var layerMask = LayerMask.NameToLayer("3D");
            var defaultID = 10001001;
            var defaultLoModel = await ObjectPoolUtility.GetModel(ObjectPoolUtility.ModelTypeEnum.low,
                defaultID, isCachedOnly:true);
            var defaultHiModel = await ObjectPoolUtility.GetModel(ObjectPoolUtility.ModelTypeEnum.middle,
                defaultID, isCachedOnly:true);
            
            for (int i = 0; i < args.Pending.supportDetailList.Length; i++)
            {
                var cur = args.Pending.supportDetailList[i];
                if (cur.supportType > 2) continue;
                UnityEngine.Object character = default;
                if (i == 0)
                {
                    character = await ObjectPoolUtility.GetModel(ObjectPoolUtility.ModelTypeEnum.middle,
                        cur.mCharaId);
                    if (character == default)
                    {
                        character = Instantiate(defaultHiModel);
                        ObjectPoolUtility.RegisterToPool(ObjectPoolUtility.ModelTypeEnum.middle, cur.mCharaId, character);
                    }
                }
                else
                {
                    if (cur.mCharaId != defaultID)
                    {
                        character = await ObjectPoolUtility.GetModel(ObjectPoolUtility.ModelTypeEnum.low,
                            cur.mCharaId);
                        if (character == default)
                        {
                            character = Instantiate(defaultLoModel);
                            ObjectPoolUtility.RegisterToPool(ObjectPoolUtility.ModelTypeEnum.low, cur.mCharaId, character);
                        }
                    }
                    else
                    {
                        character = defaultLoModel;
                    }
                }

                if (character != default)
                {
                    var fxUtil = (character as GameObject).GetComponent<CharacterEffectsUtility>();
                    if (fxUtil != null)
                    {
                        fxUtil.SetInfo(TagHelperUtility.GroupEnum.VFX, cur.mCharaId.ToString());
                    }
                }
                // await UniTask.Delay(20);
            }
        }
        
        protected async UniTask Bind3DModels(long[] charList, long mCharaId, long practiceType)
        {
            async UniTask animBinding(TrainingDirectorBindingUtility.BindingData.TrackData trackData, string ClipName, ObjectPoolUtility.ModelTypeEnum modelType, long charId)
            {
                var overwriteClip = trackData.OverwriteClips.Find(x => x.Name.Equals(ClipName));
                if (overwriteClip != null)
                {
                    overwriteClip.ClipOverwrite = await ObjectPoolUtility.GetAnimationAssetOnly(
                        modelType,
                        (ObjectPoolUtility.TrainingTypeEnum)practiceType,
                        charId,
                        overwriteClip.ClipTag);
                }
            }

            var layerMask = LayerMask.NameToLayer("3D");
            ObjectPoolUtility.ResetPool();
            var helper =
                TrainingDirectorBindingUtility.GetHelper(TagHelperUtility.GroupEnum.Model3D, $"training_{practiceType}") as
                    TrainingDirectorBindingUtility;
            helper.SetPlaySpeed(speed:1f);
            var model = await ObjectPoolUtility.GetModel(ObjectPoolUtility.ModelTypeEnum.middle, mCharaId, layerMask) as GameObject;

            if (model != null)
            {
                model.transform.SetParent(helper.transform);
                model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                var trackData = helper.GetTrackData($"char_{1}", 0);
                //Bind target
                trackData.target = model;
                //Bind animation
                await animBinding(trackData, "Main", ObjectPoolUtility.ModelTypeEnum.middle, mCharaId);
                await animBinding(trackData, "Slave1", ObjectPoolUtility.ModelTypeEnum.middle, mCharaId);
                helper.SetToBind($"char_{1}", true);

                //Bind FX
                var fxUtils = model.GetComponent<CharacterEffectsUtility>();
                if (fxUtils != null)
                {
                    helper.SetAttachment($"char_{1}", attachmentName => fxUtils.GetAttachmentByName(attachmentName));
                    for (int i = 1; i < 3; i++)
                    {
                        helper.SetToBind($"char_{i + 1}", false);
                    }
                }
                else
                {
                    CruFramework.Logger.LogError(
                        $"[Bind3DModels] No CharacterEffectsUtility settings on model {model.name}");
                }

                if (charList != null)
                {
                    for (int i = 0; i < charList.Length; i++)
                    {
                        model = await ObjectPoolUtility.GetModel(ObjectPoolUtility.ModelTypeEnum.low, charList[i],
                            layerMask) as GameObject;
                        model.transform.SetParent(helper.transform);
                        model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                        //Bind FX
                        fxUtils = model.GetComponent<CharacterEffectsUtility>();
                        if (fxUtils != null)
                        {
                            helper.SetAttachment($"char_{i + 2}",
                                attachmentName => fxUtils.GetAttachmentByName(attachmentName));
                        }
                        else
                        {
                            CruFramework.Logger.LogError(
                                $"[Bind3DModels] No CharacterEffectsUtility settings on model {model.name}");
                        }

                        trackData = helper.GetTrackData($"char_{i + 2}", 0);
                        //Bind model
                        trackData.target = model;

                        //Bind animation
                        await animBinding(trackData, "Main", ObjectPoolUtility.ModelTypeEnum.low, charList[i]);

                        helper.SetToBind($"char_{i + 2}", true);
                    }
                }
            }

            if (helper.ValidateRebind(false)) helper.RebindPlayable();
        }
        
        public void DisplayAuraFX(TrainingMainArguments args)
        {
            if(currentPracticeType < 0)return;
            var trainingChars = new List<long>();
            trainingChars.Add(args.TrainingCharacter.MCharId);
            trainingChars.AddRange(args.SupportCharacterDatas.Select(x => (long)x.MCharId));
            var helpers = CharacterEffectsUtility.GetFXListFromIDs(trainingChars.ToArray());
            foreach (var helper in helpers)
            {
                var fx = helper.GetComponent<CharacterEffectsUtility>();
                fx.Animate("EyeAura", currentPlaySpeed);
                fx.Animate("BodyAura", currentPlaySpeed);
            }
        }

        public void SetSpeed(long bonus)
        {
            if(currentPracticeType < 0)return;
            var helper = (TrainingDirectorBindingUtility)TrainingDirectorBindingUtility.GetHelper(TagHelperUtility.GroupEnum.Model3D, $"training_{currentPracticeType}");
            if(helper != null)
            {
                helper.SetSpeedByBonusValue(bonus);
                currentPlaySpeed = helper.playSpeed;
            }
        }
        
        /// <summary>開く</summary>
        public void PlayOpenAnimation(TrainingMainArguments arguments, Action onFinished)
        {
            long cardId = arguments.TrainingCardId;
            // アクティブに
            gameObject.SetActive(true);
            
            // マスタ
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(cardId);
            // スペシャルカード？
            bool isSpecial = mCard.cardGroupType == TrainingCardGroup.Special;
            
            // アニメーション再生
            // 3D
            if(TrainingUtility.IsPlay3DEffect)
            {
                var data = MasterManager.Instance.trainingCardMaster.FindData(cardId);
                currentPracticeType = data.practiceType;
                TagHelperUtility.SetEnable(TagHelperUtility.GroupEnum.Model3D, $"training_{currentPracticeType}", true);

                // FlowZone演出
                if (arguments.IsFlow())
                {
                    effectRoot.gameObject.SetActive(true);
                    flowZonePracticeEffect.PlayEffect(arguments.GetConcentrationEffectId(), 1.0f);
                }
                Wait3DEffect(onFinished).Forget();
            }
            // 2D
            else
            {
                PlayAnimationAsync(isSpecial ? OpenSpecialAnimation : OpenAnimation, onFinished).Forget();
            }
        }
        
        private async UniTask Wait3DEffect(Action onFinished)
        {
            await UniTask.Delay((int)(TrainingUtility.Config.DerayTrainingSceneEffect * 1000.0f));
            onFinished();
        }
        
        /// <summary>閉じる</summary>
        public void PlayCloseAnimation(Action onFinished)
        {
            // アクティブに
            gameObject.SetActive(true);
            PlayAnimationAsync(CloseAnimation, onFinished).Forget();
        }
        
        /// <summary>開く</summary>
        private async UniTask PlayAnimationAsync(string animation, Action onFinished)
        {
            await AnimatorUtility.WaitStateAsync(animator, animation);
            gameObject.SetActive(false);
            effectRoot.gameObject.SetActive(false);
            onFinished();
        }
    }
}