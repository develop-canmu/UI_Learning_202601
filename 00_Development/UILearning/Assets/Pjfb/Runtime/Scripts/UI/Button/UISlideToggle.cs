using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pjfb
{
    public class UISlideToggle : Slider,IBeginDragHandler
    {
        #region Params
        [SerializeField] private SE onSoundType = SE.se_common_icon_tap;
        /// <summary>押下時の鳴らす音</summary>
        public SE OnSoundType{get{return onSoundType;}set{onSoundType = value;}}
        
        [SerializeField] private SE offSoundType = SE.se_common_cancel;
        /// <summary>押下時の鳴らす音</summary>
        public SE OffSoundType{get{return offSoundType;}set{offSoundType = value;}}

        [SerializeField] private bool m_isOn = true;
        [SerializeField] private Image handleImage;
        [SerializeField] private Image bgImage;
        [SerializeField] private Sprite imageOn;
        [SerializeField] private Sprite imageOff;
        [SerializeField] private TextMeshProUGUI textOn;
        [SerializeField] private TextMeshProUGUI textOff;
        [SerializeField,ColorValue] private string colorOn;
        [SerializeField,ColorValue] private string colorOff;
        [SerializeField] private float switchDuration = 0.3f;
        [SerializeField] private float moveDuration = 0.5f;

        public new Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();
        public bool isOn
        {
            get { return m_isOn; }
            set { SetToggleValue(value);}
        }

        private bool isMoving = false;
        private bool isDrag = false;
        
        #endregion

        #region EventListeners
        public void OnSliderValueChanged(float sliderValue)
        {
            float ratio =  switchDuration / moveDuration;
            float t =  ratio > 1 ? 0 : (1 - ratio) / 2;
            float ratioValue = (sliderValue - t) <= 0 ? 0 : (sliderValue - t) / (1 - 2 * t);

            if (value == 0)
                SetToggleValue(false);
            else if (value >= maxValue)
                SetToggleValue(true);
            else
                bgImage.color = Color.Lerp(ColorValueAssetLoader.Instance[colorOff], ColorValueAssetLoader.Instance[colorOn], ratioValue);
        }

        private async UniTask UpdateValue()
        {
            Color targetColor;
            float targetValue;
            float mduration = moveDuration;
            float sduration = switchDuration;
            
            if (value == 0 && !isDrag)
            {
                targetValue = maxValue;
                targetColor =  ColorValueAssetLoader.Instance[colorOn];
            }
            else if (value >= maxValue && !isDrag)
            {
                targetValue = 0;
                targetColor =  ColorValueAssetLoader.Instance[colorOff];
            }
            else
            {
                targetValue = Mathf.RoundToInt(value);
                targetColor = targetValue > 0 ? ColorValueAssetLoader.Instance[colorOn] : ColorValueAssetLoader.Instance[colorOff];
                float diff = Mathf.Abs(value - targetValue);
                sduration = diff * switchDuration;
                mduration = diff * moveDuration;
            }
            
            await UniTask.WhenAll(this.DOValue(targetValue, mduration).ToUniTask(), bgImage.DOColor(targetColor, sduration).ToUniTask());
        }
        
        public override async void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (isMoving) return;
            
            if(!isDrag) PlayToggleSe(!isOn);
            
            isMoving = true;
            await UpdateValue();
            isMoving = false;
            isDrag = false;
        }

        
        public override void OnPointerDown(PointerEventData eventData) { }

        public void OnBeginDrag(PointerEventData eventData) { isDrag = true; }
        
        #endregion
        
        #region Other
        public void SetToggleValue(bool toggleValue,bool sendCallback = true )
        {
            SetValueWithoutNotify(toggleValue ? 1 : 0);
            m_isOn = toggleValue;
            
            if(textOn != null) textOn.gameObject.SetActive(toggleValue);
            if(textOff != null) textOff.gameObject.SetActive(!toggleValue);
            if(imageOn != null && imageOff != null)handleImage.sprite = (toggleValue) ? imageOn : imageOff;
            bgImage.color = toggleValue ? ColorValueAssetLoader.Instance[colorOn] : ColorValueAssetLoader.Instance[colorOff];
            
            if (toggleValue != m_isOn && sendCallback)
            {
                onValueChanged?.Invoke(toggleValue);
            }
            
            if(isDrag && sendCallback) PlayToggleSe(isOn);
        }
        
        public void SetIsOnWithoutNotify(bool value)
        {
            SetToggleValue(value, false);
        }

        private void PlayToggleSe(bool toggleValue)
        {
            //Play SE
            SE se = toggleValue ? onSoundType : offSoundType;
            SEManager.PlaySE(se);
        }

        #endregion
    }
}