using CruFramework.Page;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Common
{
    public class TrainingCardRewardSheetDescription : Sheet
    {
        [SerializeField] private Image baseImage;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI NoDescriptionText;

        public void SetDisplay(string description)
        {
            if (string.IsNullOrEmpty(description) == false)
            {
                descriptionText.text = description;
            }
            else
            {
                baseImage.gameObject.SetActive(false);
                descriptionText.gameObject.SetActive(false);
                NoDescriptionText.gameObject.SetActive(true);
            }
        }
    }
}