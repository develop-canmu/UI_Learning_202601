using Pjfb.Extensions;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pjfb.Common
{
    public class PossessionItemUi : MonoBehaviour
    {
        [SerializeField] private IconImage iconImage;
        [SerializeField] private TMP_Text beforeCountText;
        [SerializeField] private TMP_Text afterCountText;
        [SerializeField] private Image arrowImage;

        [SerializeField] [ColorValue] private string gainColorKey;
        [SerializeField] [ColorValue] private string shortageColorKey;
        [SerializeField] [ColorValue] private string normalColorKey;

        private long currentCount;



        public void SetPossessionUi(long pointId, bool isComma = false)
        {
            SetCount(pointId, GetPointValue(pointId), isComma);
        }

        public void SetAfterCountByAmount(long pointId, long afterAmount)
        {
            long count = GetPointValue(pointId);
            SetAfterCount(pointId, count, count + afterAmount);
        }

        public void SetAfterCount(long pointId, long beforeCount, long afterCount)
        {
            if(iconImage != null) iconImage.SetTexture(pointId);
            currentCount = beforeCount;
            ShowBeforeCount();
            beforeCountText.text = beforeCount.ToString();
            SetAfterCountText(afterCount);
        }

        public void UpdateAfterAmount(long amount)
        {
            SetAfterCountText(currentCount + amount);
        }
        public void SetCount(long pointId, long count, bool isComma = false)
        {
            if(iconImage != null) iconImage.SetTexture(pointId);
            currentCount = count;
            HideBeforeCount();
            SetAfterCountText(currentCount, isComma);
        }

        private void ShowBeforeCount()
        {
            arrowImage.gameObject.SetActive(true);
            beforeCountText.gameObject.SetActive(true);
        }

        private void HideBeforeCount()
        {
            arrowImage.gameObject.SetActive(false);
            beforeCountText.gameObject.SetActive(false);
        }

        private long GetPointValue(long pointId)
        {
            var point = UserDataManager.Instance.point.Find(pointId);
            return point?.value ?? 0;
        }

        private void SetAfterCountText(long count, bool isComma = false)
        {
            afterCountText.text = isComma ? count.GetStringNumberWithComma() : count.ToString();
            SetColor(count);
        }

        private void SetColor(long afterCount)
        {
            if (currentCount == afterCount)
            {
                afterCountText.color = ColorValueAssetLoader.Instance[normalColorKey];
            }
            else if (currentCount > afterCount)
            {
                afterCountText.color = ColorValueAssetLoader.Instance[shortageColorKey];
            }
            else
            {
                afterCountText.color = ColorValueAssetLoader.Instance[gainColorKey];
            }
        }

        public void SetColor(Color color)
        {
            afterCountText.color = color;
        }
        
        public void SetShortageColor()
        {
            afterCountText.color = ColorValueAssetLoader.Instance[shortageColorKey];
        }

        public void SetRequiredCount(long pointId, long requiredCount)
        {
            if(iconImage != null) iconImage.SetTexture(pointId);
            currentCount = GetPointValue(pointId);
            HideBeforeCount();
            afterCountText.text = requiredCount.ToString();
            afterCountText.color =
                ColorValueAssetLoader.Instance[(currentCount >= requiredCount) ? normalColorKey : shortageColorKey];
        }

        public void UpdateRequiredCount(long value)
        {
            afterCountText.text = value.ToString();
            afterCountText.color =
                ColorValueAssetLoader.Instance[(currentCount >= value) ? normalColorKey : shortageColorKey];
        }
    }
}
