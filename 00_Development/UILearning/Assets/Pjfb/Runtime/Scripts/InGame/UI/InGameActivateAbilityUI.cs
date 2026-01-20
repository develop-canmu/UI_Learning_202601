using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pjfb.Master;

namespace Pjfb.InGame
{
    public class InGameActivateAbilityUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Slider toActivateSlider;
        [SerializeField] private Slider timeLimitSlider;
        [SerializeField] private List<ParticleSystem> sliderEffects;
        [SerializeField] private Image handleImage;
        [SerializeField] private AbilityNameImage abilityNameImage;

        private const string OpenNormalTrigger = "Open";
        private const string CloseTrigger = "Close";
        private const string OpenFlowTrigger = "OpenFlowSkill";
        private bool isSwiped;
        private float remainTime;
        private IDisposable updateObservable = null;
        private bool isDragging;
        private int processedSliderValue; // floatのvalueすべてでupdateするのエグそうなので.

        private bool isPause;
        private bool autoSwipe;
        
        private void Awake()
        {
            var toActivateSliderEventTrigger = toActivateSlider.AddComponent<EventTrigger>();
            var pointerDownEvent = new EventTrigger.Entry();
            pointerDownEvent.eventID = EventTriggerType.PointerDown;
            pointerDownEvent.callback.AddListener(_ => isDragging = true);
            toActivateSliderEventTrigger.triggers.Add(pointerDownEvent);
            
            var endDragEvent = new EventTrigger.Entry();
            endDragEvent.eventID = EventTriggerType.EndDrag;
            endDragEvent.callback.AddListener(_ => isDragging = false);
            toActivateSliderEventTrigger.triggers.Add(endDragEvent);
            
            gameObject.SetActive(false);
        }

        public void Open(long abilityId, bool _autoSwipe)
        {
            isSwiped = false;
            isPause = false;
            autoSwipe = _autoSwipe;
            StopUpdate();
            gameObject.SetActive(true);
            toActivateSlider.interactable = true;
            remainTime = BattleConst.AbilityActivationWaitSec;
            toActivateSlider.value = 0.0f;
            timeLimitSlider.value = 1.0f;
            updateObservable = gameObject.UpdateAsObservable().Subscribe(_ => UpdateSlider()).AddTo(gameObject);

            // スキルの種類によってアニメーションを切り替える
            animator.SetTrigger(GetAnimTrigger(abilityId));
            toActivateSlider.onValueChanged.AddListener(OnActivateSliderValueChanged);

            abilityNameImage.SetTextureAsync(abilityId).Forget();
            if (autoSwipe)
            {
                toActivateSlider.interactable = false;
            }
        }

        /// <summary> abilityIdからアニメーションTriggerを取得 </summary>
        private string GetAnimTrigger(long abilityId)
        {
            // アビリティマスタ
            AbilityMasterObject abilityMaster = MasterManager.Instance.abilityMaster.FindData(abilityId);
            
            // スキル種類によってトリガーを返す
            switch (abilityMaster.CategoryEnum)
            {
                // 通常スキル
                case AbilityMasterObject.AbilityCategory.Normal:
                    return OpenNormalTrigger;
                // FLOWスキル
                case AbilityMasterObject.AbilityCategory.Flow:
                    return OpenFlowTrigger;
            }

            return null;
        }
        
#if !PJFB_REL
        public void DebugOpen(long abilityId)
        {
            gameObject.SetActive(true);
            toActivateSlider.interactable = false;
            remainTime = BattleConst.AbilityActivationWaitSec;
            Invoke(nameof(Close),remainTime);
            animator.SetTrigger(GetAnimTrigger(abilityId));

            abilityNameImage.SetTextureAsync(abilityId).Forget();
        }
#endif
        private void StopUpdate()
        {
            if (updateObservable != null)
            {
                updateObservable.Dispose();
                updateObservable = null;
            }
        }

        private void Close()
        {
            animator.SetTrigger(CloseTrigger);
        }

        private void UpdateSlider()
        {
            if (isPause)
            {
                return;
            }
            
            remainTime -= Time.deltaTime;
            if (remainTime <= 0.0f)
            {
                timeLimitSlider.value = 0.0f;
                OnSwiped();
                return;
            }
            
            timeLimitSlider.value = remainTime / BattleConst.AbilityActivationWaitSec;

            if (BattleDataMediator.Instance.IsAutoSwipe || autoSwipe)
            {
                toActivateSlider.value += Time.deltaTime;
            }
            else if (!isDragging)
            {
                toActivateSlider.value -= Time.deltaTime / 3.0f;
            }
        }

        private void OnActivateSliderValueChanged(float value)
        {
            // 適当にスワイプしても気持ちよく発動出来るように1.0じゃなくしておく. 値調整
            if (value >= 0.95f)
            {
                OnSwiped();
            }

            // 10%区切りでAlpha反映
            var processValue = (int)(value * 10);
            if (processValue != processedSliderValue)
            {
                processedSliderValue = processValue;
                var alphaValue = 1.0f - (processValue / 10.0f);
                foreach (var particle in sliderEffects)
                {
                    var startColor = particle.main.startColor.color;
                    startColor.a = alphaValue;
                }
                
                var handleScale = 1.0f + value * 0.4f;
                handleImage.transform.localScale = new Vector3(handleScale, handleScale, 1.0f);
            }
        }

        private void OnSwiped()
        {
            if (isSwiped)
            {
                return;
            }
            
            isSwiped = true;
            StopUpdate();
            Close();
            BattleEventDispatcher.Instance.OnSwipedAbilityCallback(true);
            toActivateSlider.onValueChanged.RemoveAllListeners();
        }

        public void Pause()
        {
            isPause = true;
        }
        
        public void Resume()
        {
            isPause = false;
        }
    }
}