using System;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using CruFramework;
using CruFramework.Adv;
using Pjfb.UI;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;

namespace Pjfb.Training
{
    public class TrainingEventResultAnimation : MonoBehaviour
    {
        /// <summary>アニメーション名</summary>
        private static readonly string SkillAnimation = "OpenSkillGet";
        /// <summary>アニメーション名</summary>
        private static readonly string FlowSkillGetAnimation = "OpenFlowSkillGet";
        /// <summary>アニメーション名</summary>
        private static readonly string CloseAnimation = "Close";
        /// <summary>アニメーション名</summary>
        private static readonly string PerformanceAnimation = "OpenPerformanceLvUp";
        /// <summary>アニメーション名</summary>
        private static readonly string PracticeAnimation = "OpenPracticeLvUp";
        /// <summary>アニメーション名</summary>
        private static readonly string OpenBoostLvUpAnimation = "OpenBoostLvUp";
        /// <summary>アニメーション名</summary>
        private static readonly string OpenExtraSkillGetAnimation = "OpenExtraSkillGet";

        /// <summary> インスピレーション獲得演出 </summary>
        private static readonly string InspirationGetKey = "InspirationGetEffect";

        /// <summary> Flowインスピレーション獲得演出 </summary>
        private static readonly string FlowInspirationGetKey = "FlowInspirationGetEffect";
        
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private CharacterDetailSkillView skillView = null;
        [SerializeField]
        private PoolListContainer skillListContainer = null;
        [SerializeField]
        private GameObject touchGuard = null;
        

        private bool isPlayingCloseAnimation = false;
        /// <summary>閉じるアニメーション再生中</summary>
        public bool IsPlayingCloseAnimation{get{return isPlayingCloseAnimation;}}
        
        /// <summary>パフォーマンスのLvアップ再生中？</summary>
        public bool IsPlayingPerformanceAnimation{get{return isPlayingCloseAnimation;}}
        
        // インスピレーション
        private long inspirationGetAnimationId = 0;
        private TrainingEventResultGetInspirationAnimation inspirationGetAnimation = null;
        
        /// <summary>ブーストのスキルデータ</summary>
        private List<TrainingEventResultSkillPoolListItem.TrainingEventResultSkillPoolListItemParam> boostItemParams = new ();
        
        public void Hide()
        {

        }

        /// <summary> スキル取得時の再生アニメーション名 </summary>
        private string GetSkillGetAnimationName(long rarity, AbilityMasterObject.AbilityCategory category)
        {
            // Flow時
            if (category == AbilityMasterObject.AbilityCategory.Flow)
            {
                return FlowSkillGetAnimation;
            }
            else
            {
                return SkillAnimation + rarity.ToString("000");
            }
        }
        
        /// <summary>スキル表示</summary>
        public void PlayGetSkillAnimation(TrainingAbility skill, AbilityMasterObject.AbilityCategory category, Action onComplete)
        {
            PlayGetSkillAnimationAsync(skill, category, onComplete).Forget();
        }
        
        /// <summary>スキル表示</summary>
        private async UniTask PlayGetSkillAnimationAsync(TrainingAbility skill, AbilityMasterObject.AbilityCategory category, Action onComplete)
        {
            // タッチガード
            touchGuard.SetActive(true);
            gameObject.SetActive(true);
            
            AbilityMasterObject mAbility = MasterManager.Instance.abilityMaster.FindData(skill.id);
            // スキルをセット
            skillView.SetSkillId(skill.id, skill.level);
            // アニメーション
            await AnimatorUtility.WaitStateAsync(animator, GetSkillGetAnimationName(mAbility.rarity, category) );            
            // タッチガード
            touchGuard.SetActive(false);
            // 完了通知
            onComplete();
        }
        
        /// <summary>練習カードLv</summary>
        public void PlayPracticeLvUpAnimation(long cardId, long lv, bool isLvMax, Action onComplete)
        {
            PlayPracticeLvUpAnimationAsync(cardId, lv, isLvMax, onComplete).Forget();
        }
        
        /// <summary>練習カードLv</summary>
        public async UniTask PlayPracticeLvUpAnimationAsync(long cardId, long lv, bool isLvMax, Action onComplete)
        {
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(cardId);
            // タッチガード
            touchGuard.SetActive(true);
            gameObject.SetActive(true);
            
            skillView.SetIconActive(false);
            skillView.SetName( StringValueAssetLoader.Instance[$"practice_name{mCard.practiceType}"] );
            skillView.SetLv(lv, isLvMax);
            skillView.SetRarity(1);
            skillView.SetEnableDetail(false);

            // アニメーション
            await AnimatorUtility.WaitStateAsync(animator, PracticeAnimation );            
            // タッチガード
            touchGuard.SetActive(false);
            // 完了通知
            onComplete();
        }

        public void PlayGetInspirationAnimation(TrainingInspire[] getInspirations, TrainingInspire[] acquiredInspirations,  TrainingUtility.InspirationType type, Action<bool> onComplete)
        {
            PlayGetInspirationAnimationAsync(getInspirations, acquiredInspirations, type, onComplete).Forget();
        }
        
        /// <summary>インスピレーション獲得表示</summary>
        public async UniTask PlayGetInspirationAnimationAsync(TrainingInspire[] getInspirations, TrainingInspire[] acquiredInspirations,  TrainingUtility.InspirationType type, Action<bool> onComplete)
        {
            // タッチガード
            touchGuard.SetActive(true);
            gameObject.SetActive(true);

            // エフェクトの読み込みが必要か？
            bool needLoadEffect = true;
            long effectId = TrainingUtility.GetMaxGrade(getInspirations);
            // 取得インスピレーションカードリスト
            TrainingInspirationCardList[] inspirationCardList = TrainingUtility.GetGetInspirationList(getInspirations, acquiredInspirations);
            
            if (inspirationGetAnimation != null)
            {
                // 同一の演出タイプかチェック
                if (inspirationGetAnimation.EffectType == type)
                {
                    // 同一のエフェクトが読み込み済みなら読み込み不要
                    if (inspirationGetAnimationId == effectId)
                    {
                        needLoadEffect = false;
                    }
                }
            }
            
            // エフェクトの生成
            if(needLoadEffect)
            {
                // 前のエフェクトを破棄
                if(inspirationGetAnimation != null)
                {
                    GameObject.Destroy(inspirationGetAnimation.gameObject);
                    inspirationGetAnimation = null;
                    // オブジェクト消したのでいったんId初期化しとく
                    inspirationGetAnimationId = -1;
                }
                
                string path = string.Empty;
                
                switch (type)
                {
                    case TrainingUtility.InspirationType.Normal:
                    {
                        path = ResourcePathManager.GetPath(InspirationGetKey, effectId);
                        break;
                    }
                    case TrainingUtility.InspirationType.Flow:
                    {
                        path = ResourcePathManager.GetPath(FlowInspirationGetKey);
                        break;
                    }
                    default:
                    {
                        CruFramework.Logger.LogError($"Not Find EffectKey : {type}");
                        break;
                    }
                }

                
                // エフェクトの読み込み
                await PageResourceLoadUtility.LoadAssetAsync<GameObject>(path, (effect)=>
                    {
                        inspirationGetAnimation = GameObject.Instantiate<GameObject>(effect, transform).GetComponent<TrainingEventResultGetInspirationAnimation>();
                    },
                    gameObject.GetCancellationTokenOnDestroy()
                );
            }
            
            // 読み込まれているエフェクトIdを更新
            inspirationGetAnimationId = effectId;
            
            // スクロールにセット
            inspirationGetAnimation.SetItems(type, inspirationCardList);
            
            // エフェクトをアクティブ
            inspirationGetAnimation.gameObject.SetActive(true);
            // 初期化
            inspirationGetAnimation.Begin();
            // アニメーション
            await AnimatorUtility.WaitStateAsync(inspirationGetAnimation.InspirationAnimator, "Open");  
            // スクロールアニメーション
            bool isSkiped = await inspirationGetAnimation.ScrollAnimationAsync();
            
            // 初期化
            inspirationGetAnimation.End();
            // タッチガード
            touchGuard.SetActive(false);
            // 完了通知
            onComplete(isSkiped);
        }
        
        /// <summary>パフォーマンスLv</summary>
        public void PlayParforceLvUpAnimation(long lv, bool isLvMax, Action onComplete)
        {
            PlayParforceLvUpAnimationAsync(lv, isLvMax, onComplete).Forget();
        }
        
        /// <summary>パフォーマンスLv</summary>
        public async UniTask PlayParforceLvUpAnimationAsync(long lv, bool isLvMax, Action onComplete)
        {
            // タッチガード
            touchGuard.SetActive(true);
            gameObject.SetActive(true);
            
            skillView.SetIconActive(false);
            skillView.SetName(StringValueAssetLoader.Instance["training.perfomace.lv_name"]);
            skillView.SetLv(lv, isLvMax);
            skillView.SetRarity(1);
            skillView.SetEnableDetail(false);
            
            // アニメーション
            await AnimatorUtility.WaitStateAsync(animator, PerformanceAnimation );            
            // タッチガード
            touchGuard.SetActive(false);
            // 完了通知
            onComplete();
        }
        
        /// <summary>インスピレーションのブーストLv</summary>
        public async UniTask PlayInspirationBoostLvUpAnimationAsync(Action onComplete)
        {
            gameObject.SetActive(true);
            // アニメーション
            await AnimatorUtility.WaitStateAsync(animator, OpenBoostLvUpAnimation );
            // 完了通知
            onComplete();
        }

        /// <summary>追加スキル表示</summary>
        public async UniTask PlayExtraSkillGetAnimationAsync(WrapperIntList[] abilityList, AdvManager adv, Action onComplete)
        {
            gameObject.SetActive(true);
            boostItemParams.Clear();
            // アイテムを非表示にする
            skillListContainer.Clear();
            
            foreach (WrapperIntList ability in abilityList)
            {
                // 0番目にm_ability_training_point_statusのid
                AbilityTrainingPointStatusMasterObject abilityTrainingPointStatusMasterObject = MasterManager.Instance.abilityTrainingPointStatusMaster.FindData(ability.l[0]);
                // 1番目にlevelが入っている
                // typeが1の場合にブーストのみの表示を行う
                boostItemParams.Add(new TrainingEventResultSkillPoolListItem.TrainingEventResultSkillPoolListItemParam(
                    abilityTrainingPointStatusMasterObject.mAbilityId, ability.l[1], abilityTrainingPointStatusMasterObject.type == 1));
            }

            // ブーストのみアビリティを前に持ってくる
            boostItemParams = boostItemParams.OrderByDescending(data => data.IsBoostOnly).ToList();
            // アニメーション
            await AnimatorUtility.WaitStateAsync(animator, OpenExtraSkillGetAnimation);

            // １つずつログを追加する
            foreach (TrainingEventResultSkillPoolListItem.TrainingEventResultSkillPoolListItemParam itemParam in boostItemParams)
            {
                AbilityMasterObject mAbility = MasterManager.Instance.abilityMaster.FindData(itemParam.SkillId);
                adv.AddMessageLog(StringValueAssetLoader.Instance["training.log.boost_chance.effect"], string.Format(StringValueAssetLoader.Instance["training.log.skill_get"], mAbility.name), 0);
            }
            // 完了通知
            onComplete();
        }

        /// <summary>AnimationEvent</summary>
        public void OnSlideIn()
        {
            SlideIn().Forget();
        }

        
        /// <summary>SlideInしてくるAnimation</summary>
        private async UniTask SlideIn()
        {
            // データ設定
            await skillListContainer.SetDataList(boostItemParams);
        }
        
        /// <summary>閉じる</summary>
        public void CloseStatusUpAnimation(Action onCompleted)
        {
            CloseStatusUpAnimationAsync(onCompleted).Forget();
        }
        
        private async UniTask CloseStatusUpAnimationAsync(Action onCompleted)
        {
            // すでに再生中
            if(isPlayingCloseAnimation)return;
            // 再生中に
            isPlayingCloseAnimation = true;
            // 再生
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            gameObject.SetActive(false);
            isPlayingCloseAnimation = false;
            // コールバック
            onCompleted();
        }
        
        /// <summary>閉じる</summary>
        public void CloseInspirationGetAnimation(Action onCompleted)
        {
            CloseInspirationGetAnimationAsync(onCompleted).Forget();
        }
        
        private async UniTask CloseInspirationGetAnimationAsync(Action onCompleted)
        {
            // すでに再生中
            if(isPlayingCloseAnimation)return;
            // 再生中に
            isPlayingCloseAnimation = true;
            // 再生
            await AnimatorUtility.WaitStateAsync(inspirationGetAnimation.InspirationAnimator, CloseAnimation);
            
            inspirationGetAnimation.gameObject.SetActive(false);
            gameObject.SetActive(false);
            isPlayingCloseAnimation = false;
            // コールバック
            onCompleted();
        }
    }
}
