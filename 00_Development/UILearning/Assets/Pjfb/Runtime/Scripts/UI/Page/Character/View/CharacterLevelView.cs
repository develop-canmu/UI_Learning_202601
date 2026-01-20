using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class CharacterLevelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;

        public void SetValue(long value)
        {
            levelText.text = value.ToString();
        }

        public void SetEmpty()
        {
            levelText.text = StringValueAssetLoader.Instance["character.empty_level"];
        }
    }
}
