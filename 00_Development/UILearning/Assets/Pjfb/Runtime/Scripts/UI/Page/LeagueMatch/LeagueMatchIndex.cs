using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchIndex : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI indexText;

        public void IniUI(string index, Color color)
        {
            indexText.text = index;
            image.color = color;
        }
    }
}