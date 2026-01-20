using System;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Rivalry
{
    public class RivalryStaminaView : MonoBehaviour
    {
        #region ViewParams
        public class ViewParams
        {
            public StaminaBase staminaBase;
            public StaminaMasterObject staminaMaster;

            public ViewParams(StaminaBase staminaBase, StaminaMasterObject staminaMaster)
            {
                this.staminaBase = staminaBase;
                this.staminaMaster = staminaMaster;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private TMP_Text countText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private UIProgress progress;
        #endregion

        #region PrivateProperties
        private ViewParams _viewParams;
        private int _lastSecond = -1;
        private Action<ViewParams> _onClickPlusButton;
        #endregion
        
        #region PublicMethods
        public void Init(Action<ViewParams> onClickPlusButton)
        {
            _onClickPlusButton = onClickPlusButton;
            ResetDisplay();
        }

        /// <summary>APIで取得して更新</summary>
        public async UniTask UpdateAsync()
        {
            await StaminaUtility.UpdateStaminaAsync();
            var staminaBase = StaminaUtility.GetStaminaBase((long)StaminaUtility.StaminaType.RivalryBattle);
            var staminaMaster = MasterManager.Instance.staminaMaster.FindData(staminaBase?.mStaminaId ?? 0);
            _viewParams = staminaMaster != null ? new ViewParams(staminaBase, staminaMaster) : null;
            ResetDisplay();
        }

        private void Update()
        {
            if (_viewParams == null || _lastSecond == AppTime.Now.Second) return;
            
            _lastSecond = AppTime.Now.Second;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var now = AppTime.Now;
            var currentStaminaBase = _viewParams.staminaBase.currentStamina;
            var maxStamina = _viewParams.staminaMaster.max;
            var cureSecond = _viewParams.staminaMaster.cureSecond;
            var staminaCuredAt = _viewParams.staminaBase.staminaCuredAt.TryConvertToDateTime();

            if (currentStaminaBase >= maxStamina) {
                timerText.text = string.Empty;
                countText.text = $"{currentStaminaBase}/{maxStamina}";
                progress.SetProgress(min: 0, max: maxStamina, value: currentStaminaBase);
            } else {
                var timeSpan = staminaCuredAt - now;
                var staminaPercentage = Mathf.Max(0, (float)timeSpan.TotalSeconds) / cureSecond;
                var currentStamina = (int)Mathf.Min(maxStamina, MathF.Floor(staminaPercentage * maxStamina));
                if (timeSpan.TotalHours > 0) timerText.text = $"あと{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
                else if (timeSpan.TotalMinutes > 0) timerText.text = $"あと{timeSpan.Minutes}:{timeSpan.Seconds}";
                else if (timeSpan.TotalSeconds > 0) timerText.text = $"あと{timeSpan.Seconds}";
                else timerText.text = string.Empty;
                countText.text = $"{currentStamina}/{maxStamina}";
                progress.SetProgress(min: 0, max: maxStamina, value: currentStamina);
            }
        }
        #endregion

        #region PrivateMethods
        private void ResetDisplay()
        {
            _lastSecond = -1;
            countText.text = string.Empty;
            timerText.text = string.Empty;
            progress.SetProgress(0);
        }
        #endregion
        
        #region EventListener
        public void OnClickPlusButton()
        {
            if(_viewParams != null) _onClickPlusButton?.Invoke(_viewParams);
        }
        #endregion
    }
}