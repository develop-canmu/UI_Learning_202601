using TMPro;
using UnityEngine;

namespace Pjfb.Club
{
    public class ClubNameView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                nameText.text = StringValueAssetLoader.Instance["club.empty_name"];
                return;
            }
            
            nameText.text = value;
        }
    }
}
