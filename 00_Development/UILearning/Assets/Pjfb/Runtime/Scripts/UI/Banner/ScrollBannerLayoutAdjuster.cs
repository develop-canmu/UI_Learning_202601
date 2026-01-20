using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.UI
{
    public class ScrollBannerLayoutAdjuster : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform childPrefab;
        [SerializeField] private int maximumSpacing = 10;
        #endregion

        #region PrivateFields
        private Action _onEnable = null;
        #endregion
        
        #region PrivateMethods
        private void OnEnable()
        {
            _onEnable?.Invoke();
            _onEnable = null;
        }

        private async void ResetSpacing()
        {
            // ScrollBanner.csではchildをdestroyした途端にResetSpacingを実行するところ、destroyしているGameObjectがまだカウントされてしまうため、EndOfFrameにずらします
            await UniTask.WaitForEndOfFrame(this);
            
            var childCount = rectTransform.childCount;
            var childWidth = childPrefab.rect.width;
            var width = rectTransform.rect.width;
            var childDistance = width / Math.Max(1, childCount - 1);
            var spacing = childDistance - childWidth;
            layoutGroup.spacing = Math.Min(maximumSpacing, spacing);
        }
        #endregion
        
        #region PublicMethods
        [ContextMenu(nameof(TryResetSpacing))]
        public void TryResetSpacing()
        {
            if (gameObject.activeInHierarchy) ResetSpacing();
            else _onEnable = ResetSpacing;
        }
        #endregion
    }
}